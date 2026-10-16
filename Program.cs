using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RaceDayConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<PasswordHasher<AppUser>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();

app.MapGet("/", () => Results.Ok(new
{
    Message = "RaceDay API is running."
}));

// AUTHENTICATION

app.MapPost("/api/auth/register", async (
    RegisterRequest request,
    RaceDayDbContext db,
    PasswordHasher<AppUser> hasher) =>
{
    if (string.IsNullOrWhiteSpace(request.FirstName) ||
        string.IsNullOrWhiteSpace(request.LastName) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new
        {
            Message = "Please provide all required details."
        });
    }

    if (request.Password.Length < 8)
    {
        return Results.BadRequest(new
        {
            Message = "Password must contain at least 8 characters."
        });
    }

    string email = request.Email.Trim().ToLowerInvariant();

    if (await db.Users.AnyAsync(u => u.Email == email))
    {
        return Results.Conflict(new
        {
            Message = "That email address is already registered."
        });
    }

    string role = request.Role.Trim();

    if (!role.Equals("Organiser",
            StringComparison.OrdinalIgnoreCase) &&
        !role.Equals("Participant",
            StringComparison.OrdinalIgnoreCase))
    {
        return Results.BadRequest(new
        {
            Message = "Role must be Organiser or Participant."
        });
    }

    var user = new AppUser
    {
        FirstName = request.FirstName.Trim(),
        LastName = request.LastName.Trim(),
        Email = email,
        Phone = request.Phone?.Trim() ?? "",
        Role = role.Equals("Organiser",
            StringComparison.OrdinalIgnoreCase)
            ? "Organiser"
            : "Participant"
    };

    user.PasswordHash = hasher.HashPassword(
        user, request.Password);

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created("/api/profile/me", new
    {
        user.UserId,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role
    });
});

app.MapPost("/api/auth/login", async (
    LoginRequest request,
    RaceDayDbContext db,
    PasswordHasher<AppUser> hasher,
    HttpContext http) =>
{
    string email = request.Email.Trim().ToLowerInvariant();

    var user = await db.Users.FirstOrDefaultAsync(
        u => u.Email == email);

    if (user == null)
    {
        return Results.Unauthorized();
    }

    var verification = hasher.VerifyHashedPassword(
        user, user.PasswordHash, request.Password);

    if (verification == PasswordVerificationResult.Failed)
    {
        return Results.Unauthorized();
    }

    http.Session.SetInt32("UserId", user.UserId);
    http.Session.SetString("Role", user.Role);

    return Results.Ok(new
    {
        Message = "Login successful.",
        user.UserId,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role
    });
});

app.MapPost("/api/auth/logout", (HttpContext http) =>
{
    if (http.Session.GetInt32("UserId") == null)
    {
        return Results.Unauthorized();
    }

    http.Session.Clear();

    return Results.Ok(new
    {
        Message = "You have logged out."
    });
});

// PROFILE

app.MapGet("/api/profile/me", async (
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    var user = await db.Users.FindAsync(userId.Value);

    if (user == null)
        return Results.NotFound();

    return Results.Ok(new
    {
        user.UserId,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Phone,
        user.Role
    });
});

app.MapPut("/api/profile/me", async (
    UpdateProfileRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    var user = await db.Users.FindAsync(userId.Value);

    if (user == null)
        return Results.NotFound();

    user.FirstName = request.FirstName.Trim();
    user.LastName = request.LastName.Trim();
    user.Phone = request.Phone.Trim();

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        Message = "Profile updated.",
        user.UserId,
        user.FirstName,
        user.LastName,
        user.Phone
    });
});

// EVENT TYPES

app.MapGet("/api/eventtypes", async (RaceDayDbContext db) =>
{
    return Results.Ok(await db.EventTypes
        .Select(t => new
        {
            t.EventTypeId,
            t.TypeName,
            t.Description
        })
        .ToListAsync());
});

