using System;
using DrakesItemForge.Localization;
using DrakesItemForge.Runtime.FieldApplicators;
using DrakesItemForge.Schema;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

namespace DrakesItemForge.Runtime;

internal static class ItemBuilder
{
    public static BuiltItem? TryBuild(ItemDefinitionDto dto)
    {
        if (!TemplateRegistry.TryParse(dto.Template, out var template))
            return null;

        if (!ReferenceCache.IsReady || !ReferenceCache.Instance.TryResolveItem(dto.Clone, out var clonePrefab))
            return null;

        string prefabName = GetPrefabName(dto.Id!);
        string nameToken = ItemForgeLocalization.GetNameToken(dto.Id!);
        string descToken = ItemForgeLocalization.GetDescriptionToken(dto.Id!);

        var config = new ItemConfig
        {
            Name = string.IsNullOrWhiteSpace(dto.Name) ? nameToken : dto.Name,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? descToken : dto.Description,
        };

        if (dto.Weight.HasValue)
            config.Weight = dto.Weight.Value;

        if (dto.StackSize.HasValue && dto.StackSize.Value > 0)
            config.StackSize = dto.StackSize.Value;

        if (dto.Recipe != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.Recipe.Station) &&
                StationAliases.TryResolve(dto.Recipe.Station, out var stationPrefab))
            {
                config.CraftingStation = stationPrefab;
            }

            config.MinStationLevel = Math.Max(1, dto.Recipe.Level);

            if (dto.Recipe.Resources != null)
            {
                foreach (var res in dto.Recipe.Resources)
                {
                    if (string.IsNullOrWhiteSpace(res.Item) || res.Amount < 1)
                        continue;
                    if (!ReferenceCache.Instance.TryResolveItem(res.Item, out var resPrefab))
                        continue;
                    config.AddRequirement(resPrefab, res.Amount, res.Amount);
                }
            }
        }

        var customItem = new CustomItem(prefabName, clonePrefab, config);

        if (customItem.ItemDrop == null)
            return null;

        GeneralApplicator.Apply(dto, customItem.ItemDrop);

        switch (template)
        {
            case ItemTemplate.Weapon:
            case ItemTemplate.Bow:
                WeaponApplicator.Apply(dto, customItem.ItemDrop);
                break;
            case ItemTemplate.Shield:
            case ItemTemplate.Armor:
            case ItemTemplate.Helmet:
            case ItemTemplate.Cape:
                ArmorApplicator.Apply(dto, customItem.ItemDrop);
                break;
            case ItemTemplate.Food:
                FoodApplicator.Apply(dto, customItem.ItemDrop);
                break;
            case ItemTemplate.Tool:
                ToolApplicator.Apply(dto, customItem.ItemDrop);
                break;
            case ItemTemplate.Material:
            case ItemTemplate.Ammo:
            case ItemTemplate.Utility:
                break;
        }

        return new BuiltItem
        {
            Definition = dto,
            CustomItem = customItem,
            PrefabName = prefabName,
            NameToken = nameToken,
            DescriptionToken = descToken,
        };
    }

    public static string GetPrefabName(string id) => $"ItemForge_{id}";
}
