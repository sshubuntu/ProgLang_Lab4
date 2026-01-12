using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GradebookApp.Data;
using GradebookApp.Models;

namespace GradebookApp
{
    public class Gradebook
    {
        private readonly GradebookContext _context;
        
        public IQueryable<GradeRecord> GradeRecords => _context.GradeRecords;

        public Gradebook(GradebookContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public GradeRecord AddRecord(string studentName, string subjectName, int mark)
        {
            if (studentName is null) throw new ArgumentNullException(nameof(studentName));
            if (subjectName is null) throw new ArgumentNullException(nameof(subjectName));

            var normalizedStudent = studentName.Trim();
            var normalizedSubject = subjectName.Trim();

            if (string.IsNullOrWhiteSpace(normalizedStudent))
                throw new ArgumentException("Student name cannot be empty", nameof(studentName));
            if (string.IsNullOrWhiteSpace(normalizedSubject))
                throw new ArgumentException("Subject name cannot be empty", nameof(subjectName));
            if (mark < 0 || mark > 100)
                throw new ArgumentException("Mark must be between 0 and 100", nameof(mark));

            var record = new GradeRecord(normalizedStudent, normalizedSubject, mark);
            _context.GradeRecords.Add(record);
            _context.SaveChanges();
            return record;
        }

        public int DeleteRecord(string? studentName, string? subjectName)
        {
            var hasStudent = !string.IsNullOrWhiteSpace(studentName);
            var hasSubject = !string.IsNullOrWhiteSpace(subjectName);

            if (!hasStudent && !hasSubject)
                throw new ArgumentException("At least student name or subject name must be provided.");

            var normalizedStudent = hasStudent ? studentName!.Trim().ToLower() : null;
            var normalizedSubject = hasSubject ? subjectName!.Trim().ToLower() : null;

            IQueryable<GradeRecord> query = _context.GradeRecords;

            if (hasStudent)
            {
                query = query.Where(r => r.StudentName.Trim().ToLower() == normalizedStudent);
            }

            if (hasSubject)
            {
                query = query.Where(r => r.SubjectName.Trim().ToLower() == normalizedSubject);
            }

            var toDelete = query.ToList();
            if (!toDelete.Any())
            {
                return 0;
            }

            _context.GradeRecords.RemoveRange(toDelete);
            _context.SaveChanges();
            return toDelete.Count;
        }

        public List<GradeRecord> SearchBySubject(string subjectName)
        {
            if (subjectName is null) throw new ArgumentNullException(nameof(subjectName));

            var normalizedSubject = subjectName.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(normalizedSubject))
                throw new ArgumentException("Subject name cannot be empty", nameof(subjectName));

            var records = _context.GradeRecords
                .Where(r => r.SubjectName.Trim().ToLower() == normalizedSubject)
                .OrderBy(r => r.StudentName)
                .ToList();

            return records;
        }
        
        public List<GradeRecord> GetAllRecords()
        {
            return _context.GradeRecords
                .OrderBy(r => r.StudentName)
                .ThenBy(r => r.SubjectName)
                .ToList();
        }
        
        public Dictionary<string, object> GetSubjectStatistics(string subjectName)
        {
            if (subjectName is null) throw new ArgumentNullException(nameof(subjectName));

            var normalizedSubject = subjectName.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(normalizedSubject))
                throw new ArgumentException("Subject name cannot be empty", nameof(subjectName));

            var marks = _context.GradeRecords
                .Where(r => r.SubjectName.Trim().ToLower() == normalizedSubject)
                .Select(r => r.Mark)
                .ToList();

            if (!marks.Any())
            {
                return new Dictionary<string, object>
                {
                    { "Count", 0 },
                    { "Average", 0.0 },
                    { "Max", 0 },
                    { "Min", 0 }
                };
            }

            return new Dictionary<string, object>
            {
                { "Count", marks.Count },
                { "Average", marks.Average() },
                { "Max", marks.Max() },
                { "Min", marks.Min() }
            };
        }
    }
}
