namespace Core.Dto.Score
{
    public class CreateOrEditScoreDto
    {
        public string Description { get; set; }
        public double? Qualification { get; set; }
        public virtual int? StudentId { get; set; }
        public virtual int? SubjectId { get; set; }
        public virtual int? TeacherId { get; set; }
    }
}
