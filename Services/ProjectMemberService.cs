using backend_apis.ApiModels;

namespace backend_apis.Services
{
    public class ProjectMemberService
    {
        public SortedDictionary<string, List<MemberConnection>> ConnectionFactory { get; set; } = [];
        public void Add(string projectId, MemberConnection memberConnection)
        {
            var project = ConnectionFactory.GetValueOrDefault(projectId);
            if (project == null)
            {
                ConnectionFactory.Add(projectId, [memberConnection]);
            }
            else
            {
                project.Add(memberConnection);
            }
            ConnectionFactory.Add(projectId, project);
        }
    }
}