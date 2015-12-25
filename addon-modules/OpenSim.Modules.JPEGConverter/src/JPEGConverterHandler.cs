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

            try
            {
                if (asset.IsTextualAsset)
                {
                    OpenMetaverse.Imaging.ManagedImage jpegImageData = new OpenMetaverse.Imaging.ManagedImage(256, 256, OpenMetaverse.Imaging.ManagedImage.ImageChannels.Color);

                    if (OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(asset.Data, out jpegImageData))
                    {
                        httpResponse.ContentType = "image/jpeg";

                        Stream imageStream = new MemoryStream(jpegImageData.ExportRaw());
                        Stream saveStream = new MemoryStream(jpegImageData.ExportRaw());
                        BinaryReader saveStreamReader = new BinaryReader(saveStream);

                        System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
                        image.Save(saveStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return saveStreamReader.ReadBytes((int)saveStream.Length);
                    }

                    httpResponse.ContentType = "text/plain";
                    return GetBytes("ERROR - NOT A TEXTUR");
                }

                httpResponse.ContentType = "text/plain";
                return GetBytes("ERROR");
            }
            catch(Exception e)
            {
                m_JPEGConverter.Log.Error(e);
                httpResponse.ContentType = "text/plain";
                return GetBytes(e.Message);
            }
        }
    }
}
