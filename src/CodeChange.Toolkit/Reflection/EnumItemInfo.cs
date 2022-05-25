namespace System
{
    /// <summary>
    /// Represents information about a single enum item
    /// </summary>
    /// <param name="Value">The items value</param>
    /// <param name="Name">The items name</param>
    /// <param name="Description">The items description</param>
    public record class EnumItemInfo(int Value, string Name, string? Description);
}
