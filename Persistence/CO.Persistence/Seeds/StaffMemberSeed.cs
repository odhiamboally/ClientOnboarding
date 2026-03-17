using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.Seeds;

public static class StaffMemberSeed
{
    private static readonly DateTimeOffset _seededAt = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    // Fixed GUIDs so migrations are deterministic
    private static readonly Guid Rm001 = new("11111111-0000-0000-0000-000000000001");
    private static readonly Guid Rm002 = new("11111111-0000-0000-0000-000000000002");
    private static readonly Guid Rm003 = new("11111111-0000-0000-0000-000000000003");
    private static readonly Guid Rm004 = new("11111111-0000-0000-0000-000000000004");
    private static readonly Guid Rm005 = new("11111111-0000-0000-0000-000000000005");
    private static readonly Guid Rm006 = new("11111111-0000-0000-0000-000000000006");

    public static IEnumerable<object> GetSeedData()
    {
        // Using anonymous objects because private setters
        // prevent EF from using the factory method in seed context
        return
        [
            new
            {
                Id = Rm001,
                FirstName = "James",
                LastName = "Ochieng",
                StaffNumber = "RM001",
                Department = "Corporate Banking",
                Email = "j.ochieng@bank.co.tz",

                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
                
            },
            new
            {
                Id = Rm002,
                FirstName = "Amina",
                LastName = "Hassan",
                StaffNumber = "RM002",
                Department = "Corporate Banking",
                Email = "a.hassan@bank.co.tz",
                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
            },
            new
            {
                Id = Rm003,
                FirstName = "Peter",
                LastName = "Mwangi",
                StaffNumber = "RM003",
                Department = "SME Banking",
                Email = "p.mwangi@bank.co.tz",
                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
            },
            new
            {
                Id = Rm004,
                FirstName = "Grace",
                LastName = "Wanjiku",
                StaffNumber = "RM004",
                Department = "Corporate Banking",
                Email = "g.wanjiku@bank.co.tz",
                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
            },
            new
            {
                Id = Rm005,
                FirstName = "David",
                LastName = "Kamau",
                StaffNumber = "RM005",
                Department = "Retail Banking",
                Email = "d.kimani@bank.co.tz",
                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
            },
            new
            {
                Id = Rm006,
                FirstName = "Fatuma",
                LastName = "Ally",
                StaffNumber = "RM006",
                Department = "SME Banking",
                Email = "f.ally@bank.co.tz",
                CreatedAt   = _seededAt,
                CreatedBy   = "SYSTEM",
                UpdatedAt   = (DateTimeOffset?)null,
                UpdatedBy   = (string?)null,
                IsDeleted   = false,
            }
        ];
    }


}
