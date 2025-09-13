using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Exceptions
{
    public class EmployeeInputException : Exception
    {
        public EmployeeInputException(string message) : base(message) { }
    }
}
