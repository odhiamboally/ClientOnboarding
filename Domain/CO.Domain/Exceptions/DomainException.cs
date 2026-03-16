using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message: message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

