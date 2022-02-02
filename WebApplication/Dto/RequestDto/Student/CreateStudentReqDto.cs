using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Student
{
    public class CreateStudentReqDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}
