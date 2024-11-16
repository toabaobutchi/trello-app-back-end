using backend_apis.ApiModels.RequestModels;
using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.Repositories
{
    public interface IListRepository
    {
        Task<(List?, string)> CreateListAsync(CreateListModel model, ProjectCookie projectCookie);
        Task<string?> ChangeListOrderAsync(UpdateListOrder model, ProjectCookie projectCookie);
        Task<List?> UpdateAsync(UpdateListModel model, string listId, ProjectCookie projectCookie);
        Task<List?> DeleteAsync(string listId, ProjectCookie projectCookie);
    }
}