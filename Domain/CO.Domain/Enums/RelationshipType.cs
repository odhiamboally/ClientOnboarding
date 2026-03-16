using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CO.Domain.Enums;

public enum RelationshipType
{
    [Description("Director")]
    Director,

    [Description("Shareholder")]
    Shareholder,

    [Description("Signatory")]
    Signatory,

    [Description("Beneficial Owner")]
    BeneficialOwner
}
