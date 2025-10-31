namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;

public class CreateCourseRequest
{
    public required string FullName { get; set; }
    public required string ShortName { get; set; }
    public required int CategoryId { get; set; }
}
