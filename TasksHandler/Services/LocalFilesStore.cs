using TasksHandler.Models;

namespace TasksHandler.Services
{
    public class LocalFilesStore : IFilesRepository
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalFilesStore(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task Delete(string path, string container)
        {
           if(string.IsNullOrWhiteSpace(path))
            {
                return Task.CompletedTask;
            }
            
            var fileName = Path.GetFileName(path);
            
            var fileDirectory = Path.Combine(env.WebRootPath, container, fileName);
            
            if(File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            
            return Task.CompletedTask;
        }

        public async Task<StoreResultFile[]> Store(string container, IEnumerable<IFormFile> files)
        {
            var tasks = files.Select(async file =>
            {
                var orignalFileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                string folder = Path.Combine(env.WebRootPath,container);

                string path = Path.Combine(folder, fileName);
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var content = ms.ToArray();
                    await File.WriteAllBytesAsync(path, content);
                }

                var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                var urlFile = Path.Combine(url,container, fileName).Replace("\\", "/");

                return new StoreResultFile
                {
                    URL = urlFile,
                    Title = orignalFileName
                };
            });

            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}
