namespace Dfe.Sww.Ecf.Frontend.HttpClients.MoodleService.Models.Courses;

public class EnrolUserRequest
{
    public MoodleRoles RoleId { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
}
