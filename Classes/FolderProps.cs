using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                    AuthorizationRuleCollection authRuleCollection = folderSec.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule authRule in authRuleCollection)
                    {
                        string group = authRule.IdentityReference.ToString();

                        if (group.Contains(prefix))
                        {
                            Dictionary<string, string> folderInfo = new Dictionary<string, string>();
                            folderInfo.Add("Group", group.Replace(prefix, ""));
                            folderInfo.Add("SAMAccountName", dirInfo.Name);
                            folderInfo.Add("FullName", dirInfo.FullName);
                            groups.Add(folderInfo);

                        }
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Filed to get access groups. \n \n Make sure that: \n - Folders paths are correct \n - Program was launched as Administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            Dictionary<string, string> folderInfo = new Dictionary<string, string>();
                            folderInfo.Add("Group", group.Replace(prefix, ""));
                            folderInfo.Add("SAMAccountName", child.Name);
                            folderInfo.Add("FullName", child.FullName);
                            groups.Add(folderInfo);
                        }
                    }
                }
            }
            return groups;
        }


        public ArrayList ShowAccessGroupsOfChildsParallel(string prefix)
        {
            ArrayList groups = new ArrayList();
            string[] contentOfFoldersTextBox = this.foldersTextBox;

            Parallel.ForEach<string>(contentOfFoldersTextBox, (item) =>
            {
                var childDirsCollection = new DirectoryInfo(item).EnumerateDirectories("*", SearchOption.AllDirectories);

                Parallel.ForEach<DirectoryInfo>(childDirsCollection, (child) =>
                {
                    DirectorySecurity folderSec = child.GetAccessControl();
                    AuthorizationRuleCollection authRuleCollection = folderSec.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule authRule in authRuleCollection)
                    {
                        string group = authRule.IdentityReference.ToString();

                        if (group.Contains(prefix))
                        {
                            Dictionary<string, string> folderInfo = new Dictionary<string, string>();
                            folderInfo.Add("Group", group.Replace(prefix, ""));
                            folderInfo.Add("SAMAccountName", child.Name);
                            folderInfo.Add("FullName", child.FullName);
                            groups.Add(folderInfo);
                        }
                    }
                });
            });
            return groups;
        }
    }
}
