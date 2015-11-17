using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace OpenSim.Region.ScriptEngine.XEngine
{
    class AppDomainUserHandler
    {
        private static List<AppDomainUser> userScriptAppDomain = new List<AppDomainUser>();

        public static AppDomain getUserAppDomainByUUID(UUID user)
        {
            foreach (AppDomainUser userDomain in userScriptAppDomain)
            {
                if (userDomain.UUID == user)
                {
                    userDomain.usedByScripts = userDomain.usedByScripts + 1;
                    return userDomain.appDomain;
                }
            }

            return null;
        }

        public static bool removeUserAppDomain(AppDomain userAppDomain)
        {
            foreach (AppDomainUser userDomain in userScriptAppDomain)
            {
                if (userDomain.appDomain.Id == userAppDomain.Id)
                {
                    userDomain.usedByScripts = userDomain.usedByScripts - 1;

                    if (userDomain.usedByScripts == 0)
                    {
                        userDomain.appDomain = null;
                        userScriptAppDomain.Remove(userDomain);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }


        public static void saveUserAppDomain(UUID user, AppDomain userAppDomain)
        {
            AppDomainUser tmpDomain = new AppDomainUser(userAppDomain, user);
            userScriptAppDomain.Add(tmpDomain);
        }
    }
}
