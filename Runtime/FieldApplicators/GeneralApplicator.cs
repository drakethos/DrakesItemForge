using DrakesItemForge.Schema;
using UnityEngine;

namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class GeneralApplicator
{
    public static void Apply(ItemDefinitionDto dto, ItemDrop itemDrop)
    {
        var shared = itemDrop.m_itemData.m_shared;

        if (dto.StackSize.HasValue && dto.StackSize.Value > 0)
            shared.m_maxStackSize = dto.StackSize.Value;

        if (dto.Value.HasValue)
            shared.m_value = dto.Value.Value;

        if (dto.Weight.HasValue)
            shared.m_weight = dto.Weight.Value;

        string? colorHex = dto.Color ?? dto.Tint;
        if (!string.IsNullOrWhiteSpace(colorHex))
            ApplyColorTint(itemDrop, colorHex);
    }

    private static void ApplyColorTint(ItemDrop itemDrop, string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color color))
            return;

        foreach (var renderer in itemDrop.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer.material != null)
                renderer.material.color = color;
        }
    }
}
