using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public sealed class AccountController(IDbConnectionFactory _connection, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExists(registerDto.Email))
        {
            return BadRequest("Email taken");
        }
        
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = registerDto.Email,
            DisplayName = registerDto.DisplayName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        using var connection = await _connection.CreateConnectionAsync();

        const string sql = """
            Insert into users (Id, Email, DisplayName, PasswordHash, PasswordSalt)
            Values (@Id, @Email, @DisplayName, @PasswordHash, @PasswordSalt);
            """;

        await connection.ExecuteScalarAsync(sql, user);

        return user.ToDto(tokenService);
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        using var connection = await _connection.CreateConnectionAsync();

        const string sql = "Select * From users Where Lower(Email) = Lower(@Email);";

        var user = await connection.QuerySingleOrDefaultAsync<AppUser>(sql, new{ Email = loginDto.Email});

        if (user == null)
        {
            return Unauthorized("Invalid email address");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (var i = 0; i<computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid password");
            }
        }

        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        using var connection = await _connection.CreateConnectionAsync();

        const string sql = "Select 1 From users Where Lower(Email) = Lower(@Email);";

        var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Email = email });

        return result.HasValue;
    }
}
