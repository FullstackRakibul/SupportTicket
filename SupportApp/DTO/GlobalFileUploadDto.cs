using System.ComponentModel.DataAnnotations;

namespace SupportApp.DTO
{
	public class GlobalFileUploadDto
	{
		public int TicketId { get; set; }
		public string? FolderIndex { get; set; }
        public IFormFile? UploadedFile { get; set; }
        public string? FilePathUrl { get; set; }
	}
}
