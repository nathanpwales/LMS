using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submission = new HashSet<Submission>();
        }

        public uint AssnId { get; set; }
        public uint AssnCategoryId { get; set; }
        public string Name { get; set; }
        public uint MaxPoints { get; set; }
        public string Contents { get; set; }
        public DateTime DueDate { get; set; }

        public virtual AssignmentCategory AssnCategory { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
