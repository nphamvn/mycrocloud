using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.ReverseProxyServer.Constraints;

public class DateTimeConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        throw new NotImplementedException();
    }
}
