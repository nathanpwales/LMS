using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategory = new HashSet<AssignmentCategory>();
            Enrolled = new HashSet<Enrolled>();
        }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Location { get; set; }
        public uint SemesterId { get; set; }
        public uint CourseId { get; set; }
        public string Professor { get; set; }
        public uint ClassId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual ICollection<AssignmentCategory> AssignmentCategory { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
    }
}
