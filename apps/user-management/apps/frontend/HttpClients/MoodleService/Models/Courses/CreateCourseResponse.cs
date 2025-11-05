namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;

public class CreateCourseResponse
{
    public bool Successful { get; set; } = true;

    public int? Id { get; set; }
    public string? ShortName { get; set; }
}
