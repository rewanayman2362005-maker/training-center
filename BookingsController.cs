using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;
using TrainingCenter.Dtos;
using TrainingCenter.Models;

namespace TrainingCenter.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;
    public BookingsController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAll()
    {
        var bookings = await _db.Bookings
            .Include(b => b.Trainee)
            .Include(b => b.Course)
            .OrderByDescending(b => b.Id)
            .Select(b => new BookingDto(
                b.Id,
                b.TraineeId,
                b.Trainee != null ? b.Trainee.Name : "متدرب محذوف",
                b.CourseId,
                b.Course != null ? b.Course.Name : "كورس محذوف",
                b.BookingDate,
                b.Status))
            .ToListAsync();
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(BookingCreateDto dto)
    {
        var trainee = await _db.Trainees.FindAsync(dto.TraineeId);
        var course = await _db.Courses.FindAsync(dto.CourseId);
        if (trainee is null || course is null)
            return BadRequest(new { message = "المتدرب أو الكورس غير موجود" });

        var booking = new Booking
        {
            TraineeId = dto.TraineeId,
            CourseId = dto.CourseId,
            BookingDate = dto.BookingDate,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "pending" : dto.Status
        };
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return Ok(new BookingDto(booking.Id, trainee.Id, trainee.Name, course.Id, course.Name, booking.BookingDate, booking.Status));
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, BookingStatusUpdateDto dto)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null) return NotFound();

        booking.Status = dto.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null) return NotFound();

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
