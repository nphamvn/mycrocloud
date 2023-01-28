namespace MockServer.Web.Models.Project;
public class ProjectIndexViewModel
{
    public ProjectSearchModel Search { get; set; } = new ProjectSearchModel();
    public ICollection<ProjectIndexItem> Projects { get; set; }
}

public class ProjectSearchModel
{
    public string Query { get; set; }
    public string Accessibility { get; set; }
    public string Sort { get; set; }
}