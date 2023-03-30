using TasksHandler.Models;

namespace TasksHandler.Services
{
    public interface IFilesRepository
    {
        Task Delete(string path, string container);
        Task<StoreResultFile[]> Store(string container, IEnumerable<IFormFile> files);
    }
}
