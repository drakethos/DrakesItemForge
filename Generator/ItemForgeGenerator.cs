using System;
using BepInEx.Logging;

namespace DrakesItemForge.Generator;

internal static class ItemForgeGenerator
{
    public static int GenerateSingle(string clonePrefabName, bool force, ManualLogSource log)
    {
        EnsureCache();
        TemplateWriter.WriteTemplate(clonePrefabName, force);
        log.LogInfo($"ItemForge generated template: {clonePrefabName}.template.json");
        return 1;
    }

    public static int GenerateCategory(GenerateCategory category, bool force, ManualLogSource log)
    {
        EnsureCache();
        int count = 0;
        foreach (var name in CategoryScanner.GetPrefabNamesForCategory(category))
        {
            try
            {
                TemplateWriter.WriteTemplate(name, force);
                count++;
            }
            catch (Exception ex)
            {
                log.LogWarning($"ItemForge skipped {name}: {ex.Message}");
            }
        }

        log.LogInfo($"ItemForge generated {count} template(s) for category {category}.");
        return count;
    }

    public static int GenerateFromConfig(bool force, ManualLogSource log)
    {
        EnsureCache();
        int total = 0;
        foreach (var cat in CategoryScanner.GetEnabledCategoriesFromConfig())
            total += GenerateCategory(cat, force, log);
        return total;
    }

    private static void EnsureCache()
    {
        Runtime.ItemForgePaths.EnsureDirectories();
        if (!Runtime.ReferenceCache.IsReady)
            Runtime.ReferenceCache.EnsureBuilt();
    }
}
