using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Exceptions;

public class CreatingDuplicateException(string message = null!) : CustomException(message: message)
{
}
