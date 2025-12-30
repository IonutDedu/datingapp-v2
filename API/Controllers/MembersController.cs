using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace API.Controllers
{
    public sealed class MembersController(IDbConnectionFactory _connection) : BaseApiController
    {
       [HttpGet]
       public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers(){

           using var connection = await _connection.CreateConnectionAsync();

           const string sql = "Select * from users;";

           var members = (await connection.QueryAsync<AppUser>(sql)).ToList();

           return members;
       }

       [Authorize]
       [HttpGet("{id}")] //localhost:5001/api/members/bod-id
       public async Task<ActionResult<AppUser>> GetMember(string id)
        {
           using var connection = await _connection.CreateConnectionAsync();

           const string sql = "Select * from users Where Id = @Id;";

           var member = await connection.QueryFirstOrDefaultAsync<AppUser>(sql, new {Id = id});

           if (member == null )
            {
                return NotFound();
            }

           return member;
        }
    }
}
