using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.Error
{
    /// <summary>
    /// The error message.
    /// Encapsulate exception
    /// </summary>
    public class ErrorMessage
    {

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
