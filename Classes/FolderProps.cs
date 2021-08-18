using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;

namespace Sharing_Inspector
{
    class FoldersProps
    {
        private string[] foldersTextBox;

        public FoldersProps(string provideFoldersIntextBox)
        {
            string foldersOneLine = Regex.Replace(provideFoldersIntextBox, @"\r\n?|\n", "");
            this.foldersTextBox = foldersOneLine.Split(';');
            Array.Resize(ref this.foldersTextBox, this.foldersTextBox.Length - 1);
        }

        public ArrayList ShowAccessGroupsOfParentOnly(string prefix)
        {
            ArrayList groups = new ArrayList();

            foreach (var folder in this.foldersTextBox)
            {
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    DirectorySecurity folderSec = dirInfo.GetAccessControl();
                    var authRuleColl = folderSec.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule dsaRule in authRuleColl)
                    {
                        string group = dsaRule.IdentityReference.ToString();

                        if (group.Contains(prefix))
                        {
                            string[] folderInfoArray = new string[2];
                            folderInfoArray[0] = group.Replace(prefix, "");
                            folderInfoArray[1] = dirInfo.Name;
                            groups.Add(folderInfoArray);

                            /*
                            string[] groupSplitArr = groupFullName.Split(new string[] { @"\" }, StringSplitOptions.None);
                            folderInfoArray[0] = groupSplitArr[1];
                            */
                        }
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Filed to get access groups. Check folder(s) path(s).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                    //throw;
                }
            }
            return groups;
        }
    }
}
