using System.Text.Json.Serialization;
using WebApplication.Dto.ResponseDto.Subject;

namespace WebApplication.Dto.ResponseDto.Class
{
    public class ClassResDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("subject")]
        public SubjectResDto Subject { get; set; }
    }
}
