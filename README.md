<img width="256" height="256" alt="DrakesItemForge icon" src="icon.png" />

# DrakesItemForge

**DrakesWorkshop** — curated custom item variants for Valheim via JSON. Clone vanilla items, tweak gameplay fields and recipes, and register safely with Jotunn.

## Quick start (server owner)

1. Install **DrakesItemForge**, **Jotunn**, and **DrakesCustomizeLibs** on server and clients. The mod package includes `Newtonsoft.Json.dll` next to `DrakesItemForge.dll` (required for JSON loading).
2. Start the server once, then open:
   - `BepInEx/config/com.drakesworkshop.itemforge.cfg`
   - `BepInEx/config/com.drakesworkshop.itemforge/ItemForge/`
3. Generate a template (in-game console, admin or dedicated server):
   - `itemforge_generate SwordIron`
4. Copy `generated/SwordIron.template.json` to `items/mystorm.json`, edit values, set a unique `id`.
5. Restart the server. Fix any errors in `logs/failed_items.txt`.
6. Spawn or craft your item (`ItemForge_<id>` prefab name, e.g. `ItemForge_mystorm`).

### Hello World smoke test

On first run, if `items/` has no JSON files, Item Forge writes **`hello_world_sword.json`** automatically.

1. Load a world (host or dedicated server).
2. Check the log for `ItemForge seeded hello_world_sword.json`.
3. Admin spawn: `spawn ItemForge_hello_world_sword` — or craft at **workbench** (10 Wood, 2 Leather Scraps).
4. The item should show as **Hello World Sword** with higher slash damage than a normal wooden sword.

Remove or edit `items/hello_world_sword.json` anytime; it is only auto-created when the folder is empty.

See `Examples/stormfang.json` for a fuller weapon example.

## Folders

| Path | Purpose |
|------|---------|
| `ItemForge/items/` | **Runtime** loads JSON from here only |
| `ItemForge/generated/` | Generator output (not loaded at runtime) |
| `ItemForge/cache/` | Prefab name lists for validation |
| `ItemForge/logs/failed_items.txt` | Skipped definitions and reasons |

## JSON `clone` and recipe `item` names

Use **prefab / spawn names** — the same ID as `spawn SwordBronze` in the console (PascalCase usual). **Not** localization tokens like `$item_sword_bronze` and not the English display name "Bronze sword". Names are matched **case-insensitive** (`swordBronze` → `SwordBronze`).

Run `itemforge_items` (or open `ItemForge/cache/items.txt` after loading a world) to see every valid name.

## Console commands

| Command | Description |
|---------|-------------|
| `itemforge_generate SwordIron` | Write one template JSON |
| `itemforge_generate weapons` | Batch by category |
| `itemforge_items` | List vanilla item prefab names |
| `itemforge_validate` | Validate `items/*.json` without registering |

## Config

- **01 Runtime** — `Enabled`, `MaxItemsPerLoad` (server-synced)
- **02 Generator** — category toggles, `IncludeFields`, `GenerateOnStartup` (client-only)

## Templates (V1)

`weapon`, `bow`, `shield`, `armor`, `helmet`, `cape`, `tool`, `food`, `material`, `ammo`, `utility`

## Restrictions (V1)

No custom meshes, asset bundles, hot reload, pieces, creatures, or arbitrary Unity/component editing.
