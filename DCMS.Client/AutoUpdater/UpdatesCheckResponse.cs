namespace DCMS.Client.AutoUpdater
{
    public class UpdatesCheckResponse
    {
        public UpdateInfo UpdateInfo { get; set; }
        public bool IsNewVersionAvailable { get; set; }

        public UpdatesCheckResponse(bool isNewVersionAvailable, UpdateInfo updateInfo)
        {
            IsNewVersionAvailable = isNewVersionAvailable;
            UpdateInfo = updateInfo;
        }
    }
}
