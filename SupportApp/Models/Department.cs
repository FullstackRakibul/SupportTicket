
namespace SupportApp.Models;

public class Department
{
    public int Id { get; set; }
    public string DepartmentName { get; set; }=String.Empty;
    public int? DepartmentCategoryId { get; set; }
    public string? Note { get; set; }
    public byte Status { get; set; } = 1;

    public ICollection<Target> Targets { get; set; } = new List<Target>();

}