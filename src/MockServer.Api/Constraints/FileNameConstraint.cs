using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.Api.Constraints;

public class FileNameConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        throw new NotImplementedException();
    }
}
