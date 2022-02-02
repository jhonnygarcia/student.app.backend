using Core.Dto.Student;

namespace Core.Dto.Score
{
    public class BetterTenStudensDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public SubjectBetterTenDto[] Subjects { get; set; }
    }

    public class SubjectBetterTenDto
    {
        public string SubjectName { get; set; }
        public SemesterBetterTenDto[] Semesters { get; set; }
    }

    public class SemesterBetterTenDto
    {
        public string Semester { get; set; }
        public QualificationBetterTen[] Students { get; set; }
    }

    public class QualificationBetterTen : StudentDto
    {
        public double Score { get; set; }
    }
}
