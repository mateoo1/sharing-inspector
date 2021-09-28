using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using System.Windows;

namespace Sharing_Inspector
{
    class ADactions
    {
        public PrincipalContext ctx;
        public string domain;
        public string domainPrefix;
        public string ContainerPath;
        public bool domainAvailability;

        public ADactions()
        {
            string domainString = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string[] domainElements = domainString.Split('.');
            this.domain = domainElements[0];
            this.domainPrefix = domainElements[0].ToUpper() + @"\";

            foreach (string domainElement in domainElements)
            {
                this.ContainerPath += "DC=" + domainElement + ",";
            }

            this.ContainerPath = ContainerPath.Remove(ContainerPath.Length - 1);

            try
            {
                this.ctx = new PrincipalContext(ContextType.Domain, this.domain, this.ContainerPath);
                this.domainAvailability = true;
            }
            catch (Exception)
            {
                this.domainAvailability = false;
            }  
        }

        /*
        public Dictionary<string, string> IdentifyDomain()
        {

            Dictionary<string, string> domainInfo = new Dictionary<string, string>();

            string domainString = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string[] domainElements = domainString.Split('.');

            // Prefixes
            domainInfo.Add("domain", domainElements[0]);
            domainInfo.Add("domainPrefix", domainElements[0].ToUpper() + @"\");

            // Build Container path
            string ContainerPath = "";
            foreach (string domainElement in domainElements)
            {
                ContainerPath += "DC=" + domainElement + ",";
            }
            domainInfo.Add("ContainerPath", ContainerPath.Remove(ContainerPath.Length - 1));

            return domainInfo;
        }
        */


        public ArrayList ShowMembers(string wantedGroup)
        {
            ArrayList listOfUsersAssignedToThisGroup = new ArrayList();

            if (wantedGroup.Contains("Domain Users"))
            {

                listOfUsersAssignedToThisGroup.Add("All Domain Users");
                return listOfUsersAssignedToThisGroup;

            }
            else
            {
                try
                {
                    GroupPrincipal ADGroup = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, wantedGroup);

                    if (ADGroup != null)
                    {
                        var wantedGroupMembers = ADGroup.GetMembers(true);

                        //foreach (Principal member in wantedGroupMembers)
                        //{
                        //    listOfUsersAssignedToThisGroup.Add(member.Name);
                        //}

                        Parallel.ForEach<Principal>(wantedGroupMembers, (member) =>
                        {
                            listOfUsersAssignedToThisGroup.Add(member.Name);
                        });

                        ADGroup.Dispose();
                    }
                    else
                    {
                        listOfUsersAssignedToThisGroup.Add(wantedGroup);
                    }

                    return listOfUsersAssignedToThisGroup;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw ex;
                }
            }
        }

        public Dictionary<string, string> AccountStatus(string samAccontName)
        {
            UserPrincipal usr;
            Dictionary<string, string> userData = new Dictionary<string, string>();

            try
            {
                usr = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, samAccontName);

                if (usr != null)
                {
                    userData.Add("FullName", (usr.GivenName + " " + usr.Surname));
                    userData.Add("SAMAccountName", usr.Name);

                    if (usr.Enabled == true)
                    {
                        userData.Add("Status", "Enabled");

                    }
                    else if (usr.Enabled == false)
                    {

                        userData.Add("Status", "Disabled");
                    }
                    else
                    {
                        userData.Add("Status", "Unknown");
                    }

                    return userData; 
                }
                else
                {
                    userData.Add("SAMAccountName", "Unknown");
                    userData.Add("Status", "Not available");
                    userData.Add("FullName", samAccontName);
                    return userData;
                }
            }
            catch (Exception ex2)
            {
                // MessageBox.Show(ex2.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // throw ex2;
                userData.Add("SAMAccountName", "Unknown");
                userData.Add("Status", "Not available");
                userData.Add("FullName", samAccontName);
                return userData;
            }
        }

        public void DisposeContext()
        {
            if (this.ctx != null)
            {
                ctx.Dispose();
            }
        }

    }
}
