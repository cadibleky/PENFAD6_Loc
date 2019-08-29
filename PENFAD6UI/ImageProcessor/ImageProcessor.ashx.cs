using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PENFAD6UI.ImageProcessor
{
    /// <summary>
    /// Summary description for ImageProcessor
    /// </summary>
    public class ImageProcessor : IHttpHandler
    {


        public void ProcessRequest(HttpContext context)
        {
            string mFileName;

            if (context.Request.QueryString["Filedata"] != null)
                mFileName = (context.Request.QueryString["Filedata"]).ToString();
            else
                throw new ArgumentException("No parameter specified");

            byte[] Pict_Image = ImageToByte(mFileName);

            Stream strm = new MemoryStream(Pict_Image);
            byte[] buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }

        }

        public static byte[] ImageToByte(string image_path)
        {
            byte[] bufferb = null;
            try
            {

                // provide read access to the file
                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/user_data/"));
                FileStream fs = new FileStream(image_path, FileMode.Open, FileAccess.Read);
                // Create a byte array of file stream length
                byte[] ImageData = new byte[fs.Length];
                //Read block of bytes from stream into the byte array
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                //Close the File Stream
                fs.Close();

                return ImageData;

            }
            catch (Exception ex)
            {
                return bufferb;
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}