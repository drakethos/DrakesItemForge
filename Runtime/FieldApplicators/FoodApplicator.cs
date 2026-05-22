using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class FoodApplicator
{
    public static void Apply(ItemDefinitionDto dto, ItemDrop itemDrop)
    {
        var shared = itemDrop.m_itemData.m_shared;

        if (dto.Hp.HasValue)
            shared.m_food = dto.Hp.Value;

        if (dto.Stamina.HasValue)
            shared.m_foodStamina = dto.Stamina.Value;

        if (dto.Eitr.HasValue)
            shared.m_foodEitr = dto.Eitr.Value;

        if (dto.Duration.HasValue)
            shared.m_foodBurnTime = dto.Duration.Value;

        if (dto.Regen.HasValue)
            shared.m_foodRegen = dto.Regen.Value;
    }
}
