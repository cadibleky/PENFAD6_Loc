
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Oracle.ManagedDataAccess.Client;
using System.Transactions;
using Dapper;

using DataTableLibrary;
using PenFad6Starter.Modules;


namespace PenFad6Starter
{
    public partial class Form1 : Form
    {
        private cDataForStart cdataforstart = new cDataForStart();
        private cSecurity security = new cSecurity();
        private Setup setup = new Setup();
        private MajorModule majormodule = new MajorModule();
        private CRM_11 crm = new CRM_11();
        private Remittance_12 remit = new Remittance_12();
        private GL gl = new GL();
        private Investment_13 invest = new Investment_13();
        private c_App_settings c_App_setting = new c_App_settings();

        public Form1()
        {
            InitializeComponent();
        }

       
        private void btnNavSecurity_Click(object sender, EventArgs e)
        {
            try
            {

                string date = "03/04/2016";
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                security.add_Security_Modules();

                MessageBox.Show("Security Nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAddInitialDate_Click(object sender, EventArgs e)
        {
            try
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                //DateTime dt = DateTime.ParseExact("2010-01-01 23:00:00", format, CultureInfo.InvariantCulture);
                
                DateTime dd = DateTime.Now ;
                //string date = "01/APR/2016";
                string date = "2016/APR/01";
                string timenow = DateTime.Now.TimeOfDay.ToString();

                string date_time = (date + " " + timenow);
                
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);
                string new_date = transdate.ToString("yyyy/MM/dd hh24:mi:ss");
                //DateTime new_date_Now = Convert.ToDateTime(new_date);
                //DateTime dt = DateTime.ParseExact(dd.ToString(), format, CultureInfo.InvariantCulture, "yyyy/mm/dd hh24:mi:ss");

                majormodule.add_Majormodules();
                gl.add_GL_Modules();
                cdataforstart.Add_Initial_Data(transdate);

                MessageBox.Show("Initial Datas Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnNavSetup_Click(object sender, EventArgs e)
        {
            try
            {
                string date = "03/04/2016";
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                setup.add_Setup_Modules();

                MessageBox.Show("Setup nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



       

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Random random = new Random();
                //int randomNumber = random.Next(10000, 90000);

                // string ss=  randomNumber.ToString();

                //buildTreenodes();
                buildTreenodes_Better();

            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
                //throw;
            }
        }

    

        private void buildTreenodes_Better()
        {
            try
            {
                sec_TreeNode sec_tn = new sec_TreeNode();

                DataSet data = sec_tn.GetModules_DataSetsql();// (101);s
                DataRelation dr = new DataRelation("RelationT", data.Tables[0].Columns["ModuleID"], data.Tables[0].Columns["ParentId"],false);
                data.Relations.Add(dr);
                
                foreach (DataRow r in data.Tables[0].Rows)
                {
                    string pa_id = r["ParentId"].ToString();
                    
                    if (r["ParentId"].ToString() =="0")
                    {
                        TreeNode n = new TreeNode();
                        n.Text = r["ModuleName"].ToString();
                        n.Name = r["ModuleID"].ToString();
                        
                        RecursivelyLoadTree(r, ref n);
                        this.treeView1.Nodes.Add(n);
                    }
                    

                }
 
               // MessageBox.Show(nnnn);
            }
            catch (Exception ex)
            {
                string sss = ex.ToString();
                throw;
            }
        }

        private void RecursivelyLoadTree(DataRow row,ref TreeNode node)
        {
            foreach (DataRow cr in row.GetChildRows("RelationT"))
            {
                TreeNode n = new TreeNode();
                n.Text = cr["ModuleName"].ToString();
                n.Name = cr["ModuleID"].ToString();

                node.Nodes.Add(n);
                RecursivelyLoadTree(cr, ref n);
            }
        }

        private void btnCRM_Click(object sender, EventArgs e)
        {
            try
            {

                crm.add_CRM_Modules();
               // remit();

                MessageBox.Show("CRM nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btnRemittance_Click(object sender, EventArgs e)
        {
            try
            {

                remit.add_Remiitance_Modules();
                // ();

                MessageBox.Show("Remittance nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                invest.add_Investment_Modules();
                // ();

                MessageBox.Show("Investment nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_CreateDefaultValues_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(this.txtTrustee.Text))
                {
                    MessageBox.Show("Enter Trustee's name");
                    return;
                }

                string date = this.dtpStartDate.Value.ToShortDateString();
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                c_App_setting.Add_App_settings(transdate, this.txtTrustee.Text);
                cdataforstart.Add_Initial_Data(transdate);

                majormodule.add_Majormodules();
                gl.add_GL_Modules();
                security.add_Security_Modules();
                setup.add_Setup_Modules();
                crm.add_CRM_Modules();
                remit.add_Remiitance_Modules();
                invest.add_Investment_Modules();


                MessageBox.Show("Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_AppSettings_Click(object sender, EventArgs e)
        {
            try
            {
                string date = this.dtpStartDate.Value.ToShortDateString();
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                c_App_setting.Add_App_settings(transdate, this.txtTrustee.Text);
                MessageBox.Show("Done!!!");

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnNavUtilities_Click(object sender, EventArgs e)
        {
            try
            {
                string date = "03/04/2016";
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                setup.add_Utilities_Modules();

                MessageBox.Show("Setup nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnNavGL_Click(object sender, EventArgs e)
        {
            try
            {
                string date = "03/04/2016";
                string timenow = DateTime.Now.TimeOfDay.ToString();
                DateTime transdate = Convert.ToDateTime(date + " " + timenow);

                gl.add_GL_Modules();

                MessageBox.Show("GL nav Done!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
