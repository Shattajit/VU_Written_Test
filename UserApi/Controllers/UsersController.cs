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
    private const string UsersCachePrefix = "AllUsers";

    public UsersController(
        UserDbContext context,
        IMemoryCache cache,
        ILogger<UsersController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    private void ClearAllCache()
    {
        if (_cache is MemoryCache memoryCache)
        {
            memoryCache.Compact(1.0); // Remove 100% cache entries
            _logger.LogInformation("All cache entries cleared.");
        }
    }

    // =========================
    // Create Single User
    // =========================
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

        ClearAllCache();

        _logger.LogInformation("Created user with ID: {UserId}", user.Id);

        return CreatedAtAction(nameof(GetUsers), new { page = 1, pageSize = 100 }, user);
    }

    // =========================
    // Bulk Create Users
    // =========================
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

        var users = faker.Generate(10000);

        const int batchSize = 1000;
        for (int i = 0; i < users.Count; i += batchSize)
        {
            var batch = users.Skip(i).Take(batchSize);
            await _context.Users.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
        }

        ClearAllCache();

        var duration = (DateTime.UtcNow - startTime).TotalSeconds;

        return Ok(new
        {
            Message = "Successfully created 10,000 users",
            Count = users.Count,
            DurationSeconds = duration
        });
    }

    // =========================
    // Fetch Users with Pagination + Cache
    // =========================
    [HttpGet("fetch-users")]
    public async Task<ActionResult<object>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 100;
        if (pageSize > 1000) pageSize = 1000;

        var cacheKey = $"{UsersCachePrefix}_Page{page}_Size{pageSize}";

        if (!_cache.TryGetValue(cacheKey, out object? cachedResult))
        {
            _logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);

            var totalUsers = await _context.Users.CountAsync();

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

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(cacheKey, result, cacheOptions);

            return Ok(result);
        }

        _logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
        return Ok(cachedResult);
    }

    // =========================
    // Clear Cache Endpoint
    // =========================
    [HttpDelete("clear-cache")]
    public ActionResult ClearCache()
    {
        ClearAllCache();
        return Ok(new { Message = "Cache cleared successfully" });
    }

    // =========================
    // Get Total Count
    // =========================
    [HttpGet("users/count")]
    public async Task<ActionResult<object>> GetUserCount()
    {
        var count = await _context.Users.CountAsync();
        return Ok(new { TotalUsers = count });
    }
}