app.MapPost("/api/eventtypes", async (
    CreateEventTypeRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    if (http.Session.GetInt32("UserId") == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    if (string.IsNullOrWhiteSpace(request.TypeName))
        return Results.BadRequest("Type name is required.");

    var type = new EventType
    {
        TypeName = request.TypeName.Trim(),
        Description = request.Description?.Trim() ?? ""
    };

    db.EventTypes.Add(type);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/eventtypes/{type.EventTypeId}", type);
});

// EVENTS: BOTH ROLES CAN VIEW

app.MapGet("/api/events", async (RaceDayDbContext db) =>
{
    var events = await db.Events
        .Select(e => new
        {
            e.EventId,
            e.EventName,
            e.Description,
            e.EventDate,
            e.Location,
            e.Distance,
            e.EventTypeId,
            EventType = e.EventType!.TypeName,
            e.OrganiserId
        })
        .ToListAsync();

    return Results.Ok(events);
});

app.MapGet("/api/events/{id:int}", async (
    int id,
    RaceDayDbContext db) =>
{
    var item = await db.Events
        .Where(e => e.EventId == id)
        .Select(e => new
        {
            e.EventId,
            e.EventName,
            e.Description,
            e.EventDate,
            e.Location,
            e.Distance,
            e.EventTypeId,
            EventType = e.EventType!.TypeName,
            e.OrganiserId
        })
        .FirstOrDefaultAsync();

    return item == null
        ? Results.NotFound()
        : Results.Ok(item);
});

// EVENTS: ORGANISER'S OWN EVENTS

app.MapGet("/api/events/mine", async (
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var events = await db.Events
        .Where(e => e.OrganiserId == userId.Value)
        .ToListAsync();

    return Results.Ok(events);
});

app.MapPost("/api/events", async (
    CreateEventRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    if (string.IsNullOrWhiteSpace(request.EventName) ||
        string.IsNullOrWhiteSpace(request.Location) ||
        request.Distance <= 0 ||
        request.EventDate < DateTime.UtcNow.Date)
    {
        return Results.BadRequest(new
        {
            Message = "Check the event name, location, date and distance."
        });
    }

    bool typeExists = await db.EventTypes.AnyAsync(
        t => t.EventTypeId == request.EventTypeId);

    if (!typeExists)
        return Results.BadRequest("Invalid event type.");

    var item = new RaceEvent
    {
        EventName = request.EventName.Trim(),
        Description = request.Description?.Trim() ?? "",
        EventDate = request.EventDate,
        Location = request.Location.Trim(),
        Distance = request.Distance,
        EventTypeId = request.EventTypeId,
        OrganiserId = userId.Value
    };

    db.Events.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/api/events/{item.EventId}", item);
});

app.MapPut("/api/events/{id:int}", async (
    int id,
    UpdateEventRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var item = await db.Events.FindAsync(id);

    if (item == null)
        return Results.NotFound();

    if (item.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    bool typeExists = await db.EventTypes.AnyAsync(
        t => t.EventTypeId == request.EventTypeId);

    if (!typeExists)
        return Results.BadRequest("Invalid event type.");

    item.EventName = request.EventName.Trim();
    item.Description = request.Description?.Trim() ?? "";
    item.EventDate = request.EventDate;
    item.Location = request.Location.Trim();
    item.Distance = request.Distance;
    item.EventTypeId = request.EventTypeId;

    await db.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapDelete("/api/events/{id:int}", async (
    int id,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var item = await db.Events.FindAsync(id);

    if (item == null)
        return Results.NotFound();

    if (item.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    bool hasEnrolments = await db.Enrolments.AnyAsync(
        e => e.EventId == id);

    if (hasEnrolments)
    {
        return Results.Conflict(
            "An event with enrolments cannot be deleted.");
    }

    db.Events.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// CATEGORIES

app.MapGet("/api/categories/event/{eventId:int}", async (
    int eventId,
    RaceDayDbContext db) =>
{
    var categories = await db.Categories
        .Where(c => c.EventId == eventId)
        .Select(c => new
        {
            c.CategoryId,
            c.EventId,
            c.CategoryName,
            c.Description
        })
        .ToListAsync();

    return Results.Ok(categories);
});

app.MapPost("/api/categories", async (
    CreateCategoryRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var item = await db.Events.FirstOrDefaultAsync(
        e => e.EventId == request.EventId &&
             e.OrganiserId == userId.Value);

    if (item == null)
        return Results.NotFound(
            "Event not found or you do not own it.");

    if (string.IsNullOrWhiteSpace(request.CategoryName))
        return Results.BadRequest("Category name is required.");

    var category = new Category
    {
        EventId = request.EventId,
        CategoryName = request.CategoryName.Trim(),
        Description = request.Description?.Trim() ?? ""
    };

    db.Categories.Add(category);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/categories/{category.CategoryId}", category);
});

app.MapPut("/api/categories/{id:int}", async (
    int id,
    CreateCategoryRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var category = await db.Categories
        .Include(c => c.Event)
        .FirstOrDefaultAsync(c => c.CategoryId == id);

    if (category == null)
        return Results.NotFound();

    if (category.Event!.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    if (request.EventId != category.EventId)
        return Results.BadRequest(
            "A category cannot be moved to another event.");

    category.CategoryName = request.CategoryName.Trim();
    category.Description = request.Description?.Trim() ?? "";

    await db.SaveChangesAsync();

    return Results.Ok(category);
});

app.MapDelete("/api/categories/{id:int}", async (
    int id,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var category = await db.Categories
        .Include(c => c.Event)
        .FirstOrDefaultAsync(c => c.CategoryId == id);

    if (category == null)
        return Results.NotFound();

    if (category.Event!.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    if (await db.Enrolments.AnyAsync(e => e.CategoryId == id))
        return Results.Conflict(
            "This category has enrolments and cannot be deleted.");

    db.Categories.Remove(category);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// ENROLMENTS

app.MapPost("/api/enrolments", async (
    CreateEnrolmentRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Participant")
        return Results.StatusCode(403);

    var item = await db.Events.FindAsync(request.EventId);

    if (item == null)
        return Results.NotFound("Event not found.");

    if (item.EventDate < DateTime.UtcNow.Date)
        return Results.BadRequest(
            "You cannot enrol in an event that has already passed.");

    var category = await db.Categories.FirstOrDefaultAsync(
        c => c.CategoryId == request.CategoryId &&
             c.EventId == request.EventId);

    if (category == null)
        return Results.BadRequest(
            "The category does not belong to this event.");

    bool alreadyEnrolled = await db.Enrolments.AnyAsync(e =>
        e.ParticipantId == userId.Value &&
        e.EventId == request.EventId);

    if (alreadyEnrolled)
        return Results.Conflict(
            "You have already enrolled in this event.");

    var enrolment = new Enrolment
    {
        ParticipantId = userId.Value,
        EventId = request.EventId,
        CategoryId = request.CategoryId,
        EnrolmentDate = DateTime.UtcNow,
        Status = "Registered"
    };

    db.Enrolments.Add(enrolment);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/enrolments/{enrolment.EnrolmentId}",
        enrolment);
});

// Organisers view enrolments for their own events.

app.MapGet("/api/enrolments/event/{eventId:int}", async (
    int eventId,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    bool ownsEvent = await db.Events.AnyAsync(
        e => e.EventId == eventId &&
             e.OrganiserId == userId.Value);

    if (!ownsEvent)
        return Results.NotFound("Event not found.");

    var enrolments = await db.Enrolments
        .Where(e => e.EventId == eventId)
        .Select(e => new
        {
            e.EnrolmentId,
            e.EventId,
            e.ParticipantId,
            ParticipantName = e.Participant!.FirstName
                + " " + e.Participant.LastName,
            e.CategoryId,
            CategoryName = e.Category!.CategoryName,
            e.EnrolmentDate,
            e.Status
        })
        .ToListAsync();

    return Results.Ok(enrolments);
});

// Organisers update enrolment status for their own events.

app.MapPut("/api/enrolments/{id:int}/status", async (
    int id,
    UpdateEnrolmentStatusRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var enrolment = await db.Enrolments
        .Include(e => e.Event)
        .FirstOrDefaultAsync(e => e.EnrolmentId == id);

    if (enrolment == null)
        return Results.NotFound();

    if (enrolment.Event!.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    var allowedStatuses = new[]
    {
        "Registered",
        "Confirmed",
        "Cancelled",
        "Completed"
    };

    if (!allowedStatuses.Contains(
            request.Status,
            StringComparer.OrdinalIgnoreCase))
    {
        return Results.BadRequest("Invalid enrolment status.");
    }

    enrolment.Status = allowedStatuses.First(s =>
        s.Equals(request.Status,
            StringComparison.OrdinalIgnoreCase));

    await db.SaveChangesAsync();

    return Results.Ok(enrolment);
});

// RESULTS

app.MapPost("/api/results", async (
    CreateResultRequest request,
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Organiser")
        return Results.StatusCode(403);

    var enrolment = await db.Enrolments
        .Include(e => e.Event)
        .FirstOrDefaultAsync(
            e => e.EnrolmentId == request.EnrolmentId);

    if (enrolment == null)
        return Results.NotFound("Enrolment not found.");

    if (enrolment.Event!.OrganiserId != userId.Value)
        return Results.StatusCode(403);

    if (request.FinishTime < TimeSpan.Zero ||
        request.FinishPosition < 1)
    {
        return Results.BadRequest(
            "Finish time and position must be valid.");
    }

    bool resultExists = await db.Results.AnyAsync(
        r => r.EnrolmentId == request.EnrolmentId);

    if (resultExists)
        return Results.Conflict(
            "A result already exists for this enrolment.");

    var result = new RaceResult
    {
        EnrolmentId = request.EnrolmentId,
        FinishTime = request.FinishTime,
        FinishPosition = request.FinishPosition
    };

    db.Results.Add(result);

    enrolment.Status = "Completed";

    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/results/{result.ResultId}", result);
});

// Participants can view only their own results.

app.MapGet("/api/results/mine", async (
    HttpContext http,
    RaceDayDbContext db) =>
{
    int? userId = http.Session.GetInt32("UserId");

    if (userId == null)
        return Results.Unauthorized();

    if (http.Session.GetString("Role") != "Participant")
        return Results.StatusCode(403);

    var results = await db.Results
        .Where(r => r.Enrolment!.ParticipantId == userId.Value)
        .Select(r => new
        {
            r.ResultId,
            r.EnrolmentId,
            EventName = r.Enrolment!.Event!.EventName,
            r.FinishTime,
            r.FinishPosition
        })
        .ToListAsync();

    return Results.Ok(results);
});


app.Run();