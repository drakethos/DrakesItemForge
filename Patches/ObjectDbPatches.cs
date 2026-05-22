using System;
using DrakesItemForge.Generator;
using DrakesItemForge.Localization;
using DrakesItemForge.Runtime;
using HarmonyLib;

namespace DrakesItemForge.Patches;

[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
internal static class ObjectDbPatches
{
    /// <summary>
    /// Run after ObjectDB fills <see cref="ObjectDB.m_items"/> (Prefix ran too early and missed prefabs like SwordBronze).
    /// </summary>
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(ObjectDB __instance)
    {
        try
        {
            ReferenceCache.Reset();
            ReferenceCache.EnsureBuilt(__instance, forceRebuild: true, ItemForgePlugin.LogSource);

            if (ItemForgeConfig.GenerateOnStartup)
            {
                int generated = ItemForgeGenerator.GenerateFromConfig(false, ItemForgePlugin.LogSource);
                ItemForgePlugin.LogSource.LogInfo($"ItemForge startup generator wrote {generated} template(s).");
            }

            ItemForgeRuntime.RunPipeline();
        }
        catch (Exception ex)
        {
            ItemForgePlugin.LogSource?.LogError($"ItemForge ObjectDB hook failed: {ex}");
        }
    }
}

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.OnDestroy))]
internal static class ZNetSceneDestroyPatches
{
    private static void Postfix()
    {
        ItemForgeRuntime.ResetForWorldReload();
        ReferenceCache.Reset();
        ItemRegistrar.Reset();
        ItemForgeLocalization.Init();
    }
}
