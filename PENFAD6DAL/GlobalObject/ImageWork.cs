using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace PENFAD6DAL.GlobalObject
{
    public class ImageWork
    {
        public static string Current_Image_Path { get; set; }
        public static string Current_Signature_Path { get; set; }
        //private sec_TreeNode sec = new sec_TreeNode();

        public static string Current_Path_For_Other_Files
        {
            get { return HttpContext.Current.Session["FilePath"].ToString(); }
            set { HttpContext.Current.Session["FilePath"] = value; }
        }

        public static string Current_Image_Path_To_Diplay
        {
            get { return HttpContext.Current.Session["ImagePath"].ToString(); }
            set { HttpContext.Current.Session["ImagePath"] = value; }
        }

        public static string Current_Image_Path_To_Save
        {
            get { return HttpContext.Current.Session["ImagePathto_save"].ToString(); }
            set { HttpContext.Current.Session["ImagePathto_save"] = value; }
        }
        public static string Upload_Any_File_Not_Image(HttpPostedFile file_posted)
        {
            try
            {
                ////HttpPostedFile file_posted = this.GetCmp<FileUploadField>("btnloadfile").PostedFile;

                string extension = Path.GetExtension(file_posted.FileName);
                string fil_name = GlobalValue.User_ID;
                //check if user_id contains (.)
                if (fil_name.Contains("."))
                {
                    fil_name = fil_name.Replace(".", "_");
                }

                string addto = System.DateTime.Now.ToString();
                if (addto.Contains(":"))
                {
                    addto = addto.Replace(":", "_");
                }

                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/User_Data/"+ addto + "/"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string physicalPath = (filePath + "/"  + fil_name + extension);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                file_posted.SaveAs(physicalPath);
                Current_Path_For_Other_Files = physicalPath;
                return Current_Path_For_Other_Files;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ImagePhysicalPath(HttpPostedFile file_posted)
        {
            try
            {
                string filename = file_posted.FileName;
                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/userdata"));

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //resize and save new image
                //string physicalPath = (filePath + "/" + filename);
                string physicalPath = System.IO.Path.Combine (filePath, filename);
                file_posted.SaveAs(physicalPath);
                

                Image new_image = Resize_Image_With_FilePath(physicalPath);
                new_image.Save(physicalPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                return physicalPath;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public static string Upload_Image_And_Display(HttpPostedFile file_posted, string image_type_pic_or_sig_or_id)
        {
            try
            {
                ////HttpPostedFile file_posted = this.GetCmp<FileUploadField>("btnloadfile").PostedFile;

                string extension = Path.GetExtension(file_posted.FileName);
                string extension_check = extension.ToLower();
                if (extension_check != ".png" && extension_check != ".jpg")
                {
                    return "";
                }
                string fil_name = GlobalValue.User_ID + "_" + image_type_pic_or_sig_or_id;
                //check if user_id contains (.)
                if (fil_name.Contains("."))
                {
                    fil_name = fil_name.Replace(".", "_");
                }

                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/User_Data/"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string physicalPath = (filePath + "/" + fil_name + extension);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                file_posted.SaveAs(physicalPath);

                //resize and save new image
                Image new_image = Resize_Image_With_FilePath(physicalPath);
                string physicalPath_new = (filePath + "/" + fil_name + "_new" + extension);
                new_image.Save(physicalPath_new, System.Drawing.Imaging.ImageFormat.Jpeg);

                //new image path
                string display_path = "../User_Data/" + fil_name + "_new" + extension;
                //set current image path
                Current_Image_Path_To_Diplay = display_path;
                Current_Image_Path_To_Save = physicalPath_new;
                return display_path;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }




        private static string Save_Image_And_Display_2(HttpPostedFile file_posted)
        {
            try
            {
                ////HttpPostedFile file_posted = this.GetCmp<FileUploadField>("btnloadfile").PostedFile;

                string extension = Path.GetExtension(file_posted.FileName);
                string extension_check = extension.ToLower();
                if (extension_check != ".png" && extension_check != ".jpg")
                {
                    return "";
                }
                string fil_name = GlobalValue.User_ID;
                //check if user_id contains (.)
                if (fil_name.Contains("."))
                {
                    fil_name = fil_name.Replace(".", "_");
                }

                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/User_Data/"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string physicalPath = (filePath + "/" + fil_name + extension);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                file_posted.SaveAs(physicalPath);

                ////resize and save new image
                //Image new_image = Resize_ImageFilePath(physicalPath);
                string physicalPath_new = (filePath + "/" + fil_name + "_new" + extension);
                //new_image.Save(physicalPath_new, System.Drawing.Imaging.ImageFormat.Jpeg);

                Image original = Image.FromFile(physicalPath);
                Image resized = ResizeImage_2ndOption(original, new Size(180, 180));
                resized.Save(physicalPath_new, System.Drawing.Imaging.ImageFormat.Jpeg);


                //new image path
                string display_path = "../User_Data/" + fil_name + "_new" + extension;

                return display_path;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
      
        private string Resize_Image_3rd_Option(HttpPostedFile file_posted)
        {
            try
            {
                ////http://www.c-sharpcorner.com/blogs/resizing-image-in-c-sharp-without-losing-quality1
                string extension = Path.GetExtension(file_posted.FileName);
                string extension_check = extension.ToLower();
                if (extension_check != ".png" && extension_check != ".jpg")
                {
                    return "";
                }
                string fil_name = GlobalValue.User_ID;
                //check if user_id contains (.)
                if (fil_name.Contains("."))
                {
                    fil_name = fil_name.Replace(".", "_");
                }

                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/User_Data/"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string physicalPath = (filePath + "/" + fil_name + extension);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                file_posted.SaveAs(physicalPath);

                ///theirs
                //fileupload1.SaveAs(Server.MapPath("images/tempImage/" + fileupload1.FileName));
                // System.Drawing.Image image = System.Drawing.Image.FromFile(Server.MapPath("images/tempImage/" + fileupload1.FileName));
                System.Drawing.Image image = Image.FromFile(physicalPath);
                int newwidthimg = 200;
                float AspectRatio = (float) image.Size.Width / (float) image.Size.Height;
                int newHeight = 200;
                Bitmap bitMAP1 = new Bitmap(newwidthimg, newHeight);
                Graphics imgGraph = Graphics.FromImage(bitMAP1);
                //bitMAP1.imgQuality = CompositingQuality.HighQuality;
                //bitMAP1.imgQuality = CompositingQuality.HighQuality;
                //bitMAP1.SmoothingMode = SmoothingMode.HighQuality;
                //bitMAP1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //var imgDimesions = new Rectangle(0, 0, newwidthimg, newHeight);
                //bitMAP1.DrawImage(image, imageRectangle);
                //bitMAP1.Save(Server.MapPath("images/Shops/" + fileupload1.filename), ImageFormat.Jpeg);
                //bitMAP1.Dispose();
                //bitMAP1.Dispose();
                //image.Dispose();

                return "";
            }
            catch (Exception)
            {

                throw;
            }
        }


        private static Image Resize_Image_With_FilePath(string image_path)
        {
            Bitmap bm_dest = new Bitmap(250, 280);
            try
            {
                Bitmap bm_source = new Bitmap(image_path);

                //Dim bm_dest As New Bitmap(300, 320)
                //Dim bm_dest As New Bitmap(250, 280)
                int width = 250;
                // bm.Width - (bm.Width * percentResize) 'image width. 300

                int height = 280;
                // bm.Height - (bm.Height * percentResize)

                Graphics gr_dest = Graphics.FromImage(bm_dest);
                gr_dest.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                // Copy the source image into the destination bitmap. 
                //gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width + 1, bm_dest.Height + 1)
                gr_dest.DrawImage(bm_source, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bm_source.Width, bm_source.Height), GraphicsUnit.Pixel);
                //PictureBox1.Image = bm_dest
                //bm_dest.Save(AppDomain.CurrentDomain.BaseDirectory & "i.jpg", System.Drawing.Imaging.ImageFormat.Jpeg)
                bm_source.Dispose();
                return bm_dest;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private static Image ResizeImage_2ndOption(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float) size.Width / (float) originalWidth;
                float percentHeight = (float) size.Height / (float) originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int) (originalWidth * percent);
                newHeight = (int) (originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;

            ////##### how to implement
            //// C#
            //Image original = Image.FromFile(@"C:\path\to\some.jpg");
            //Image resized = ResizeImage(original, new Size(1024, 768));
            //MemoryStream memStream = new MemoryStream();
            //resized.Save(memStream, ImageFormat.Jpeg);
        }


        public static byte[] ImageToByte(string image_path)
        {
            byte[] bufferb = null;
            try
            {

                // provide read access to the file
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


        public static string ByteToImage_And_Display(byte[] byte_image , string image_type_pic_or_sig_or_id)
        {
            try
            {
                // Create a byte array
                byte[] byteData = new byte[0];
                // fetch the value of Oracle parameter into the byte array
                //byteData = (byte[]) ((OracleBlob) (cmd.Parameters[1].Value)).Value;  // how to supply database blob value
                byteData = byte_image;
                // get the length of the byte array
                int ArraySize = new int();
                ArraySize = byteData.GetUpperBound(0);
                // Write the Blob data fetched from database to the filesystem at the
                // destination location

                string fil_name = GlobalValue.User_ID + "_" +  image_type_pic_or_sig_or_id;
                string extension = ".jpg";
                //check if user_id contains (.)
                if (fil_name.Contains("."))
                {
                    fil_name = fil_name.Replace(".", "_");
                }
                string filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/User_Data/"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string physicalPath = (filePath + "/" + fil_name + extension);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                ////file_posted.SaveAs(physicalPath);

                ////resize and save new image
                //Image new_image = Resize_Image_With_FilePath(physicalPath);
                //string physicalPath_new = (filePath + "/" + fil_name + "_new" + extension);
                //new_image.Save(physicalPath_new, System.Drawing.Imaging.ImageFormat.Jpeg);

                //new image path
                string display_path = "../User_Data/" + fil_name  + extension;
                //set current image path
                Current_Image_Path_To_Diplay = display_path;
                Current_Image_Path_To_Save = physicalPath;

                FileStream fs1 = new FileStream(physicalPath, FileMode.OpenOrCreate, FileAccess.Write);
                fs1.Write(byteData, 0, ArraySize);
                fs1.Close();

                return display_path;
            }
            catch (Exception ex)
            {
                return "";
            }

        }







    }//end class
}