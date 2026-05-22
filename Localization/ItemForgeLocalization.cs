using System;
using System.Collections.Generic;
using Jotunn.Managers;

namespace DrakesItemForge.Localization;

internal static class ItemForgeLocalization
{
    private static readonly Dictionary<string, string> PendingEnglish = new(StringComparer.Ordinal);

    public static string GetNameToken(string id) => $"$itemforge_{id}";
    public static string GetDescriptionToken(string id) => $"$itemforge_{id}_desc";

    public static void RegisterItemStrings(string id, string? name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            PendingEnglish[GetNameToken(id).TrimStart('$')] = name;

        if (!string.IsNullOrWhiteSpace(description))
            PendingEnglish[GetDescriptionToken(id).TrimStart('$')] = description;
    }

    public static void FlushToJotunn()
    {
        if (PendingEnglish.Count == 0)
            return;

        LocalizationManager.Instance.AddLocalization("English", new Dictionary<string, string>(PendingEnglish));
        PendingEnglish.Clear();
    }

    public static void Init()
    {
        PendingEnglish.Clear();
    }
}
