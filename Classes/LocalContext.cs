using System;
using System.DirectoryServices.AccountManagement;
using System.Collections;

namespace Sharing_Inspector
{
    class LocalContext
    {
        public ArrayList GetGroupMembers(string localGroupName)
        {
            ArrayList allLocalGroupMemebrs = new ArrayList();

            try
            {
                GroupPrincipal groupPrincipal = GetGroup(localGroupName);
                PrincipalSearchResult<Principal> localGroupMembers = groupPrincipal.GetMembers();
                foreach (Principal localGroupMember in localGroupMembers)
                {
                    allLocalGroupMemebrs.Add(localGroupMember.Name);
                }
                return allLocalGroupMemebrs;
            }
            catch (Exception)
            {
                allLocalGroupMemebrs.Add(localGroupName);
                return allLocalGroupMemebrs;
            }
        }

        public GroupPrincipal GetGroup(string localGroupName)
        {
            PrincipalContext localCtx = GetPrincipalContext();
            GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(localCtx, localGroupName);
            return groupPrincipal;
        }

        public PrincipalContext GetPrincipalContext()
        {
            PrincipalContext localCtx = new PrincipalContext(ContextType.Machine);
            return localCtx;
        }
    }
}