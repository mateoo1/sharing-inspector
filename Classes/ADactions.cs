using System;
using System.Collections;
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

        public ArrayList ShowMembers(string groupName)
        {
            ArrayList accesslist = new ArrayList();
            GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, groupName);

            if (grp != null)
            {
                foreach (Principal p in grp.GetMembers(true))
                {
                    string memberName = p.Name;
                    accesslist.Add(memberName);
                }

                grp.Dispose();
            }
            else
            {
                string userName =  groupName;
                accesslist.Add(userName);
            }

            return accesslist;
        }

        public string[] AccountStatus(string samAccontName)
        {
            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, samAccontName);

            string[] userData = new string[2];

            userData[0] = usr.GivenName + " " + usr.Surname;

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
