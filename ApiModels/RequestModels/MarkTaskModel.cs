using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public sealed record MarkTaskModel
    {
        public bool? IsCompleted { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public bool? IsReOpened { get; set; }
        public void Update(ref Models.Task task)
        {
            // có cập nhật đánh dấu hoàn thành tác vụ
            if (IsCompleted != null)
            {
                task.IsCompleted = IsCompleted.Value;
                task.StatusChangeAt = DateTimeUtils.GetSeconds();
            }
            if (IsReOpened != null)
            {
                task.IsReOpened = IsReOpened.Value;
                if (IsReOpened.Value)
                {
                    task.IsCompleted = false;
                    task.StatusChangeAt = DateTimeUtils.GetSeconds();
                }
            }
            task.IsMarkedNeedHelp = IsMarkedNeedHelp ?? task.IsMarkedNeedHelp;
        }
    }
}