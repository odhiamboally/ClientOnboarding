using CO.Application.Contracts.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Infrastructure.Contracts.Implementations.Services;

internal sealed class UserService : IUserService
{
    public string UserId => "1";
    public string UserName => "BRUSER1";
}
