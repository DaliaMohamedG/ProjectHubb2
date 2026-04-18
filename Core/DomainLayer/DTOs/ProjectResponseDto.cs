using System.Text.Json.Serialization;

public class ProjectResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("authorId")]
    public string AuthorId { get; set; }

    [JsonPropertyName("authorName")]
    public string AuthorName { get; set; }
    [JsonPropertyName("UserImage")]
    public string? UserImage { get; set; }

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("documentUrl")]
    public string? DocumentUrl { get; set; }

    [JsonPropertyName("githubUrl")]
    public string? GithubUrl { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}