using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Teacher
{
    public class CreateTeacherReqDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}
