namespace Sharing_Inspector
{
    class AccessRecord
    {

        public string LocalPath;
        public string AdGroupName;
        public string SamAccountName;
        public string Status;

        public AccessRecord(string param1, string param2, string param3, string param4)
        {
            this.LocalPath = param1;
            this.AdGroupName = param2;
            this.SamAccountName = param3;
            this.Status = param4;
        }
    }
}
