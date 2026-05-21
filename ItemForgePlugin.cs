using BepInEx;
using DrakesWorkshopLibs;

namespace DrakesItemForge;

[BepInPlugin(GUID, ModName, Version)]
[BepInDependency(CustomizeLibsPlugin.GUID, BepInDependency.DependencyFlags.HardDependency)]
public partial class ItemForgePlugin : BaseUnityPlugin
{
    private void Awake() => Logger.LogInfo($"{ModName} {Version} loaded (Phase 1 stub).");
}
