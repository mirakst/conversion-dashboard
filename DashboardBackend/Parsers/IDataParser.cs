using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DashboardBackend
{
    ///<summary>
    /// These classes would make more sense in the following setup:<br/>
    ///   - The Controller triggers a method in the DatabaseHandler when an update timer is invoked<br/>
    ///   - The DataHandler sends the request on to the Database (SqlDatabase?)<br/>
    ///   - SqlDatabase retrieves the raw data from the state database and returns it<br/>
    ///   - The DataHandler sends the data into a specific Parser(TM) class, which returns some new data and updates existing data as a side effect.<br/>
    ///   - The new data is then added to the Conversion
    ///</summary>
    public interface IDataParser<TInput, TOutput> 
        where TInput : class
        where TOutput : class
    {
        TOutput Parse(IList<TInput> data);
    }
}
