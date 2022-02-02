using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DbModel.Entities
{
    /// <summary>
    /// Asignaturas
    /// </summary>
    [Table("Subjects")]
    public class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Semester { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Score> Scores { get; set; }
    }
}
