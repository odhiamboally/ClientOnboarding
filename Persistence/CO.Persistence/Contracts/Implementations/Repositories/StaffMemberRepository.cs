using CO.Domain.Contracts.Interfaces.Repositories;
using CO.Domain.Entities;
using CO.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.Contracts.Implementations.Repositories;


internal sealed class StaffMemberRepository(DBContext context) : Repository<StaffMember>(context), IStaffMemberRepository
{

}

