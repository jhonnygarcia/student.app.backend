using Core.Dto.Student;

namespace Core.Dto.Score
{
    public class BetterFiveStudensDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public SemesterBetterFive[] Semesters { get; set; }
    }

    public class SemesterBetterFive
    {
        public string Semester { get; set; }
        public QualificationBetterFive[] Students { get; set; }
    }

    public class QualificationBetterFive : StudentDto
    {
        public double Score { get; set; }
    }
}
