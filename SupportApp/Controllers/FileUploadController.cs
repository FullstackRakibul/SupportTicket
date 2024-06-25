using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository;
using SupportApp.Repository.IReposiroty;
using SupportApp.Service;
using System.Net.Mail;

namespace SupportApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileUploadController : ControllerBase
	{
		private readonly SupportAppDbContext _context;
        private readonly IGlobalFileUploadInterface _globalFileUploadInterface;

		public FileUploadController (SupportAppDbContext context, IGlobalFileUploadInterface globalFileUploadInterface)
		{
			_context = context;
            _globalFileUploadInterface = globalFileUploadInterface;
		}

		[HttpGet]
		public ActionResult Index()
		{
			try{

                return Ok("This controller is working Fine.");

            }
            catch(Exception ex) {
					Console.WriteLine(ex.ToString());
				return BadRequest(ex.ToString());
				}
			
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromForm] GlobalFileUploadDto globalFileUploadDto )
		{
			try {
                if (globalFileUploadDto == null || globalFileUploadDto.UploadedFile == null)
                {
                    return BadRequest("No file uploaded.");
                }

                //string folderPath = @"C:\inetpub\wwwroot\UplodedFiles";

                //root path for the uploaded file
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                //create folder if not exist
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                //sub path for the uploaded file
                string folderPath = Path.Combine(wwwrootPath, globalFileUploadDto.FilePathUrl ?? "uploads");
                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Get original file name and extension
                string originalFileName = globalFileUploadDto.UploadedFile.FileName;
                string fileExtension = Path.GetExtension(originalFileName);

                // Create custom file name with prefix, datetime, and postfix
                Random random = new Random();
                int randomNumber = random.Next(1000, 9999);
                string customFileName = $"supportApp_{DateTime.Now:yyyyMMddHHmmssfff}_{randomNumber}{fileExtension}";

                // Construct full file path
                string filePath = Path.Combine(folderPath, customFileName);

                // Save the uploaded file to the specified path
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    globalFileUploadDto.UploadedFile.CopyTo(stream);
                }

                // Insert into database
                try
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        return BadRequest("File path cannot be null or empty.");
                    }

                    var insertFileDataIntoDB = new GlobalFileUpload
                    {
                        TicketId = globalFileUploadDto.TicketId,
                        FolderIndex = globalFileUploadDto.FolderIndex,
                        IsActive = true,
                        UpdatedAt = DateTime.Now.AddHours(6),
                        CreatedAt = DateTime.Now.AddHours(6),
                        FilePathUrl = filePath,
                    };
                    _context.GlobalFileUpload.Add(insertFileDataIntoDB);
                    await _context.SaveChangesAsync();

                    return Ok("Upload File saved success...");
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception details
                    Console.WriteLine($"Error occurred while saving changes to the database: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return StatusCode(500, "An error occurred while saving changes to the database. Please try again later.");
                }
            }
            catch(Exception ex) {
                Console.WriteLine($"Error occurred while saving file: {ex.Message}");
                return BadRequest("Failed to save the uploaded file.");
            }
		}


        //::::::::::::: File upload Repository 

        [HttpPost]
        [Route("global-file-uplaod", Name ="globalFileUpload")]
        public async Task<IActionResult> CreateGlobalFile([FromForm] GlobalFileUploadDto globalFileUploadDto)
        {
            var fileUploadData = await _globalFileUploadInterface.UploadFile(globalFileUploadDto);

            return Ok(new ApiResponseDto<string>
            {
                Status = true,
                Message = "File upload success.",
                Data = fileUploadData.ToString()
            }) ;

        }



        //download file 

        [HttpGet("get-file-{ticketId}")]
        public async Task<IActionResult> DownloadFile(int ticketId)
        {
            var fileResult = await _globalFileUploadInterface.DownloadFileAsync(ticketId);

            if (fileResult == null)
            {
                return NotFound();
            }

            return fileResult;
        }

    }
}