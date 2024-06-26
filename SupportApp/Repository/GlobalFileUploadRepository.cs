using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;

namespace SupportApp.Repository
{
    public class GlobalFileUploadRepository:IGlobalFileUploadInterface
    {
        private readonly SupportAppDbContext _supportAppDbContext ;
        public GlobalFileUploadRepository(SupportAppDbContext supportAppDbContext)
        {
            _supportAppDbContext = supportAppDbContext;
        }

        public async Task<GlobalFileUpload> UploadFile(GlobalFileUploadDto globalFileUploadDto)
        {
            try
            {
                if (globalFileUploadDto == null || globalFileUploadDto.UploadedFile == null)
                {
                    return null;
                }

                // File upload logic
                string filePath = await SaveFile(globalFileUploadDto);

                // Database insertion logic
                var insertFileDataIntoDB = new GlobalFileUpload
                {
                    TicketId = globalFileUploadDto.TicketId,
                    FolderIndex = globalFileUploadDto.FolderIndex,
                    IsActive = true,
                    UpdatedAt = DateTime.Now.AddHours(6),
                    CreatedAt = DateTime.Now.AddHours(6),
                    FilePathUrl = filePath,
                };

                _supportAppDbContext.GlobalFileUpload.Add(insertFileDataIntoDB);
                await _supportAppDbContext.SaveChangesAsync();

                return insertFileDataIntoDB;              
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error occurred during file upload: {ex.Message}");
                return null;
            }
        }

        private async Task<string> SaveFile(GlobalFileUploadDto globalFileUploadDto)
        {
            //root path for the uploaded file
            //string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string wwwrootPath = @"D:\ApplicationFiles\supportDesk";

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

            return filePath;
        }





        //download file 

        public async Task<string> GetFileDownloadLink(int trackId)
        {
            try
            {
                var fileDownloadLink = await _supportAppDbContext.GlobalFileUpload
                    .Where(data => data.TicketId == trackId)
                    .FirstOrDefaultAsync();

                return fileDownloadLink?.FilePathUrl ?? "null";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<FileResult> DownloadFileAsync(int trackId)
        {
            var filePath = await GetFileDownloadLink(trackId);

            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                return null;
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            var mimeType = GetMimeType(fileExtension);
            var fileName = Path.GetFileName(filePath);

            return new FileStreamResult(memory, mimeType)
            {
                FileDownloadName = fileName
            };
        }

        private string GetMimeType(string extension)
        {
            switch (extension)
            {
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/vnd.ms-word";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".csv": return "text/csv";
                default: return "application/octet-stream";
            }
        }

    }
}
