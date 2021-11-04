using System;
using System.Collections.Generic;

namespace Model
{
    public class ValidationReport
    {
        public ValidationReport()
        {

        }

        public DateTime LastModified { get; set; } //DateTime.Now when modified (through compilation function or just test added?), event and handler maybe?
        public List<ValidationTest> ValidationTests { get; set; } = new();
    }
}
