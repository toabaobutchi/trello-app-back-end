using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{

    public class InvitationRepository : IInvitationRepository
    {
        private readonly ProjectManagerDbContext _db;

        public InvitationRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        private async Task<InvitationResult?> AcceptProjectInvitationAsync(string invitationId)
        {
            try
            {
                var invitation = await _db.ProjectInvitations
                    .Include(i => i.Project)
                    .FirstOrDefaultAsync(i => i.Id == invitationId);
                if (invitation == null)
                    return null;
                var user = await _db.Users.Select(u => new { u.Id, u.Email }).FirstOrDefaultAsync(u => u.Email == invitation.InvitedEmail);
                if (user == null)
                {
                    return null;
                }
                var assignment = new Assignment()
                {
                    ProjectId = invitation.ProjectId,
                    UserId = user.Id,
                    Id = TextUtils.CreateAssignmentId(invitation.ProjectId, user.Id),
                    Permission = invitation.Permission,
                    JoinAt = DateTimeUtils.GetSeconds(),
                };

                var invitationResult = InvitationResult.Create(invitation, invitation.Project, true, assignment.Id);

                await _db.Assignments.AddAsync(assignment);
                var entity = _db.ProjectInvitations.Remove(invitation);
                await _db.SaveChangesAsync();
                return invitationResult;
            }
            catch
            {
                return null;
            }
        }
        private async Task<InvitationResult?> RejectProjectInvitationAsync(string invitationId)
        {
            try
            {
                var invitation = await _db.ProjectInvitations.FindAsync(invitationId);
                if (invitation == null)
                    return null;
                var invitationResult = InvitationResult.Create(invitation, null, false, null);
                var entity = _db.ProjectInvitations.Remove(invitation);
                await _db.SaveChangesAsync();
                return invitationResult;
            }
            catch
            {
                return null;
            }
        }
        public async Task<InvitationResult?> HandleProjectInvitationAsync(string invitationId, string action)
        {
            try
            {
                switch (action)
                {
                    case "accept":
                        return await AcceptProjectInvitationAsync(invitationId);
                    case "reject":
                        return await RejectProjectInvitationAsync(invitationId);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}