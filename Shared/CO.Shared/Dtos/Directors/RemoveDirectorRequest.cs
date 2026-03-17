using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Directors;

public record RemoveDirectorRequest(Guid ClientId, Guid DirectorId);
