using System;
using System.Collections.Generic;
using System.IO;
using DrakesItemForge.Runtime.FieldApplicators;
using DrakesItemForge.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrakesItemForge.Generator;

internal static class TemplateWriter
{
    public static void WriteTemplate(string clonePrefabName, bool force = false)
    {
        Runtime.ItemForgePaths.EnsureDirectories();

        string path = Path.Combine(
            Runtime.ItemForgePaths.GeneratedDirectory,
            $"{clonePrefabName}.template.json");

        if (File.Exists(path) && !force)
            return;

        var prefab = ObjectDB.instance?.GetItemPrefab(clonePrefabName);
        if (prefab == null || !prefab.TryGetComponent(out ItemDrop drop))
            throw new InvalidOperationException($"Prefab '{clonePrefabName}' not found or is not an item.");

        var template = TemplateRegistry.InferFromClone(drop);
        var include = ParseIncludeFields(ItemForgeConfig.IncludeFields);

        var obj = new JObject
        {
            ["version"] = 1,
            ["id"] = SuggestId(clonePrefabName),
            ["template"] = template.ToString().ToLowerInvariant(),
            ["clone"] = clonePrefabName,
        };

        var shared = drop.m_itemData.m_shared;

        if (include.Contains(IncludeField.Name))
        {
            obj["name"] = global::Localization.instance?.Localize(shared.m_name) ?? clonePrefabName;
        }

        if (include.Contains(IncludeField.Description))
        {
            obj["description"] = global::Localization.instance?.Localize(shared.m_description) ?? "";
        }

        if (include.Contains(IncludeField.Weight))
            obj["weight"] = shared.m_weight;

        if (include.Contains(IncludeField.StackSize) && shared.m_maxStackSize > 0)
            obj["stackSize"] = shared.m_maxStackSize;

        if (include.Contains(IncludeField.Value))
            obj["value"] = shared.m_value;

        if (include.Contains(IncludeField.Durability) && shared.m_maxQuality > 0)
            obj["durability"] = shared.m_maxQuality;

        if (include.Contains(IncludeField.Armor) && shared.m_armor > 0)
            obj["armor"] = shared.m_armor;

        if (include.Contains(IncludeField.MovementModifier) && shared.m_movementModifier != 0)
            obj["movementModifier"] = shared.m_movementModifier;

        if (include.Contains(IncludeField.Damage))
        {
            var damage = new JObject();
            foreach (HitData.DamageType dt in Enum.GetValues(typeof(HitData.DamageType)))
            {
                float val = DamageTypeMapper.GetDamage(shared.m_damages, dt);
                if (val > 0)
                    damage[dt.ToString().ToLowerInvariant()] = val;
            }

            if (damage.HasValues)
                obj["damage"] = damage;
        }

        if (include.Contains(IncludeField.Food) && shared.m_food > 0)
        {
            obj["hp"] = shared.m_food;
            if (shared.m_foodStamina > 0)
                obj["stamina"] = shared.m_foodStamina;
            if (shared.m_foodEitr > 0)
                obj["eitr"] = shared.m_foodEitr;
            if (shared.m_foodBurnTime > 0)
                obj["duration"] = shared.m_foodBurnTime;
            if (shared.m_foodRegen > 0)
                obj["regen"] = shared.m_foodRegen;
        }

        if (include.Contains(IncludeField.Recipe))
        {
            var recipe = TryBuildRecipeFromClone(clonePrefabName);
            if (recipe != null)
                obj["recipe"] = recipe;
        }

        File.WriteAllText(path, obj.ToString(Formatting.Indented));
    }

    private static JObject? TryBuildRecipeFromClone(string clonePrefabName)
    {
        if (ObjectDB.instance == null)
            return null;

        foreach (var recipe in ObjectDB.instance.m_recipes)
        {
            if (recipe == null)
                continue;

            string? recipeItem = recipe.m_item?.name;
            if (recipeItem != clonePrefabName)
                continue;

            var resources = new JArray();
            if (recipe.m_resources != null)
            {
                foreach (var req in recipe.m_resources)
                {
                    if (req?.m_resItem == null)
                        continue;
                    resources.Add(new JObject
                    {
                        ["item"] = req.m_resItem.name,
                        ["amount"] = req.m_amount,
                    });
                }
            }

            string station = recipe.m_craftingStation != null
                ? recipe.m_craftingStation.name
                : "";

            return new JObject
            {
                ["station"] = FriendlyStationName(station),
                ["level"] = recipe.m_minStationLevel,
                ["resources"] = resources,
            };
        }

        return null;
    }

    private static string FriendlyStationName(string prefab)
    {
        foreach (var key in Schema.StationAliases.KnownFriendlyNames)
        {
            if (Schema.StationAliases.TryResolve(key, out var resolved) &&
                string.Equals(resolved, prefab, StringComparison.OrdinalIgnoreCase))
                return key;
        }

        return prefab;
    }

    private static string SuggestId(string clonePrefabName) =>
        clonePrefabName.ToLowerInvariant();

    private static HashSet<IncludeField> ParseIncludeFields(string csv)
    {
        var set = new HashSet<IncludeField>();
        if (string.IsNullOrWhiteSpace(csv))
            return set;

        foreach (var part in csv.Split(',', ';'))
        {
            if (Enum.TryParse<IncludeField>(part.Trim(), true, out var field))
                set.Add(field);
        }

        return set;
    }
}

internal enum IncludeField
{
    Name,
    Description,
    Damage,
    Durability,
    Weight,
    StackSize,
    Value,
    Armor,
    MovementModifier,
    Food,
    Recipe,
}
