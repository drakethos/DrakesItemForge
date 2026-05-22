using System.IO;
using System.Linq;

namespace DrakesItemForge.Runtime;

internal static class DefaultItems
{
    public const string HelloWorldFileName = "hello_world_sword.json";

    private const string HelloWorldJson = """
        {
          "version": 1,
          "id": "hello_world_sword",
          "template": "weapon",
          "clone": "SwordWood",
          "name": "Hello World Sword",
          "description": "If you hold this blade, Item Forge loaded custom JSON successfully.",
          "damage": {
            "slash": 30
          },
          "durability": 200,
          "weight": 0.8,
          "recipe": {
            "station": "workbench",
            "level": 1,
            "resources": [
              { "item": "Wood", "amount": 10 },
              { "item": "LeatherScraps", "amount": 2 }
            ]
          }
        }
        """;

    /// <summary>
    /// Writes the Hello World smoke-test item when <c>items/</c> has no JSON yet.
    /// </summary>
    public static void SeedHelloWorldIfEmpty(BepInEx.Logging.ManualLogSource log)
    {
        ItemForgePaths.EnsureDirectories();

        if (Directory.GetFiles(ItemForgePaths.ItemsDirectory, "*.json").Any())
            return;

        string path = Path.Combine(ItemForgePaths.ItemsDirectory, HelloWorldFileName);
        File.WriteAllText(path, HelloWorldJson.Trim());
        log.LogInfo($"ItemForge seeded {HelloWorldFileName} — spawn ItemForge_hello_world_sword after loading a world.");
    }
}
