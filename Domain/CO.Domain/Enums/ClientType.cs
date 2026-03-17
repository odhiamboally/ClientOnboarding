using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CO.Domain.Enums;

public enum ClientType
{
    [Description("Individual")]
    Individual,

    [Description("Corporate")]
    Corporate,

    [Description("Institutional")]
    Institutional,

    [Description("SME")]
    SmallMediumEnterprise,

    [Description("Enterprise")]
    Enterprise
}
