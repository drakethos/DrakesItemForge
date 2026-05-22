using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime;

/// <summary>
/// Vanilla item prefab names from <see cref="ObjectDB.m_items"/> (Unity <c>GameObject.name</c>).
/// Same IDs as console spawn, e.g. <c>SwordBronze</c> — not <c>$item_sword_bronze</c> display tokens.
/// </summary>
internal sealed class ReferenceCache
{
    private static ReferenceCache? _instance;

    public static ReferenceCache Instance => _instance ?? throw new InvalidOperationException("Reference cache not built yet.");

    public static bool IsReady => _instance != null;

    public IReadOnlyCollection<string> ItemPrefabNames { get; private set; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string> ItemPrefabToType { get; private set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
    public IReadOnlyCollection<string> StationPrefabNames { get; private set; } = Array.Empty<string>();

    private Dictionary<string, string> _itemsByKey = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _stationSet = new(StringComparer.Ordinal);

    public static void Reset() => _instance = null;

    public static void EnsureBuilt(ObjectDB? db = null, bool forceRebuild = false, ManualLogSource? log = null)
    {
        if (_instance != null && !forceRebuild)
            return;

        db ??= ObjectDB.instance;
        if (db == null)
            throw new InvalidOperationException("ObjectDB is not available.");

        var items = new List<string>();
        var types = new Dictionary<string, string>(StringComparer.Ordinal);
        var byKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var go in db.m_items)
        {
            if (go == null)
                continue;

            string name = go.name;
            if (byKey.ContainsKey(name))
                continue;

            byKey[name] = name;
            items.Add(name);
            if (go.TryGetComponent(out ItemDrop drop))
                types[name] = drop.m_itemData.m_shared.m_itemType.ToString();
        }

        items.Sort(StringComparer.Ordinal);

        var stations = new HashSet<string>(StringComparer.Ordinal);
        foreach (var alias in StationAliases.KnownFriendlyNames)
        {
            if (StationAliases.TryResolve(alias, out var prefab))
                stations.Add(prefab);
        }

        foreach (var go in db.m_items)
        {
            if (go == null || !go.TryGetComponent(out Piece piece))
                continue;
            if (piece.m_craftingStation != null)
                stations.Add(piece.m_craftingStation.name);
        }

        foreach (var stationName in new[] { "forge", "piece_workbench", "piece_cauldron", "piece_stonecutter",
                     "piece_artisanstation", "piece_blackforge", "piece_magetable", "piece_preptable", "piece_cookingstation" })
        {
            if (db.GetItemPrefab(stationName) != null || ZNetScene.instance?.GetPrefab(stationName) != null)
                stations.Add(stationName);
        }

        _instance = new ReferenceCache
        {
            ItemPrefabNames = items,
            ItemPrefabToType = types,
            StationPrefabNames = stations.OrderBy(s => s, StringComparer.Ordinal).ToList(),
            _itemsByKey = byKey,
            _stationSet = new HashSet<string>(stations, StringComparer.Ordinal),
        };

        WriteCacheFiles(_instance);
        log?.LogInfo($"ItemForge reference cache built: {items.Count} item prefabs (use spawn names like SwordBronze).");
    }

    /// <summary>
    /// Resolves JSON <c>clone</c> / recipe <c>item</c> to the canonical prefab name (case-insensitive).
    /// </summary>
    public bool TryResolveItem(string? name, out string resolved)
    {
        resolved = "";
        if (string.IsNullOrWhiteSpace(name))
            return false;

        string trimmed = name.Trim();
        if (_itemsByKey.TryGetValue(trimmed, out resolved!))
            return true;

        var prefab = ObjectDB.instance?.GetItemPrefab(trimmed);
        if (prefab != null)
        {
            resolved = prefab.name;
            _itemsByKey[resolved] = resolved;
            return true;
        }

        foreach (var known in ItemPrefabNames)
        {
            if (string.Equals(known, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                resolved = known;
                return true;
            }
        }

        return false;
    }

    public bool HasItem(string name) => TryResolveItem(name, out _);

    public bool HasStation(string prefabName) =>
        _stationSet.Contains(prefabName) || ZNetScene.instance?.GetPrefab(prefabName) != null;

    public string? SuggestItem(string name) =>
        Util.Levenshtein.FindClosest(name, ItemPrefabNames);

    public string? SuggestStation(string name) =>
        Util.Levenshtein.FindClosest(name, StationPrefabNames);

    private static void WriteCacheFiles(ReferenceCache cache)
    {
        ItemForgePaths.EnsureDirectories();
        File.WriteAllLines(ItemForgePaths.ItemsCacheFile, cache.ItemPrefabNames);
        File.WriteAllLines(
            ItemForgePaths.ItemTypesCacheFile,
            cache.ItemPrefabToType.Select(kv => $"{kv.Key}\t{kv.Value}"));
        File.WriteAllLines(ItemForgePaths.StationsCacheFile, cache.StationPrefabNames);
    }

    public static bool TryLoadFromDisk()
    {
        if (!File.Exists(ItemForgePaths.ItemsCacheFile))
            return false;

        var items = File.ReadAllLines(ItemForgePaths.ItemsCacheFile)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();
        if (items.Count == 0)
            return false;

        var stations = File.Exists(ItemForgePaths.StationsCacheFile)
            ? File.ReadAllLines(ItemForgePaths.StationsCacheFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToList()
            : new List<string>();

        var types = new Dictionary<string, string>(StringComparer.Ordinal);
        var byKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (File.Exists(ItemForgePaths.ItemTypesCacheFile))
        {
            foreach (var line in File.ReadAllLines(ItemForgePaths.ItemTypesCacheFile))
            {
                var parts = line.Split('\t');
                if (parts.Length >= 2)
                    types[parts[0]] = parts[1];
            }
        }

        foreach (var item in items)
            byKey[item] = item;

        _instance = new ReferenceCache
        {
            ItemPrefabNames = items,
            ItemPrefabToType = types,
            StationPrefabNames = stations,
            _itemsByKey = byKey,
            _stationSet = new HashSet<string>(stations, StringComparer.Ordinal),
        };
        return true;
    }
}
