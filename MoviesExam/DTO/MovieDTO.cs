using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoviesExam.DTO;

    public class MovieDto
{
    [JsonPropertyName("id")]

    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]

    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]

    public string Description { get; set; } = string.Empty;

}