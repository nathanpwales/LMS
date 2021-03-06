using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public string UId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime Dob { get; set; }
        public string DeptAbbr { get; set; }

        public virtual Department DeptAbbrNavigation { get; set; }
    }
}
