using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Common.Exceptions
{
    public class ValidationException2: Exception
{
    public ValidationException2(string message) : base(message) { }
}
}