using Microsoft.AspNetCore.Identity;
using StarCellar.Api.Data;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using StarCellar.Api.Utils;
using static Microsoft.AspNetCore.Http.Results;

namespace StarCellar.Api.Handlers
{
    public static class Users
    {
        internal static async Task<IResult> SignUpAsync(
            AppDbContext appContext, 
            UserManager<User> userManager, 
            UserCreateDTO user
            )
        {
            if (user is null) return BadRequest();
            if (!MiniValidator.TryValidate(user, out var errors)) return BadRequest(errors);

            if (userManager.Users.Any(u => u.Email == user.Email))
                return Conflict("Invalid `email`: A user with this email address already exists.");

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                user.Username = string.Join('_', user.FullName.Split(' ')).ToLower();
                if (userManager.Users.Any(u => u.UserName == user.Username))
                    user.Username = user.Username + '_' + Guid.NewGuid().ToString("N").Substring(0, 4);
            }
            else if (userManager.Users.Any(u => u.UserName == user.Username))
                return Conflict("Invalid `username`: A user with this username already exists.");

            var newUser = new User(user);
            var result = await userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Created($"/users/{newUser.Id}", newUser);
        }

        internal static async Task<IResult> SignInAsync
        (
            UserManager<User> userManager,
            TokenGenerator tokenGenerator,
            TokenDbContext tokenContext,
            UserLoginDTO credentials,
            HttpResponse response
        )
        {
            if (credentials is null) return BadRequest();
            if (!MiniValidator.TryValidate(credentials, out var errors)) return BadRequest(errors);

            var isUsingEmailAddress = EmailAddressValidator.TryValidate(credentials.Login, out var _);
            var user = isUsingEmailAddress
                ? await userManager.FindByEmailAsync(credentials.Login)
                : await userManager.FindByNameAsync(credentials.Login);
            if (user is null) return NotFound("User with this email address not found.");

            var result = await userManager.CheckPasswordAsync(user, credentials.Password);
            if (!result) return BadRequest("Incorrect password.");

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var (refreshTokenId, refreshToken) = tokenGenerator.GenerateRefreshToken();

            await tokenContext.Tokens.AddAsync(new Token { Id = refreshTokenId, UserId = user.Id });
            await tokenContext.SaveChangesAsync();

            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true,
                MaxAge = new TimeSpan(1, 0, 0, 0),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(accessToken);
        }

        internal static async Task<IResult> RefreshTokenAsync
        (
            HttpRequest request,
            HttpResponse response,
            TokenDbContext tokenContext,
            TokenValidator tokenValidator,
            TokenGenerator tokenGenerator,
            AppDbContext appContext
        )
        {
            var refreshToken = request.Cookies["refresh_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Please include a refresh token in the request.");

            var tokenIsValid = tokenValidator.TryValidate(refreshToken, out var tokenId);
            if (!tokenIsValid) return BadRequest("Invalid refresh token.");

            var token = await tokenContext.Tokens.Where(token => token.Id == tokenId).FirstOrDefaultAsync();
            if (token is null) return BadRequest("Refresh token not found.");

            var user = await appContext.Users.Where(u => u.Id == token.UserId).FirstOrDefaultAsync();
            if (user is null) return BadRequest("User not found.");

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var (newRefreshTokenId, newRefreshToken) = tokenGenerator.GenerateRefreshToken();

            tokenContext.Tokens.Remove(token);
            await tokenContext.Tokens.AddAsync(new Token { Id = newRefreshTokenId, UserId = user.Id });
            await tokenContext.SaveChangesAsync();

            response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
                IsEssential = true,
                MaxAge = new TimeSpan(1, 0, 0, 0),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(accessToken);
        }

        internal static async Task<IResult> GetProfileAsync(
            IHttpContextAccessor httpContextAccessor,
            AppDbContext appContext)
        {
            if (!UserClaimsValidator.TryValidate(httpContextAccessor.HttpContext?.User, out var user, out var errMsg))
                return BadRequest(errMsg);

            var dbUser = await appContext.Users.FindAsync(user.Id);
            return dbUser == null ? NotFound() : Ok(dbUser);

        }

        internal static async Task<IResult> SignOutAsync
        (
            HttpRequest request,
            HttpResponse response,
            TokenDbContext tokenContext,
            TokenValidator tokenValidator
        )
        {
            var refreshToken = request.Cookies["refresh_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Please include a refresh token in the request.");

            var tokenIsValid = tokenValidator.TryValidate(refreshToken, out var tokenId);
            if (!tokenIsValid) return BadRequest("Invalid refresh token.");

            var token = await tokenContext.Tokens.Where(token => token.Id == tokenId).FirstOrDefaultAsync();
            if (token is null) return BadRequest("Refresh token not found.");

            tokenContext.Tokens.Remove(token);
            await tokenContext.SaveChangesAsync();

            response.Cookies.Delete("refresh_token");
            return NoContent();
        }
    }
}
