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

namespace OpenSim.Modules.AutoRestart
{
    class AutoRestart : INonSharedRegionModule 
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Scene m_scene;
        private Timer m_timer;
        private int m_restartCounter = 0;

        #region IRegionModule interface

        public void RegionLoaded(Scene scene)
        {
            m_scene = scene;
        }

        public void AddRegion(Scene scene)
        {
            m_scene = scene;
        }

        public void RemoveRegion(Scene scene)
        {

        }

        public void Initialise(IConfigSource config)
        {
            m_log.Info("[AutoRestart] ENABLE AUTO RESTART FOR EVERY 24 HOURS !!!");
            
            m_timer = new Timer(3600000);
            m_timer.Elapsed += new ElapsedEventHandler(timerEvent);
            m_timer.Start();
        }

        public void timerEvent(object sender, ElapsedEventArgs e)
        {
            if (m_restartCounter <= 6)
            {
                m_restartCounter++;
            }
            else
            {
                if (m_scene.GetRootAgentCount() == 0)
                {
                    m_scene.Backup(true);
                    Environment.Exit(0);
                }
                else
                {
                    m_restartCounter = 0;
                }
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
