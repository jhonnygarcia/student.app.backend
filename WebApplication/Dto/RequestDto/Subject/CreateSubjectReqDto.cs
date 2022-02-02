using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Subject
{
    public class CreateSubjectReqDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("semester")]
        public string Semester { get; set; }
        [JsonPropertyName("teacherId")]
        public int? TeacherId { get; set; }
    }
}
