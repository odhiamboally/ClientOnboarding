using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CO.Domain.Enums;

public enum ClientStatus
{
    [Description("Draft")]
    Draft,

    [Description("Active")]
    Active,

    [Description("Suspended")]
    Suspended,

    [Description("Closed")]
    Closed,

    [Description("Pending Approval")]
    PendingApproval
}
