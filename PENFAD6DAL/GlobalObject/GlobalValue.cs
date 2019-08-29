
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Data;
using System.Reflection;
using System.Web;
using System.Data.Linq;
using PENFAD6DAL.DbContext;
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;
using System.Web.Security;
using Dapper;

namespace PENFAD6DAL.GlobalObject
{
    public class GlobalValue
    {
        public static void GlobalValue_Ini()
        {
            try
            {
                string date = "03/04/2016";
                string default_date = "01/01/1950";
                string timenow = DateTime.Now.TimeOfDay.ToString();
                //DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                Scheme_Today_Date = Convert.ToDateTime(date + " " + timenow);
                Default_Date_Value = Convert.ToDateTime(default_date);
                User_ID = HttpContext.Current.User.Identity.Name;
                
                //User_GroupID = "G100";

                var app = new AppSettings();
                ConString = app.conString();

                Report_Param_1 = "na";
                Report_Index_Id = "na";
                Report_Param_2 = DateTime.Now;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static DateTime Scheme_Today_Date
        {
            get { return (DateTime)HttpContext.Current.Session["Scheme_Today_Date"]; }
            set { HttpContext.Current.Session["Scheme_Today_Date"] = value; }
        }
        public static DateTime App_Setting_Penfad_Start_Date
        {
            get { return (DateTime)HttpContext.Current.Session["App_Setting_Penfad_Start_Date"]; }
            set { HttpContext.Current.Session["App_Setting_Penfad_Start_Date"] = value; }
        }
        public static string User_ID
        {
            get { return HttpContext.Current.Session["Username"].ToString(); }
            set { HttpContext.Current.Session["Username"] = value; }
        }
        public static string User_GroupID
        {
            get { return HttpContext.Current.Session["UserGroupID"].ToString(); }
            set { HttpContext.Current.Session["UserGroupID"] = value; }
        }

        public static int Module_Code
        {
            get { return (int)HttpContext.Current.Session["Module_Code"]; }
            set { HttpContext.Current.Session["Module_Code"] = value; }
        }

        public static DateTime Default_Date_Value
        {
            get { return (DateTime)HttpContext.Current.Session["defDate"]; }
            set { HttpContext.Current.Session["defDate"] = value; }
        }

        //public static DateTime Posting_Date
        //{
        //    get { return Convert.ToDateTime(HttpContext.Current.Session["Postingdate"]); }
        //    set { HttpContext.Current.Session["Postingdate"] = value; }
        //}

        public static string ConString
        {
            get { return HttpContext.Current.Session["dbase"].ToString(); }
            set { HttpContext.Current.Session["dbase"] = value; }
        }


        //----Report Variables
        public static DateTime Report_Param_Age1_date
        {
            get { return Convert.ToDateTime(HttpContext.Current.Session["Report_Param_Age1_date"]); }
            set { HttpContext.Current.Session["Report_Param_Age1_date"] = value; }
        }
        public static DateTime Report_Param_Age2_date
        {
            get { return Convert.ToDateTime(HttpContext.Current.Session["Report_Param_Age2_date"]); }
            set { HttpContext.Current.Session["Report_Param_Age2_date"] = value; }
        }
        public static string Report_Param_1
        {
            get { return HttpContext.Current.Session["Report_Param_1"].ToString(); }
            set { HttpContext.Current.Session["Report_Param_1"] = value; }
        }
		public static DateTime? Report_Param_1_date
		{
			get { return Convert.ToDateTime(HttpContext.Current.Session["Report_Param_1_date"]); }
			set { HttpContext.Current.Session["Report_Param_1_date"] = value; }
		}
		public static DateTime? Report_Param_2
        {
            get { return Convert.ToDateTime(HttpContext.Current.Session["Report_Param_2"]); }
            set { HttpContext.Current.Session["Report_Param_2"] = value; }
        }

        public static string Report_Param_2_string
        {
            get { return HttpContext.Current.Session["Report_Param_2_string"].ToString(); }
            set { HttpContext.Current.Session["Report_Param_2_string"] = value; }
        }
        public static string Report_Param_3_string
        {
            get { return HttpContext.Current.Session["Report_Param_3_string"].ToString(); }
            set { HttpContext.Current.Session["Report_Param_3_string"] = value; }
        }
        public static string Report_Index_Id
        {
            get { return HttpContext.Current.Session["Rep_Employee_Tree_Id"].ToString(); }
            set { HttpContext.Current.Session["Rep_Employee_Tree_Id"] = value; }
        }






        public static DateTime LastDayOfMonth(DateTime dt)
        {
            try
            {
                DateTime t = new DateTime(dt.Year, dt.Month, 28);
                return t.AddDays(4 - t.AddDays(4).Day);
            }
            catch (Exception )
            {
                return DateTime.Now.Date;
            }
        }

        public static string lg_MyFilePath(string file_name)
        {
            try
            {
                // '''Return HttpContext.Current.Session(HttpContext.Current.Server.MapPath("~/Resources/Images/" & cSecurity.lg_user_ID & "/" + file_name))
                return HttpContext.Current.Server.MapPath("~/Resources/Images/" + file_name);
            }
            catch (Exception)
            {

                return HttpContext.Current.Server.MapPath("~/Resources/Images/");
            }
        }
        public static byte[] ImageToBinary(string imagepath)
        {
            byte[] bufferb = null;
            try
            {
                FileInfo finfo = new FileInfo(imagepath);
                FileStream fs = new FileStream(imagepath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long numbyte = Convert.ToInt32(finfo.Length);
                bufferb = br.ReadBytes((int)numbyte);
                fs.Read(bufferb, 0, (int)fs.Length);
                fs.Close();
                fs.Dispose();
                // '''MsgBox("Image size is " & bufferb.Length)
                return bufferb;

            }
            catch (Exception)
            {
            }
            return bufferb;
        }





        public static Image Resize_ImageFilePath(string image_path)
        {
            Bitmap bm_source = new Bitmap(image_path);

            //Dim bm_dest As New Bitmap(300, 320)
            Bitmap bm_dest = new Bitmap(250, 280);
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

        public static Image BinaryToImage(Binary binaryData)
        {
            if (binaryData == null) return null;

            byte[] buffer = binaryData.ToArray();
            var memStream = new MemoryStream();
            memStream.Write(buffer, 0, buffer.Length);
            return Image.FromStream(memStream);
        }

        public static bool DeleteAllFilesForAUser()
        {
            try
            {
                string save_path = HttpContext.Current.Server.MapPath("~/Resources/Images/");

                if (Directory.Exists(save_path))
                {
                    // ''Dim strDirectory As String = "C:\Folder\Folder"
                    // ''For Each foundFile As String In My.Computer.FileSystem.GetFiles(strDirectory, "*.*")
                    // ''    My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.DeletePermanently)
                    // ''Next

                    string filenamee = save_path + GlobalValue.User_ID;
                    foreach (string foundFile in Directory.GetFiles(save_path, "*.*"))
                    {
                        if (foundFile.StartsWith(filenamee))
                        {
                            File.Delete(foundFile);
                        }
                    }

                    //        My.Computer.FileSystem.DeleteFile("C:\test.txt", _
                    //FileIO.UIOption.AllDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.DoNothing)

                }
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }





        public static string AES_Encrypt(string input_text, string security_Code)
        {
            // ''' security_Code= teksolencrypt$@teksol.com987908123
            System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
            System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string encrypted = "";
            try
            {
                byte[] hash = new byte[32];
                byte[] temp = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(security_Code));
                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);
                AES.Key = hash;
                AES.Mode = System.Security.Cryptography.CipherMode.ECB;
                //'Security.Cryptography.CipherMode.ECB
                System.Security.Cryptography.ICryptoTransform DESEncrypter = AES.CreateEncryptor();
                byte[] Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(input_text);
                encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));

            }
            catch (Exception ex)
            {
            }
            return encrypted;
        }
        public static string AES_Decrypt(string input_text, string securitycode)
        {
            // ''' security_Code= teksolencrypt$@teksol.com987908123
            System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
            System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string decrypted = "";
            try
            {
                byte[] hash = new byte[32];
                byte[] temp = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(securitycode));
                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);
                AES.Key = hash;
                AES.Mode = CipherMode.ECB;
                // Security.Cryptography.CipherMode.ECB
                System.Security.Cryptography.ICryptoTransform DESDecrypter = AES.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(input_text);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
            }
            return decrypted;
        }

        public static void Get_Scheme_Today_Date(string Scheme_id)
        {
            var app = new AppSettings();
            using (OracleConnection conp = new OracleConnection(app.conString()))
            {
                try
                {

                    if (string.IsNullOrEmpty(Scheme_id))
                        Scheme_id = "0";
                    //Get connection
                    conp.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conp;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SEL_SCHEME_TODAYS";
                        cmd.Parameters.Add("p_Scheme_Id", OracleDbType.Varchar2, ParameterDirection.Input).Value = Scheme_id;
                        cmd.Parameters.Add("p_result", OracleDbType.RefCursor, ParameterDirection.Output);

                        OracleDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            Scheme_Today_Date = Convert.ToDateTime(dr["TODAY_DATE"].ToString()); // p_result.Value.ToString();
                        }

                    }

                    // return  Scheme_Today_Date;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conp.State == ConnectionState.Open)
                    {
                        conp.Close();
                        conp.Dispose();
                    }
                }

            }
        }
    }

}

