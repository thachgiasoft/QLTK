using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BMS.Utils;
using System.Diagnostics;
using System.Net;
using System.Data;
using Microsoft.VisualBasic.Devices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using BMS.Model;
using BMS.Business;
using Microsoft.Win32;
using DevExpress.Utils;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace BMS
{
    public partial class frmMain : _Forms
    {
        private const string CURRENT_VERSION = "14.07.2014";
        //bool _loginOK = false;
        bool _isUpdated = false;
        bool _logOut = false;

        public frmMain()
        {
            if (Application.StartupPath.Contains("Share"))
            {
                frmCloseForm frm = new frmCloseForm();
                frm.ShowDialog();
            }

            string strPath = Application.StartupPath + @"\LastLog.ini";
            Global.DefaultFileName = "default.ini";
            Global.ComputerName = TextUtils.GetHostName();
            //"NVTHAO"
            #region Check Update
            try
            {
                if (Global.ComputerName != "NVTHAO" && Global.ComputerName != "MrGrey" && Global.ComputerName != "DELL-NVTHAO")
                {
                    string pathServer = TextUtils.GetPathServer();
                    string fileNumberLocal = Application.StartupPath + "\\UpdateNumber.ini";
                    string fileNumberServer = pathServer + "\\UpdateNumber.ini";
                    if (!File.Exists(fileNumberLocal))
                    {
                        _isUpdated = true;
                    }
                    else
                    {
                        string valueLocal = File.ReadAllText(fileNumberLocal);
                        string valueServer = File.ReadAllText(fileNumberServer);
                        if (valueLocal != valueServer)
                        {
                            _isUpdated = true;
                        }
                    }
                }                
            }
            catch
            {
                _isUpdated = false;
            }

            #endregion check update            

            if (!_isUpdated)// && ConnectdToBD)
            {
                frmLogin frm = new frmLogin();
                frm.ShowDialog();
                //_loginOK = frm.loginSuccess;
                if (frm.loginSuccess == false)
                { Application.Exit(); return; }
            }
           
            InitializeComponent();            
        }

        #region Các sụ kiện liên quan đến phím tắt.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool handled = false;
            //if (keyData.ToString().Equals("Escape"))
            //    return true;
            if (keyData.ToString().Equals("Alt + F4"))
                Application.Exit();
            ShortcutKey.LoadFormShortcutKey(this, keyData.ToString(), ref handled);
            //return base.ProcessCmdKey(ref msg, keyData);
            return (handled || base.ProcessCmdKey(ref msg, keyData));
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            if (Application.StartupPath.Contains("Share"))
            {
                _isUpdated = false;
                this.Close();
            }

            if (_isUpdated)
            {
                this.Close();
            }

            timer1.Enabled = notifyIconPartWarning.Visible = notifyYCMVTwarning.Visible = Global.DepartmentID == 6;

            #region Thông báo
            //notifyIconPartWarning.Visible = Global.DepartmentID == 1;
            //if (Global.DepartmentID == 1)//phòng thiết kế
            //{
            //    //Theo dõi vật tư
            //    frmMainVatTu frm = new frmMainVatTu();
            //    frm.WindowState = FormWindowState.Minimized;
            //    TextUtils.OpenForm(frm);
            //    frm.Hide();
            //}

            ////Theo dõi công việc
            ////frmTheoDoiCongViec frm1 = new frmTheoDoiCongViec();
            //frmWorkDiaryManager frm1 = new frmWorkDiaryManager();
            //frm1.WindowState = FormWindowState.Minimized;
            //TextUtils.OpenForm(frm1);
            //frm1.Hide();
            #endregion

            PopupInformationSys();
            SetMainView();

            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\International", true);
                if (!rkey.GetValue("sShortDate", "MM/dd/yy").ToString().Contains("M/yyyy"))
                {
                    //if (MessageBox.Show("Định dạng ngày tháng trên máy của bạn không phải là định dạng của Việt Nam (ngày/tháng/năm - dd/MM/yyyy)\n Bạn có muốn đổi lại định dạng ngày tháng không ?", TextUtils.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //{
                    rkey.SetValue("sShortDate", "dd/MM/yyyy");
                    //}
                }
                rkey.Close();
            }
            catch
            {                
            }            

            DefValues.Sql_MinDate = new DateTime(1900, 01, 01);            
        }       

        private void CheckAutoUpdate()
        {
            try
            {
                if (!_isUpdated)
                    return;
                try
                {
                    this.Hide();
                    MessageBox.Show("Đã có phiên bản mới của phần mềm để cập nhật.\nBạn hãy chắc chắn rằng mình đang đăng nhập vào Windows với quyền Admin để update phần mềm", TextUtils.Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    using (Process compiler = new Process())
                    {
                        compiler.StartInfo.FileName = Application.StartupPath + @"\AutoUpdater.exe";
                        compiler.StartInfo.UseShellExecute = false;
                        compiler.StartInfo.RedirectStandardOutput = false;
                        compiler.Start();
                    }
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, TextUtils.Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Kết nối tới máy chủ thất bại!\nHãy liên hệ với quản trị hệ thống để được trợ giúp", TextUtils.Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, TextUtils.Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetMainView()
        {
            //switch (TextUtils.ToInt(Global.MainViewID))
            //{
                //case 2:
                //    ribbonControl1.SelectedPage = ribbonAccounting;
                //    break;
                //case 3:
                //    ribbonControl1.SelectedPage = ribbonBuildingManage;
                //    break;
                //default: //case 4:
                    //ribbonControl1.SelectedPage = ribbonWareHouse;
                    //break;
                //case 5:
                    ribbonControl1.SelectedPage = ribbonCSKH;
                //    break;
                //case 6:
                //    ribbonControl1.SelectedPage = ribbonHKP;
                //    break;
                //default:
                //     ribbonControl1.SelectedPage = ribbonDefault;
                //     break;
            //}
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            this.Close();
        }

        private void đĂToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnExit_Click(null, null);           
        }

        public void PopupInformationSys()
        {
            try
            {
                lblVersion.Text = string.Format("Phiên bản : {0}   |   ", CURRENT_VERSION);
                // Get Connection string.
                string[] _Conn = Global.ConnectionString.Split(';');
                lblServer.Text = _Conn[0].Split('=')[1].ToString().Trim() + " / " + _Conn[1].Split('=')[1].ToString().Trim() + "   |   ";

                lblBusinessDate.Text = "System Date: " + TextUtils.GetSystemDate().ToString("dd/MM/yyyy") + "   |  ";
                if (Global.IsNotCreateSession) { lblUser.Text = "Login By: " + Global.AppFullName + "  |  "; }
                else { lblUser.Text = "Login By: " + Global.AppFullName + " (Shift ID: " + Global.ShiftID + ")   |  "; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi" + ex.Message);
            }
        }

        private void btnExit_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn thoát chương trình không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.Close();
            else
                return;
        }

        private void mnu_Report_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //_Forms _frmReport = (_Forms)Application.TextUtils.OpenForms["frmReportMains"];
            //if (_frmReport == null)
            //    _frmReport = new frmReportMains();
            //_frmReport.Show();
            //if (_frmReport.WindowState == FormWindowState.Minimized)
            //    _frmReport.WindowState = _frmReport.VisibleFormState;
            //_frmReport.Activate();
            //using (frmReportMains frm = new frmReportMains())
            //{
            //    TextUtils.OpenForm(frm);
            //}
        }

        private void btnStaffManager_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmStaffManager frm = new frmStaffManager();
            TextUtils.OpenForm(frm); //TextUtils.TextUtils.OpenForm();
        }
       
        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Application.Exit();
            //TestReportForm frm = new TestReportForm();
            //frm.Show();
        }

        private void btnDepartment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDepartment frm = new frmDepartment();
            TextUtils.OpenForm(frm);
        }

        private void btnPermission_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPermissionManager frm = new frmPermissionManager();
            TextUtils.OpenForm(frm);
        }

        private void btnMakeRole_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmRole frm = new frmRole();
            TextUtils.OpenForm(frm);
        }

        bool _update = false;
        private void cậpNhậtPhầnMềmAutoUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có thực sự muốn update phần mềm không?",TextUtils.Caption,MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _update = true;
                this.Close();
            }            
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword();
            frm.ShowDialog();
        }

        private void btnPermission_Click(object sender, EventArgs e)
        {
            frmPermissionManager frm = new frmPermissionManager();
            TextUtils.OpenForm(frm);
        }

        private void btnMakeRole_Click(object sender, EventArgs e)
        {
            frmRole frm = new frmRole();
            TextUtils.OpenForm(frm);
        }

        private void btnProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmProjectManager frm = new frmProjectManager();
            TextUtils.OpenForm(frm);
        }

        private void btnProduct_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmProductManager frm = new frmProductManager();
            TextUtils.OpenForm(frm);
        }

        private void btnWorks_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmWorkManager frm = new frmWorkManager();
            frmWorkingDiariesManager frm = new frmWorkingDiariesManager();
            TextUtils.OpenForm(frm);
        }

        private void btnModuleManager_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmModuleManager frm = new frmModuleManager();
            TextUtils.OpenForm(frm);
        }       

        private void ConfigSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConfig frm = new frmConfig();
            TextUtils.OpenForm(frm);
        }

        private void btn3DDT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frm3DDienTuManager frm = new frm3DDienTuManager();
            //TextUtils.OpenForm(frm);
        }

        private void btnDien_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmFileEplanManager frm = new frmFileEplanManager();
            //TextUtils.OpenForm(frm);
        }

        private void btnMaterial_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (WaitDialogForm fWait = new WaitDialogForm("Vui lòng chờ trong giây lát...", "Đang load dữ liệu cho vật tư!"))
            {
                try
                {
                    //TextUtils.ExcuteProcedure("spDongBoMaterialQLSX_1", null, null);//update gía
                    //TextUtils.ExcuteProcedure("spDongBoMaterialQLSX", null, null);//update số ngày giao dịch, ngày giao dịch gần nhất
                }
                catch (Exception)
                {
                }
                frmMaterialManagement frm = new frmMaterialManagement();
                TextUtils.OpenForm(frm);
            }
        }

        private void btnMaterialReplace_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmVatTuThayThe frm = new frmVatTuThayThe();
            TextUtils.OpenForm(frm);
        }

        #region HỖ TRỢ THIẾT KẾ
        private void btnProfileSearch_Click(object sender, EventArgs e)
        {
            //frmCheckCAD frm = new frmCheckCAD();
            //TextUtils.OpenForm(frm);
        }

        private void btnCheckCTTK_Click(object sender, EventArgs e)
        {
            //frmCheckCauTruc frm = new frmCheckCauTruc(); 
            //TextUtils.OpenForm(frm);            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmTaoCTTK frm = new frmTaoCTTK();
            TextUtils.OpenForm(frm);
        }

        private void btnHSTK_Click(object sender, EventArgs e)
        {
            frmHSTK frm = new frmHSTK();
            TextUtils.OpenForm(frm);
        }

        private void btnCheckDataDesign_Click(object sender, EventArgs e)
        {
            frmCheckDataDesign frm = new frmCheckDataDesign();
            TextUtils.OpenForm(frm);
        }

        private void btnCheckFile3D_Click(object sender, EventArgs e)
        {
            frmCheck3D frm = new frmCheck3D();
            TextUtils.OpenForm(frm);
        }

        private void btnRenameScanFile_Click(object sender, EventArgs e)
        {
            frmRenameScanFile frm = new frmRenameScanFile();
            TextUtils.OpenForm(frm);
        }
        #endregion HỖ TRỢ THIẾT KẾ

        private void btnSuppliers_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCustomer frm = new frmCustomer();
            TextUtils.OpenForm(frm);
        }

        private void btnMaterialTracking_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmMainVatTu frm = new frmMainVatTu();
            TextUtils.OpenForm(frm);
        }

        private void btnTester_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Form1 frm = new Form1();
            //frm.Show();
            //if (Global.AppUserName.ToLower() == "thao.nv")
            //{
            Form2 frm = new Form2();
            TextUtils.OpenForm(frm);
            //}
            //if (Global.AppUserName.ToLower() == "hien.nt")
            //{                
            //}
         
        }          

        private void btnShowNhatKy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNhatKyCongViec frm = new frmNhatKyCongViec();
            TextUtils.OpenForm(frm);
        }

        private void btnBaoLoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBaoLoi frm = new frmBaoLoi();
            TextUtils.OpenForm(frm);
        }

        private void btnDistribution_Click(object sender, EventArgs e)
        {
            frmPhanBoDuLieu frm = new frmPhanBoDuLieu();
            TextUtils.OpenForm(frm);
        }

        private void btnTheoDoiCongViec_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmTheoDoiCongViec frm = new frmTheoDoiCongViec();
            frmWorkDiaryManager frm = new frmWorkDiaryManager();
            TextUtils.OpenForm(frm);
        }

        private void btnBaoCaoLoi_Click(object sender, EventArgs e)
        {
            frmBaoCaoLoi frm = new frmBaoCaoLoi();
            TextUtils.OpenForm(frm);
        }

        private void btnMisMatch_Click(object sender, EventArgs e)
        {
            frmMisMatchManager frm = new frmMisMatchManager();
            TextUtils.OpenForm(frm);
        }
       
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logOut = true;
            this.Close();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_logOut)
            {
                string path = Application.StartupPath + "\\DesignSupportSystem.exe";
                Process.Start(path);
            }
            if (_update || _isUpdated)
            {
                string path = Application.StartupPath + "\\UpdateVersion.exe";
                Process.Start(path);
            }   
        }

        private void btnCostGroup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmCostGroup frm = new frmCostGroup();
            //TextUtils.OpenForm(frm);
            frmCCostManagement frm = new frmCCostManagement();
            TextUtils.OpenForm(frm);
        }

        private void btnBaiTH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBaiThucHanhManger frm = new frmBaiThucHanhManger();
            TextUtils.OpenForm(frm);
        }

        private void btnCheckVT_Click(object sender, EventArgs e)
        {
            frmCheckVT frm = new frmCheckVT();
            TextUtils.OpenForm(frm);
        }

        private void btnSendTHTK_Click(object sender, EventArgs e)
        {
            frmSendMailTHTK frm = new frmSendMailTHTK();
            TextUtils.OpenForm(frm);
        }

        private void btnConvertModule_Click(object sender, EventArgs e)
        {
            frmConvertModule frm = new frmConvertModule();
            TextUtils.OpenForm(frm);
        }

        private void btnBaoGia_Click(object sender, EventArgs e)
        {
            frmBaoGiaManagement frm = new frmBaoGiaManagement();
            TextUtils.OpenForm(frm);
            //frmQuotationManagement frm = new frmQuotationManagement();
            //TextUtils.OpenForm(frm);
        }

        private void btnMaterialNon_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmMaterialNonManager frm = new frmMaterialNonManager();
            TextUtils.OpenForm(frm);
        }

        #region PHONG VAT TU
        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            frmListOrder frm = new frmListOrder();
            TextUtils.OpenForm(frm);
        }

        private void btnSetPriceYCMVT_Click(object sender, EventArgs e)
        {
            frmSetPriceYCMVT frm = new frmSetPriceYCMVT();
            TextUtils.OpenForm(frm);
        }

        private void btnShowSuppliers_Click(object sender, EventArgs e)
        {
            frmSupplierManager frm = new frmSupplierManager();
            TextUtils.OpenForm(frm);
        }

        private void btnCreateYCMVT_Click(object sender, EventArgs e)
        {
            frmCreateYCMVT frm = new frmCreateYCMVT();
            TextUtils.OpenForm(frm);
        }

        private void btnPartWarning_Click(object sender, EventArgs e)
        {
            if (Global.DepartmentID == 6)
            {
                frmPartWarning frm = new frmPartWarning();
                TextUtils.OpenForm(frm);
            }
        }

        public void showPopUp(string content, int type)
        {
            popupYCMVTwarning.TitleText = popupNotifierPartWarning.TitleText = "Thông Báo";
            popupYCMVTwarning.ShowCloseButton = popupNotifierPartWarning.ShowCloseButton = true;
            popupYCMVTwarning.ShowOptionsButton = popupNotifierPartWarning.ShowOptionsButton = false;
            popupYCMVTwarning.ShowGrip = popupNotifierPartWarning.ShowGrip = false;
            popupYCMVTwarning.BorderColor = popupNotifierPartWarning.BorderColor = Color.Green;
            popupYCMVTwarning.Delay = popupNotifierPartWarning.Delay = 5000;//thời gian popup hiển thị trên màn hình
            popupYCMVTwarning.AnimationInterval = popupNotifierPartWarning.AnimationInterval = 10;
            popupYCMVTwarning.AnimationDuration = popupNotifierPartWarning.AnimationDuration = 1000;
            popupYCMVTwarning.TitlePadding = popupNotifierPartWarning.TitlePadding = new Padding(0);
            popupYCMVTwarning.ContentPadding = popupNotifierPartWarning.ContentPadding = new Padding(0);
            popupYCMVTwarning.ImagePadding = popupNotifierPartWarning.ImagePadding = new Padding(0);
            popupYCMVTwarning.Scroll = popupNotifierPartWarning.Scroll = true;
            popupYCMVTwarning.Size = popupNotifierPartWarning.Size = new Size(400, 200);
            popupYCMVTwarning.TitleColor = popupNotifierPartWarning.TitleColor = Color.Red;
            
            if (type == 0)
            {
                popupNotifierPartWarning.ContentText = content;
                popupNotifierPartWarning.Popup();
            }
            if (type == 1)
            {
                popupYCMVTwarning.TitleText = content;
                popupYCMVTwarning.Popup();
            }
        }

        decimal countPart()
        {
            ConfigSystemModel model = (ConfigSystemModel)ConfigSystemBO.Instance.FindByAttribute("KeyName", "CanhBaoHanVeVT_period")[0];

            string sqlPart = "SELECT * FROM [vRequireBuyPart] with(nolock) where (status = 2) and [DateAboutE] is not null and ([DateAboutF] is null or [DateAboutF] = '')"
                        + " and datediff(dd,getdate(),DateAboutE)<= " + model.KeyValue + " and Account = '" + Global.AppUserName + "'";
            DataTable dtData = LibQLSX.Select(sqlPart);

            string sqlOut = "SELECT * FROM [vRequestOut] with(nolock) where ([ProposalStatus] = 2) and [DateAboutE] is not null and ([DateAboutF] is null or [DateAboutF] = '')"
                        + " and datediff(dd,getdate(),DateAboutE)<= " + model.KeyValue + " and Account = '" + Global.AppUserName + "'";
            DataTable dtOut = LibQLSX.Select(sqlOut);

            string sqlMaterial = "SELECT * FROM [vRequestMaterial] with(nolock) where ([ProposalStatus] = 2) and [DateAboutE] is not null and ([DateAboutF] is null or [DateAboutF] = '')"
                        + " and datediff(dd,getdate(),DateAboutE) <= " + model.KeyValue + " and Account = '" + Global.AppUserName + "'";
            DataTable dtMaterial = LibQLSX.Select(sqlMaterial);

            if (dtMaterial.Rows.Count > 0)
            {
                dtData.Merge(dtMaterial);
            }
            if (dtOut.Rows.Count > 0)
            {
                dtData.Merge(dtOut);
            }

            return dtData.Rows.Count;
        }

        decimal countYCMVT()
        {
            DataTable dt = new DataTable();
            if (Global.AppUserName == "khoi.pd")
            {
                dt = LibQLSX.Select("select * from vYCMVTwarning where datediff(day,DateCreate,getdate()) >= 7 and Status = 1 order by ProposalId desc");
            }
            else
            {
                dt = LibQLSX.Select("select * from vYCMVTwarning where Account = '" + Global.AppUserName
                + "' and datediff(day,DateCreate,getdate()) >= 7 and Status = 1 order by ProposalId desc");
            }    
            return dt.Rows.Count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Global.DepartmentID == 6)
                {
                    ConfigSystemModel model = (ConfigSystemModel)ConfigSystemBO.Instance.
                        FindByAttribute("KeyName", "CanhBaoHanVeVT")[0];

                    List<string> listTime = model.KeyValue.Split(',').ToList();
                    string now = DateTime.Now.ToString("HH:mm:ss");
                    if (listTime.Contains(now))
                    {
                        decimal countVT = countPart();
                        decimal countYC = countYCMVT();

                        if (countVT > 0)
                        {
                            showPopUp("Có " + countVT + " vật tư sắp đến ngày về.", 0);
                        }

                        if (countYC > 0)
                        {
                            showPopUp("Có " + countYC + " YCMVT đã quá hạn mà chưa tạo PO.", 1);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void notifyIconPartWarning_DoubleClick(object sender, EventArgs e)
        {
            if (Global.DepartmentID == 6)
            {
                frmPartWarning frm = new frmPartWarning();
                frm.WindowState = FormWindowState.Maximized;
                TextUtils.OpenForm(frm);
            }
        }

        private void popupNotifierPartWarning_Click(object sender, EventArgs e)
        {
            if (Global.DepartmentID == 6)
            {
                frmPartWarning frm = new frmPartWarning();
                frm.WindowState = FormWindowState.Maximized;
                TextUtils.OpenForm(frm);
            }
        }

        private void popupYCMVTwarning_Click(object sender, EventArgs e)
        {
            if (Global.DepartmentID == 6)
            {
                frmYCMVTwarning frm = new frmYCMVTwarning();
                //frm.WindowState = FormWindowState.Maximized;
                TextUtils.OpenForm(frm);
            }
        }

        private void notifyYCMVTwarning_DoubleClick(object sender, EventArgs e)
        {
            if (Global.DepartmentID == 6)
            {
                frmYCMVTwarning frm = new frmYCMVTwarning();
                //frm.WindowState = FormWindowState.Maximized;
                TextUtils.OpenForm(frm);
            }
        }

        private void bÁOGIÁToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBaoGiaManagement frm = new frmBaoGiaManagement();
            TextUtils.OpenForm(frm);
        }
        #endregion PHONG VAT TU

        private void btnImportModule_Click(object sender, EventArgs e)
        {
            frmImportModuleToProject frm = new frmImportModuleToProject();
            TextUtils.OpenForm(frm);
        }

        private void btnImportProduct_Click(object sender, EventArgs e)
        {
            frmImportProducts frm = new frmImportProducts();
            TextUtils.OpenForm(frm);
        }

        private void btnBangKeThanhToan_Click(object sender, EventArgs e)
        {
            frmPaymentTableManager frm = new frmPaymentTableManager(); 
            TextUtils.OpenForm(frm);
        }

        private void btnBieuMauKT_Click(object sender, EventArgs e)
        {
            frmBieuMauKyThuat frm = new frmBieuMauKyThuat();
            TextUtils.OpenForm(frm);
        }

        private void kHÁCHHÀNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCustomerManagerKD frm = new frmCustomerManagerKD();
            TextUtils.OpenForm(frm);
        }

        private void yÊUCẦUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRequestManager frm = new frmRequestManager();
            TextUtils.OpenForm(frm);
        }

        private void bÁOGIÁToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmQuotationManagement frm = new frmQuotationManagement();
            TextUtils.OpenForm(frm);
        }

        private void xUẤTGIÁVẬTTƯCHOYCMVTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGetPriceForYCMVT frm = new frmGetPriceForYCMVT();
            TextUtils.OpenForm(frm);
        }

        private void btnErrorAndNewFeature_Click(object sender, EventArgs e)
        {
            frmErrorAndFeatureMonitor frm = new frmErrorAndFeatureMonitor();
            TextUtils.OpenForm(frm);
        }

        private void xEMGIÁVẬTTƯToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmXemGiaVatTu frm = new frmXemGiaVatTu();
            TextUtils.OpenForm(frm);
        }

        private void quảnLýCơCấuMẫuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCoCauMauManagement frm = new frmCoCauMauManagement();
            TextUtils.OpenForm(frm);
        }

        private void xemDanhSáchĐNXKCủaVậtTưToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmViewTranOfParts frm = new frmViewTranOfParts();
            TextUtils.OpenForm(frm);
        }

        private void xemDanhSáchĐNNKCủaSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmViewTranOfProduct frm = new frmViewTranOfProduct();
            TextUtils.OpenForm(frm);
        }

        private void pHÂNQUYỀNTHEONHÓMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPermissionGroup frm = new frmPermissionGroup();
            TextUtils.OpenForm(frm);
        }

        private void btnShowYeuCau_Click(object sender, EventArgs e)
        {
            frmRequireManagement frm = new frmRequireManagement();
            TextUtils.OpenForm(frm);
        }

        private void btnShowGiaiPhap_Click(object sender, EventArgs e)
        {
            frmSolutionManagement frm = new frmSolutionManagement();
            TextUtils.OpenForm(frm);
        }

        private void đNNKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmImportRequrieManager frm = new frmImportRequrieManager();
            //TextUtils.OpenForm(frm);
        }

        private void quảnLýLỗiVậtTưThiếtBịKCSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowKcsNG frm = new frmShowKcsNG();
            TextUtils.OpenForm(frm);
        }

        private void xemNhậtKýCôngViệcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowWorkHistoryKCS frm = new frmShowWorkHistoryKCS();
            TextUtils.OpenForm(frm);
        }

        private void btnPartBorrow_Click(object sender, EventArgs e)
        {
            frmPartBorrowManager frm = new frmPartBorrowManager();
            TextUtils.OpenForm(frm);
        }

        private void đỀNGHỊNHẬPKHOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmImportRequrieManager frm = new frmImportRequrieManager();
            TextUtils.OpenForm(frm);
        }

        private void yCMVTĐẾNHẠNTẠOPOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmYCMVTwarning frm = new frmYCMVTwarning();
            frmYCMVTwarningNew frm = new frmYCMVTwarningNew();
            TextUtils.OpenForm(frm);
        }

        private void vẬTTƯHÀNGHÓAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMaterialManagerQLSX frm = new frmMaterialManagerQLSX();
            TextUtils.OpenForm(frm);
        }

        private void yÊUCẦUVẬTTƯToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmYCVTmanager frm = new frmYCVTmanager();
            TextUtils.OpenForm(frm);
        }

        private void tạoCấuTrúcGiảiPhápToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSolutionCTTM frm = new frmSolutionCTTM();
            TextUtils.OpenForm(frm);
        }

        private void btnProjectProblemManager_Click(object sender, EventArgs e)
        {
            frmProjectProblemManager frm = new frmProjectProblemManager();
            TextUtils.OpenForm(frm);
        }

        private void btnCaseManagement_Click(object sender, EventArgs e)
        {
            frmVoucherDebtManagement frm = new frmVoucherDebtManagement();
            TextUtils.OpenForm(frm);
        }

        private void tỔNGHỢPTHIẾTBỊDỰÁNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEquipmentManagement frm = new frmEquipmentManagement();
            TextUtils.OpenForm(frm);
        }

        private void thêmNhómVậtTưVàoDMVTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                
                using (WaitDialogForm fWait = new WaitDialogForm("Vui lòng chờ trong giây lát...",
                        "Đang thêm nhóm vật tư vào DMVT..."))
                {
                    Excel.Application objXLApp = default(Excel.Application);
                    Excel.Workbook objXLWb = default(Excel.Workbook);
                    Excel.Worksheet objXLWs = default(Excel.Worksheet);

                    try
                    {
                        objXLApp = new Excel.Application();
                        objXLApp.Workbooks.Open(filePath);
                        objXLWb = objXLApp.Workbooks[1];
                        objXLWs = (Excel.Worksheet) objXLWb.Worksheets[1];

                        DataTable dtDMVT = TextUtils.ExcelToDatatableNoHeader(filePath, "DMVT");
                        dtDMVT = dtDMVT.AsEnumerable()
                            .Where(row => TextUtils.ToInt(row.Field<string>("F1") == "" ||
                                row.Field<string>("F1") == null ? "0" : row.Field<string>("F1").Substring(0, 1)) > 0)
                            .CopyToDataTable();
                        //L-12;M-13
                        for (int i = 0; i < dtDMVT.Rows.Count; i++)
                        {
                            string partCode = TextUtils.ToString(dtDMVT.Rows[i][3]);
                            if (partCode == "" || partCode.StartsWith("TPAD."))
                            {
                                continue;
                            }
                            DataTable dtGroup =
                                TextUtils.Select("select * from [vMaterial] with(nolock) where Code = '" + partCode + "'");
                            string groupCode = "";
                            string groupName = "";
                            if (dtGroup.Rows.Count > 0)
                            {
                                groupCode = TextUtils.ToString(dtGroup.Rows[0]["MaterialGroupCode"]);
                                groupName = TextUtils.ToString(dtGroup.Rows[0]["MaterialGroupName"]);

                                objXLWs.Cells[7 + i, 12] = groupCode;
                                objXLWs.Cells[7 + i, 13] = groupName;
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        objXLApp.ActiveWorkbook.Save();
                        objXLApp.Workbooks.Close();
                        objXLApp.Quit();
                    }
                }
            }
        }

        private void dANHSÁCHYCMVTCHẬMTIẾNĐỘToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void bÁOCÁOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportMaterial frm = new frmReportMaterial();
            TextUtils.OpenForm(frm);
        }

        private void btnShowNXT_Click(object sender, EventArgs e)
        {
            frmNXT frm = new frmNXT();
            TextUtils.OpenForm(frm);
        }

        private void nHÓMSẢNPHẨMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCProductGroup frm = new frmCProductGroup();
            TextUtils.OpenForm(frm);
        }

        private void cHIPHÍToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCCostManagement frm = new frmCCostManagement();
            TextUtils.OpenForm(frm);
        }

        private void danhSáchThiếtBịCủaDựÁnDạngCâyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void danhSáchModuleCủaDựÁnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListModuleOfProjectTPAD frm = new frmListModuleOfProjectTPAD();
            TextUtils.OpenForm(frm);
        }

        private void cấuHìnhThờiGianLàmChoVậtTưToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConfigTimeProjectDirectionType frm = new frmConfigTimeProjectDirectionType();
            TextUtils.OpenForm(frm);
        }

        private void danhSáchChỉThịToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProjectDirectionManagement frm = new frmProjectDirectionManagement();
            TextUtils.OpenForm(frm);
        }

        private void btnShowPartAccessories_Click(object sender, EventArgs e)
        {
            frmPartAccessoriesManager frm = new frmPartAccessoriesManager();
            TextUtils.OpenForm(frm);
        }

        private void btnConfigVTP_Click(object sender, EventArgs e)
        {
            frmPartsConfigLink frm = new frmPartsConfigLink();
            TextUtils.OpenForm(frm);
        }

        private void btnGetVTPofProject_Click(object sender, EventArgs e)
        {
            frmGetVTPofProject frm = new frmGetVTPofProject();
            TextUtils.OpenForm(frm);
        }

        private void tổngHợpVậtTưDựÁnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGetPartOfProject frm = new frmGetPartOfProject();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoTiếnĐộToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void báoCáoTiếnĐộTheoDựaÁnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportProgressProject frm = new frmReportProgressProject();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoTiếnĐộTheoThiếtBịToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListModuleOfProject frm = new frmListModuleOfProject();
            TextUtils.OpenForm(frm);
        }

        private void kIỂMTRAHÓAĐƠNDỰÁNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCheckInvoice frm = new frmCheckInvoice();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoChiPhíToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportChiPhi frm = new frmReportChiPhi();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoDoanhThuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportDoanhThu frm = new frmReportDoanhThu();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoTổngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportChiPhi_DoanhThu frm = new frmReportChiPhi_DoanhThu();
            TextUtils.OpenForm(frm);
        }

        private void cẤUHÌNHTỶLỆPHÂNBỔToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSetRateOfPhanXuong_KMP frm = new frmSetRateOfPhanXuong_KMP();
            TextUtils.OpenForm(frm);
        }

        private void iMPORTTHÔNGTINFCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmImportInfoFCM frm = new frmImportInfoFCM();
            TextUtils.OpenForm(frm);
        }

        private void cẤUHÌNHCHIPHÍPHÒNGBANToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPhanXuong_KMP_Link frm = new frmPhanXuong_KMP_Link();
            TextUtils.OpenForm(frm);
        }

        private void qUẢNLÝNHÓMKHOẢNMỤCPHÍToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmKMP_GROUP_management frm = new frmKMP_GROUP_management();
            TextUtils.OpenForm(frm);
        }

        private void qUẢNLÝFCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFCMmanagement frm = new frmFCMmanagement();
            TextUtils.OpenForm(frm);
        }

        private void chiTiếtDoanhThuPhòngBanTheoFCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportFCMDepartment frm = new frmReportFCMDepartment();
            TextUtils.OpenForm(frm);
        }

        private void báoCáoChiPhíDoanhThuTheoPhòngBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportChiPhi_DoanhThu_PhongBan frm = new frmReportChiPhi_DoanhThu_PhongBan();
            TextUtils.OpenForm(frm);
        }

        private void tổngHợpChiPhíCủaFCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmReportChiPhi_FCM frm = new frmReportChiPhi_FCM();
            frmReportChiPhi_FCM_SX frm = new frmReportChiPhi_FCM_SX();
            TextUtils.OpenForm(frm);
        }

        private void bÁOGIÁTESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmQuotationManagementKD frm = new frmQuotationManagementKD();
            TextUtils.OpenForm(frm);
        }

        private void xUẤTLƯƠNGNHÂNVIÊNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void qUẢNLÝVẤNĐỀDỰÁNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProjectProblemManager frm = new frmProjectProblemManager();
            TextUtils.OpenForm(frm);
        }

        private void tỔNGHỢPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTongHopChucNangKeToan frm = new frmTongHopChucNangKeToan();
            TextUtils.OpenForm(frm);
        }

        private void tổngHợpChiPhíCủaFCMKinhDoanhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReportChiPhi_FCM_KD frm = new frmReportChiPhi_FCM_KD();
            TextUtils.OpenForm(frm);
        }
    }
}