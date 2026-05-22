using System;
using System.Collections.Generic;
using DrakesItemForge.Generator;
using DrakesItemForge.Runtime;
using Jotunn.Entities;
using Jotunn.Managers;

namespace DrakesItemForge.Commands;

internal static class ItemForgeCommands
{
    public static void Register()
    {
        CommandManager.Instance.AddConsoleCommand(new GenerateCommand());
        CommandManager.Instance.AddConsoleCommand(new ListItemsCommand());
        CommandManager.Instance.AddConsoleCommand(new ValidateCommand());
    }

    private abstract class ItemForgeConsoleCommand : ConsoleCommand
    {
        private readonly string _name;
        private readonly string _help;

        protected ItemForgeConsoleCommand(string name, string help)
        {
            _name = name;
            _help = help;
        }

        public override string Name => _name;
        public override string Help => _help;
        public override bool IsCheat => true;

        protected static bool CanRun()
        {
            if (ZNet.instance != null && ZNet.instance.IsServer())
                return true;

            if (SynchronizationManager.Instance?.PlayerIsAdmin == true)
                return true;

            return false;
        }

        protected static void Print(string message) => ItemForgePlugin.LogSource?.LogInfo($"[ItemForge] {message}");
    }

    private sealed class GenerateCommand : ItemForgeConsoleCommand
    {
        public GenerateCommand() : base("itemforge_generate", "Generate ItemForge template JSON") { }

        public override void Run(string[] args)
        {
            if (!CanRun())
            {
                Print("Server console or admin required.");
                return;
            }

            if (args.Length < 1)
            {
                Print("Usage: itemforge_generate <PrefabName|category> [force]");
                return;
            }

            string target = args[0];
            bool force = args.Length > 1 && string.Equals(args[1], "force", StringComparison.OrdinalIgnoreCase);

            try
            {
                if (CategoryScanner.TryParseCategory(target, out var category))
                    ItemForgeGenerator.GenerateCategory(category, force, ItemForgePlugin.LogSource);
                else
                    ItemForgeGenerator.GenerateSingle(target, force, ItemForgePlugin.LogSource);
            }
            catch (Exception ex)
            {
                Print($"Error: {ex.Message}");
            }
        }
    }

    private sealed class ListItemsCommand : ItemForgeConsoleCommand
    {
        public ListItemsCommand() : base("itemforge_items", "List vanilla item prefab names") { }

        public override void Run(string[] args)
        {
            if (!CanRun())
            {
                Print("Server console or admin required.");
                return;
            }

            try
            {
                if (!ReferenceCache.IsReady)
                {
                    if (ObjectDB.instance != null)
                        ReferenceCache.EnsureBuilt();
                    else if (!ReferenceCache.TryLoadFromDisk())
                    {
                        Print("Load into a world first (ObjectDB required).");
                        return;
                    }
                }

                string? filter = args.Length > 0 ? args[0] : null;
                int shown = 0;
                const int pageSize = 40;
                int page = args.Length > 1 && int.TryParse(args[1], out int p) ? Math.Max(0, p) : 0;
                int skip = page * pageSize;

                foreach (var name in ReferenceCache.Instance.ItemPrefabNames)
                {
                    if (filter != null && name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    if (shown++ < skip)
                        continue;

                    if (shown - skip > pageSize)
                    {
                        Print($"--- page {page} (itemforge_items [filter] {page + 1}) ---");
                        break;
                    }

                    Print(name);
                }

                Print($"Total items: {ReferenceCache.Instance.ItemPrefabNames.Count}");
            }
            catch (Exception ex)
            {
                Print($"Error: {ex.Message}");
            }
        }
    }

    private sealed class ValidateCommand : ItemForgeConsoleCommand
    {
        public ValidateCommand() : base("itemforge_validate", "Validate ItemForge JSON definitions") { }

        public override void Run(string[] args)
        {
            if (!CanRun())
            {
                Print("Server console or admin required.");
                return;
            }

            try
            {
                var (valid, failures) = ItemForgeRuntime.ValidateOnly();
                Print($"Validated. OK entries: {valid}. Failures: {failures}.");
                if (failures > 0)
                    Print($"See {ItemForgePaths.FailureLogFile}");
            }
            catch (Exception ex)
            {
                Print($"Error: {ex.Message}");
            }
        }
    }
}
