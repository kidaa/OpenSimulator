using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Addins;
using OpenSim.Region.Framework.Interfaces;
using log4net;
using System.Reflection;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Services.Interfaces;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Framework.Servers;
using Nini.Config;

[assembly: Addin("JPEGConverter", "0.2")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]

namespace OpenSim.Modules.JPEGConverter
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule")]
    class JPEGConverter : ISharedRegionModule
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IAssetService m_assetService;
        private IConfigSource m_config;

        public IAssetService AssetService
        {
            get
            {
                return m_assetService;
            }
        }

        public ILog Log
        {
            get
            {
                return m_log;
            }
        }

        public void RegionLoaded(Scene scene)
        {
            if (scene != null)
            {
                m_assetService = scene.AssetService;
                m_log.Info("[JPEGConverter] Load into region " + scene.Name);
            }

            IHttpServer server = MainServer.GetHttpServer(0);
            server.AddStreamHandler(new JPEGConverterHandler(this));
        }

        public void PostInitialise()
        {
            m_log.Info("[JPEGConverter] PostInitialise");
        }

        public string Name
        { 
            get
            {
                return "JPEGConverter"; 
            }
        }

        public Type ReplaceableInterface 
        {
            get
            {
                return null;
            }
        }

        public void Initialise(IConfigSource source)
        {
            m_config = source;
            m_log.Info("[JPEGConverter] Initialise");
        }

        public void Close()
        {

        }

        public void AddRegion(Scene scene)
        {

        }

        public void RemoveRegion(Scene scene)
        {

        }
    }
}
