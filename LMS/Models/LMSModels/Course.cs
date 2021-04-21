using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Class = new HashSet<Class>();
        }

        public uint CourseId { get; set; }
        public string DeptAbbr { get; set; }
        public uint Number { get; set; }
        public string Name { get; set; }

        public virtual Department DeptAbbrNavigation { get; set; }
        public virtual ICollection<Class> Class { get; set; }
    }
}
