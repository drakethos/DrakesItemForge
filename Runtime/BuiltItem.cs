using DrakesItemForge.Schema;
using Jotunn.Entities;

namespace DrakesItemForge.Runtime;

internal sealed class BuiltItem
{
    public ItemDefinitionDto Definition { get; set; } = null!;
    public CustomItem CustomItem { get; set; } = null!;
    public string PrefabName { get; set; } = "";
    public string NameToken { get; set; } = "";
    public string DescriptionToken { get; set; } = "";
}
