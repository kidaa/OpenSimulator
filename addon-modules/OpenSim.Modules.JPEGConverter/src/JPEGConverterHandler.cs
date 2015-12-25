using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSim.Framework.Servers.HttpServer;
using System.IO;
using OpenSim.Framework;

namespace OpenSim.Modules.JPEGConverter
{
    class JPEGConverterHandler : BaseStreamHandler
    {
        private JPEGConverter m_JPEGConverter = null;

        public JPEGConverterHandler(JPEGConverter jpeg) : base("GET", "/jpeg/")
        {
            if (m_JPEGConverter == null)
            {
                m_JPEGConverter = jpeg;
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        protected override byte[] ProcessRequest(string path, Stream requestData, IOSHttpRequest httpRequest, IOSHttpResponse httpResponse)
        {
            Dictionary<string, object> request = new Dictionary<string, object>();
            foreach (string name in httpRequest.QueryString)
                request[name] = httpRequest.QueryString[name];

            AssetBase asset = m_JPEGConverter.AssetService.Get(Convert.ToString(request["assetID"]));

            if (asset.IsTextualAsset)
            {
                httpResponse.ContentType = "image/jpeg";

                OpenMetaverse.Imaging.ManagedImage jpegImageData;

                if (OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(asset.Data, out jpegImageData))
                {
                    return jpegImageData.ExportRaw();
                }
            }

            httpResponse.ContentType = "text/plain";
            return GetBytes("ERROR");
        }
    }
}
