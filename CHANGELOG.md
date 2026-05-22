# Changelog

## 0.2.0

- Ship `Newtonsoft.Json.dll` beside `DrakesItemForge.dll` (fixes runtime `TypeLoadException` on JSON load)
- Item Forge V1: JSON item definitions under `BepInEx/config/com.drakesworkshop.itemforge/ItemForge/`
- Runtime pipeline: load, validate, build, Jotunn register (invalid items skipped)
- Eleven item templates with curated gameplay fields
- Template generator and console commands (`itemforge_generate`, `itemforge_items`, `itemforge_validate`)
- Reference cache and failure log with typo suggestions

## 0.1.0

- Phase 1 scaffold
