using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Infrastruture.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description));
        }

        public static Result ToApplicationResult(this SignInResult result)
        {
            return result.Succeeded
                 ? Result.Success()
                 : Result.Failure(new List<string>());
        }
    }
}