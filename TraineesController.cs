using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Data;
using TrainingCenter.Dtos;
using TrainingCenter.Models;

namespace TrainingCenter.Controllers;

[ApiController]
[Route("api/trainees")]
[Authorize]
public class TraineesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TraineesController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ActionResult<List<TraineeDto>>> GetAll()
    {
        var trainees = await _db.Trainees
            .OrderByDescending(t => t.Id)
            .Select(t => new TraineeDto(t.Id, t.Name, t.Phone, t.Email))
            .ToListAsync();
        return Ok(trainees);
    }

    [HttpPost]
    public async Task<ActionResult<TraineeDto>> Create(TraineeCreateDto dto)
    {
        var trainee = new Trainee { Name = dto.Name, Phone = dto.Phone, Email = dto.Email };
        _db.Trainees.Add(trainee);
        await _db.SaveChangesAsync();
        return Ok(new TraineeDto(trainee.Id, trainee.Name, trainee.Phone, trainee.Email));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TraineeCreateDto dto)
    {
        var trainee = await _db.Trainees.FindAsync(id);
        if (trainee is null) return NotFound();

        trainee.Name = dto.Name;
        trainee.Phone = dto.Phone;
        trainee.Email = dto.Email;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainee = await _db.Trainees.FindAsync(id);
        if (trainee is null) return NotFound();

        _db.Trainees.Remove(trainee);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
