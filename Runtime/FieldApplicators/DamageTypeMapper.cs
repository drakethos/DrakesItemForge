namespace DrakesItemForge.Runtime.FieldApplicators;

internal static class DamageTypeMapper
{
    public static bool TryParse(string key, out HitData.DamageType type)
    {
        type = default;
        if (string.IsNullOrWhiteSpace(key))
            return false;

        return key.Trim().ToLowerInvariant() switch
        {
            "physical" or "blunt" => Set(HitData.DamageType.Physical, out type),
            "pierce" => Set(HitData.DamageType.Pierce, out type),
            "slash" => Set(HitData.DamageType.Slash, out type),
            "chop" => Set(HitData.DamageType.Chop, out type),
            "pickaxe" => Set(HitData.DamageType.Pickaxe, out type),
            "fire" => Set(HitData.DamageType.Fire, out type),
            "frost" => Set(HitData.DamageType.Frost, out type),
            "lightning" => Set(HitData.DamageType.Lightning, out type),
            "poison" => Set(HitData.DamageType.Poison, out type),
            "spirit" => Set(HitData.DamageType.Spirit, out type),
            _ => false,
        };
    }

    public static void SetDamage(ref HitData.DamageTypes damages, HitData.DamageType type, float value)
    {
        switch (type)
        {
            case HitData.DamageType.Physical:
                damages.m_damage = value;
                break;
            case HitData.DamageType.Pierce:
                damages.m_pierce = value;
                break;
            case HitData.DamageType.Slash:
                damages.m_slash = value;
                break;
            case HitData.DamageType.Chop:
                damages.m_chop = value;
                break;
            case HitData.DamageType.Pickaxe:
                damages.m_pickaxe = value;
                break;
            case HitData.DamageType.Fire:
                damages.m_fire = value;
                break;
            case HitData.DamageType.Frost:
                damages.m_frost = value;
                break;
            case HitData.DamageType.Lightning:
                damages.m_lightning = value;
                break;
            case HitData.DamageType.Poison:
                damages.m_poison = value;
                break;
            case HitData.DamageType.Spirit:
                damages.m_spirit = value;
                break;
        }
    }

    public static float GetDamage(HitData.DamageTypes damages, HitData.DamageType type) =>
        type switch
        {
            HitData.DamageType.Physical => damages.m_damage,
            HitData.DamageType.Pierce => damages.m_pierce,
            HitData.DamageType.Slash => damages.m_slash,
            HitData.DamageType.Chop => damages.m_chop,
            HitData.DamageType.Pickaxe => damages.m_pickaxe,
            HitData.DamageType.Fire => damages.m_fire,
            HitData.DamageType.Frost => damages.m_frost,
            HitData.DamageType.Lightning => damages.m_lightning,
            HitData.DamageType.Poison => damages.m_poison,
            HitData.DamageType.Spirit => damages.m_spirit,
            _ => 0f,
        };

    private static bool Set(HitData.DamageType t, out HitData.DamageType type)
    {
        type = t;
        return true;
    }
}
