using System.Text.Json.Serialization;

namespace WebApplication.Dto.ResponseDto.Student
{
    public class StudentResDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}
