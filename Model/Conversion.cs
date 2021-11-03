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
        public List<Execution> Executions { get; private set; } //Created on new entry in [dbo].[EXECUTIONS]
        public ValidationReport ValidationReport { get; private set; }
        public HealthReport HealthReport { get; private set; }
        #endregion
    }
}
