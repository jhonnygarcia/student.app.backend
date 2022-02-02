using Core.Dto.Student;
using Core.Dto.Subject;
using Core.Dto.Teacher;

namespace Core.Dto.Score
{
    public class ScoreDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Qualification { get; set; }
        public virtual StudentDto Student { get; set; }
        public virtual SubjectDto Subject { get; set; }
        public virtual TeacherDto Teacher { get; set; }
    }
}
