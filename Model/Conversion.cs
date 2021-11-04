using System;
using System.Collections.Generic;

namespace Model
{
    public class Conversion
    {
        #region Constructors
        public Conversion()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; } //Assigned by user in dialog popup.
        public DateTime DateModified { get; private set; } //DateTime.Now when configuration is updated.
        public List<Execution> Executions { get; set; } = new(); //Created on new entry in [dbo].[EXECUTIONS]
        public Execution ActiveExecution { get => Executions.Last(); } 
        public HealthReport HealthReport { get; set; }
        #endregion
    }
}
