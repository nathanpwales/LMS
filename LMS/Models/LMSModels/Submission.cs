using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string SId { get; set; }
        public uint AssnId { get; set; }
        public uint Score { get; set; }
        public string Contents { get; set; }
        public DateTime Time { get; set; }

        public virtual Assignment Assn { get; set; }
        public virtual Student S { get; set; }
    }
}
