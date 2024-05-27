namespace SupportApp.DTO
{
    public class TaskItemDto
    {
        public string TaskItemTitle { get; set; }
        public int? AssignedTo { get; set; }
        public string? CreatedBy { get; set; }
        public int? Status { get; set; }
    }
}
