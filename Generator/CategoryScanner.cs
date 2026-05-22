using System.Collections.Generic;
using DrakesItemForge.Schema;

namespace DrakesItemForge.Generator;

internal enum GenerateCategory
{
    Weapons,
    Armor,
    Helmet,
    Bow,
    Shield,
    Food,
    Tool,
    Material,
    Ammo,
    Utility,
}

internal static class CategoryScanner
{
    public static IEnumerable<string> GetPrefabNamesForCategory(GenerateCategory category)
    {
        if (!Runtime.ReferenceCache.IsReady)
            yield break;

        foreach (var name in Runtime.ReferenceCache.Instance.ItemPrefabNames)
        {
            var prefab = ObjectDB.instance?.GetItemPrefab(name);
            if (prefab == null || !prefab.TryGetComponent(out ItemDrop drop))
                continue;

            if (MatchesCategory(drop, category))
                yield return name;
        }
    }

    public static bool MatchesCategory(ItemDrop drop, GenerateCategory category)
    {
        var template = TemplateRegistry.InferFromClone(drop);
        return category switch
        {
            GenerateCategory.Weapons => template == ItemTemplate.Weapon,
            GenerateCategory.Armor => template == ItemTemplate.Armor,
            GenerateCategory.Helmet => template == ItemTemplate.Helmet,
            GenerateCategory.Bow => template == ItemTemplate.Bow,
            GenerateCategory.Shield => template == ItemTemplate.Shield,
            GenerateCategory.Food => template == ItemTemplate.Food,
            GenerateCategory.Tool => template == ItemTemplate.Tool,
            GenerateCategory.Material => template == ItemTemplate.Material,
            GenerateCategory.Ammo => template == ItemTemplate.Ammo,
            GenerateCategory.Utility => template == ItemTemplate.Utility,
            _ => false,
        };
    }

    public static bool TryParseCategory(string input, out GenerateCategory category)
    {
        category = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return input.Trim().ToLowerInvariant() switch
        {
            "weapons" or "weapon" => Set(GenerateCategory.Weapons, out category),
            "armor" => Set(GenerateCategory.Armor, out category),
            "helmet" or "helmets" => Set(GenerateCategory.Helmet, out category),
            "bow" or "bows" => Set(GenerateCategory.Bow, out category),
            "shield" or "shields" => Set(GenerateCategory.Shield, out category),
            "food" => Set(GenerateCategory.Food, out category),
            "tool" or "tools" => Set(GenerateCategory.Tool, out category),
            "material" or "materials" => Set(GenerateCategory.Material, out category),
            "ammo" => Set(GenerateCategory.Ammo, out category),
            "utility" => Set(GenerateCategory.Utility, out category),
            _ => false,
        };
    }

    private static bool Set(GenerateCategory c, out GenerateCategory category)
    {
        category = c;
        return true;
    }

    public static IEnumerable<GenerateCategory> GetEnabledCategoriesFromConfig()
    {
        if (ItemForgeConfig.GenerateWeapons)
            yield return GenerateCategory.Weapons;
        if (ItemForgeConfig.GenerateArmor)
            yield return GenerateCategory.Armor;
        if (ItemForgeConfig.GenerateHelmet)
            yield return GenerateCategory.Helmet;
        if (ItemForgeConfig.GenerateBow)
            yield return GenerateCategory.Bow;
        if (ItemForgeConfig.GenerateShield)
            yield return GenerateCategory.Shield;
        if (ItemForgeConfig.GenerateFood)
            yield return GenerateCategory.Food;
        if (ItemForgeConfig.GenerateTool)
            yield return GenerateCategory.Tool;
        if (ItemForgeConfig.GenerateMaterial)
            yield return GenerateCategory.Material;
        if (ItemForgeConfig.GenerateAmmo)
            yield return GenerateCategory.Ammo;
        if (ItemForgeConfig.GenerateUtility)
            yield return GenerateCategory.Utility;
    }
}
