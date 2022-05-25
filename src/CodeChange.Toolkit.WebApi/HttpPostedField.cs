namespace System.Web.Http;

/// <summary>
/// Represents a single posted field
/// </summary>
/// <param name="Name">The name of the field</param>
/// <param name="Value">The field value</param>
public record class HttpPostedField(string Name, string Value);
