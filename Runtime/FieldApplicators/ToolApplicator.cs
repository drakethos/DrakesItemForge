using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class ToolApplicator
{
    public static void Apply(ItemDefinitionDto dto, ItemDrop itemDrop)
    {
        var shared = itemDrop.m_itemData.m_shared;

        if (dto.Durability.HasValue)
            shared.m_maxQuality = dto.Durability.Value;

        if (dto.StaminaUse.HasValue)
        {
            var attack = itemDrop.GetComponent<Attack>();
            if (attack != null)
                attack.m_attackStamina = dto.StaminaUse.Value;
        }

        if (dto.Weight.HasValue)
            shared.m_weight = dto.Weight.Value;
    }
}
