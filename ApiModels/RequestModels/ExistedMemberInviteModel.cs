namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ExistedMemberInviteModel
    {
        public string ProjectId { get; set; } = null!;
        public ProjectSelectOptions Options { get; set; } = null!;

        public KeyValuePair<string, ProjectSelectOptions> GetKeyValuePair()
        {
            return new KeyValuePair<string, ProjectSelectOptions>(ProjectId, Options);
        }
    }
    public sealed record ProjectSelectOptions
    {
        public bool? IsAll { get; set; }
        public IEnumerable<SelectedAssignment> SelectedAssignments { get; set; } = [];
    }
    public sealed record SelectedAssignment
    {
        public string Id { get; set; } = null!;
        public string Permission { get; set; } = null!;
    }
}