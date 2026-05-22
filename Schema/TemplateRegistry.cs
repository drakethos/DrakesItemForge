using System;
using System.Collections.Generic;

namespace DrakesItemForge.Schema;

internal enum ItemTemplate
{
    Weapon,
    Bow,
    Shield,
    Armor,
    Helmet,
    Cape,
    Tool,
    Food,
    Material,
    Ammo,
    Utility,
}

internal static class TemplateRegistry
{
    private static readonly HashSet<string> CoreFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "version", "id", "template", "clone",
    };

    private static readonly HashSet<string> GeneralFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "name", "description", "stackSize", "value", "recipe", "icon", "color", "tint",
    };

    private static readonly HashSet<string> WeaponFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "damage", "knockback", "staminaUse", "durability", "weight", "movementModifier", "maxQuality",
    };

    private static readonly HashSet<string> ArmorFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "armor", "durability", "movementModifier", "weight",
    };

    private static readonly HashSet<string> FoodFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "hp", "stamina", "eitr", "duration", "regen",
    };

    public static bool TryParse(string? value, out ItemTemplate template)
    {
        template = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim().ToLowerInvariant() switch
        {
            "weapon" => Set(ItemTemplate.Weapon, out template),
            "bow" => Set(ItemTemplate.Bow, out template),
            "shield" => Set(ItemTemplate.Shield, out template),
            "armor" => Set(ItemTemplate.Armor, out template),
            "helmet" => Set(ItemTemplate.Helmet, out template),
            "cape" => Set(ItemTemplate.Cape, out template),
            "tool" => Set(ItemTemplate.Tool, out template),
            "food" => Set(ItemTemplate.Food, out template),
            "material" => Set(ItemTemplate.Material, out template),
            "ammo" => Set(ItemTemplate.Ammo, out template),
            "utility" => Set(ItemTemplate.Utility, out template),
            _ => false,
        };
    }

    private static bool Set(ItemTemplate t, out ItemTemplate template)
    {
        template = t;
        return true;
    }

    public static HashSet<string> GetAllowedPropertyNames(ItemTemplate template)
    {
        var set = new HashSet<string>(CoreFields, StringComparer.OrdinalIgnoreCase);
        foreach (var g in GeneralFields)
            set.Add(g);

        switch (template)
        {
            case ItemTemplate.Weapon:
            case ItemTemplate.Bow:
                foreach (var f in WeaponFields)
                    set.Add(f);
                break;
            case ItemTemplate.Shield:
            case ItemTemplate.Armor:
            case ItemTemplate.Helmet:
            case ItemTemplate.Cape:
                foreach (var f in ArmorFields)
                    set.Add(f);
                break;
            case ItemTemplate.Food:
                foreach (var f in FoodFields)
                    set.Add(f);
                break;
            case ItemTemplate.Tool:
                set.Add("durability");
                set.Add("weight");
                set.Add("staminaUse");
                break;
            case ItemTemplate.Material:
            case ItemTemplate.Ammo:
            case ItemTemplate.Utility:
                set.Add("weight");
                break;
        }

        return set;
    }

    public static ItemTemplate InferFromClone(ItemDrop itemDrop)
    {
        var type = itemDrop.m_itemData.m_shared.m_itemType;
        return type switch
        {
            ItemDrop.ItemData.ItemType.OneHandedWeapon => ItemTemplate.Weapon,
            ItemDrop.ItemData.ItemType.TwoHandedWeapon => ItemTemplate.Weapon,
            ItemDrop.ItemData.ItemType.Bow => ItemTemplate.Bow,
            ItemDrop.ItemData.ItemType.Shield => ItemTemplate.Shield,
            ItemDrop.ItemData.ItemType.Helmet => ItemTemplate.Helmet,
            ItemDrop.ItemData.ItemType.Chest => ItemTemplate.Armor,
            ItemDrop.ItemData.ItemType.Legs => ItemTemplate.Armor,
            ItemDrop.ItemData.ItemType.Shoulder => ItemTemplate.Cape,
            ItemDrop.ItemData.ItemType.Ammo => ItemTemplate.Ammo,
            ItemDrop.ItemData.ItemType.Material => ItemTemplate.Material,
            ItemDrop.ItemData.ItemType.Consumable => itemDrop.m_itemData.m_shared.m_food > 0
                ? ItemTemplate.Food
                : ItemTemplate.Utility,
            ItemDrop.ItemData.ItemType.Tool => ItemTemplate.Tool,
            _ => ItemTemplate.Utility,
        };
    }
}
