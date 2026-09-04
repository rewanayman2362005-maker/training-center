namespace TrainingCenter.Models;

public class Booking
{
    public int Id { get; set; }

    public int TraineeId { get; set; }
    public Trainee? Trainee { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateOnly? BookingDate { get; set; }

    // pending | confirmed | cancelled
    public string Status { get; set; } = "pending";
}
