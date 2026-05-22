using System;
using System.Collections.Generic;

namespace DrakesItemForge.Schema;

internal static class StationAliases
{
    private static readonly Dictionary<string, string> FriendlyToPrefab = new(StringComparer.OrdinalIgnoreCase)
    {
        ["forge"] = "forge",
        ["workbench"] = "piece_workbench",
        ["cauldron"] = "piece_cauldron",
        ["stonecutter"] = "piece_stonecutter",
        ["artisan"] = "piece_artisanstation",
        ["blackforge"] = "piece_blackforge",
        ["galdr_table"] = "piece_magetable",
        ["magetable"] = "piece_magetable",
        ["preparation"] = "piece_preptable",
        ["preptable"] = "piece_preptable",
        ["cooking"] = "piece_cookingstation",
        ["cookingstation"] = "piece_cookingstation",
    };

    public static bool TryResolve(string friendlyOrPrefab, out string prefabName)
    {
        if (string.IsNullOrWhiteSpace(friendlyOrPrefab))
        {
            prefabName = "";
            return false;
        }

        if (FriendlyToPrefab.TryGetValue(friendlyOrPrefab.Trim(), out prefabName!))
            return true;

        prefabName = friendlyOrPrefab.Trim();
        return true;
    }

    public static IEnumerable<string> KnownFriendlyNames => FriendlyToPrefab.Keys;
}
