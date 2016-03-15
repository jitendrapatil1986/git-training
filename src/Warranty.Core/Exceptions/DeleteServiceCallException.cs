using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Exceptions
{
    public class DeleteServiceCallException : ApplicationException
    {
        public DeleteServiceCallException(string message) : base(message + " Please contact the warranty service representative.") { }
    }
}
