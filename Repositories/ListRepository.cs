using backend_apis.ApiModels;
using backend_apis.ApiModels.RequestModels;
using backend_apis.Data;
using backend_apis.Models;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class ListRepository : IListRepository
    {
        private readonly ProjectManagerDbContext _db;
        private readonly ChangeLogService _changeLogService;

        public ListRepository(ProjectManagerDbContext db, ChangeLogService changeLogService)
        {
            _db = db;
            _changeLogService = changeLogService;
        }

        private void CheckProjectCookie(ProjectCookie cookie)
        {
            if (!cookie.IsValid) throw new Exception("Project cookie is not valid");
        }

        public async Task<string?> ChangeListOrderAsync(UpdateListOrder model, ProjectCookie projectCookie)
        {
            try
            {
                CheckProjectCookie(projectCookie);

                var subjectList = await _db.Lists.AsNoTracking().FirstOrDefaultAsync(l => l.Id == model.SubjectId);
                if (subjectList == null)
                {
                    return null;
                }
                var objectList = await _db.Lists.AsNoTracking().FirstOrDefaultAsync(l => l.Id == model.ObjectId);
                if (objectList == null)
                {
                    return null;
                }

                var project = await _db.Projects.FindAsync(projectCookie.ProjectId);
                if (project == null)
                {
                    return null;
                }
                else
                {
                    var logDetail = LogDetail.Create(
                        subjectList.Id,
                        $"Change list order from {HtmlHelper.EntityName(subjectList.Name, "list-name")} to {HtmlHelper.EntityName(objectList.Name, "list-name")}",
                        ELogAction.Update,
                        EEntityType.List
                    );

                    var changeLog = Generator.CreateChangeLog(projectCookie, logDetail);


                    project.ChangeLogs.Add(changeLog);
                    project.ListOrder = model.NewListOrder;
                    await _db.SaveChangesAsync();

                    _ = _changeLogService.SendChangeLogAsync(projectCookie.ProjectId, changeLog);

                    return project.ListOrder;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<(List?, string)> CreateListAsync(CreateListModel model, ProjectCookie projectCookie)
        {
            try
            {
                var list = model.ToList();
                var project = await _db.Projects.FindAsync(projectCookie.ProjectId);
                if (project == null)
                {
                    return (null, "");
                }
                else
                {
                    list.ProjectId = projectCookie.ProjectId;
                    project.Lists.Add(list);

                    // cập nhật lại listOrder
                    string newListOrder;
                    if (string.IsNullOrEmpty(project.ListOrder))
                    {
                        newListOrder = list.Id;
                    }
                    else
                    {
                        newListOrder = project.ListOrder + "," + list.Id;
                    }
                    project.ListOrder = newListOrder;

                    var logDetail = LogDetail.Create(list.Id, $"Create a new list {HtmlHelper.EntityName(list.Name, "list-name")}", ELogAction.Create, EEntityType.List);

                    var changeLog = Generator.CreateChangeLog(projectCookie, logDetail);

                    project.ChangeLogs.Add(changeLog);

                    await _db.SaveChangesAsync();

                    _ = _changeLogService.SendChangeLogAsync(projectCookie.ProjectId, changeLog);

                    return (list, newListOrder);
                }
            }
            catch
            {
                return (null, "");
            }
        }

        public async Task<List?> DeleteAsync(string listId, ProjectCookie projectCookie)
        {
            try
            {
                var deletedList = await _db.Lists.Include(l => l.Tasks).FirstOrDefaultAsync(l => l.Id == listId);
                if (deletedList == null) return null;

                // đang có task trong list thì không xoá list
                if (deletedList.Tasks.Count > 0)
                {
                    return null;
                }
                var project = await _db.Projects.FindAsync(projectCookie.ProjectId);
                if (project == null) return null;

                var logDetail = LogDetail.Create(null, $"Delete list {HtmlHelper.EntityName(deletedList.Name, "list-name")}", ELogAction.Delete, EEntityType.List);

                var changeLog = Generator.CreateChangeLog(projectCookie, logDetail);

                project.ChangeLogs.Add(changeLog);
                project.Lists.Remove(deletedList);
                project.ListOrder = string.Join(',', project.ListOrder?.Split(',').Where(item => item != deletedList.Id));
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectCookie.ProjectId, changeLog);

                return deletedList;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List?> UpdateAsync(UpdateListModel model, string listId, ProjectCookie projectCookie)
        {
            try
            {
                var list = await _db.Lists.Include(l => l.Tasks).FirstOrDefaultAsync(l => l.Id == listId && l.ProjectId == projectCookie.ProjectId);
                if (list == null)
                {
                    return null;
                }
                list.Name = !string.IsNullOrEmpty(model.Name) ? model.Name : list.Name;
                list.Description = model.Description;
                if (model.WipLimit != null)
                {
                    list.WipLimit = list.Tasks.Count >= model.WipLimit ? list.Tasks.Count : model.WipLimit;
                }

                // ghi change log
                var logDetail = LogDetail.Create(null, $"Update list information", ELogAction.Update, EEntityType.List);
                var changeLog = Generator.CreateChangeLog(projectCookie, logDetail);

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectCookie.ProjectId, changeLog);

                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}