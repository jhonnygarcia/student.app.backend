using Core.Dto.Subject;

namespace Core.Dto.Class
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SubjectDto Subject { get; set; }
    }
}
