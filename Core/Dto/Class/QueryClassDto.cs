using Core.Dto.Base;

namespace Core.Dto.Class
{
    public class QueryClassDto : PagedQueryDto
    {
        public string StudentName { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
    }
}
