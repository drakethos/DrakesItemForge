using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class WeaponApplicator
{
    public static void Apply(ItemDefinitionDto dto, ItemDrop itemDrop)
    {
        var shared = itemDrop.m_itemData.m_shared;

        if (dto.Damage != null)
        {
            var damages = shared.m_damages;
            foreach (var kv in dto.Damage)
            {
                if (!DamageTypeMapper.TryParse(kv.Key, out var damageType))
                    continue;
                DamageTypeMapper.SetDamage(ref damages, damageType, kv.Value);
            }

            shared.m_damages = damages;
        }

        if (dto.StaminaUse.HasValue)
        {
            var attack = itemDrop.GetComponent<Attack>();
            if (attack != null)
                attack.m_attackStamina = dto.StaminaUse.Value;
        }

        if (dto.Durability.HasValue)
            shared.m_maxQuality = dto.Durability.Value;

        if (dto.MaxQuality.HasValue)
            shared.m_maxQuality = dto.MaxQuality.Value;

        if (dto.MovementModifier.HasValue)
            shared.m_movementModifier = dto.MovementModifier.Value;

        if (dto.Weight.HasValue)
            shared.m_weight = dto.Weight.Value;
    }
}
