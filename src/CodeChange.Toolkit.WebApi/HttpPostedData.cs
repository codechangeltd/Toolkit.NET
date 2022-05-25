namespace System.Web.Http;

/// <summary>
/// Represents the posted data split into fields and files
/// </summary>
/// <param name="Fields">The fields in the posted data</param>
/// <param name="Files">The files in the posted data</param>
public record class HttpPostedData(IDictionary<string, HttpPostedField> Fields, IDictionary<string, HttpPostedFile> Files)
{
    /// <summary>
    /// Gets a specific file that is required
    /// </summary>
    /// <param name="name">The name of the file to get</param>
    /// <returns>The matching file</returns>
    public HttpPostedFile GetRequiredFile(string name)
    {
        if (Files.ContainsKey(name))
        {
            return Files[name];
        }
        else
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }
}
