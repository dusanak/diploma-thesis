using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace DiplomaThesis.Client;

public class UserWithRolesFactory
    : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public UserWithRolesFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)user.Identity;

            var roleClaims = identity.FindAll(identity.RoleClaimType).ToArray();
            if (!roleClaims.Any()) return user;

            foreach (var existingClaim in roleClaims)
            {
                identity.RemoveClaim(existingClaim);
            }

            var rolesElem = account.AdditionalProperties[identity.RoleClaimType];
            if (rolesElem is not JsonElement roles) return user;

            if (roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray())
                {
                    identity.AddClaim(new Claim(options.RoleClaim, role.GetString()!));
                }
            }
            else
            {
                identity.AddClaim(new Claim(options.RoleClaim, roles.GetString()!));
            }

            return user;
        }

        return user;
    }
}