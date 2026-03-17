using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Client;

public record ClientListRequest(

    Guid? Cursor = null,
    int PageSize = 50
);
