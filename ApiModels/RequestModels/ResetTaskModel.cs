namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ResetTaskModel
    {
        public bool? ResetPriority { get; set; }
        public bool? ResetDescription { get; set; }
        public bool? ResetDueDate { get; set; }
        public bool? ResetStartDate { get; set; }

        public void Reset(ref Models.Task task)
        {
            // code cập nhật lại thông tin tác vụ
            //...
            if (ResetPriority != null && ResetPriority.Value == true)
            {
                task.Priority = null;
            }
            if (ResetDescription != null && ResetDescription.Value == true)
            {
                task.Description = null;
            }
            if (ResetDueDate != null && ResetDueDate.Value == true)
            {
                task.DueDate = null;
            }
            if (ResetStartDate != null && ResetStartDate.Value == true)
            {
                task.StartedAt = null;
            }
        }
    }
}