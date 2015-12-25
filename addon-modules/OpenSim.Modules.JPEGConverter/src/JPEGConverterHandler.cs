using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSim.Framework.Servers.HttpServer;
using System.IO;
using OpenSim.Framework;
using OpenMetaverse;
using System.Drawing;

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

        private static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        protected override byte[] ProcessRequest(string path, Stream requestData, IOSHttpRequest httpRequest, IOSHttpResponse httpResponse)
        {
            Dictionary<string, object> request = new Dictionary<string, object>();
            foreach (string name in httpRequest.QueryString)
                request[name] = httpRequest.QueryString[name];

            if (m_JPEGConverter.AssetService != null)
            {
                string assetUUID = Convert.ToString(request["assetID"]);

                if(m_JPEGConverter.AssetService.AssetsExist(new String[] {assetUUID})[0])
                {
                    AssetBase asset = m_JPEGConverter.AssetService.Get(assetUUID);

                    if (asset != null)
                    {
                        if (asset.Type == (sbyte)AssetType.Texture)
                        {
                            OpenMetaverse.Imaging.ManagedImage jpegImageData = new OpenMetaverse.Imaging.ManagedImage(256, 256, OpenMetaverse.Imaging.ManagedImage.ImageChannels.Color);

                            if (OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(asset.Data, out jpegImageData))
                            {
                                Stream imageStream = new MemoryStream(ImageToByte(jpegImageData.ExportBitmap()));
                                Stream saveStream = new MemoryStream();

                                System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);
                                image.Save(saveStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                                httpResponse.ContentType = "image/jpeg";
                                return ReadToEnd(saveStream);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
