namespace RaceDay.Api.DTOs;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role,
    string Phone);

public record LoginRequest(
    string Email,
    string Password);

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string Phone);

public record CreateEventRequest(
    string EventName,
    string Description,
    DateTime EventDate,
    string Location,
    double Distance,
    int EventTypeId);

public record UpdateEventRequest(
    string EventName,
    string Description,
    DateTime EventDate,
    string Location,
    double Distance,
    int EventTypeId);

public record CreateEventTypeRequest(
    string TypeName,
    string Description);

public record CreateCategoryRequest(
    int EventId,
    string CategoryName,
    string Description);

public record CreateEnrolmentRequest(
    int EventId,
    int CategoryId);

public record UpdateEnrolmentStatusRequest(
    string Status);

public record CreateResultRequest(
    int EnrolmentId,
    TimeSpan? FinishTime,
    int? FinishPosition);