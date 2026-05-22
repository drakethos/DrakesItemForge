using System;
using System.Collections.Generic;
using BepInEx.Logging;
using Jotunn.Managers;

namespace DrakesItemForge.Runtime;

internal static class ItemRegistrar
{
    private static readonly HashSet<string> RegisteredIds = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> RegisteredPrefabs = new(StringComparer.OrdinalIgnoreCase);

    public static List<ValidationFailure> RegisterAll(
        IEnumerable<BuiltItem> items,
        ManualLogSource log)
    {
        var failures = new List<ValidationFailure>();

        foreach (var built in items)
        {
            string id = built.Definition.Id!;
            if (!RegisteredIds.Add(id))
            {
                failures.Add(new ValidationFailure
                {
                    FileName = built.Definition.SourceFile,
                    Error = $"Item id '{id}' was already registered.",
                    Field = "id",
                });
                continue;
            }

            if (!RegisteredPrefabs.Add(built.PrefabName))
            {
                failures.Add(new ValidationFailure
                {
                    FileName = built.Definition.SourceFile,
                    Error = $"Prefab name '{built.PrefabName}' collision.",
                    Field = "id",
                });
                continue;
            }

            bool ok = ItemManager.Instance.AddItem(built.CustomItem);
            if (!ok)
            {
                failures.Add(new ValidationFailure
                {
                    FileName = built.Definition.SourceFile,
                    Error = $"Jotunn rejected item '{built.PrefabName}' (invalid clone or duplicate prefab).",
                    Field = "clone",
                });
                RegisteredIds.Remove(id);
                RegisteredPrefabs.Remove(built.PrefabName);
                continue;
            }

            log.LogInfo($"ItemForge registered {built.PrefabName} (clone: {built.Definition.Clone})");
        }

        return failures;
    }

    public static void Reset()
    {
        RegisteredIds.Clear();
        RegisteredPrefabs.Clear();
    }
}
