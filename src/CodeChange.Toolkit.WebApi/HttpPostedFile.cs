namespace System.Web.Http;

/// <summary>
/// Represents a single posted file
/// </summary>
/// <param name="Name">The name to call the file</param>
/// <param name="Filename">The original file name</param>
/// <param name="FileContents">The file contents (represented as a byte array)</param>
/// <param name="ContentType">The file content type</param>
public record class HttpPostedFile(string Name, string Filename, byte[] FileContents, string ContentType);
