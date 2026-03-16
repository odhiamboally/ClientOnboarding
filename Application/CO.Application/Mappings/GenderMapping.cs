using CO.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Mappings;

public static class GenderMapping
{
    public static GenderEnum MapFromString(this string gender)
    {
        return gender switch
        {
            "Male" => GenderEnum.Male,
            "Female" => GenderEnum.Female,
            _ => GenderEnum.Other
        };
    }

    public static string MapToString(this GenderEnum gender)
    {
        return gender switch
        {
            GenderEnum.Male => "Male",
            GenderEnum.Female => "Female",
            _ => "Other"
        };
    }
}
