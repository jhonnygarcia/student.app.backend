using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Score
{
    public class CreateScoreReqDto
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("qualification")]
        public double? Qualification { get; set; }
        [JsonPropertyName("studentId")]
        public virtual int? StudentId { get; set; }
        [JsonPropertyName("subjectId")]
        public virtual int? SubjectId { get; set; }
        [JsonPropertyName("teacherId")]
        public virtual int? TeacherId { get; set; }
    }
}
