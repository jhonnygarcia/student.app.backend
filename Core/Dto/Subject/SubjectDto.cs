using Core.Dto.Teacher;

namespace Core.Dto.Subject
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Semester { get; set; }
        public TeacherDto Teacher { get; set; }
    }
}
