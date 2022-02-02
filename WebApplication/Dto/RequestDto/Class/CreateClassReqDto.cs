using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Class
{
    public class CreateClassReqDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("subjectId")]
        public int? SubjectId { get; set; }
    }
}
