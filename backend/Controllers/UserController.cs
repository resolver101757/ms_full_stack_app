using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using UserAPI.Models;
using Microsoft.Extensions.Logging;
using System.IO;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private void LogAudit(string action, string details)
        {
            var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {action}: {details}";
            var logPath = Path.Combine("/app/audits", "user_audit.log");
            System.IO.File.AppendAllText(logPath, logEntry + Environment.NewLine);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var users = await connection.QueryAsync<User>("SELECT * FROM Users");
                LogAudit("GetAllUsers", $"Retrieved {users.Count()} users");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                LogAudit("GetAllUsers", $"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var user = await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
            LogAudit("GetUser", $"Retrieved user with ID: {id}");
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email); SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await connection.ExecuteScalarAsync<int>(sql, user);
            user.Id = id;
            LogAudit("CreateUser", $"Created user with ID: {id}");
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, user.Name, user.Email });
            LogAudit("UpdateUser", $"Updated user with ID: {id}");
            return affectedRows > 0 ? Ok(user) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
            LogAudit("DeleteUser", $"Deleted user with ID: {id}");
            return affectedRows > 0 ? NoContent() : NotFound();
        }
    }
}