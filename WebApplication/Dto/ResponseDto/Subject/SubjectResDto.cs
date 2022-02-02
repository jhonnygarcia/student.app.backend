using System.Text.Json.Serialization;
using WebApplication.Dto.ResponseDto.Teacher;

namespace WebApplication.Dto.ResponseDto.Subject
{
    public class SubjectResDto
    {
        [JsonPropertyName("id")] 
        public int Id { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("description")] 
        public string Description { get; set; }

        [JsonPropertyName("semester")] 
        public string Semester { get; set; }

        [JsonPropertyName("teacher")] 
        public TeacherResDto Teacher { get; set; }
    }
}
