using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrakesItemForge.Schema;

internal sealed class ItemDefinitionDto
{
    [JsonProperty("version")]
    public int Version { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("template")]
    public string? Template { get; set; }

    [JsonProperty("clone")]
    public string? Clone { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("stackSize")]
    public int? StackSize { get; set; }

    [JsonProperty("value")]
    public int? Value { get; set; }

    [JsonProperty("weight")]
    public float? Weight { get; set; }

    [JsonProperty("durability")]
    public int? Durability { get; set; }

    [JsonProperty("armor")]
    public float? Armor { get; set; }

    [JsonProperty("knockback")]
    public float? Knockback { get; set; }

    [JsonProperty("staminaUse")]
    public float? StaminaUse { get; set; }

    [JsonProperty("movementModifier")]
    public float? MovementModifier { get; set; }

    [JsonProperty("maxQuality")]
    public int? MaxQuality { get; set; }

    [JsonProperty("hp")]
    public float? Hp { get; set; }

    [JsonProperty("stamina")]
    public float? Stamina { get; set; }

    [JsonProperty("eitr")]
    public float? Eitr { get; set; }

    [JsonProperty("duration")]
    public float? Duration { get; set; }

    [JsonProperty("regen")]
    public float? Regen { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

    [JsonProperty("color")]
    public string? Color { get; set; }

    [JsonProperty("tint")]
    public string? Tint { get; set; }

    [JsonProperty("damage")]
    public Dictionary<string, float>? Damage { get; set; }

    [JsonProperty("recipe")]
    public RecipeDto? Recipe { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }

    public string SourceFile { get; set; } = "";
}

internal sealed class RecipeDto
{
    [JsonProperty("station")]
    public string? Station { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; } = 1;

    [JsonProperty("resources")]
    public List<RecipeResourceDto>? Resources { get; set; }
}

internal sealed class RecipeResourceDto
{
    [JsonProperty("item")]
    public string? Item { get; set; }

    [JsonProperty("amount")]
    public int Amount { get; set; }
}
