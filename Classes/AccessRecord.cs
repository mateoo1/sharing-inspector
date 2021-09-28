namespace Sharing_Inspector
{
    public class AccessRecord
    {
        public string Folder { get; set; }
        public string FullName { get; set; }
        public string AdGroupName { get; set; }
        public string SamAccountName { get; set; }
        public string Status { get; set; }
        public string FullPath { get; set; }
        public string groupType { get; set; }

        public AccessRecord() { }

        public AccessRecord(string Folder, string AdGroupName, string groupType, string FullName, string SamAccountName, string Status, string FullPath)
        {
            this.Folder = Folder;
            this.FullName = FullName;
            this.AdGroupName = AdGroupName;
            this.SamAccountName = SamAccountName;
            this.groupType = groupType;
            this.Status = Status;
            this.FullPath = FullPath;
        }
    }
}
