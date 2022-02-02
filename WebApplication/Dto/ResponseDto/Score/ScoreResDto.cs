using System.Text.Json.Serialization;
using WebApplication.Dto.ResponseDto.Student;
using WebApplication.Dto.ResponseDto.Subject;
using WebApplication.Dto.ResponseDto.Teacher;

namespace WebApplication.Dto.ResponseDto.Score
{
    public class ScoreResDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("qualification")]
        public double Qualification { get; set; }
        [JsonPropertyName("student")]
        public virtual StudentResDto Student { get; set; }
        [JsonPropertyName("subject")]
        public virtual SubjectResDto Subject { get; set; }
        [JsonPropertyName("teacher")]
        public virtual TeacherResDto Teacher { get; set; }
    }
}
