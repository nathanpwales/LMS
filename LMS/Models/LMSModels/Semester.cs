using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Semester
    {
        public Semester()
        {
            Class = new HashSet<Class>();
        }

        public uint SemesterId { get; set; }
        public uint Year { get; set; }
        public string Season { get; set; }

        public virtual ICollection<Class> Class { get; set; }
    }
}
