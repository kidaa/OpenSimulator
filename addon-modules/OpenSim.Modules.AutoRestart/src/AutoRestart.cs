using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSim.Region.Framework.Scenes;
using Nini.Config;
using log4net;
using System.Reflection;
using OpenSim.Region.Framework.Interfaces;
using System.Timers;
using Mono.Addins;

[assembly: Addin("AutoRestart", "1.0")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]

namespace OpenSim.Modules.AutoRestart
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule")]
    class AutoRestart : ISharedRegionModule 
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<Scene> m_scene = new List<Scene>();
        private Timer m_timer;
        private int m_restartCounter = 0;

        #region IRegionModule interface

        public void RegionLoaded(Scene scene)
        {
            m_scene.Add(scene);
        }

        public void AddRegion(Scene scene)
        {
            m_scene.Add(scene);

            m_log.Info("[AutoRestart] ENABLE AUTO RESTART FOR EVERY 6 HOURS !!!");

            m_timer = new Timer(3600000);
            m_timer.Elapsed += new ElapsedEventHandler(timerEvent);
            m_timer.Start();
        }

        public void RemoveRegion(Scene scene)
        {

        }

        public void Initialise(IConfigSource config)
        {

        }

        public void timerEvent(object sender, ElapsedEventArgs e)
        {
            if (m_restartCounter <= 6)
            {
                m_restartCounter++;
            }
            else
            {

                int agentCount = 0;

                foreach (Scene s in m_scene)
                {
                    if (s.GetRootAgentCount() != 0)
                    {
                        agentCount = agentCount + s.GetRootAgentCount();
                        s.Backup(true);
                    }                    
                }

                if (agentCount == 0)Environment.Exit(0);
                m_restartCounter = 0;
                m_log.Info("[AutoRestart] REGION IS NOT EMPTRY! MOVE RESTART.");
            }
        }

        public void PostInitialise()
        {

        }

        public void Close()
        {
        }

        public string Name
        {
            get { return "AutoRestart"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }


        #endregion
    }
}
