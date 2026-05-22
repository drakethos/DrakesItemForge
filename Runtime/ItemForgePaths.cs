using System.IO;
using BepInEx;

namespace DrakesItemForge.Runtime;

internal static class ItemForgePaths
{
    public const string DataFolderName = "ItemForge";

    public static string DataRoot =>
        Path.Combine(Paths.ConfigPath, ItemForgePlugin.GUID, DataFolderName);

    public static string ItemsDirectory => Path.Combine(DataRoot, "items");
    public static string GeneratedDirectory => Path.Combine(DataRoot, "generated");
    public static string CacheDirectory => Path.Combine(DataRoot, "cache");
    public static string LogsDirectory => Path.Combine(DataRoot, "logs");

    public static string ItemsCacheFile => Path.Combine(CacheDirectory, "items.txt");
    public static string ItemTypesCacheFile => Path.Combine(CacheDirectory, "item_types.txt");
    public static string StationsCacheFile => Path.Combine(CacheDirectory, "stations.txt");
    public static string FailureLogFile => Path.Combine(LogsDirectory, "failed_items.txt");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(ItemsDirectory);
        Directory.CreateDirectory(GeneratedDirectory);
        Directory.CreateDirectory(CacheDirectory);
        Directory.CreateDirectory(LogsDirectory);
    }
}
