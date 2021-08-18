using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Collections;

namespace Sharing_Inspector
{
    class ADactions
    {
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


        public ArrayList ShowMembers(string groupName, string domainName, string containerPath)
        {
            ArrayList accesslist = new ArrayList();
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName, containerPath);

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

            ctx.Dispose();
            return accesslist;
        }
    }
}
