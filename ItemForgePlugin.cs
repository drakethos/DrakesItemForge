using BepInEx;
using DrakesItemForge.Commands;
using DrakesItemForge.Localization;
using DrakesItemForge.Runtime;
using DrakesWorkshopLibs;
using HarmonyLib;
using Jotunn;
using static DrakesItemForge.ItemForgeConfig;

namespace DrakesItemForge;

[BepInPlugin(GUID, ModName, Version)]
[BepInDependency(Main.ModGuid)]
[BepInDependency(CustomizeLibsPlugin.GUID, BepInDependency.DependencyFlags.HardDependency)]
public partial class ItemForgePlugin : BaseUnityPlugin
{
    public static ItemForgePlugin? Instance { get; private set; }
    internal static BepInEx.Logging.ManualLogSource LogSource { get; private set; } = null!;

    private readonly Harmony _harmony = new("drakesmod.DrakesItemForge");

    private void Awake()
    {
        Instance = this;
        LogSource = Logger;
        ItemForgePaths.EnsureDirectories();
        DefaultItems.SeedHelloWorldIfEmpty(Logger);
        Log = Logger;
        Bind(Config);
        ItemForgeLocalization.Init();
        ItemForgeRuntime.Initialize(Logger);
        ItemForgeCommands.Register();
        _harmony.PatchAll();

        Logger.LogInfo($"{ModName} {Version} loaded.");
    }

}
