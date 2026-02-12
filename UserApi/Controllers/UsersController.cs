using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Bogus;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.Controllers;

[ApiController]
[Route("api")]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UsersController> _logger;
    private const string UsersCacheKey = "AllUsers";

    public UsersController(
        UserDbContext context,
        IMemoryCache cache,
        ILogger<UsersController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Creates a single user with provided data
    /// </summary>
    [HttpPost("create-users")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User userData)
    {
        var user = new User
        {
            Name = userData.Name,
            Age = userData.Age,
            Email = userData.Email,
            TimeStamp = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // Invalidate cache since we added a new user
        _cache.Remove(UsersCacheKey);
        
        _logger.LogInformation("Created user with ID: {UserId}, Name: {Name}", user.Id, user.Name);
        
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
    }

    /// <summary>
    /// Creates 10,000 users with randomly generated data
    /// </summary>
    [HttpPost("create-bulk-users")]
    public async Task<ActionResult<object>> CreateBulkUsers()
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting bulk user creation...");

        var faker = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Age, f => f.Random.Int(18, 80))
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.TimeStamp, f => DateTime.UtcNow);

        // Generate 10,000 users
        var users = faker.Generate(10000);
        
        // Add in batches for better performance
        const int batchSize = 1000;
        for (int i = 0; i < users.Count; i += batchSize)
        {
            var batch = users.Skip(i).Take(batchSize);
            await _context.Users.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved batch {BatchNumber} of users", (i / batchSize) + 1);
        }
        
        // Invalidate cache since we added new users
        _cache.Remove(UsersCacheKey);
        
        var endTime = DateTime.UtcNow;
        var duration = (endTime - startTime).TotalSeconds;
        
        _logger.LogInformation("Bulk user creation completed in {Duration} seconds", duration);
        
        return Ok(new
        {
            Message = "Successfully created 10,000 users",
            Count = users.Count,
            DurationSeconds = duration
        });
    }

    /// <summary>
    /// Fetches all users from the database with caching and pagination
    /// </summary>
    [HttpGet("fetch-users")]
    public async Task<ActionResult<object>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
    {
        _logger.LogInformation("Fetching users... Page: {Page}, PageSize: {PageSize}", page, pageSize);

        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 100;
        if (pageSize > 1000) pageSize = 1000; // Max 1000 per page

        var cacheKey = $"{UsersCacheKey}_Page{page}_Size{pageSize}";

        // Try to get users from cache
        if (!_cache.TryGetValue(cacheKey, out object? cachedResult))
        {
            _logger.LogInformation("Cache miss - fetching from database");
            
            // Get total count
            var totalUsers = await _context.Users.CountAsync();
            
            // If not in cache, get from database with pagination
            var users = await _context.Users
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var result = new
            {
                Page = page,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                Users = users
            };
            
            // Set cache options
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                .SetPriority(CacheItemPriority.Normal);
            
            // Save data in cache
            _cache.Set(cacheKey, result, cacheEntryOptions);
            
            _logger.LogInformation("Cached page {Page} with {Count} users", page, users.Count);
            
            return Ok(result);
        }
        else
        {
            _logger.LogInformation("Cache hit - returning cached page {Page}", page);
            return Ok(cachedResult);
        }
    }

    /// <summary>
    /// Clears the cache (utility endpoint for testing)
    /// </summary>
    [HttpDelete("clear-cache")]
    public ActionResult ClearCache()
    {
        _cache.Remove(UsersCacheKey);
        _logger.LogInformation("Cache cleared");
        return Ok(new { Message = "Cache cleared successfully" });
    }

    /// <summary>
    /// Gets total user count
    /// </summary>
    [HttpGet("users/count")]
    public async Task<ActionResult<object>> GetUserCount()
    {
        var count = await _context.Users.CountAsync();
        return Ok(new { TotalUsers = count });
    }
}
