namespace Core.Dto.Subject
{
    public class CreateOrEditSubjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Semester { get; set; }
        public int? TeacherId { get; set; }
    }
}
