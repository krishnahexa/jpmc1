using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Exceptions
{

    public class KeyNotFoundException : System.Exception
    {
        public KeyNotFoundException(string message) : base(message)
        { }
    }

}
