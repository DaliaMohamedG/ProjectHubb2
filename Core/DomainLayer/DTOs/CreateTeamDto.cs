public class CreateTeamDto
{
    public string Name { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public string SupervisorId { get; set; }
    public List<string> MemberIds { get; set; } = new();
}