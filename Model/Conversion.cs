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
        public string Name { get; set; }
        public DateTime DateModified { get; private set; }
        public List<Execution> Executions { get; private set; }
        public ValidationReport ValidationReport { get; private set; }
        #endregion
    }
}
