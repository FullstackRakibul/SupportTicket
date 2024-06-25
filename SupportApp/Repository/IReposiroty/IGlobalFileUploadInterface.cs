using Microsoft.AspNetCore.Mvc;
using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Repository.IReposiroty
{
    public interface IGlobalFileUploadInterface
    {
        Task<GlobalFileUpload> UploadFile(GlobalFileUploadDto globalFileUploadDto);
        Task<FileResult> DownloadFileAsync(int trackId);
    }
}
