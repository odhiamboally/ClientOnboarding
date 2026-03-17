using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CO.Domain.Enums;

public enum IdentificationType
{
    [Description("Certificate of Incorporation")]
    CertificateOfIncorporation,

    [Description("Tax Identification Number")]
    TIN,

    [Description("Business License")]
    BusinessLicense,

    [Description("Company Registration Certificate")]
    CompanyRegistrationCertificate
}
