using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Control.Events
{
    public class SanityCheckException : Exception
    {
        public SanityCheckException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
