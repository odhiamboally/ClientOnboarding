using CO.Domain.Contracts.Interfaces.Repositories;
using CO.Domain.Contracts.Specifications;
using CO.Domain.Entities;
using CO.Persistence.DataContext;
using CO.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.Contracts.Implementations.Repositories;

internal sealed class ClientRepository(DBContext context) : Repository<Client>(context), IClientRepository
{
    
}
