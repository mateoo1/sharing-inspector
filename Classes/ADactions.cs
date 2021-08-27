using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

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
            GroupPrincipal ADGroup = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, wantedGroup);

            if (ADGroup != null)
            {
                foreach (Principal p in ADGroup.GetMembers(true))
                {
                    listOfUsersAssignedToThisGroup.Add(p.Name);
                }

                ADGroup.Dispose();
            }
            else
            {
                listOfUsersAssignedToThisGroup.Add(wantedGroup);
            }

            return listOfUsersAssignedToThisGroup;
        }

        public string[] AccountStatus(string samAccontName)
        {
            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, samAccontName);

            string[] userData = new string[3];

            userData[0] = usr.GivenName + " " + usr.Surname;
            userData[2] = usr.Name;

            if (usr.Enabled == true)
            {
                userData[1] = "Enabled";
                
            }
            else if (usr.Enabled == false) 
            {

                userData[1] = "Disabled";
            }
            else
            {
                userData[1] = "Unknown";
            }

            return userData;
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
