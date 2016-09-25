## Migrating from ASP.NET Identity 2.0

- roles names need to be normalized (user.roles)
	- Default uppercase - tell people if they customize this they have to deal with custom migration

- normalization by uppercase:
	- add IdentityRole.NormalizedName
	- add IdentityUser.NormalizedUserName, IdentityUser.NormalizedEmail
- LockoutEndDateUtc - type changed in code, but I think it is still the same in db

- Should't cause a problem, but FYI, UserLoginInfo.ProviderDisplayName was added
