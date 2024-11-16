using backend_apis.ApiModels.ResponseModels;

namespace backend_apis.Filters
{
    public class MemberProjectRole : ProjectRoleAttribute
    {
        public MemberProjectRole() : base(ContextResponse.Admin, ContextResponse.Owner, ContextResponse.Member)
        {}
    }
}