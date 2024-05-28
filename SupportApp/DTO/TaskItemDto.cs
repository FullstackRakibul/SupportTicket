namespace SupportApp.DTO
{
    public class TaskItemDto
    {
        public int? Id { get; set; }
        public string TaskItemTitle { get; set; }
        public string? AssignedTo { get; set; }
        public string? CreatedBy { get; set; }
        public int? Status { get; set; }
    }
}
