using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace OpenSim.Region.ScriptEngine.XEngine
{
    class AppDomainUser
    {
        public AppDomain appDomain;
        public UUID UUID;
        public int usedByScripts = 1;

        public AppDomainUser(AppDomain l_appDomain, UUID l_uuid)
        {
            appDomain = l_appDomain;
            UUID = l_uuid;
        }
    }
}
