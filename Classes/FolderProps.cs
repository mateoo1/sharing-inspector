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
        private List<string> listOfFolderPaths = new List<string>();
        private List<string> pathsNotExists = new List<string>();

        public FoldersProps(string provideFoldersIntextBox)
        {
            string foldersOneLine = Regex.Replace(provideFoldersIntextBox, @"\r\n?|\n", "");
            this.foldersTextBox = foldersOneLine.Split(';');
            Array.Resize(ref this.foldersTextBox, this.foldersTextBox.Length - 1);

            foreach (var item in this.foldersTextBox)
            {
                if (Directory.Exists(item))
                {
                    this.listOfFolderPaths.Add(item);
                }
                else
                {
                    this.pathsNotExists.Add(item);
                }
            }
        }

        public List<string> NotExistingFolders()
        {
            return this.pathsNotExists;
        }

        public Dictionary<string, string> folderRecord (string Group, string SAMAccountName, string FullName, string Type)
        {
            Dictionary<string, string> folderInfo = new Dictionary<string, string>();
            folderInfo.Add("Group", Group);
            folderInfo.Add("SAMAccountName", SAMAccountName);
            folderInfo.Add("FullName", FullName);
            folderInfo.Add("Type", Type);
            return folderInfo;
        }

        public ArrayList ShowAccessGroupsOfParentOnly(string prefix)
        {
            ArrayList groups = new ArrayList();

            foreach (var folder in this.listOfFolderPaths)
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
                            groups.Add(folderRecord(group.Replace(prefix, ""), dirInfo.Name, dirInfo.FullName, "Domain"));

                        }
                        else if (group != "NT AUTHORITY\\SYSTEM" && group.Contains("\\"))
                        {
                            groups.Add(folderRecord(group, dirInfo.Name, dirInfo.FullName, "Local"));
                        }
                        else
                        {

                        }
                    }
                }

                catch (Exception ex1)
                {
                    MessageBox.Show("Failed to get access groups. \n \n Make sure that folders paths are correct.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show(ex1.Message);
                    // groups.Add(folderRecord("Folder not found", "Folder not found", folder, "None"));
                    continue;
                }
            }
            return groups;
        }

        //public ArrayList ShowAccessGroupsOfChilds(string prefix)
        //{
        //    ArrayList groups = new ArrayList();

        //    foreach (var item in this.foldersTextBox)
        //    {
        //        var childDirsCollection = new DirectoryInfo(item).EnumerateDirectories("*", SearchOption.AllDirectories);

        //        foreach (var child in childDirsCollection)
        //        {
        //            DirectorySecurity folderSec = child.GetAccessControl();
        //            var authRuleCollection = folderSec.GetAccessRules(true, false, typeof(NTAccount));

        //            foreach (FileSystemAccessRule authRule in authRuleCollection)
        //            {
        //                string group = authRule.IdentityReference.ToString();

        //                if (group.Contains(prefix))
        //                {
        //                    groups.Add(folderRecord(group, child.Name, child.FullName, "Domain"));

        //                }
        //                else if (group != "NT AUTHORITY\\SYSTEM" && group.Contains("\\"))
        //                {
        //                    groups.Add(folderRecord(group, child.Name, child.FullName, "Local"));
        //                }
        //                else
        //                {

        //                }
        //            }
        //        }
        //    }
        //    return groups;
        //}


        public ArrayList ShowAccessGroupsOfChildsParallel(string prefix)
        {
            try
            {
                ArrayList groups = new ArrayList();
                List<string> contentOfFoldersTextBox = this.listOfFolderPaths;

                Parallel.ForEach<string>(contentOfFoldersTextBox, (item) =>
                {

                    var childDirsCollection = new DirectoryInfo(item).EnumerateDirectories("*", SearchOption.AllDirectories);

                    Parallel.ForEach<DirectoryInfo>(childDirsCollection, (child) =>
                    {
                        DirectorySecurity folderSec = child.GetAccessControl();
                        AuthorizationRuleCollection authRuleCollection = folderSec.GetAccessRules(true, false, typeof(NTAccount));

                        foreach (FileSystemAccessRule authRule in authRuleCollection)
                        {
                            string group = authRule.IdentityReference.ToString();

                            if (group.Contains(prefix))
                            {
                                groups.Add(folderRecord(group, child.Name, child.FullName, "Domain"));

                            }
                            else if (group != "NT AUTHORITY\\SYSTEM" && group.Contains("\\"))
                            {
                                groups.Add(folderRecord(group, child.Name, child.FullName, "Local"));
                            }
                            else
                            {

                            }
                        }
                    });
                });


                return groups;
            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message);
                throw;
            }
        }
    }
}
