using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradebookApp.Models
{
    public class GradeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public int Mark { get; set; }
     
        public GradeRecord(string studentName, string subjectName, int mark)
        {
            StudentName = studentName;
            SubjectName = subjectName;
            Mark = mark;
        }
        
        public string GetStudentName() => StudentName;

        public void SetStudentName(string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName)) throw new ArgumentException("Student name cannot be empty");
            StudentName = studentName;
        }
        
        public string GetSubjectName() => SubjectName;
        public void SetSubjectName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName)) throw new ArgumentException("Subject name cannot be empty");
            SubjectName = subjectName;
        }
        public int GetMark() => Mark;
        
        public void SetMark(int mark)
        {
            if (mark < 0 || mark > 100) throw new ArgumentException("Mark must be between 0 and 100");
            Mark = mark;
        }

        public override string ToString()
        {
            return $"{StudentName} - {SubjectName}: {Mark}";
        }
    }
}
