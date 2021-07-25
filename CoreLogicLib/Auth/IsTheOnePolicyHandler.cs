using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace CoreLogicLib.Auth
{
    public class IsTheOnePolicyHandler : AuthorizationHandler<IsTheOnePolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsTheOnePolicyRequirement requirement)
        {
            if (context.User.Identity.Name == requirement.EmailAddress)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
