namespace TrainingCenter.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Instructor { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public DateOnly? StartDate { get; set; }
}
