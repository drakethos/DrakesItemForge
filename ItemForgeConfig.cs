using BepInEx.Configuration;
using BepInEx.Logging;
using DrakesWorkshopLibs.Sync;

namespace DrakesItemForge;

public static class ItemForgeConfig
{
    private const int ExpectedSyncedEntryCount = 3;

    private const string SectionRuntime = "01 Runtime";
    private const string SectionGenerator = "02 Generator";

    private const string DisplayRuntime = "Runtime";
    private const string DisplayGenerator = "Generator";

    private static readonly DrakeConfigSync _drakeConfigSync = DrakeConfigSync.Create(
        ItemForgePlugin.ModName,
        ItemForgePlugin.ModName,
        ItemForgePlugin.Version);

    private static ConfigEntry<bool> _enabled = null!;
    private static ConfigEntry<int> _maxItemsPerLoad = null!;
    private static ConfigEntry<bool> _lockSyncedConfig = null!;
    private static ConfigEntry<bool> _generateOnStartup = null!;
    private static ConfigEntry<bool> _generateWeapons = null!;
    private static ConfigEntry<bool> _generateArmor = null!;
    private static ConfigEntry<bool> _generateHelmet = null!;
    private static ConfigEntry<bool> _generateBow = null!;
    private static ConfigEntry<bool> _generateShield = null!;
    private static ConfigEntry<bool> _generateFood = null!;
    private static ConfigEntry<bool> _generateTool = null!;
    private static ConfigEntry<bool> _generateMaterial = null!;
    private static ConfigEntry<bool> _generateAmmo = null!;
    private static ConfigEntry<bool> _generateUtility = null!;
    private static ConfigEntry<string> _includeFields = null!;

    internal static ManualLogSource? Log { get; set; }

    public static bool Enabled => _enabled.Value;
    public static int MaxItemsPerLoad => _maxItemsPerLoad.Value;
    public static bool LockSyncedConfig => _lockSyncedConfig.Value;
    public static bool GenerateOnStartup => _generateOnStartup.Value;
    public static bool GenerateWeapons => _generateWeapons.Value;
    public static bool GenerateArmor => _generateArmor.Value;
    public static bool GenerateHelmet => _generateHelmet.Value;
    public static bool GenerateBow => _generateBow.Value;
    public static bool GenerateShield => _generateShield.Value;
    public static bool GenerateFood => _generateFood.Value;
    public static bool GenerateTool => _generateTool.Value;
    public static bool GenerateMaterial => _generateMaterial.Value;
    public static bool GenerateAmmo => _generateAmmo.Value;
    public static bool GenerateUtility => _generateUtility.Value;
    public static string IncludeFields => _includeFields.Value;

    public static void Bind(ConfigFile config)
    {
        _enabled = _drakeConfigSync.BindSynced(
            config, SectionRuntime, DisplayRuntime,
            "Enabled", true,
            "When false, ItemForge does not load items from JSON at world start.");

        _maxItemsPerLoad = _drakeConfigSync.BindSynced(
            config, SectionRuntime, DisplayRuntime,
            "MaxItemsPerLoad", 0,
            "Maximum JSON items to register per load (0 = unlimited). Extra files are skipped and logged.");

        _lockSyncedConfig = _drakeConfigSync.BindSynced(
            config, SectionRuntime, DisplayRuntime,
            "LockSyncedConfig", true,
            "When true, only server admins can change synced runtime settings (Enabled, MaxItemsPerLoad). Generator section stays per-client.");
        _drakeConfigSync.AddLockingConfigEntry(_lockSyncedConfig);

        _generateOnStartup = _drakeConfigSync.BindClientOnly(
            config, SectionGenerator, DisplayGenerator,
            "GenerateOnStartup", false,
            "When true, scans vanilla items once at startup and writes templates to ItemForge/generated/.");

        _generateWeapons = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateWeapons", false, "Include weapons in category generation.");
        _generateArmor = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateArmor", false, "Include chest/leg armor in category generation.");
        _generateHelmet = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateHelmet", false, "Include helmets in category generation.");
        _generateBow = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateBow", false, "Include bows in category generation.");
        _generateShield = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateShield", false, "Include shields in category generation.");
        _generateFood = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateFood", false, "Include food consumables in category generation.");
        _generateTool = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateTool", false, "Include tools in category generation.");
        _generateMaterial = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateMaterial", false, "Include materials in category generation.");
        _generateAmmo = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateAmmo", false, "Include ammo in category generation.");
        _generateUtility = _drakeConfigSync.BindClientOnly(config, SectionGenerator, DisplayGenerator, "GenerateUtility", false, "Include utility consumables in category generation.");

        _includeFields = _drakeConfigSync.BindClientOnly(
            config, SectionGenerator, DisplayGenerator,
            "IncludeFields", "Damage,Durability,Weight,Recipe",
            "Comma-separated fields to include in generated templates.");

        _drakeConfigSync.FinalizeBinding(Log, ExpectedSyncedEntryCount, () => LockSyncedConfig);
    }
}
