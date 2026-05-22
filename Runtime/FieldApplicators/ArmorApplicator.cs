using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class ArmorApplicator
{
    public static void Apply(ItemDefinitionDto dto, ItemDrop itemDrop)
    {
        var shared = itemDrop.m_itemData.m_shared;

        if (dto.Armor.HasValue)
            shared.m_armor = dto.Armor.Value;

        if (dto.Durability.HasValue)
            shared.m_maxQuality = dto.Durability.Value;

        if (dto.MovementModifier.HasValue)
            shared.m_movementModifier = dto.MovementModifier.Value;

        if (dto.Weight.HasValue)
            shared.m_weight = dto.Weight.Value;
    }
}
