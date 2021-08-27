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
                    var authRuleCollection = folderSec.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule authRule in authRuleCollection)
                    {
                        string group = authRule.IdentityReference.ToString();

                        if (group.Contains(prefix))
                        {
                            string[] folderInfoArray = new string[3];
                            folderInfoArray[0] = group.Replace(prefix, "");
                            folderInfoArray[1] = dirInfo.Name;
                            folderInfoArray[2] = dirInfo.FullName;
                            groups.Add(folderInfoArray);

                        }
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Filed to get access groups. Verify folder(s) path(s) or run program as Administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                    //throw;
                }
            }
            return groups;
        }

        public ArrayList ShowAccessGroupsOfChilds(string prefix)
        {
            ArrayList groups = new ArrayList();

            foreach (var item in this.foldersTextBox)
            {
                var childDirsCollection = new DirectoryInfo(item).EnumerateDirectories("*", SearchOption.AllDirectories);

                foreach (var child in childDirsCollection)
                {
                    DirectorySecurity folderSec = child.GetAccessControl();
                    var authRuleCollection = folderSec.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule authRule in authRuleCollection)
                    {
                        string group = authRule.IdentityReference.ToString();

                        if (group.Contains(prefix))
                        {
                            string[] folderInfoArray = new string[3];
                            folderInfoArray[0] = group.Replace(prefix, "");
                            folderInfoArray[1] = child.Name;
                            folderInfoArray[2] = child.FullName;
                            groups.Add(folderInfoArray);
                        }
                    }
                }
            }
            return groups;
        }
    }
}
