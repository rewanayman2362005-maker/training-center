namespace TrainingCenter.Dtos;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, string Username);

public record CourseDto(int Id, string Name, string? Instructor, decimal Price, int Capacity, DateOnly? StartDate);
public record CourseCreateDto(string Name, string? Instructor, decimal Price, int Capacity, DateOnly? StartDate);

public record TraineeDto(int Id, string Name, string? Phone, string? Email);
public record TraineeCreateDto(string Name, string? Phone, string? Email);

public record BookingDto(int Id, int TraineeId, string TraineeName, int CourseId, string CourseName, DateOnly? BookingDate, string Status);
public record BookingCreateDto(int TraineeId, int CourseId, DateOnly? BookingDate, string Status);
public record BookingStatusUpdateDto(string Status);
