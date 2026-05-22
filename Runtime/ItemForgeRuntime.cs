using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using DrakesItemForge.Localization;

namespace DrakesItemForge.Runtime;

internal static class ItemForgeRuntime
{
    private static bool _ran;
    private static ManualLogSource? _log;

    public static void Initialize(ManualLogSource log) => _log = log;

    public static void RunPipeline()
    {
        if (_ran || _log == null)
            return;

        if (!ItemForgeConfig.Enabled)
        {
            _log.LogInfo("ItemForge runtime is disabled in config.");
            _ran = true;
            return;
        }

        try
        {
            if (ObjectDB.instance != null)
                ReferenceCache.EnsureBuilt(ObjectDB.instance, forceRebuild: true, _log);
            else
                ReferenceCache.EnsureBuilt();
        }
        catch (Exception ex)
        {
            _log.LogError($"ItemForge could not build reference cache: {ex.Message}");
            return;
        }

        FailureLogWriter.Clear();

        var (loaded, loadFailures) = ItemLoader.LoadAll();
        var (valid, validationFailures) = ItemValidator.Validate(loaded);

        var allFailures = new List<ValidationFailure>();
        allFailures.AddRange(loadFailures);
        allFailures.AddRange(validationFailures);

        if (ItemForgeConfig.MaxItemsPerLoad > 0 && valid.Count > ItemForgeConfig.MaxItemsPerLoad)
        {
            foreach (var skipped in valid.Skip(ItemForgeConfig.MaxItemsPerLoad))
            {
                allFailures.Add(new ValidationFailure
                {
                    FileName = skipped.SourceFile,
                    Error = $"Skipped: exceeded MaxItemsPerLoad ({ItemForgeConfig.MaxItemsPerLoad}).",
                    Field = "id",
                });
            }

            valid = valid.Take(ItemForgeConfig.MaxItemsPerLoad).ToList();
        }

        var built = new List<BuiltItem>();
        foreach (var dto in valid)
        {
            var item = ItemBuilder.TryBuild(dto);
            if (item == null)
            {
                allFailures.Add(new ValidationFailure
                {
                    FileName = dto.SourceFile,
                    Error = "Failed to build item (invalid clone or template).",
                    Field = "clone",
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
                ItemForgeLocalization.RegisterItemStrings(dto.Id!, dto.Name, dto.Description);
            built.Add(item);
        }

        ItemForgeLocalization.FlushToJotunn();

        var registerFailures = ItemRegistrar.RegisterAll(built, _log);
        allFailures.AddRange(registerFailures);

        if (allFailures.Count > 0)
            FailureLogWriter.WriteAll(allFailures);

        _log.LogInfo($"ItemForge loaded {built.Count} item(s); {allFailures.Count} failure(s) logged.");
        _ran = true;
    }

    public static (int ValidCount, int FailureCount) ValidateOnly()
    {
        ItemForgePaths.EnsureDirectories();
        FailureLogWriter.Clear();

        if (ObjectDB.instance != null)
        {
            ReferenceCache.Reset();
            ReferenceCache.EnsureBuilt(ObjectDB.instance, forceRebuild: true);
        }
        else if (!ReferenceCache.IsReady && !ReferenceCache.TryLoadFromDisk())
        {
            throw new InvalidOperationException("Reference cache unavailable. Load into a world first.");
        }

        var (loaded, loadFailures) = ItemLoader.LoadAll();
        var (valid, validationFailures) = ItemValidator.Validate(loaded);
        var failures = new List<ValidationFailure>();
        failures.AddRange(loadFailures);
        failures.AddRange(validationFailures);

        if (failures.Count > 0)
            FailureLogWriter.WriteAll(failures);

        return (valid.Count, failures.Count);
    }

    public static void ResetForWorldReload()
    {
        _ran = false;
        ReferenceCache.Reset();
        ItemRegistrar.Reset();
    }
}
