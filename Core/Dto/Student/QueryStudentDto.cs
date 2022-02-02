using Core.Dto.Base;

namespace Core.Dto.Student
{
    public class QueryStudentDto: PagedQueryDto
    {
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
    }
}
