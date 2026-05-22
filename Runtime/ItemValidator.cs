using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DrakesItemForge.Schema;

namespace DrakesItemForge.Runtime;

internal static class ItemValidator
{
    private static readonly Regex IdPattern = new("^[a-z0-9_]+$", RegexOptions.Compiled);

    public static (List<ItemDefinitionDto> Valid, List<ValidationFailure> Failures) Validate(
        IEnumerable<ItemDefinitionDto> items)
    {
        var valid = new List<ItemDefinitionDto>();
        var failures = new List<ValidationFailure>();
        var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dto in items)
        {
            var itemFailures = ValidateOne(dto, seenIds);
            if (itemFailures.Count > 0)
                failures.AddRange(itemFailures);
            else
                valid.Add(dto);
        }

        return (valid, failures);
    }

    private static List<ValidationFailure> ValidateOne(ItemDefinitionDto dto, HashSet<string> seenIds)
    {
        var failures = new List<ValidationFailure>();
        string file = dto.SourceFile;

        if (dto.Version != 1)
        {
            failures.Add(Fail(file, $"Unsupported version '{dto.Version}'. Expected version 1.", "version"));
            return failures;
        }

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            failures.Add(Fail(file, "Missing required field 'id'.", "id"));
            return failures;
        }

        if (!IdPattern.IsMatch(dto.Id))
        {
            failures.Add(Fail(file, "Id must be lowercase letters, numbers, and underscores only.", "id"));
            return failures;
        }

        if (!seenIds.Add(dto.Id))
        {
            failures.Add(Fail(file, $"Duplicate item id '{dto.Id}'.", "id"));
            return failures;
        }

        if (!TemplateRegistry.TryParse(dto.Template, out var template))
        {
            failures.Add(Fail(file, $"Unknown template '{dto.Template}'.", "template"));
            return failures;
        }

        if (string.IsNullOrWhiteSpace(dto.Clone))
        {
            failures.Add(Fail(file, "Missing required field 'clone'.", "clone"));
            return failures;
        }

        if (!ReferenceCache.IsReady)
        {
            failures.Add(Fail(file, "Reference cache is not ready (ObjectDB not loaded).", "clone"));
            return failures;
        }

        var cache = ReferenceCache.Instance;
        if (!cache.TryResolveItem(dto.Clone, out _))
        {
            var suggestion = cache.SuggestItem(dto.Clone);
            failures.Add(Fail(
                file,
                $"Unknown item prefab: {dto.Clone}. Use the spawn/console name (e.g. SwordBronze), not $item_ tokens or display names.",
                "clone",
                suggestion != null ? $"Did you mean {suggestion}?" : "Run itemforge_items to list valid prefab names."));
            return failures;
        }

        var allowed = TemplateRegistry.GetAllowedPropertyNames(template);
        if (dto.ExtensionData != null)
        {
            foreach (var key in dto.ExtensionData.Keys)
            {
                if (!allowed.Contains(key))
                {
                    failures.Add(Fail(file, $"Property '{key}' is not allowed for template '{dto.Template}'.", key));
                }
            }
        }

        if (dto.Recipe != null)
            failures.AddRange(ValidateRecipe(dto, file, cache));

        return failures;
    }

    private static IEnumerable<ValidationFailure> ValidateRecipe(
        ItemDefinitionDto dto,
        string file,
        ReferenceCache cache)
    {
        var failures = new List<ValidationFailure>();
        var recipe = dto.Recipe!;

        if (!string.IsNullOrWhiteSpace(recipe.Station))
        {
            if (!StationAliases.TryResolve(recipe.Station, out var stationPrefab))
            {
                failures.Add(Fail(file, $"Unknown crafting station '{recipe.Station}'.", "recipe.station"));
            }
            else if (!cache.HasStation(stationPrefab))
            {
                var suggestion = cache.SuggestStation(stationPrefab);
                failures.Add(Fail(
                    file,
                    $"Unknown crafting station prefab: {stationPrefab}",
                    "recipe.station",
                    suggestion != null ? $"Did you mean {suggestion}?" : null));
            }
        }

        if (recipe.Resources != null)
        {
            foreach (var res in recipe.Resources)
            {
                if (string.IsNullOrWhiteSpace(res.Item))
                {
                    failures.Add(Fail(file, "Recipe resource is missing 'item'.", "recipe.resources"));
                    continue;
                }

                if (res.Amount < 1)
                {
                    failures.Add(Fail(file, $"Resource amount for '{res.Item}' must be at least 1.", "recipe.resources"));
                    continue;
                }

                if (!cache.TryResolveItem(res.Item, out _))
                {
                    var suggestion = cache.SuggestItem(res.Item);
                    failures.Add(Fail(
                        file,
                        $"Unknown item prefab: {res.Item}",
                        "recipe.resources",
                        suggestion != null ? $"Did you mean {suggestion}?" : null));
                }
            }
        }

        return failures;
    }

    private static ValidationFailure Fail(string file, string error, string field, string? suggestion = null) =>
        new()
        {
            FileName = file,
            Error = error,
            Field = field,
            Suggestion = suggestion,
        };
}
