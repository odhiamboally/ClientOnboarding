using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Domain.Entities;

public class StaffMember : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string StaffNumber { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;

    private StaffMember() { } // EF Core

    public static StaffMember Create(string fName, string lName, string staffNumber,string department) => new()
    {
        Id = Guid.CreateVersion7(),
        FirstName = fName,
        LastName = lName,
        StaffNumber = staffNumber,
        Department = department
    };
}
