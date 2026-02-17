using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static List<User> _users = new List<User>
    {
        new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
        new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow }
    };
    private static int _nextId = 3;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        _logger.LogInformation("GetAllUsers called - Retrieving {Count} users", _users.Count);
        return Ok(_users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        _logger.LogInformation("GetUserById called - Looking for user with ID {UserId}", id);
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }
        _logger.LogInformation("User with ID {UserId} found - Name: {UserName}", id, user.Name);
        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] User user)
    {
        _logger.LogInformation("CreateUser called - Attempting to create user with Name: {UserName}, Email: {UserEmail}", user.Name, user.Email);
        
        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
        {
            _logger.LogWarning("CreateUser failed - Invalid data: Name or Email is missing");
            return BadRequest(new { message = "Name and Email are required." });
        }

        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);

        _logger.LogInformation("User created successfully - ID: {UserId}, Name: {UserName}", user.Id, user.Name);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public ActionResult<User> UpdateUser(int id, [FromBody] User updatedUser)
    {
        _logger.LogInformation("UpdateUser called - Attempting to update user with ID {UserId}", id);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            _logger.LogWarning("UpdateUser failed - User with ID {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        if (string.IsNullOrWhiteSpace(updatedUser.Name) || string.IsNullOrWhiteSpace(updatedUser.Email))
        {
            _logger.LogWarning("UpdateUser failed - Invalid data: Name or Email is missing");
            return BadRequest(new { message = "Name and Email are required." });
        }

        _logger.LogInformation("Updating user {UserId} - Old Name: {OldName}, New Name: {NewName}", id, user.Name, updatedUser.Name);
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;

        _logger.LogInformation("User {UserId} updated successfully", id);
        return Ok(user);
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        _logger.LogInformation("DeleteUser called - Attempting to delete user with ID {UserId}", id);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            _logger.LogWarning("DeleteUser failed - User with ID {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        _users.Remove(user);
        _logger.LogInformation("User {UserId} deleted successfully - Name: {UserName}", id, user.Name);
        return NoContent();
    }
}