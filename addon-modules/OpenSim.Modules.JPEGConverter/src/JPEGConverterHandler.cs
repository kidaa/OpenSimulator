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

            httpResponse.ContentType = "text/plain";

            if (m_JPEGConverter.AssetService != null)
            {
                AssetBase asset = m_JPEGConverter.AssetService.Get(Convert.ToString(request["assetID"]));

                if (asset != null)
                {
                    try
                    {
                        if (asset.IsTextualAsset)
                        {
                            OpenMetaverse.Imaging.ManagedImage jpegImageData = new OpenMetaverse.Imaging.ManagedImage(256, 256, OpenMetaverse.Imaging.ManagedImage.ImageChannels.Color);

                            if (jpegImageData != null)
                            {
                                if (OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(asset.Data, out jpegImageData))
                                {
                                    Stream imageStream = new MemoryStream(jpegImageData.ExportRaw());
                                    Stream saveStream = new MemoryStream();

                                    if (imageStream != null)
                                    {
                                        if (saveStream != null)
                                        {
                                            BinaryReader saveStreamReader = new BinaryReader(saveStream);

                                            if (saveStreamReader != null)
                                            {
                                                System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
                                                image.Save(saveStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                                httpResponse.ContentType = "image/jpeg";
                                                return saveStreamReader.ReadBytes((int)saveStreamReader.BaseStream.Length);
                                            }
                                            else
                                            {
                                                m_JPEGConverter.Log.Error("saveStreamReader object is a null object");
                                                return GetBytes("ERROR");
                                            }
                                        }
                                        else
                                        {
                                            m_JPEGConverter.Log.Error("saveStream object is a null object");
                                            return GetBytes("ERROR");
                                        }
                                    }
                                    else
                                    {
                                        m_JPEGConverter.Log.Error("imageStream object is a null object");
                                        return GetBytes("ERROR");
                                    }
                                }
                                else
                                {
                                    m_JPEGConverter.Log.Error("ERROR AT DECODE IMAGE");
                                    return GetBytes("ERROR");
                                }
                            }
                            else
                            {
                                m_JPEGConverter.Log.Error("jpegImageData object is a null object");
                                return GetBytes("ERROR");
                            }
                        }
                        else
                        {
                            return GetBytes("ERROR - NOT A TEXTUR");
                        }
                    }
                    catch (Exception e)
                    {
                        m_JPEGConverter.Log.Error(e);
                        return GetBytes(e.Message);
                    }
                }
                else
                {
                    m_JPEGConverter.Log.Error("AssetBase object is a null object");
                    return GetBytes("ERROR");
                }
            }
            else
            {
                m_JPEGConverter.Log.Error("Asset server object is a null object");
                return GetBytes("ERROR");
            }
        }
    }
}
