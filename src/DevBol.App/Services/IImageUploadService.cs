using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevBol.Presentation.Services
{
    public interface IImageUploadService
    {
        public Task<bool> UploadArquivo(ModelStateDictionary modelstate, 
                                        IFormFile arquivo, string imgPrefixo);
    }
}