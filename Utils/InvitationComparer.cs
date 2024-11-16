using System.Diagnostics.CodeAnalysis;
using backend_apis.ApiModels;

namespace backend_apis.Utils
{
    public class InvitationComparer : IEqualityComparer<ExistedUserInvitationResult>
    {
        public bool Equals(ExistedUserInvitationResult? x, ExistedUserInvitationResult? y)
        {
            if (x == null || y == null) return false;
            return x.UserId == y.UserId;
        }

        public int GetHashCode([DisallowNull] ExistedUserInvitationResult obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}