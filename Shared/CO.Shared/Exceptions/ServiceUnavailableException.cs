using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Exceptions;

public class ServiceUnavailableException(string message = null!) : CustomException(message: message)
{
}
