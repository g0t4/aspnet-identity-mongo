## Microsoft.AspNetCore.Identity.MongoDB

This is a MongoDB provider for the ASP.NET Core Identity framework.

What frameworks are targeted, with rationale:

- Microsoft.AspNetCore.Identity - supports net451 and netstandard1.3
- MongoDB.Driver v2.3 - supports net45 and netstandard1.5
- Thus, the lowest common denominators are net451 (of net45 and net451) and netstandard1.5 (of netstandard1.3 and netstandard1.5) 
- FYI net451 supports netstandard1.2, that's obviously too low for a single target

## Migrating from ASP.NET Identity 2.0


- roles names need to be normalized (user.roles)
	- Default uppercase - tell people if they customize this they have to deal with custom migration

- normalization by uppercase:
	- add IdentityRole.NormalizedName
	- add IdentityUser.NormalizedUserName, IdentityUser.NormalizedEmail
- LockoutEndDateUtc - type changed in code, but I think it is still the same in db

- Should't cause a problem, but FYI, IdentityUserLogin.ProviderDisplayName, I believe this was a change to Identity v2
