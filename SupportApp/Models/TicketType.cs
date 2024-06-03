namespace SupportApp.Models;

public class TicketType
{
    public int Id { get; set; }
    public string TypeName { get; set; } = String.Empty;
    public byte Status { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
}