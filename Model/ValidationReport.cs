using System;
using System.Collections.Generic;

namespace Model
{
    public class OnModifiedEventArgs
    {
        public OnModifiedEventArgs(string text) { Text = text; }
        public string Text { get; }
    }

    public class ValidationReport
    {
        public ValidationReport()
        {

        }

        public delegate void OnModifiedEventHandler(object sender, OnModifiedEventArgs e);

        public event OnModifiedEventHandler OnModified;

        public DateTime LastModified { get; set; } //DateTime.Now when modified (through compilation function or just test added?), event and handler maybe?
        public List<ValidationTest> ValidationTests { get => ValidationTests; set
            {
                ValidationTests = value;

            }
        }
    }
}
