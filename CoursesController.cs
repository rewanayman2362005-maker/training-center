using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;
using TrainingCenter.Dtos;
using TrainingCenter.Models;

namespace TrainingCenter.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CoursesController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAll()
    {
        var courses = await _db.Courses
            .OrderByDescending(c => c.Id)
            .Select(c => new CourseDto(c.Id, c.Name, c.Instructor, c.Price, c.Capacity, c.StartDate))
            .ToListAsync();
        return Ok(courses);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(CourseCreateDto dto)
    {
        var course = new Course
        {
            Name = dto.Name,
            Instructor = dto.Instructor,
            Price = dto.Price,
            Capacity = dto.Capacity,
            StartDate = dto.StartDate
        };
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return Ok(new CourseDto(course.Id, course.Name, course.Instructor, course.Price, course.Capacity, course.StartDate));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CourseCreateDto dto)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is null) return NotFound();

        course.Name = dto.Name;
        course.Instructor = dto.Instructor;
        course.Price = dto.Price;
        course.Capacity = dto.Capacity;
        course.StartDate = dto.StartDate;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is null) return NotFound();

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
