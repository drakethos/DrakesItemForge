using System;
using System.Collections.Generic;
using System.IO;
using DrakesItemForge.Schema;
using Newtonsoft.Json;

namespace DrakesItemForge.Runtime;

internal static class ItemLoader
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static (List<ItemDefinitionDto> Items, List<ValidationFailure> Failures) LoadAll()
    {
        ItemForgePaths.EnsureDirectories();
        var items = new List<ItemDefinitionDto>();
        var failures = new List<ValidationFailure>();

        if (!Directory.Exists(ItemForgePaths.ItemsDirectory))
            return (items, failures);

        foreach (var path in Directory.GetFiles(ItemForgePaths.ItemsDirectory, "*.json"))
        {
            string fileName = Path.GetFileName(path);
            try
            {
                string json = File.ReadAllText(path);
                var dto = JsonConvert.DeserializeObject<ItemDefinitionDto>(json, Settings);
                if (dto == null)
                {
                    failures.Add(new ValidationFailure
                    {
                        FileName = fileName,
                        Error = "File is empty or could not be parsed.",
                    });
                    continue;
                }

                dto.SourceFile = fileName;
                items.Add(dto);
            }
            catch (JsonException ex)
            {
                failures.Add(new ValidationFailure
                {
                    FileName = fileName,
                    Error = $"JSON parse error: {ex.Message}",
                });
            }
            catch (Exception ex)
            {
                failures.Add(new ValidationFailure
                {
                    FileName = fileName,
                    Error = $"Failed to read file: {ex.Message}",
                });
            }
        }

        return (items, failures);
    }
}
