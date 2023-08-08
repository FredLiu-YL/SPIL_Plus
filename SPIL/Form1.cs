using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;//https://jerry5217.pixnet.net/blog/post/312240331
using System.Management;
using Microsoft.VisualBasic.FileIO;
using YuanliCore.ImageProcess.Caliper;
using YuanliCore.ImageProcess;
using YuanliCore.Interface;
using SPIL.model;
using YuanliCore.ImageProcess.Match;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using SPIL.Model;
using YuanliCore.Communication;
using System.Windows.Threading;

namespace SPIL
{
    public partial class Form1 : Form
    {
        private Logger logger = new Logger("SPIL");
        private Bitmap sharpnessImage;
        private Bitmap aoiImage1;
        private Bitmap aoiImage2;
        private Bitmap aoiImage3;
        private string password = "11084483";
        private string sharpnessImagesFolder = "D:\\SharpnessImages";
        private HostCommunication hostCommunication;
    
        private string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SPILmachine";
        private MachineSetting machineSetting { get; set; } = new MachineSetting();
        private SPILRecipe sPILRecipe { get; set; }
        private AOIFlow aoIFlow;
        private SharpnessFlow sharpnessFlow;

        public Form1()
        {
            InitializeComponent();

        }



        #region Var
        string Setup_Data_address = System.Windows.Forms.Application.StartupPath + "\\Setup\\Setup_Data.xml";
        Variable_Data variable_data;
        static int auto_log_out_Delay = 120;//sec
        int auto_log_out_times = 10;
        int now_delay = 0;
        bool log_in = false;


        char[] can_key_in_data = { 'R', 'D', 'W', '_', '-', '.', 'T', '*', 'F', 'S' };

        string Save_File_Address = "";
        string Save_File_Folder = "";
        Image I_Red = new Bitmap(SPIL.Properties.Resources.Red);
        Image I_Green = new Bitmap(SPIL.Properties.Resources.Green);
        DirectoryInfo folder_info;
        static int button_click_Delay = 5;//sec
        int button_click_times = 10;
        int now_button_click_delay = 0;
        //    int OLS_Initial_Now_Step = 0;
        int[,] point_data_write_0 = new int[2, 9];
        int[,] point_data_write_45 = new int[2, 9];
        int[] recipe_data_Write = new int[2];
        int[] Wafer_ID_data_Write = new int[2];
        int[] Date_data_Write = new int[2];
        int[] Time_data_Write = new int[2];
        int[] Slot_data_Write = new int[2];

        string write_date;
        string write_time;
        SPILBumpMeasure AOI_Measurement, Hand_Measurement;
        int check_delete_time = 0;//刪除超過檔案時間(天數)
        bool open_hide_1 = false;
        bool open_hide_1_Old = false;
        bool open_hide_2 = false;
        bool open_hide_2_Old = false;
        object sender1;
        EventArgs e1;
        bool copy_poir_once = false;
        //   int count = 1; //執行toolblock前存圖計數參數
        string[] Save_AOI_file_name = new string[3];
        string img_file_last_name = "COLOR2D"; //AOI檢測時讀取檔名檢查
        bool is_hand_measurement = false; //手動模式
        bool is_test_mode = false;
        TextBox[] textBoxes_0_1_20 = new TextBox[21];
        TextBox[] textBoxes_CuNi_1_20 = new TextBox[21];
        TextBox[] textBoxes_Cu_1_20 = new TextBox[21];
        #endregion

        #region socket server
        List<string> ethernet_card = new List<string>();
        //   Socket Socketserver_OLS;
        //    Socket Socketserver_Motion;
        //  Socket clientSocket_OLS;
        //   Socket clientSocket_Motion;
        bool connect_OLS_client = false;
        bool connect_Motion_client = false;
        #endregion

        #region Sub Function
        private void combine_text_box()
        {
            textBoxes_0_1_20[1] = textBox_Mesument_1_0;
            textBoxes_0_1_20[2] = textBox_Mesument_2_0;
            textBoxes_0_1_20[3] = textBox_Mesument_3_0;
            textBoxes_0_1_20[4] = textBox_Mesument_4_0;
            textBoxes_0_1_20[5] = textBox_Mesument_5_0;
            textBoxes_0_1_20[6] = textBox_Mesument_6_0;
            textBoxes_0_1_20[7] = textBox_Mesument_7_0;
            textBoxes_0_1_20[8] = textBox_Mesument_8_0;
            textBoxes_0_1_20[9] = textBox_Mesument_9_0;
            textBoxes_0_1_20[10] = textBox_Mesument_10_0;
            textBoxes_0_1_20[11] = textBox_Mesument_11_0;
            textBoxes_0_1_20[12] = textBox_Mesument_12_0;
            textBoxes_0_1_20[13] = textBox_Mesument_13_0;
            textBoxes_0_1_20[14] = textBox_Mesument_14_0;
            textBoxes_0_1_20[15] = textBox_Mesument_15_0;
            textBoxes_0_1_20[16] = textBox_Mesument_16_0;
            textBoxes_0_1_20[17] = textBox_Mesument_17_0;
            textBoxes_0_1_20[18] = textBox_Mesument_18_0;
            textBoxes_0_1_20[19] = textBox_Mesument_19_0;
            textBoxes_0_1_20[20] = textBox_Mesument_20_0;
            //
            textBoxes_CuNi_1_20[1] = textBox_Mesument_1_45;
            textBoxes_CuNi_1_20[2] = textBox_Mesument_2_45;
            textBoxes_CuNi_1_20[3] = textBox_Mesument_3_45;
            textBoxes_CuNi_1_20[4] = textBox_Mesument_4_45;
            textBoxes_CuNi_1_20[5] = textBox_Mesument_5_45;
            textBoxes_CuNi_1_20[6] = textBox_Mesument_6_45;
            textBoxes_CuNi_1_20[7] = textBox_Mesument_7_45;
            textBoxes_CuNi_1_20[8] = textBox_Mesument_8_45;
            textBoxes_CuNi_1_20[9] = textBox_Mesument_9_45;
            textBoxes_CuNi_1_20[10] = textBox_Mesument_10_45;
            textBoxes_CuNi_1_20[11] = textBox_Mesument_11_45;
            textBoxes_CuNi_1_20[12] = textBox_Mesument_12_45;
            textBoxes_CuNi_1_20[13] = textBox_Mesument_13_45;
            textBoxes_CuNi_1_20[14] = textBox_Mesument_14_45;
            textBoxes_CuNi_1_20[15] = textBox_Mesument_15_45;
            textBoxes_CuNi_1_20[16] = textBox_Mesument_16_45;
            textBoxes_CuNi_1_20[17] = textBox_Mesument_17_45;
            textBoxes_CuNi_1_20[18] = textBox_Mesument_18_45;
            textBoxes_CuNi_1_20[19] = textBox_Mesument_19_45;
            textBoxes_CuNi_1_20[20] = textBox_Mesument_20_45;
            //
            textBoxes_Cu_1_20[1] = textBox_Mesument_1_Cu;
            textBoxes_Cu_1_20[2] = textBox_Mesument_2_Cu;
            textBoxes_Cu_1_20[3] = textBox_Mesument_3_Cu;
            textBoxes_Cu_1_20[4] = textBox_Mesument_4_Cu;
            textBoxes_Cu_1_20[5] = textBox_Mesument_5_Cu;
            textBoxes_Cu_1_20[6] = textBox_Mesument_6_Cu;
            textBoxes_Cu_1_20[7] = textBox_Mesument_7_Cu;
            textBoxes_Cu_1_20[8] = textBox_Mesument_8_Cu;
            textBoxes_Cu_1_20[9] = textBox_Mesument_9_Cu;
            textBoxes_Cu_1_20[10] = textBox_Mesument_10_Cu;
            textBoxes_Cu_1_20[11] = textBox_Mesument_11_Cu;
            textBoxes_Cu_1_20[12] = textBox_Mesument_12_Cu;
            textBoxes_Cu_1_20[13] = textBox_Mesument_13_Cu;
            textBoxes_Cu_1_20[14] = textBox_Mesument_14_Cu;
            textBoxes_Cu_1_20[15] = textBox_Mesument_15_Cu;
            textBoxes_Cu_1_20[16] = textBox_Mesument_16_Cu;
            textBoxes_Cu_1_20[17] = textBox_Mesument_17_Cu;
            textBoxes_Cu_1_20[18] = textBox_Mesument_18_Cu;
            textBoxes_Cu_1_20[19] = textBox_Mesument_19_Cu;
            textBoxes_Cu_1_20[20] = textBox_Mesument_20_Cu;
        }
        //Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            int counter = 0;


            //防止開啟第二次
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1) { this.Close(); }

            logger.WriteLog("Start Program");
            auto_log_out_times = auto_log_out_Delay * 1000 / timer_Log_in_Out.Interval;
            if (File.Exists(Setup_Data_address))
            {
                Load_Setup_Data();
            }
            else
            {
                Initial_Setup_Data();
            }

            //當測試檔案存在時
            //test mode
            if (File.Exists("test.txt"))
            {
                is_test_mode = true;
            }


            // 演算法 模組
            string configFile = $"{systemPath}\\machineConfig.cfg";
            if (!File.Exists(configFile))
            {
                machineSetting.Save(configFile);
            }
            else
            {
                machineSetting = MachineSetting.Load(configFile);
            }
            if (machineSetting.AOIVppPath != null)
            {

                tbx_AOIPath.Text = machineSetting.AOIVppPath;
                tbx_SharpPath.Text = machineSetting.SharpVppPath;
                tBx_RecipeName.Text = "Default";
                aoIFlow = new AOIFlow(machineSetting.AOIVppPath, machineSetting.AOIAlgorithms, logger);
                sharpnessFlow = new SharpnessFlow(machineSetting.SharpVppPath, machineSetting.SharpAlgorithms);
                sPILRecipe = new SPILRecipe(machineSetting.AOIAlgorithms, machineSetting.SharpAlgorithms);
                //預設把 toolBlock 的參數先拿來用
                sPILRecipe.AOIParams = aoIFlow.CogMethods.Select(m => m.method.RunParams).ToList();
                sPILRecipe.ClarityParams = sharpnessFlow.CogMethods.Select(m => m.method.RunParams).ToList();

                //新增到UI 做顯示
                foreach (var item in machineSetting.AOIAlgorithms)
                {
                    listBox_AOIAlgorithmList.Items.Add(item);
                }

                //新增到UI 做顯示
                foreach (var item in machineSetting.SharpAlgorithms)
                {
                    listBox_SharpnessAlgorithmList.Items.Add(item);
                }

                cogRecordDisplay2.AutoFit = true;
                cogRecordDisplay2.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.Touch;

                cogRcdDisp_Distance1.AutoFit = true;
                cogRcdDisp_Distance1.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.Touch;

            }




            //開啟socket server
            //取得此電腦上ip位置
            Search_Ethernet_Card();
            for (int i = 0; i < ethernet_card.Count; i++)
            {
                Search_IP(i);
            }


            //選擇一個乙太卡開啟socket server
            if (comboBox_IP.Items.Count == 0)
            {
                return;
            }
            comboBox_IP.SelectedIndex = 0;
            comboBox_IP_Motion.SelectedIndex = 0;

            button_Connect_Click(sender, e);
            button_Start_Server_Click(sender, e);
            button_Start_Click(sender, e);
            combine_text_box();



            if (is_test_mode)
            {
                logger.WriteLog("test mode");
                groupBox_test_item.Visible = true;
                string vpp_file_test_path = "";
                StreamReader file = new StreamReader("test.txt");
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0)
                    {
                        if (line == "0")
                        {
                            radioButton_Degree_0.Checked = true;
                        }
                        else if (line == "45")
                        {
                            radioButton_Degree_45.Checked = true;
                        }
                    }
                    else if (counter == 1)
                    {
                        vpp_file_test_path = line;
                    }
                    logger.WriteLog(line);
                    counter++;
                }
                file.Close();

                AOI_Measurement = new SPILBumpMeasure(vpp_file_test_path);
                ////綁定cogRecordDisplay 用來存toolblock結果圖
                AOI_Measurement.cogRecord_save_result_img = cogRecordDisplay1;
                AOI_Measurement.CogDisplay_result_1 = cogDisplay1;
                AOI_Measurement.CogDisplay_result_2 = cogDisplay2;
                AOI_Measurement.CogDisplay_result_3 = cogDisplay3;
                AOI_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                AOI_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                AOI_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                AOI_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                AOI_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                AOI_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;
                //載入手動量測
                Hand_Measurement = new SPILBumpMeasure("Setup//Vision//Hand_Measurement.vpp");
                Hand_Measurement.cogRecord_save_result_img = cogRecordDisplay1;
                Hand_Measurement.CogDisplay_result_1 = cogDisplay1;
                Hand_Measurement.CogDisplay_result_2 = cogDisplay2;
                Hand_Measurement.CogDisplay_result_3 = cogDisplay3;
                Hand_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                Hand_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                Hand_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                Hand_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                Hand_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                Hand_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;



            }
            else
            {
                groupBox_test_item.Visible = false;
            }
        }
        private void Load_Setup_Data()
        {
            logger.WriteLog("Load Parameter");
            variable_data = new Variable_Data(Setup_Data_address);
            Show_Data();

            if (variable_data.delete_data_setting != -1)
            {
                DeleteDataSetting(variable_data.delete_data_setting);
            }
            if (variable_data.Degree_height_A[0] != "")
            {
                textBox_0_degree_height_A.Text = variable_data.Degree_height_A[0];
            }
            if (variable_data.Degree_Num[0] != "")
            {
                textBox_0_degree_height_Num.Text = variable_data.Degree_Num[0];
            }
            // test
            folder_info = new DirectoryInfo(variable_data.OLS_Folder);
            //folder_info = new DirectoryInfo(@"D:\test\");
            //
            logger.WriteLog("Load Parameter Successful");
        }
        private void Initial_Setup_Data()
        {
            logger.WriteErrorLog("Parameter File Not Found");
            textBox_45_Ratio.Text = "1";

        }
        private void Show_Data()
        {
            //Motion Server
            textBox_Port.Text = Convert.ToString(variable_data.IP_Port);
            //AOI
            textBox_Vision_Pro_File.Text = variable_data.Vision_Pro_File;
            //OLS
            textBox_OLS_Program_Name.Text = variable_data.OLS_Name;
            textBox_Windows_Name.Text = variable_data.Windows_Name;
            textBox_OLS_Folder.Text = variable_data.OLS_Folder;
            textBox_Cover_Start_X1.Text = Convert.ToString(variable_data.Cover_Start_X1);
            textBox_Cover_Start_Y1.Text = Convert.ToString(variable_data.Cover_Start_Y1);
            textBox_Cover_End_X1.Text = Convert.ToString(variable_data.Cover_End_X1);
            textBox_Cover_End_Y1.Text = Convert.ToString(variable_data.Cover_End_Y1);
            textBox_Cover_Start_X2.Text = Convert.ToString(variable_data.Cover_Start_X2);
            textBox_Cover_Start_Y2.Text = Convert.ToString(variable_data.Cover_Start_Y2);
            textBox_Cover_End_X2.Text = Convert.ToString(variable_data.Cover_End_X2);
            textBox_Cover_End_Y2.Text = Convert.ToString(variable_data.Cover_End_Y2);
            checkBox_Step_1.Checked = variable_data.Initial_Step_1;
            textBox_Step_1_X.Text = Convert.ToString(variable_data.Initial_Step_1_X);
            textBox_Step_1_Y.Text = Convert.ToString(variable_data.Initial_Step_1_Y);
            checkBox_Step_2.Checked = variable_data.Initial_Step_2;
            textBox_Step_2_X.Text = Convert.ToString(variable_data.Initial_Step_2_X);
            textBox_Step_2_Y.Text = Convert.ToString(variable_data.Initial_Step_2_Y);
            checkBox_Step_3.Checked = variable_data.Initial_Step_3;
            textBox_Step_3_X.Text = Convert.ToString(variable_data.Initial_Step_3_X);
            textBox_Step_3_Y.Text = Convert.ToString(variable_data.Initial_Step_3_Y);
            checkBox_Step_4.Checked = variable_data.Initial_Step_4;
            textBox_step4_Delay.Text = Convert.ToString(variable_data.Initial_Step_4_Delay_Time);
            checkBox_Step_5.Checked = variable_data.Initial_Step_5;
            textBox_Step_5_X.Text = Convert.ToString(variable_data.Initial_Step_5_X);
            textBox_Step_5_Y.Text = Convert.ToString(variable_data.Initial_Step_5_Y);
            checkBox_Step_6.Checked = variable_data.Initial_Step_6;
            textBox_step6_Delay.Text = Convert.ToString(variable_data.Initial_Step_6_Delay_Time);
            checkBox_bmp.Checked = variable_data.Need_Move_bmp;
            checkBox_poir.Checked = variable_data.Need_Move_poir;
            checkBox_xlsx.Checked = variable_data.Need_Move_xlsx;
            checkBox_csv.Checked = variable_data.Need_Move_csv;
            //Excel File
            textBox_Excel_File_Path_1.Text = variable_data.Save_File_Path_1;
            textBox_Excel_File_Path_2.Text = variable_data.Save_File_Path_2;
            textBox_Excel_File_Path_3.Text = variable_data.Save_File_Path_3;
            textBox_Excel_File_Name.Text = variable_data.Save_File_Name;
            textBox_Excel_File_Name.Text = variable_data.Save_File_Name;
            //
            textBox_45_Ratio.Text = Convert.ToString(variable_data.Degree_Ratio);
            //
            numericUpDown_AOI_save_idx1.Value = variable_data.AOI_save_idx_1;
            numericUpDown_AOI_save_idx2.Value = variable_data.AOI_save_idx_2;
            numericUpDown_AOI_save_idx3.Value = variable_data.AOI_save_idx_3;
            //
            numericUpDown_manual_save_idx1.Value = variable_data.manual_save_idx_1;
            numericUpDown_manual_save_idx2.Value = variable_data.manual_save_idx_2;
            numericUpDown_manual_save_idx3.Value = variable_data.manual_save_idx_3;
            //
            textBox_hand_measure_X.Text = variable_data.hand_measurement_X.ToString();
            textBox_hand_measure_Y.Text = variable_data.hand_measurement_Y.ToString();
            textBox_hand_measure_H.Text = variable_data.hand_measurement_H.ToString();
            textBox_hand_measure_W.Text = variable_data.hand_measurement_W.ToString();
            Hide_Key_In_0();
        }
        private void Hide_Key_In_0()
        {
            //
            textBox_Mesument_1_0.Enabled = true;
            textBox_Mesument_2_0.Enabled = true;
            textBox_Mesument_3_0.Enabled = true;
            textBox_Mesument_4_0.Enabled = true;
            textBox_Mesument_5_0.Enabled = true;
            textBox_Mesument_6_0.Enabled = true;
            textBox_Mesument_7_0.Enabled = true;
            textBox_Mesument_8_0.Enabled = true;
            textBox_Mesument_9_0.Enabled = true;
            //20211224-S
            textBox_Mesument_10_0.Enabled = true;
            textBox_Mesument_11_0.Enabled = true;
            textBox_Mesument_12_0.Enabled = true;
            textBox_Mesument_13_0.Enabled = true;
            textBox_Mesument_14_0.Enabled = true;
            textBox_Mesument_15_0.Enabled = true;
            textBox_Mesument_16_0.Enabled = true;
            textBox_Mesument_17_0.Enabled = true;
            textBox_Mesument_18_0.Enabled = true;
            textBox_Mesument_19_0.Enabled = true;
            textBox_Mesument_20_0.Enabled = true;
            //20211224-E
            //
            //if (textBox_Point_1_0_A.Text == "" || textBox_Point_1_45_A.Text == "")
            //    textBox_Mesument_1_0.Enabled = false;
            //if (textBox_Point_2_0_A.Text == "" || textBox_Point_2_45_A.Text == "")
            //    textBox_Mesument_2_0.Enabled = false;
            //if (textBox_Point_3_0_A.Text == "" || textBox_Point_3_45_A.Text == "")
            //    textBox_Mesument_3_0.Enabled = false;
            //if (textBox_Point_4_0_A.Text == "" || textBox_Point_4_45_A.Text == "")
            //    textBox_Mesument_4_0.Enabled = false;
            //if (textBox_Point_5_0_A.Text == "" || textBox_Point_5_45_A.Text == "")
            //    textBox_Mesument_5_0.Enabled = false;
            //if (textBox_Point_6_0_A.Text == "" || textBox_Point_6_45_A.Text == "")
            //    textBox_Mesument_6_0.Enabled = false;
            //if (textBox_Point_7_0_A.Text == "" || textBox_Point_7_45_A.Text == "")
            //    textBox_Mesument_7_0.Enabled = false;
            //if (textBox_Point_8_0_A.Text == "" || textBox_Point_8_45_A.Text == "")
            //    textBox_Mesument_8_0.Enabled = false;
            //if (textBox_Point_9_0_A.Text == "" || textBox_Point_9_45_A.Text == "")
            //    textBox_Mesument_9_0.Enabled = false;
        }
        private void Send_Server(string Send_Data)
        {
            try
            {

                hostCommunication.Send(Send_Data);
                //    clientSocket_Motion.Send(send_data);
                logger.WriteLog("Send Server : " + Send_Data);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
        private void Cal_File_Address()
        {
            DateTime Now_ = DateTime.Now;
            string D_ = Now_.ToString("yyyyMMdd");
            string T_ = Now_.ToString("hhmmss");
            write_date = D_;
            write_time = T_;
            //
            string file_add = "";
            string folder_add = "";
            file_add = variable_data.Save_File_Path_1 + "\\";
            folder_add = file_add;
            if (variable_data.Save_File_Path_2 != "")
            {
                string second_ = variable_data.Save_File_Path_2.Replace("*R*", textBox_Recipe_Name.Text);
                second_ = second_.Replace("*D*", D_);
                second_ = second_.Replace("*T*", T_);
                second_ = second_.Replace("*W*", textBox_Wafer_ID.Text);
                second_ = second_.Replace("*RF*", textBox_RFID.Text);
                second_ = second_.Replace("*S*", textBox_Slot.Text);
                file_add = file_add + second_ + "\\";
                folder_add = file_add;
                Check_Folder_Exist(folder_add);
            }
            if (variable_data.Save_File_Path_3 != "")
            {
                string second_ = variable_data.Save_File_Path_3.Replace("*R*", textBox_Recipe_Name.Text);
                second_ = second_.Replace("*D*", D_);
                second_ = second_.Replace("*T*", T_);
                second_ = second_.Replace("*W*", textBox_Wafer_ID.Text);
                second_ = second_.Replace("*RF*", textBox_RFID.Text);
                second_ = second_.Replace("*S*", textBox_Slot.Text);
                file_add = file_add + second_ + "\\";
                folder_add = file_add;
                Check_Folder_Exist(folder_add);
            }
            if (variable_data.Save_File_Name != "")
            {
                string second_ = variable_data.Save_File_Name.Replace("*R*", textBox_Recipe_Name.Text);
                second_ = second_.Replace("*D*", D_);
                second_ = second_.Replace("*T*", T_);
                second_ = second_.Replace("*W*", textBox_Wafer_ID.Text);
                second_ = second_.Replace("*RF*", textBox_RFID.Text);
                second_ = second_.Replace("*S*", textBox_Slot.Text);
                //file_add = file_add + second_ + ".xml";
                file_add = file_add + second_ + ".csv";
            }
            //
            Save_File_Folder = folder_add;
            Save_File_Address = file_add;
        }
        private void Check_Folder_Exist(string folder_address)
        {
            if (!Directory.Exists(folder_address))
            {
                Directory.CreateDirectory(folder_address);
            }
        }
        private int A_to_Num(string A)
        {
            int Byte_to_Int = 0;
            if (A.Length <= 1)
            {
                Byte A_to_Byte = Convert.ToByte(Convert.ToChar(A));
                Byte_to_Int = Convert.ToInt32(A_to_Byte) - 64;
            }
            else
            {
                Byte A_to_Byte = Convert.ToByte(Convert.ToChar(A[1]));
                Byte_to_Int = Convert.ToInt32(A_to_Byte) - 64;
                Byte_to_Int = Byte_to_Int + (Convert.ToInt32(Convert.ToByte(Convert.ToChar(A[0]))) - 64) * 26;
            }
            return Byte_to_Int;
        }
        int DeleteDataGetting()
        {
            if (radioButton1.Checked)
            {
                return 1;
            }
            else if (radioButton2.Checked)
            {
                return 2;
            }
            else if (radioButton3.Checked)
            {
                return 3;
            }
            else if (radioButton4.Checked)
            {
                return 4;
            }
            else if (radioButton5.Checked)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }
        void DeleteDataSetting(int value)
        {
            switch (value)
            {
                case 1:
                    radioButton1.Checked = true;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    break;
                case 3:
                    radioButton3.Checked = true;
                    break;
                case 4:
                    radioButton4.Checked = true;
                    break;
                case 5:
                    radioButton5.Checked = true;
                    break;
                case 6:
                    radioButton6.Checked = true;
                    break;

            }
        }
        private void Search_IP(int card_number)
        {

            try
            {
                // 指定查詢網路介面卡組態 ( IPEnabled 為 True 的 )
                string strQry = "Select * from Win32_NetworkAdapterConfiguration where IPEnabled=True";

                // ManagementObjectSearcher 類別 , 根據指定的查詢擷取管理物件的集合。
                ManagementObjectSearcher objSc = new ManagementObjectSearcher(strQry);
                // 使用 Foreach 陳述式 存取集合類別中物件 (元素)
                // Get 方法 , 叫用指定的 WMI 查詢 , 並傳回產生的集合。
                foreach (ManagementObject objQry in objSc.Get())
                {
                    //判斷是否與選取網卡名稱一樣
                    if (Convert.ToString(objQry["Caption"]) == ethernet_card[card_number])
                    {
                        Object aaa = objQry["IPAddress"];
                        Object[] asda = (Object[])aaa;
                        if (asda != null && asda.Length > 0)
                        {
                            comboBox_IP.Items.Add(Convert.ToString(((Object[])aaa)[0]));
                            comboBox_IP_Motion.Items.Add(Convert.ToString(((Object[])aaa)[0]));
                        }
                        else
                        {
                            comboBox_IP.Items.Add("NA");
                            comboBox_IP_Motion.Items.Add("NA");
                        }
                    }

                }
            }
            catch (Exception error)
            {
                logger.WriteErrorLog("Search_IP" + error.ToString());
            }
        }
        private void Search_Ethernet_Card()
        {
            try
            {
                // 指定查詢網路介面卡組態 ( IPEnabled 為 True 的 )
                string strQry = "Select * from Win32_NetworkAdapterConfiguration where IPEnabled=True";

                // ManagementObjectSearcher 類別 , 根據指定的查詢擷取管理物件的集合。
                ManagementObjectSearcher objSc = new ManagementObjectSearcher(strQry);

                // 使用 Foreach 陳述式 存取集合類別中物件 (元素)
                // Get 方法 , 叫用指定的 WMI 查詢 , 並傳回產生的集合。
                foreach (ManagementObject objQry in objSc.Get())
                {
                    // 取網路介面卡資訊
                    ethernet_card.Add(Convert.ToString(objQry["Caption"])); // 將 Caption 新增至 ComboBox

                }
            }
            catch (Exception error)
            {
                logger.WriteErrorLog("Search_Ethernet_Card" + error.ToString());

            }
        }
        byte[] StringToByteArray(string str)
        {
            byte[] send_data = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                send_data[i] = Convert.ToByte(str[i]);
            }
            return send_data;
        }
        string getExcelValue(string fileName, int row, int column)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbook wb = excel.Workbooks.Open(fileName);
            Excel.Worksheet excelSheet = wb.ActiveSheet;
            string value = excelSheet.Cells[row, column].Value.ToString();
            wb.Close(0);
            excel.Quit();
            return value;
        }
        void imshowValueInMeasurementGUI(string value)
        {
            TextBox textBox = textBox_Mesument_1_0;
            if (radioButton_Degree_0.Checked)
            {
                //20211224-S
                //switch (textBox_Point.Text)
                //{
                //    case "1":
                //        textBox = textBox_Mesument_1_0;
                //        break;
                //    case "2":
                //        textBox = textBox_Mesument_2_0;
                //        break;
                //    case "3":
                //        textBox = textBox_Mesument_3_0;
                //        break;
                //    case "4":
                //        textBox = textBox_Mesument_4_0;
                //        break;
                //    case "5":
                //        textBox = textBox_Mesument_5_0;
                //        break;
                //    case "6":
                //        textBox = textBox_Mesument_6_0;
                //        break;
                //    case "7":
                //        textBox = textBox_Mesument_7_0;
                //        break;
                //    case "8":
                //        textBox = textBox_Mesument_8_0;
                //        break;
                //    case "9":
                //        textBox = textBox_Mesument_9_0;
                //        break;
                //}
                textBox = textBoxes_0_1_20[Convert.ToInt32(textBox_Point.Text)];
                //20211224-E
            }
            else
            {
                //20211224-S
                //switch (textBox_Point.Text)
                //{
                //    case "1":
                //        textBox = textBox_Mesument_1_45;
                //        break;
                //    case "2":
                //        textBox = textBox_Mesument_2_45;
                //        break;
                //    case "3":
                //        textBox = textBox_Mesument_3_45;
                //        break;
                //    case "4":
                //        textBox = textBox_Mesument_4_45;
                //        break;
                //    case "5":
                //        textBox = textBox_Mesument_5_45;
                //        break;
                //    case "6":
                //        textBox = textBox_Mesument_6_45;
                //        break;
                //    case "7":
                //        textBox = textBox_Mesument_7_45;
                //        break;
                //    case "8":
                //        textBox = textBox_Mesument_8_45;
                //        break;
                //    case "9":
                //        textBox = textBox_Mesument_9_45;
                //        break;
                //}
                textBox = textBoxes_CuNi_1_20[Convert.ToInt32(textBox_Point.Text)];
                //20211224-E
            }
            ClearAndUpdateTextbox(value, textBox);
        }
        string create_folder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }
            return path;
        }
        bool SocketConnected(Socket s)
        {
            if (s == null)
            {
                return false;
            }
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }
        string get_socket_send_data()
        {
            string data = $"LUX1:{textBox_Cover_Start_X1.Text},RDX1:{textBox_Cover_End_X1.Text},LUY1:{textBox_Cover_Start_Y1.Text},RDY1:{textBox_Cover_End_Y1.Text}," +
                          $"LUX2:{textBox_Cover_Start_X2.Text},RDX2:{textBox_Cover_End_X2.Text},LUY2:{textBox_Cover_Start_Y2.Text},RDY2:{textBox_Cover_End_Y2.Text}," +
                          $"S1X:{textBox_Step_1_X.Text},S1Y:{textBox_Step_1_Y.Text},S2X:{textBox_Step_2_X.Text},S2Y:{textBox_Step_2_Y.Text}," +
                          $"S3X:{textBox_Step_3_X.Text},S3Y:{textBox_Step_3_Y.Text},S4s:{textBox_step4_Delay.Text}," +
                          $"S5X:{textBox_Step_5_X.Text},S5Y:{textBox_Step_5_Y.Text},S6s:{textBox_step6_Delay.Text}," +
                          $"CS1:{Convert.ToInt32(checkBox_Step_1.Checked)},CS2:{Convert.ToInt32(checkBox_Step_2.Checked)},CS3:{Convert.ToInt32(checkBox_Step_3.Checked)},CS4:{Convert.ToInt32(checkBox_Step_4.Checked)}," +
                          $"CS5:{Convert.ToInt32(checkBox_Step_5.Checked)},CS6:{Convert.ToInt32(checkBox_Step_6.Checked)}," +
                          $"OLSNAME:{textBox_OLS_Program_Name.Text}," +
                          $"HBX:{textBox_hand_measure_X.Text},HBY:{textBox_hand_measure_Y.Text}," +
                          $"HBH:{textBox_hand_measure_H.Text},HBW:{textBox_hand_measure_W.Text}";
            return data;
        }

        public void SaveArrayAsCSV(List<string> arrayToSave, string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (string item in arrayToSave)
                {
                    file.WriteLine(item);
                }
            }
        }
        //
        #region Change UI
        //
        private delegate void UpdateUITextbox(string value, TextBox ctl);
        private void UpdateTextbox(string value, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUITextbox uu = new UpdateUITextbox(UpdateTextbox);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }
        private void UpdateTextboxAdd(string value, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUITextbox uu = new UpdateUITextbox(UpdateTextbox);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Text += value;
            }
        }
        private void ClearAndUpdateTextbox(string value, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUITextbox uu = new UpdateUITextbox(ClearAndUpdateTextbox);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }
        //
        private delegate void UpdateUIPicturebox(Image value, PictureBox ctl);
        private void UpdatePicturebox(Image value, PictureBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUIPicturebox uu = new UpdateUIPicturebox(UpdatePicturebox);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Image = value;
            }
        }
        //
        private delegate void UpdateUIRadioButton(bool value, RadioButton ctl);
        private void UpdateRadioButton(bool value, RadioButton ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUIRadioButton uu = new UpdateUIRadioButton(UpdateRadioButton);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Checked = value;
            }
        }
        //
        private delegate void UpdateUITextboxEnable(bool value, TextBox ctl);
        private void UpdateTextboxEnable(bool value, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUITextboxEnable uu = new UpdateUITextboxEnable(UpdateTextboxEnable);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Enabled = value; ;
            }
        }
        #endregion
        //
        #region Control Windows
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);
        //
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr HWnd, uint Msg, int WParam, int LParam);
        public const int WM_SYSCOMMAND = 0x112;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const uint WM_SYSCOMMAND2 = 0x0112;
        public const uint SC_MAXIMIZE2 = 0xF030;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        #endregion
        //
        #region Motion Server
        private string[] Cal_Recive_Data(string receive_data)
        {
            //找出結尾座標  ， 目前延用之前的工程師寫法  可能會有問題
            receive_data = receive_data.Replace(">", "");
            string[] sub_string_ = receive_data.Split(',');


            return sub_string_;

        }
        private void Receive_YuanLi()
        {
            Send_Server("Yuani,e>");
        }
        private void Receive_Init()
        {
            logger.WriteLog("Start Initial");
            //
            Initial_OLS();
        }
        private void Receive_SetRecipe(string receive_data)
        {
            UpdateTextbox(receive_data, textBox_Recipe_Name);
            //
            try
            {
                Send_Server("SetRecipe,s>");
                string recipe_name = textBox_Recipe_Name.Text;
                int recipe_len = recipe_name.Length;

                //這裡要切換 aoIFlow. sharpnessFlow 資料

                string path = $"{systemPath}\\Recipe\\{receive_data}";
                if (!Directory.Exists(path)) throw new Exception($"Not Found Recipe : {receive_data}");


                LoadRecipe(path);


                /*DirectoryInfo vpp_file_folder = new DirectoryInfo(variable_data.Vision_Pro_File);
                string vpp_file_name = vpp_file_folder.GetFiles(recipe_name.Substring(recipe_len - 4) + "*" + ".vpp")[0].FullName;              
                AOI_Measurement = new SPILBumpMeasure(vpp_file_name);*/
                AOI_Measurement = new SPILBumpMeasure(aoIFlow.MeasureToolBlock);
                //綁定cogRecordDisplay 用來存toolblock結果圖
                AOI_Measurement.cogRecord_save_result_img = cogRecordDisplay1;
                AOI_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                AOI_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                AOI_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                AOI_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                AOI_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                AOI_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;

                AOI_Measurement.CogDisplay_result_1 = cogDisplay1;
                AOI_Measurement.CogDisplay_result_2 = cogDisplay2;
                AOI_Measurement.CogDisplay_result_3 = cogDisplay3;
                //載入手動量測vpp
                Hand_Measurement = new SPILBumpMeasure("Setup//Vision//Hand_Measurement.vpp");
                Hand_Measurement.cogRecord_save_result_img = cogRecordDisplay1;
                Hand_Measurement.CogDisplay_result_1 = cogDisplay1;
                Hand_Measurement.CogDisplay_result_2 = cogDisplay2;
                Hand_Measurement.CogDisplay_result_3 = cogDisplay3;
                Hand_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                Hand_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                Hand_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                Hand_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                Hand_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                Hand_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;


                Send_Server("SetRecipe,e>");

            }
            catch (Exception ex)
            {
                logger.WriteErrorLog("Set Recipe Error! " + ex.ToString());
                Send_Server("SetRecipe,x>");
                throw ex;
            }
        }
        private void Receive_Mode(string receive_data)
        {
            if (receive_data == "Top")
            {
                UpdateRadioButton(true, radioButton_Degree_0);
                open_hide_1 = false;
                open_hide_2 = true;
                HB_off();
                //  button_hb_off_Click(sender1, e1);

                Send_Server("Mode,e>");
            }
            else if (receive_data == "Side")
            {
                UpdateRadioButton(true, radioButton_Degree_45);
                open_hide_1 = true;
                open_hide_2 = false;
                button_hb_on_Click(sender1, e1);

                Send_Server("Mode,e>");
            }
        }
        private void Receive_Start(int Totoal_Point, string wafer_ID, int now_Slot)
        {
            for (int i = 1; i < 21; i++)
            {
                UpdateTextboxEnable(false, textBoxes_0_1_20[i]);
                UpdateTextbox("0", textBoxes_0_1_20[i]);
                //
                UpdateTextboxEnable(false, textBoxes_CuNi_1_20[i]);
                UpdateTextbox("0", textBoxes_CuNi_1_20[i]);
                //
                UpdateTextboxEnable(false, textBoxes_Cu_1_20[i]);
                UpdateTextbox("0", textBoxes_Cu_1_20[i]);
            }
            for (int i = 1; i <= Totoal_Point; i++)
            {
                UpdateTextboxEnable(true, textBoxes_0_1_20[i]);
                UpdateTextboxEnable(true, textBoxes_CuNi_1_20[i]);
                UpdateTextboxEnable(true, textBoxes_Cu_1_20[i]);
            }
            //
            UpdateTextbox(Convert.ToString(wafer_ID), textBox_Wafer_ID);
            //
            UpdateTextbox(Convert.ToString(now_Slot), textBox_Slot);
            //
            Cal_File_Address();
            open_hide_1 = true;
            open_hide_2 = true;
            //

            Send_Server("Start,e>");
        }
        private void Receive_InPos(int Now_Point)
        {
            // count = 1;
            UpdateTextbox(Convert.ToString(Now_Point), textBox_Point);

            Send_Server("InPos,e>");


        }
        private void Receive_Stop(string receive_data)
        {
            if (receive_data == "0000")
            {
                Save_Excel();

                Send_Server("Stop,e>");

                open_hide_1 = false;
                open_hide_2 = false;

                HB_off();
            }
        }
        private void Receive_RFID(string receive_RFID, string receive_Wafer_Size)
        {
            UpdateTextbox(receive_RFID, textBox_RFID);
            UpdateTextbox(receive_Wafer_Size, textBox_Wafer_Size);
            //20211224-S
            Send_Server("RFID,e>");
        }
        private async Task Receive_AOI()
        {
            try
            {


                Send_Server("AOIRun,s>");
                string[] images = await PickClarity(sharpnessImagesFolder);
                AoiMeansure(images);

                //20211224-S
                Send_Server("AOIRun,e>");
            }
            catch (Exception ex)
            {
                Send_Server("SetRecipe,x>");
                throw ex;
            }
        }
        #endregion
        //
        private (double cuNi, double cu) AOI_Calculate(SPILBumpMeasure Measuremrnt, string file_address1, string file_address2, string file_address3, bool is_maunal)
        {
            logger.WriteLog("AOI Measurment Point " + textBox_Point.Text);
            double distance_CuNi, distance_Cu, cuNi = 0, cu = 0;
            Measuremrnt.Measurment(file_address1, file_address2, file_address3, is_maunal, out distance_CuNi, out distance_Cu);
            if (distance_CuNi != -1 && distance_Cu != -1)
            {
                cuNi = distance_CuNi * variable_data.Degree_Ratio;
                cu = distance_Cu * variable_data.Degree_Ratio;
                logger.WriteLog("AOI Measurment Distance" + Convert.ToString(cuNi));
                logger.WriteLog("AOI Measurment Distance1" + Convert.ToString(cu));
                UpdateTextbox(Convert.ToString(cuNi), textBoxes_CuNi_1_20[Convert.ToInt32(textBox_Point.Text)]);
                UpdateTextbox(Convert.ToString(cu), textBoxes_Cu_1_20[Convert.ToInt32(textBox_Point.Text)]);
            }
            else
            {
                string error_value_string = "量測錯誤";
                UpdateTextbox(error_value_string, textBoxes_CuNi_1_20[Convert.ToInt32(textBox_Point.Text)]);
                UpdateTextbox(error_value_string, textBoxes_Cu_1_20[Convert.ToInt32(textBox_Point.Text)]);
                logger.WriteErrorLog("AOI Error!");
            }

            return (cuNi, cu);
        }
        //
        private void Initial_OLS()
        {

            Send_Server("Init,s>");
            /*button_auto_click_Sp5_Click(sender1, e1); //點擊5倍
            Thread.Sleep(Convert.ToInt32(textBox_step4_Delay.Text));
            if (is_test_mode)
            {
                return;
            }*/
            Send_Server("Init,e>");
        }
        //
        #region mouse
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendInput(Int32 cInputs, ref INPUT pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 28)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public INPUTTYPE dwType;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBOARDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MOUSEINPUT
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 mouseData;
            public MOUSEFLAG dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KEYBOARDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public KEYBOARDFLAG dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }

        public enum INPUTTYPE : int
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags()]
        public enum MOUSEFLAG : int
        {
            MOVE = 0x1,
            LEFTDOWN = 0x2,
            LEFTUP = 0x4,
            RIGHTDOWN = 0x8,
            RIGHTUP = 0x10,
            MIDDLEDOWN = 0x20,
            MIDDLEUP = 0x40,
            XDOWN = 0x80,
            XUP = 0x100,
            VIRTUALDESK = 0x400,
            WHEEL = 0x800,
            ABSOLUTE = 0x8000
        }

        [Flags()]
        public enum KEYBOARDFLAG : int
        {
            EXTENDEDKEY = 1,
            KEYUP = 2,
            UNICODE = 4,
            SCANCODE = 8
        }
        //
        static public void LeftDown()
        {
            INPUT leftdown = new INPUT();

            leftdown.dwType = 0;
            leftdown.mi = new MOUSEINPUT();
            leftdown.mi.dwExtraInfo = IntPtr.Zero;
            leftdown.mi.dx = 0;
            leftdown.mi.dy = 0;
            leftdown.mi.time = 0;
            leftdown.mi.mouseData = 0;
            leftdown.mi.dwFlags = MOUSEFLAG.LEFTDOWN;

            SendInput(1, ref leftdown, Marshal.SizeOf(typeof(INPUT)));
        }
        static public void LeftUp()
        {
            INPUT leftup = new INPUT();

            leftup.dwType = 0;
            leftup.mi = new MOUSEINPUT();
            leftup.mi.dwExtraInfo = IntPtr.Zero;
            leftup.mi.dx = 0;
            leftup.mi.dy = 0;
            leftup.mi.time = 0;
            leftup.mi.mouseData = 0;
            leftup.mi.dwFlags = MOUSEFLAG.LEFTUP;

            SendInput(1, ref leftup, Marshal.SizeOf(typeof(INPUT)));
        }
        static public void LeftClick()
        {
            LeftDown();
            Thread.Sleep(20);
            LeftUp();
        }
        static public void RightDown()
        {
            INPUT leftdown = new INPUT();

            leftdown.dwType = 0;
            leftdown.mi = new MOUSEINPUT();
            leftdown.mi.dwExtraInfo = IntPtr.Zero;
            leftdown.mi.dx = 0;
            leftdown.mi.dy = 0;
            leftdown.mi.time = 0;
            leftdown.mi.mouseData = 0;
            leftdown.mi.dwFlags = MOUSEFLAG.RIGHTDOWN;

            SendInput(1, ref leftdown, Marshal.SizeOf(typeof(INPUT)));
        }
        static public void RightUp()
        {
            INPUT leftup = new INPUT();

            leftup.dwType = 0;
            leftup.mi = new MOUSEINPUT();
            leftup.mi.dwExtraInfo = IntPtr.Zero;
            leftup.mi.dx = 0;
            leftup.mi.dy = 0;
            leftup.mi.time = 0;
            leftup.mi.mouseData = 0;
            leftup.mi.dwFlags = MOUSEFLAG.RIGHTUP;

            SendInput(1, ref leftup, Marshal.SizeOf(typeof(INPUT)));
        }
        static public void RightClick()
        {
            RightDown();
            Thread.Sleep(20);
            RightUp();
        }
        static public void LeftDoubleClick()
        {
            LeftClick();
            Thread.Sleep(50);
            LeftClick();
        }
        #endregion
        //
        #endregion

        #region Icon Function

        //Log In/Out
        private void timer_Log_in_Out_Tick(object sender, EventArgs e)
        {
            if (log_in && now_delay >= auto_log_out_times)
            {
                log_in = false;
                tabControl_Setup.Enabled = false;
                groupBox_Excel_Data_Setup.Enabled = false;
                Show_Data();
                timer_Log_in_Out.Enabled = false;
                button_Log_in_out.Text = "Log In";
                logger.WriteLog("Log Out");
                button_Save_Setup.Enabled = false;
            }
            else if (log_in)
            {
                now_delay++;
                button_Log_in_out.Text =
                    Convert.ToString(auto_log_out_Delay - now_delay * timer_Log_in_Out.Interval / 1000) + "s";
            }
        }
        private void button_Log_in_out_Click(object sender, EventArgs e)
        {
            if (!log_in)
            {
                if (textBox_Password.Text == password)
                {
                    logger.WriteLog("Log In");
                    log_in = true;
                    textBox_Password.Text = "";
                    tabControl_Setup.Enabled = true;
                    groupBox_Excel_Data_Setup.Enabled = true;
                    button_Log_in_out.Text = "Log Out";
                    now_delay = 0;
                    timer_Log_in_Out.Enabled = true;
                    button_Save_Setup.Enabled = true;
                    //timer_OLS_File.Enabled = true;
                }
                else
                    MessageBox.Show("Password Error!");
            }
            else
            {
                logger.WriteLog("Log Out");
                log_in = false;
                textBox_Password.Text = "";
                tabControl_Setup.Enabled = false;
                groupBox_Excel_Data_Setup.Enabled = false;
                button_Log_in_out.Text = "Log In";
                now_delay = 0;
                button_Save_Setup.Enabled = false;
            }
        }
        //Text
        private void textBox_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !log_in)
                button_Log_in_out_Click(sender, e);
        }
        private void textBox_Excel_File_Path_2_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 32; i <= 126; i++)
            {
                bool can_key = false;
                for (int j = 0; j < can_key_in_data.Length; j++)
                    if (i == Convert.ToInt32(can_key_in_data[j]))
                        can_key = true;
                if (!can_key)
                {
                    if (textBox_Excel_File_Path_2.Focused)
                    {
                        textBox_Excel_File_Path_2.Text = textBox_Excel_File_Path_2.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                        textBox_Excel_File_Path_2.SelectionStart = textBox_Excel_File_Path_2.Text.Length;
                        textBox_Excel_File_Path_2.ScrollToCaret();
                        textBox_Excel_File_Path_2.Focus();
                    }
                    else if (textBox_Excel_File_Path_3.Focused)
                    {
                        textBox_Excel_File_Path_3.Text = textBox_Excel_File_Path_3.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                        textBox_Excel_File_Path_3.SelectionStart = textBox_Excel_File_Path_3.Text.Length;
                        textBox_Excel_File_Path_3.ScrollToCaret();
                        textBox_Excel_File_Path_3.Focus();
                    }
                    else if (textBox_Excel_File_Name.Focused)
                    {
                        textBox_Excel_File_Name.Text = textBox_Excel_File_Name.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                        textBox_Excel_File_Name.SelectionStart = textBox_Excel_File_Name.Text.Length;
                        textBox_Excel_File_Name.ScrollToCaret();
                        textBox_Excel_File_Name.Focus();
                    }
                }
            }
        }

        private void textBox_Point_9_0_Num_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 32; i <= 126; i++)
            {
                bool can_key = false;
                if ((i >= 48 && i <= 57))
                    can_key = true;
                if (!can_key)
                {

                    textBox_Cover_Start_X1.Text = textBox_Cover_Start_X1.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Cover_Start_Y1.Text = textBox_Cover_Start_Y1.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Cover_End_X1.Text = textBox_Cover_End_X1.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Cover_End_Y1.Text = textBox_Cover_End_Y1.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_1_X.Text = textBox_Step_1_X.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_1_Y.Text = textBox_Step_1_Y.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_2_X.Text = textBox_Step_2_X.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_2_Y.Text = textBox_Step_2_Y.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_3_X.Text = textBox_Step_3_X.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_Step_3_Y.Text = textBox_Step_3_Y.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                    textBox_step4_Delay.Text = textBox_step4_Delay.Text.Replace(Convert.ToString(Convert.ToChar(i)), "");
                }
            }
        }
        private void textBox_45_Ratio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double ratio_ = Convert.ToDouble(textBox_45_Ratio.Text);
            }
            catch (Exception)
            {
                textBox_45_Ratio.Text = "";
            }
        }
        private void textBox_IP1_KeyDown(object sender, KeyEventArgs e)
        {
            now_delay = 0;
        }
        //Button
        private void button_Vision_Pro_File_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            OpenFileDialog open_ = new OpenFileDialog();
            //
            FolderBrowserDialog folder_ = new FolderBrowserDialog();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath_Vision.txt"))
            {
                StreamReader sr_ = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath_Vision.txt");
                string read_old_file_path = sr_.ReadLine();
                sr_.Close();
                folder_.SelectedPath = read_old_file_path;
            }
            else
                folder_.SelectedPath = "c:\\";
            if (folder_.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_Vision_Pro_File.Text = folder_.SelectedPath;
                StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath_Vision.txt");
                sw.WriteLine(folder_.SelectedPath);
                sw.Close();
            }
        }
        private void button_Save_File_Path_1_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            FolderBrowserDialog folder_ = new FolderBrowserDialog();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt"))
            {
                StreamReader sr_ = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt");
                string read_old_file_path = sr_.ReadLine();
                sr_.Close();
                folder_.SelectedPath = read_old_file_path;
            }
            else
                folder_.SelectedPath = "c:\\";
            if (folder_.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_Excel_File_Path_1.Text = folder_.SelectedPath;
                StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt");
                sw.WriteLine(folder_.SelectedPath);
                sw.Close();
            }
        }
        private void button_Sample_Excel_File_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            OpenFileDialog open_ = new OpenFileDialog();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFilePath.txt"))
            {
                StreamReader sr_ = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFilePath.txt");
                string read_old_file_path = sr_.ReadLine();
                sr_.Close();
                open_.InitialDirectory = read_old_file_path;
            }
            else
                open_.InitialDirectory = "c:\\";
            open_.Filter = "Excel files (*.xlsx)|*.xlsx|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            open_.FilterIndex = 1;
            open_.RestoreDirectory = true;
            open_.Multiselect = false;
            if (open_.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (open_.CheckFileExists)
                {
                    StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFilePath.txt");
                    sw.WriteLine(open_.FileName);
                    sw.Close();
                }
            }
        }
        private void button_OLS_Folder_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            FolderBrowserDialog folder_ = new FolderBrowserDialog();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt"))
            {
                StreamReader sr_ = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt");
                string read_old_file_path = sr_.ReadLine();
                sr_.Close();
                folder_.SelectedPath = read_old_file_path;
            }
            else
                folder_.SelectedPath = "c:\\";
            if (folder_.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_OLS_Folder.Text = folder_.SelectedPath;
                StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\Setup\\LoadFolderPath.txt");
                sw.WriteLine(folder_.SelectedPath);
                sw.Close();
            }
        }
        //Save Parameter
        private void button_Save_Setup_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            try
            {
                int delete_data_setting = DeleteDataGetting();
                //
                logger.WriteLog("Save Parameter");
                DateTime Now_ = DateTime.Now;
                String Today_ = "_" +
                    Convert.ToString(Now_.Year) + "_" +
                    Convert.ToString(Now_.Month) + "_" +
                    Convert.ToString(Now_.Day) + "_" +
                    Convert.ToString(Now_.Hour) + "_" +
                    Convert.ToString(Now_.Minute) + "_" +
                    Convert.ToString(Now_.Second);
                File.Move(
                    System.Windows.Forms.Application.StartupPath + "\\Setup\\Setup_Data.xml",
                    System.Windows.Forms.Application.StartupPath + "\\Setup\\Backup\\Setup_Data" + Today_ + ".xml");
                //
                StreamWriter SW_ = new StreamWriter(Setup_Data_address);
                SW_.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");//<?xml version="1.0" encoding="utf-8" ?>
                SW_.WriteLine("<SPIL_Program_Setup>");//
                //
                SW_.WriteLine("  <Setup_Part Setup_Part=\"Motion Server\">");//
                SW_.WriteLine("    <Setup Setup=\"IP_Port\">" + Convert.ToString(textBox_Port.Text) + "</Setup>");
                SW_.WriteLine("  </Setup_Part>");//
                //
                SW_.WriteLine("  <Setup_Part Setup_Part=\"AOI\">");//
                SW_.WriteLine("    <Setup Setup=\"Vision_Pro_File\">" + Convert.ToString(textBox_Vision_Pro_File.Text) + "</Setup>");
                SW_.WriteLine("  </Setup_Part>");//
                //
                SW_.WriteLine("  <Setup_Part Setup_Part=\"OLS\">");//
                SW_.WriteLine("    <Setup Setup=\"OLS_Name\">" + Convert.ToString(textBox_OLS_Program_Name.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Windows_Name\">" + Convert.ToString(textBox_Windows_Name.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"OLS_Folder\">" + Convert.ToString(textBox_OLS_Folder.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_Start_X1\">" + Convert.ToString(textBox_Cover_Start_X1.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_Start_Y1\">" + Convert.ToString(textBox_Cover_Start_Y1.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_End_X1\">" + Convert.ToString(textBox_Cover_End_X1.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_End_Y1\">" + Convert.ToString(textBox_Cover_End_Y1.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_Start_X2\">" + Convert.ToString(textBox_Cover_Start_X2.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_Start_Y2\">" + Convert.ToString(textBox_Cover_Start_Y2.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_End_X2\">" + Convert.ToString(textBox_Cover_End_X2.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Cover_End_Y2\">" + Convert.ToString(textBox_Cover_End_Y2.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_1\">" + Convert.ToString(checkBox_Step_1.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_1_X\">" + Convert.ToString(textBox_Step_1_X.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_1_Y\">" + Convert.ToString(textBox_Step_1_Y.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_2\">" + Convert.ToString(checkBox_Step_2.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_2_X\">" + Convert.ToString(textBox_Step_2_X.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_2_Y\">" + Convert.ToString(textBox_Step_2_Y.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_3\">" + Convert.ToString(checkBox_Step_3.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_3_X\">" + Convert.ToString(textBox_Step_3_X.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_3_Y\">" + Convert.ToString(textBox_Step_3_Y.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_4\">" + Convert.ToString(checkBox_Step_4.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_4_Delay_Time\">" + Convert.ToString(textBox_step4_Delay.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_5\">" + Convert.ToString(checkBox_Step_5.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_5_X\">" + Convert.ToString(textBox_Step_5_X.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_5_Y\">" + Convert.ToString(textBox_Step_5_Y.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_6\">" + Convert.ToString(checkBox_Step_6.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Initial_Step_6_Delay_Time\">" + Convert.ToString(textBox_step6_Delay.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Need_Move_bmp\">" + Convert.ToString(checkBox_bmp.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Need_Move_poir\">" + Convert.ToString(checkBox_poir.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Need_Move_xlsx\">" + Convert.ToString(checkBox_xlsx.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Need_Move_csv\">" + Convert.ToString(checkBox_csv.Checked) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"0_Degree_height_A\">" + Convert.ToString(textBox_0_degree_height_A.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"0_degree_height_Num\">" + Convert.ToString(textBox_0_degree_height_Num.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"AOI_save_idx_1\">" + Convert.ToString(numericUpDown_AOI_save_idx1.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"AOI_save_idx_2\">" + Convert.ToString(numericUpDown_AOI_save_idx2.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"AOI_save_idx_3\">" + Convert.ToString(numericUpDown_AOI_save_idx3.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"manual_save_idx_1\">" + Convert.ToString(numericUpDown_manual_save_idx1.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"manual_save_idx_2\">" + Convert.ToString(numericUpDown_manual_save_idx2.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"manual_save_idx_3\">" + Convert.ToString(numericUpDown_manual_save_idx3.Value) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"hand_measurement_X\">" + Convert.ToString(textBox_hand_measure_X.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"hand_measurement_Y\">" + Convert.ToString(textBox_hand_measure_Y.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"hand_measurement_H\">" + Convert.ToString(textBox_hand_measure_H.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"hand_measurement_W\">" + Convert.ToString(textBox_hand_measure_W.Text) + "</Setup>");
                SW_.WriteLine("  </Setup_Part>");//
                //
                SW_.WriteLine("  <Setup_Part Setup_Part=\"Delete Data\">");//
                SW_.WriteLine("    <Setup Setup=\"Delete_Time_Setting\">" + Convert.ToString(delete_data_setting) + "</Setup>");
                SW_.WriteLine("  </Setup_Part>");//
                //
                //
                SW_.WriteLine("  <Setup_Part Setup_Part=\"Excel File\">");//
                SW_.WriteLine("    <Setup Setup=\"Save_File_Path_1\">" + Convert.ToString(textBox_Excel_File_Path_1.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Save_File_Path_2\">" + Convert.ToString(textBox_Excel_File_Path_2.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Save_File_Path_3\">" + Convert.ToString(textBox_Excel_File_Path_3.Text) + "</Setup>");
                SW_.WriteLine("    <Setup Setup=\"Save_File_Name\">" + Convert.ToString(textBox_Excel_File_Name.Text) + "</Setup>");

                SW_.WriteLine("    <Setup Setup=\"Degree_Ratio\">" + Convert.ToString(textBox_45_Ratio.Text) + "</Setup>");
                //

                SW_.WriteLine("  </Setup_Part>");//
                //
                SW_.WriteLine("</SPIL_Program_Setup>");//
                SW_.Close();
                //
                logger.WriteLog("Save Parameter Successful");
                Load_Setup_Data();
                MessageBox.Show("Save OK");
            }
            catch (Exception error)
            {
                logger.WriteLog($"Save Parameter Error {error.Message}");
            }
        }
        //
        private void timer_chek_delete_file_Tick(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                check_delete_time = 30;
            }
            else if (radioButton2.Checked)
            {
                check_delete_time = 30 * 3;
            }
            else if (radioButton3.Checked)
            {
                check_delete_time = 30 * 6;
            }
            else if (radioButton4.Checked)
            {
                check_delete_time = 30 * 9;
            }
            else if (radioButton5.Checked)
            {
                check_delete_time = 30 * 12;
            }
            else
            {
                check_delete_time = 30 * 24;
            }
            if (!backgroundWorker_delete_old_file.IsBusy)
            {
                backgroundWorker_delete_old_file.RunWorkerAsync();
            }
        }

        private void backgroundWorker_delete_old_file_DoWork(object sender, DoWorkEventArgs e)
        {
            string check_path = textBox_Excel_File_Path_1.Text + "\\" + textBox_Excel_File_Path_2.Text + "\\" + textBox_Excel_File_Path_3.Text;//要檢查刪除的資料夾位置
            if (!Directory.Exists(check_path))
            {
                return;
            }
            //取出資料夾創建時間
            DateTime file_create_time = File.GetCreationTime(check_path);
            DateTime now_time = DateTime.Now;
            var diff = now_time.Subtract(file_create_time).TotalDays;
            //刪除超過設定時間的資料夾
            if (diff > check_delete_time)
            {
                Directory.Delete(check_path);
            }

        }

        #endregion

        #region Measurement Data 
        private void button_Save_Excel_Click(object sender, EventArgs e)
        {
            try
            {
                Save_Excel();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void Save_Excel()
        {
            try
            {
                Cal_File_Address();
                //20211224-S
                //double[] zero_degree_ = new double[9];
                //zero_degree_[0] = Convert.ToDouble(textBox_Mesument_1_0.Text);
                //zero_degree_[1] = Convert.ToDouble(textBox_Mesument_2_0.Text);
                //zero_degree_[2] = Convert.ToDouble(textBox_Mesument_3_0.Text);
                //zero_degree_[3] = Convert.ToDouble(textBox_Mesument_4_0.Text);
                //zero_degree_[4] = Convert.ToDouble(textBox_Mesument_5_0.Text);
                //zero_degree_[5] = Convert.ToDouble(textBox_Mesument_6_0.Text);
                //zero_degree_[6] = Convert.ToDouble(textBox_Mesument_7_0.Text);
                //zero_degree_[7] = Convert.ToDouble(textBox_Mesument_8_0.Text);
                //zero_degree_[8] = Convert.ToDouble(textBox_Mesument_9_0.Text);
                //double[] fortyfive_degree_ = new double[9];
                //fortyfive_degree_[0] = Convert.ToDouble(textBox_Mesument_1_45.Text);
                //fortyfive_degree_[1] = Convert.ToDouble(textBox_Mesument_2_45.Text);
                //fortyfive_degree_[2] = Convert.ToDouble(textBox_Mesument_3_45.Text);
                //fortyfive_degree_[3] = Convert.ToDouble(textBox_Mesument_4_45.Text);
                //fortyfive_degree_[4] = Convert.ToDouble(textBox_Mesument_5_45.Text);
                //fortyfive_degree_[5] = Convert.ToDouble(textBox_Mesument_6_45.Text);
                //fortyfive_degree_[6] = Convert.ToDouble(textBox_Mesument_7_45.Text);
                //fortyfive_degree_[7] = Convert.ToDouble(textBox_Mesument_8_45.Text);
                //fortyfive_degree_[8] = Convert.ToDouble(textBox_Mesument_9_45.Text);
                //20211224-E
                logger.WriteLog("Save Point " + textBox_Point.Text);
                List<string> Csv_Str_List = new List<string>();
                Csv_Str_List.Add(
                    "Point," +
                    "Bump Height," +
                    "Cu+Ni Height," +
                    "Cu Height," +
                    "Ni Height," +
                    "Solder tip Height");
                for (int i = 1; i < 21; i++)
                {
                    double z_dif = Convert.ToDouble(textBoxes_0_1_20[i].Text) - Convert.ToDouble(textBoxes_CuNi_1_20[i].Text);
                    double Ni = Convert.ToDouble(textBoxes_CuNi_1_20[i].Text) - Convert.ToDouble(textBoxes_Cu_1_20[i].Text);
                    if (textBoxes_0_1_20[i].Enabled)
                        Csv_Str_List.Add(
                            $"{i}," +
                            $"{textBoxes_0_1_20[i].Text}," +
                            $"{textBoxes_CuNi_1_20[i].Text}," +
                            $"{textBoxes_Cu_1_20[i].Text}," +
                            $"{Convert.ToString(Ni)}," +
                            $"{Convert.ToString(z_dif)}");
                }
                SaveArrayAsCSV(Csv_Str_List, Save_File_Address);
                File.Copy(Save_File_Address, "C:\\Users\\Public\\Documents\\SPIL_Measurement_Data.csv", true);
                logger.WriteLog("Save OK");
            }
            catch (Exception error)
            {
                logger.WriteLog("Save Error!" + error.ToString());
            }
        }

        private void textBox_Mesument_1_0_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_1_0.Text);
            }
            catch
            {
                textBox_Mesument_1_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_2_0.Text);
            }
            catch
            {
                textBox_Mesument_2_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_3_0.Text);
            }
            catch
            {
                textBox_Mesument_3_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_4_0.Text);
            }
            catch
            {
                textBox_Mesument_4_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_5_0.Text);
            }
            catch
            {
                textBox_Mesument_5_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_6_0.Text);
            }
            catch
            {
                textBox_Mesument_6_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_7_0.Text);
            }
            catch
            {
                textBox_Mesument_7_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_8_0.Text);
            }
            catch
            {
                textBox_Mesument_8_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_9_0.Text);
            }
            catch
            {
                textBox_Mesument_9_0.Text = "";
            }
            //20211224-S
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_10_0.Text);
            }
            catch
            {
                textBox_Mesument_10_0.Text = "";
            }
            //
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_11_0.Text);
            }
            catch
            {
                textBox_Mesument_11_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_12_0.Text);
            }
            catch
            {
                textBox_Mesument_12_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_13_0.Text);
            }
            catch
            {
                textBox_Mesument_13_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_14_0.Text);
            }
            catch
            {
                textBox_Mesument_14_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_15_0.Text);
            }
            catch
            {
                textBox_Mesument_15_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_16_0.Text);
            }
            catch
            {
                textBox_Mesument_16_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_17_0.Text);
            }
            catch
            {
                textBox_Mesument_17_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_18_0.Text);
            }
            catch
            {
                textBox_Mesument_18_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_19_0.Text);
            }
            catch
            {
                textBox_Mesument_19_0.Text = "";
            }
            try
            {
                double aaa = Convert.ToDouble(textBox_Mesument_20_0.Text);
            }
            catch
            {
                textBox_Mesument_20_0.Text = "";
            }
            //20211224-E
        }
        #endregion

        #region Socket 
        private void button_Connect_Click(object sender, EventArgs e)
        {
            now_delay = 0;
            try
            {
                if (hostCommunication != null) throw new Exception("Connected");
                logger.WriteLog("Create Motion Server");
                string ip_address = comboBox_IP_Motion.Text;
                //    IPAddress ip = IPAddress.Parse(ip_address);
                int port = Convert.ToInt32(textBox_Port.Text);

                hostCommunication = new HostCommunication("127.0.0.1", 1234);

                //  hostCommunication = new HostCommunication(ip_address, port);
                hostCommunication.ReceiverMessage += ReciveMessage;
                hostCommunication.ReceiverException += ReciveException;
                //    Socketserver_Motion = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //   Socketserver_Motion.Bind(new IPEndPoint(ip, port));  //繫結IP地址：埠
                //   Socketserver_Motion.Listen(10);    //設定最多10個排隊連線請求
                logger.WriteLog("Create Motion Server Successful");
                //        timer_Server.Enabled = true;
                UpdatePicturebox(I_Green, pictureBox_Connect_Status);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                logger.WriteErrorLog("Create Motion Server Fail ! " + error.ToString());
            }
        }

        private void timer_Server_Tick(object sender, EventArgs e)
        {
            /* if (!backgroundWorker_Server.IsBusy)
             {
                 sender1 = sender;
                 e1 = e;
                 backgroundWorker_Server.RunWorkerAsync();
             }*/
        }
        private void backgroundWorker_Server_DoWork(object sender, DoWorkEventArgs e)
        {/*
            try
            {
                if (!connect_Motion_client)
                {
                    //連線成功
                    clientSocket_Motion = Socketserver_Motion.Accept();
                    connect_Motion_client = true;
                    logger.WriteLog("Connect client : " + IPAddress.Parse(((IPEndPoint)clientSocket_Motion.RemoteEndPoint).Address.ToString()) + Environment.NewLine);
                }
                else
                {
                    try
                    {
                        
                        byte[] Receive_data = new byte[256];
                        clientSocket_Motion.Receive(Receive_data);
                        string receive_data = "";
                        for (int i = 0; i < 256; i++)
                        {
                            if (Receive_data[i] == 0)
                                break;
                            else
                            {
                                receive_data += Convert.ToString(Convert.ToChar(Receive_data[i]));
                            }
                        }
                        //
                        string[] re_data = Cal_Recive_Data(receive_data);
                        logger.WriteLog("Receive : " + receive_data);
                        if (re_data != null)
                        {
                            logger.WriteLog("Calculate OK : " + receive_data);
                            if (re_data[1].IndexOf("YuanLi") >= 0)
                                Receive_YuanLi();
                            else if (re_data[1].IndexOf("Init") >= 0)
                                Receive_Init();
                            else if (re_data[1].IndexOf("SetRecipe") >= 0)
                                Receive_SetRecipe(re_data[2]);
                            else if (re_data[1].IndexOf("Mode") >= 0)
                                Receive_Mode(re_data[2]);
                            else if (re_data[1].IndexOf("Start") >= 0)
                                Receive_Start(Convert.ToInt32(re_data[2]), Convert.ToString(re_data[3]), Convert.ToInt32(re_data[4]));
                            else if (re_data[1].IndexOf("InPos") >= 0)
                                Receive_InPos(Convert.ToInt32(re_data[2]));
                            else if (re_data[1].IndexOf("Stop") >= 0)
                                Receive_Stop(re_data[2]);
                            else if (re_data[1].IndexOf("RFID") >= 0)
                                Receive_RFID(re_data[2], re_data[3]);
                            else
                                logger.WriteErrorLog("No Match Data!");
                        }
                        else
                            logger.WriteErrorLog("Motion Client Receive Error : " + receive_data);
                    }
                    catch (Exception error)
                    {
                        //MessageBox.Show(error.ToString());
                        int aaa = error.HResult;
                        if (aaa == -2147467259)
                        {
                            logger.WriteErrorLog("Motion Client Disconnected!" + error.ToString());
                            clientSocket_Motion = new Socket(SocketType.Stream, ProtocolType.Tcp);
                            UpdatePicturebox(I_Red, pictureBox_Connect_Status);
                            connect_Motion_client = false;
                            timer_Server.Enabled = false;
                        }
                        else
                        {
                            logger.WriteErrorLog("Motion Client Error! " + error.ToString());
                        }
                    }
                }
            }
            catch (Exception error)
            {
                logger.WriteErrorLog("Motion error" + error.ToString());
            }*/
        }
        #endregion

        #region OLS
        private void button_Start_Click(object sender, EventArgs e)
        {
            /* if (!timer_OLS_File.Enabled)
                 timer_OLS_File.Enabled = true;
             else
                 timer_OLS_File.Enabled = false;*/
        }
        private void timer_OLS_File_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker_OLS_File.IsBusy)
                backgroundWorker_OLS_File.RunWorkerAsync();
        }
        private void backgroundWorker_OLS_File_DoWork(object sender, DoWorkEventArgs e)
        {

            //if (!folder_info.Exists)
            //{
            //    logger.WriteErrorLog("OLS File Folder:" + folder_info.FullName + " is not found! ");
            //}
            //else
            //{
            //    if (checkBox_bmp.Checked)
            //    {
            //        if (folder_info.GetFiles("*.bmp").Length > 0)
            //        {
            //            FileInfo[] FIle_List = folder_info.GetFiles("*.bmp");
            //            //try
            //            //{
            //            for (int i = 0; i < FIle_List.Length; i++)
            //            {
            //                logger.WriteLog("New File : " + FIle_List[i].FullName);
            //                logger.WriteLog("Move File : " + Save_File_Folder + textBox_Point.Text);
            //                string[] file_list_part_name = FIle_List[i].FullName.Split('_');
            //                logger.WriteLog("file last part name:" + file_list_part_name[file_list_part_name.Length - 1]);


            //                if (radioButton_Degree_0.Checked) //0度
            //                {
            //                    //使用'_'分割檔名
            //                    string save_degree_0_name = "";
            //                    logger.WriteLog("split by _ keyword:");
            //                    string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                    //foreach(string s in split_input_file_names)
            //                    //{
            //                    //    logger.Write_Logger(s);
            //                    //}
            //                    save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                    string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0_" + file_list_part_name[file_list_part_name.Length - 1];
            //                    if (File.Exists(save_full_file_name))
            //                    {
            //                        File.Delete(save_full_file_name);
            //                        logger.WriteLog("Delete File : " + save_full_file_name);
            //                    }
            //                    File.Move(FIle_List[i].FullName, save_full_file_name);
            //                    logger.WriteLog("Move File : " + FIle_List[i].FullName + " Move To:" + save_full_file_name);

            //                }
            //                else //45度
            //                {

            //                    //使用'_'分割檔名
            //                    string save_degree_45_name = "";
            //                    logger.WriteLog("split by _ keyword:");
            //                    string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                    foreach (string s in split_input_file_names)
            //                    {
            //                        logger.WriteLog(s);
            //                    }
            //                    save_degree_45_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                    string save_full_file_name = Save_File_Folder + save_degree_45_name + textBox_Point.Text + $"_45_{count}_" + file_list_part_name[file_list_part_name.Length - 1];

            //                    if (File.Exists(save_full_file_name))
            //                    {
            //                        File.Delete(save_full_file_name);
            //                        logger.WriteLog("Delete File : " + save_full_file_name);
            //                    }
            //                    File.Move(FIle_List[i].FullName, save_full_file_name);
            //                    logger.WriteLog("Move File : " + FIle_List[i].FullName + " Move To:" + save_full_file_name);

            //                    Save_AOI_file_name[count - 1] = save_full_file_name;
            //                    logger.WriteLog("AOI input image " + count.ToString() + ": " + save_full_file_name);

            //                    count++;
            //                    //執行AOI計算
            //                    if (is_hand_measurement)
            //                    {
            //                        if (count > 3)//已經存3張
            //                        {
            //                            AOI_Calculate(Hand_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[1], Save_AOI_file_name[2], is_hand_measurement);
            //                            logger.WriteLog("手動量測");
            //                            count = 1;
            //                            logger.WriteLog("Img file 1 : " + Save_AOI_file_name[0]);
            //                            logger.WriteLog("Img file 2 : " + Save_AOI_file_name[1]);
            //                            logger.WriteLog("Img file 3 : " + Save_AOI_file_name[2]);
            //                            logger.WriteLog("AOI_Calculate");
            //                            button_hb_on_Click(sender, e);
            //                        }

            //                    }
            //                    else
            //                    {
            //                        if (count > 2)//已經存2張
            //                        {
            //                            AOI_Calculate(AOI_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[0], Save_AOI_file_name[1], is_hand_measurement);
            //                            logger.WriteLog("AOI自動量測");
            //                            count = 1;
            //                            logger.WriteLog("Img file 1 : " + Save_AOI_file_name[0]);
            //                            logger.WriteLog("Img file 2 : " + Save_AOI_file_name[1]);
            //                            logger.WriteLog("AOI_Calculate");
            //                            button_hb_on_Click(sender, e);
            //                        }

            //                    }

            //                    //if (count > 3)//已經存超過兩張
            //                    //{
            //                    //    //執行AOI計算
            //                    //    if (is_hand_measurement)
            //                    //    {
            //                    //        AOI_Calculate(Hand_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[1], Save_AOI_file_name[2]);
            //                    //        logger.Write_Logger("手動量測");
            //                    //    }
            //                    //    else
            //                    //    {
            //                    //        AOI_Calculate(AOI_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[1], Save_AOI_file_name[2]);
            //                    //        logger.Write_Logger("AOI自動量測");
            //                    //    }

            //                    //    count = 1;
            //                    //    logger.Write_Logger("Img file 1 : " + Save_AOI_file_name[0]);
            //                    //    logger.Write_Logger("Img file 2 : " + Save_AOI_file_name[1]);
            //                    //    logger.Write_Logger("Img file 3 : " + Save_AOI_file_name[2]);
            //                    //    logger.Write_Logger("AOI_Calculate");
            //                    //    button_hb_on_Click(sender, e);
            //                    //}
            //                }
            //            }

            //        }
            //    }
            //    if (checkBox_xlsx.Checked)
            //    {
            //        FileInfo[] FIle_List = folder_info.GetFiles("*.xlsx");
            //        if (FIle_List.Length > 0)
            //        {
            //            try
            //            {
            //                for (int i = 0; i < FIle_List.Length; i++)
            //                {
            //                    //取出excel資料
            //                    int row = Convert.ToInt32(textBox_0_degree_height_Num.Text);
            //                    int column = textBox_0_degree_height_A.Text.ToCharArray()[0] - 'A' + 1;
            //                    string value = getExcelValue(FIle_List[i].FullName, row, column);
            //                    //顯示在GUI介面中
            //                    imshowValueInMeasurementGUI(value);
            //                    logger.WriteLog("get measurement value: " + value);
            //                    logger.WriteLog("New File : " + FIle_List[i].FullName);
            //                    if (radioButton_Degree_0.Checked)
            //                    {
            //                        if (File.Exists(Save_File_Folder + textBox_Point.Text + "_0.xlsx"))
            //                            File.Delete(Save_File_Folder + textBox_Point.Text + "_0.xlsx");
            //                        File.Move(FIle_List[i].FullName, Save_File_Folder + textBox_Point.Text + "_0.xlsx");
            //                    }
            //                    else
            //                    {
            //                        if (File.Exists(Save_File_Folder + textBox_Point.Text + "_45.xlsx"))
            //                            File.Delete(Save_File_Folder + textBox_Point.Text + "_45.xlsx");
            //                        File.Move(FIle_List[i].FullName, Save_File_Folder + textBox_Point.Text + "_45.xlsx");
            //                    }
            //                }
            //            }
            //            catch (Exception error)
            //            {
            //                logger.WriteErrorLog("Move xlsx File Error! " + error.ToString());
            //            }
            //        }
            //    }
            //    if (checkBox_csv.Checked)
            //    {
            //        FileInfo[] FIle_List = folder_info.GetFiles("*.csv");
            //        if (FIle_List.Length > 0)
            //        {
            //            try
            //            {
            //                for (int i = 0; i < FIle_List.Length; i++)
            //                {
            //                    if (radioButton_Degree_0.Checked)
            //                    {
            //                        //取出csv內所有欄位, 存在2d list中
            //                        List<List<string>> csv_arr = new List<List<string>>();
            //                        string csv_file = FIle_List[i].FullName;
            //                        var reader = new StreamReader(File.OpenRead(csv_file));
            //                        List<List<string>> tmp = new List<List<string>>();
            //                        while (!reader.EndOfStream)
            //                        {
            //                            List<string> tmp1 = new List<string>();
            //                            var line = reader.ReadLine();
            //                            var values = line.Split(',');
            //                            foreach (string value in values)
            //                            {
            //                                tmp1.Add(value);
            //                            }
            //                            tmp.Add(tmp1);
            //                        }
            //                        reader.Close();
            //                        //從GUI介面中0度height 欄位取出對應csv數值
            //                        int row = Convert.ToInt32(textBox_0_degree_height_Num.Text) - 1;
            //                        int column = textBox_0_degree_height_A.Text.ToCharArray()[0] - 'A';
            //                        //顯示在GUI Measurement 對應 point點位中
            //                        imshowValueInMeasurementGUI(tmp[row][column]);
            //                        logger.WriteLog("get measurement value: " + tmp[row][column]);
            //                        //移動檔案
            //                        //使用'_'分割檔名
            //                        string save_degree_0_name = "";
            //                        logger.WriteLog("split by _ keyword:");
            //                        string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                        foreach (string s in split_input_file_names)
            //                        {
            //                            logger.WriteLog(s);
            //                        }
            //                        save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                        string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0.csv";
            //                        if (File.Exists(save_full_file_name))
            //                            File.Delete(save_full_file_name);
            //                        File.Move(FIle_List[i].FullName, save_full_file_name);
            //                        logger.WriteLog("New File : " + FIle_List[i].FullName +
            //                                            " Move to :" + save_full_file_name);
            //                    }
            //                    else
            //                    {
            //                        //移動檔案
            //                        //使用'_'分割檔名
            //                        string save_degree_45_name = "";
            //                        logger.WriteLog("split by _ keyword:");
            //                        string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                        foreach (string s in split_input_file_names)
            //                        {
            //                            logger.WriteLog(s);
            //                        }
            //                        save_degree_45_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                        string save_full_file_name = Save_File_Folder + save_degree_45_name + textBox_Point.Text + "_45.csv";
            //                        if (File.Exists(save_full_file_name))
            //                            File.Delete(save_full_file_name);
            //                        File.Move(FIle_List[i].FullName, save_full_file_name);
            //                        logger.WriteLog("New File : " + FIle_List[i].FullName +
            //                                            " Move to :" + save_full_file_name);
            //                    }
            //                }

            //            }
            //            catch (Exception error)
            //            {
            //                logger.WriteErrorLog("Move csv File Error! " + error.ToString());
            //            }
            //        }
            //    }
            //    if (checkBox_poir.Checked && !copy_poir_once)
            //    {
            //        FileInfo[] FIle_List = folder_info.GetFiles("*.poir");
            //        if (FIle_List.Length > 0)
            //        {
            //            try
            //            {
            //                for (int i = 0; i < FIle_List.Length; i++)
            //                {
            //                    logger.WriteLog("New File : " + FIle_List[i].FullName);
            //                    if (radioButton_Degree_0.Checked)
            //                    {
            //                        //使用'_'分割檔名
            //                        string save_degree_0_name = "";
            //                        logger.WriteLog("split by _ keyword:");
            //                        string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                        foreach (string s in split_input_file_names)
            //                        {
            //                            logger.WriteLog(s);
            //                        }
            //                        save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                        string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0.poir";
            //                        if (File.Exists(save_full_file_name))
            //                            File.Delete(save_full_file_name);
            //                        File.Copy(FIle_List[i].FullName, save_full_file_name);
            //                        copy_poir_once = true;
            //                        Thread.Sleep(15000);
            //                    }
            //                    else
            //                    {
            //                        //使用'_'分割檔名
            //                        string save_degree_45_name = "";
            //                        logger.WriteLog("split by _ keyword:");
            //                        string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
            //                        foreach (string s in split_input_file_names)
            //                        {
            //                            logger.WriteLog(s);
            //                        }
            //                        save_degree_45_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            //                        string save_full_file_name = Save_File_Folder + save_degree_45_name + textBox_Point.Text + "_45.poir";

            //                        if (File.Exists(save_full_file_name))
            //                            File.Delete(save_full_file_name);
            //                        File.Copy(FIle_List[i].FullName, save_full_file_name);
            //                        copy_poir_once = true;
            //                        Thread.Sleep(15000);
            //                    }
            //                }

            //            }
            //            catch (Exception error)
            //            {
            //                logger.WriteErrorLog("Copy poir File Error! " + error.ToString());
            //            }
            //        }
            //    }
            //    else if (checkBox_poir.Checked && copy_poir_once)
            //    {
            //        FileInfo[] FIle_List = folder_info.GetFiles("*.poir");
            //        if (FIle_List.Length > 0)
            //        {
            //            try
            //            {
            //                for (int i = 0; i < FIle_List.Length; i++)
            //                {
            //                    File.Delete(FIle_List[i].FullName);
            //                    copy_poir_once = false;
            //                }
            //            }
            //            catch (Exception error)
            //            {
            //                logger.WriteErrorLog($"Delete poir File Error!  {error.Message}");
            //            }
            //        }
            //    }
            //}

        }

        private async Task<string[]> PickClarity(string dirpath)
        {
            try
            {


                int id = Thread.CurrentThread.ManagedThreadId;
                // 取得資料夾中的所有檔案
                string[] files = Directory.GetFiles(dirpath);
                List<SharpnessResult> sharpnessResults = new List<SharpnessResult>();
                List<Bitmap> images = new List<Bitmap>();
                List<string> imageNames = new List<string>();

                // 遍歷每個檔案，檢查是否為影像檔
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();
                    string name = Path.GetFileName(file);
                    // 檢查副檔名是否為影像檔（可根據需求調整）
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                    {

                        images.Add(new Bitmap(file));
                        imageNames.Add(file);

                    }

                }
                List<string> names = new List<string>();

                var imagesIndex = await Task.Run(() => sharpnessFlow.SharpnessAnalyzeAsync(images));


                UpdateTextbox(imageNames[imagesIndex.Image1Index], txB_RecipePicName1);
                UpdateTextbox(imageNames[imagesIndex.Image2Index], txB_RecipePicName2);
                UpdateTextbox(imageNames[imagesIndex.Image3Index], txB_RecipePicName3);
                /* txB_RecipePicName1.Text = imageNames[imagesIndex.Image1Index];
                 txB_RecipePicName2.Text = imageNames[imagesIndex.Image2Index];
                 txB_RecipePicName3.Text = imageNames[imagesIndex.Image3Index];*/
                names.Add(imageNames[imagesIndex.Image1Index]);
                names.Add(imageNames[imagesIndex.Image2Index]);
                names.Add(imageNames[imagesIndex.Image3Index]);
                return names.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void AoiDegree_0(string fileName)
        {
            string[] file_list_part_name = fileName.Split('_');
            //使用'_'分割檔名
            string save_degree_0_name = "";
            logger.WriteLog("split by _ keyword:");
            string[] split_input_file_names = Path.GetFileNameWithoutExtension(fileName).Split('_');
            //foreach(string s in split_input_file_names)
            //{
            //    logger.Write_Logger(s);
            //}
            save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
            string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0_" + file_list_part_name[file_list_part_name.Length - 1];
            if (File.Exists(save_full_file_name))
            {
                File.Delete(save_full_file_name);
                logger.WriteLog("Delete File : " + save_full_file_name);
            }
            File.Move(fileName, save_full_file_name);
            logger.WriteLog("Move File : " + fileName + " Move To:" + save_full_file_name);
        }
        private void AoiOutputData()
        {
            if (checkBox_xlsx.Checked)
            {
                FileInfo[] FIle_List = folder_info.GetFiles("*.xlsx");
                if (FIle_List.Length > 0)
                {
                    try
                    {
                        for (int i = 0; i < FIle_List.Length; i++)
                        {
                            //取出excel資料
                            int row = Convert.ToInt32(textBox_0_degree_height_Num.Text);
                            int column = textBox_0_degree_height_A.Text.ToCharArray()[0] - 'A' + 1;
                            string value = getExcelValue(FIle_List[i].FullName, row, column);
                            //顯示在GUI介面中
                            imshowValueInMeasurementGUI(value);
                            logger.WriteLog("get measurement value: " + value);
                            logger.WriteLog("New File : " + FIle_List[i].FullName);
                            if (radioButton_Degree_0.Checked)
                            {
                                if (File.Exists(Save_File_Folder + textBox_Point.Text + "_0.xlsx"))
                                    File.Delete(Save_File_Folder + textBox_Point.Text + "_0.xlsx");
                                File.Move(FIle_List[i].FullName, Save_File_Folder + textBox_Point.Text + "_0.xlsx");
                            }
                            else
                            {
                                if (File.Exists(Save_File_Folder + textBox_Point.Text + "_45.xlsx"))
                                    File.Delete(Save_File_Folder + textBox_Point.Text + "_45.xlsx");
                                File.Move(FIle_List[i].FullName, Save_File_Folder + textBox_Point.Text + "_45.xlsx");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        logger.WriteErrorLog("Move xlsx File Error! " + error.ToString());
                    }
                }
            }
            if (checkBox_csv.Checked)
            {
                FileInfo[] FIle_List = folder_info.GetFiles("*.csv");
                if (FIle_List.Length > 0)
                {
                    try
                    {
                        for (int i = 0; i < FIle_List.Length; i++)
                        {
                            if (radioButton_Degree_0.Checked)
                            {
                                //取出csv內所有欄位, 存在2d list中
                                List<List<string>> csv_arr = new List<List<string>>();
                                string csv_file = FIle_List[i].FullName;
                                var reader = new StreamReader(File.OpenRead(csv_file));
                                List<List<string>> tmp = new List<List<string>>();
                                while (!reader.EndOfStream)
                                {
                                    List<string> tmp1 = new List<string>();
                                    var line = reader.ReadLine();
                                    var values = line.Split(',');
                                    foreach (string value in values)
                                    {
                                        tmp1.Add(value);
                                    }
                                    tmp.Add(tmp1);
                                }
                                reader.Close();
                                //從GUI介面中0度height 欄位取出對應csv數值
                                int row = Convert.ToInt32(textBox_0_degree_height_Num.Text) - 1;
                                int column = textBox_0_degree_height_A.Text.ToCharArray()[0] - 'A';
                                //顯示在GUI Measurement 對應 point點位中
                                imshowValueInMeasurementGUI(tmp[row][column]);
                                logger.WriteLog("get measurement value: " + tmp[row][column]);
                                //移動檔案
                                //使用'_'分割檔名
                                string save_degree_0_name = "";
                                logger.WriteLog("split by _ keyword:");
                                string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
                                foreach (string s in split_input_file_names)
                                {
                                    logger.WriteLog(s);
                                }
                                save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
                                string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0.csv";
                                if (File.Exists(save_full_file_name))
                                    File.Delete(save_full_file_name);
                                File.Move(FIle_List[i].FullName, save_full_file_name);
                                logger.WriteLog("New File : " + FIle_List[i].FullName +
                                                    " Move to :" + save_full_file_name);
                            }
                            else
                            {
                                //移動檔案
                                //使用'_'分割檔名
                                string save_degree_45_name = "";
                                logger.WriteLog("split by _ keyword:");
                                string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
                                foreach (string s in split_input_file_names)
                                {
                                    logger.WriteLog(s);
                                }
                                save_degree_45_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
                                string save_full_file_name = Save_File_Folder + save_degree_45_name + textBox_Point.Text + "_45.csv";
                                if (File.Exists(save_full_file_name))
                                    File.Delete(save_full_file_name);
                                File.Move(FIle_List[i].FullName, save_full_file_name);
                                logger.WriteLog("New File : " + FIle_List[i].FullName +
                                                    " Move to :" + save_full_file_name);
                            }
                        }

                    }
                    catch (Exception error)
                    {
                        logger.WriteErrorLog("Move csv File Error! " + error.ToString());
                    }
                }
            }
            if (checkBox_poir.Checked && !copy_poir_once)
            {
                FileInfo[] FIle_List = folder_info.GetFiles("*.poir");
                if (FIle_List.Length > 0)
                {
                    try
                    {
                        for (int i = 0; i < FIle_List.Length; i++)
                        {
                            logger.WriteLog("New File : " + FIle_List[i].FullName);
                            if (radioButton_Degree_0.Checked)
                            {
                                //使用'_'分割檔名
                                string save_degree_0_name = "";
                                logger.WriteLog("split by _ keyword:");
                                string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
                                foreach (string s in split_input_file_names)
                                {
                                    logger.WriteLog(s);
                                }
                                save_degree_0_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
                                string save_full_file_name = Save_File_Folder + save_degree_0_name + textBox_Point.Text + "_0.poir";
                                if (File.Exists(save_full_file_name))
                                    File.Delete(save_full_file_name);
                                File.Copy(FIle_List[i].FullName, save_full_file_name);
                                copy_poir_once = true;
                                Thread.Sleep(15000);
                            }
                            else
                            {
                                //使用'_'分割檔名
                                string save_degree_45_name = "";
                                logger.WriteLog("split by _ keyword:");
                                string[] split_input_file_names = Path.GetFileNameWithoutExtension(FIle_List[i].FullName).Split('_');
                                foreach (string s in split_input_file_names)
                                {
                                    logger.WriteLog(s);
                                }
                                save_degree_45_name += split_input_file_names[0] + "_" + split_input_file_names[1] + "_" + split_input_file_names[2] + "_";
                                string save_full_file_name = Save_File_Folder + save_degree_45_name + textBox_Point.Text + "_45.poir";

                                if (File.Exists(save_full_file_name))
                                    File.Delete(save_full_file_name);
                                File.Copy(FIle_List[i].FullName, save_full_file_name);
                                copy_poir_once = true;
                                Thread.Sleep(15000);
                            }
                        }

                    }
                    catch (Exception error)
                    {
                        logger.WriteErrorLog("Copy poir File Error! " + error.ToString());
                    }
                }
            }
            else if (checkBox_poir.Checked && copy_poir_once)
            {
                FileInfo[] FIle_List = folder_info.GetFiles("*.poir");
                if (FIle_List.Length > 0)
                {
                    try
                    {
                        for (int i = 0; i < FIle_List.Length; i++)
                        {
                            File.Delete(FIle_List[i].FullName);
                            copy_poir_once = false;
                        }
                    }
                    catch (Exception error)
                    {
                        logger.WriteErrorLog($"Delete poir File Error!  {error.Message}");
                    }
                }
            }
        }
        private void AoiMeansure(string[] aoiImages)
        {
            if (checkBox_bmp.Checked)
            {

                FileInfo[] FIle_List = folder_info.GetFiles("*.bmp");
                //try
                //{
                for (int i = 0; i < FIle_List.Length; i++)
                {

                    string[] file_list_part_name = FIle_List[i].FullName.Split('_');
                    logger.WriteLog("file last part name:" + file_list_part_name[file_list_part_name.Length - 1]);
                }

                if (radioButton_Degree_0.Checked) //0度
                {
                    foreach (var imageName in aoiImages)
                    {
                        AoiDegree_0(imageName);
                    }


                }
                else //45度
                {

                    //執行AOI計算
                    if (is_hand_measurement)
                    {

                        AOI_Calculate(Hand_Measurement, aoiImages[0], aoiImages[1], aoiImages[2], is_hand_measurement);
                        logger.WriteLog("手動量測");

                        logger.WriteLog("Img file 1 : " + aoiImages[0]);
                        logger.WriteLog("Img file 2 : " + aoiImages[1]);
                        logger.WriteLog("Img file 3 : " + aoiImages[2]);
                        logger.WriteLog("AOI_Calculate");
                        //   button_hb_on_Click(sender, e);


                    }
                    else
                    {

                        AOI_Calculate(AOI_Measurement, aoiImages[0], aoiImages[1], aoiImages[2], is_hand_measurement);
                        logger.WriteLog("AOI自動量測");

                        logger.WriteLog("Img file 1 : " + aoiImages[0]);
                        logger.WriteLog("Img file 2 : " + aoiImages[1]);
                        logger.WriteLog("AOI_Calculate");
                        //   button_hb_on_Click(sender, e);


                    }

                    //if (count > 3)//已經存超過兩張
                    //{
                    //    //執行AOI計算
                    //    if (is_hand_measurement)
                    //    {
                    //        AOI_Calculate(Hand_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[1], Save_AOI_file_name[2]);
                    //        logger.Write_Logger("手動量測");
                    //    }
                    //    else
                    //    {
                    //        AOI_Calculate(AOI_Measurement, Save_AOI_file_name[0], Save_AOI_file_name[1], Save_AOI_file_name[2]);
                    //        logger.Write_Logger("AOI自動量測");
                    //    }

                    //    count = 1;
                    //    logger.Write_Logger("Img file 1 : " + Save_AOI_file_name[0]);
                    //    logger.Write_Logger("Img file 2 : " + Save_AOI_file_name[1]);
                    //    logger.Write_Logger("Img file 3 : " + Save_AOI_file_name[2]);
                    //    logger.Write_Logger("AOI_Calculate");
                    //    button_hb_on_Click(sender, e);
                    //}

                }


            }
            AoiOutputData();
        }

        private void timer_Initial_Tick(object sender, EventArgs e)
        {
            //server判斷甚麼時候要做事,再傳給client端,client是按左鍵,座標是server預設好的
            /*    if (now_button_click_delay >= button_click_times)
                {
                    now_button_click_delay = 0;
                    if (OLS_Initial_Now_Step == 0 && checkBox_Step_1.Checked)
                    {
                        Cursor.Position = new Point(Convert.ToInt32(variable_data.Initial_Step_1_X), Convert.ToInt32(variable_data.Initial_Step_1_Y));
                        LeftClick();
                        OLS_Initial_Now_Step = 2;
                    }
                    else if (OLS_Initial_Now_Step == 1 && checkBox_Step_2.Checked)
                    {
                        Cursor.Position = new Point(Convert.ToInt32(variable_data.Initial_Step_2_X), Convert.ToInt32(variable_data.Initial_Step_2_Y));
                        LeftClick();
                        OLS_Initial_Now_Step = 2;
                    }
                    else if (OLS_Initial_Now_Step == 2 && checkBox_Step_3.Checked)
                    {
                        Cursor.Position = new Point(Convert.ToInt32(variable_data.Initial_Step_3_X), Convert.ToInt32(variable_data.Initial_Step_3_Y));
                        LeftClick();
                        OLS_Initial_Now_Step = 3;
                    }
                    else if (checkBox_Step_4.Checked)
                    {
                        button_click_times = variable_data.Initial_Step_4_Delay_Time * 1000 / timer_Initial.Interval;
                        OLS_Initial_Now_Step = 4;
                    }
                    else if (OLS_Initial_Now_Step == 4 || (!checkBox_Step_1.Checked && !checkBox_Step_2.Checked && !checkBox_Step_3.Checked && !checkBox_Step_3.Checked))
                    {
                        timer_Initial.Enabled = false;
                        OLS_Initial_Now_Step = 0;
                        Send_Server("07,Init,e>");
                    }
                }
                else
                {
                    now_button_click_delay++;
                }*/
        }
        private void button_Open_Hide_Click(object sender, EventArgs e)
        {
            /* try
             {
                 logger.WriteLog("Open Hide 1");
                 string send_data_str = get_socket_send_data();
                 //clientSocket_OLS.Send(StringToByteArray(send_data_str));
                 Thread.Sleep(100);
                 clientSocket_OLS.Send(StringToByteArray("open_1"));
                 Thread.Sleep(100);
             }
             catch (Exception error)
             {
                 logger.WriteErrorLog("Open Hide 1 Error! " + error.ToString());
             }*/
        }
        private void button_Close_Hide_Click(object sender, EventArgs e)
        {
            /* try
             {
                 logger.WriteLog("Close Hide 1");
                 clientSocket_OLS.Send(StringToByteArray("close_1"));
                 Thread.Sleep(100);
             }
             catch (Exception error)
             {
                 logger.WriteErrorLog("Close Hide 1 Error! " + error.ToString());
             }*/
        }
        private void button_Open_Hide_2_Click(object sender, EventArgs e)
        {
            /* try
             {
                 logger.WriteLog("Open Hide 2");
                 string send_data_str = get_socket_send_data();
                 //clientSocket_OLS.Send(StringToByteArray(send_data_str));
                 Thread.Sleep(100);
                 clientSocket_OLS.Send(StringToByteArray("open_2"));
                 Thread.Sleep(100);
             }
             catch (Exception error)
             {
                 logger.WriteErrorLog("Open Hide 2 Error! " + error.ToString());
             }*/
        }
        private void button_Close_Hide_2_Click(object sender, EventArgs e)
        {
            /* try
             {
                 logger.WriteLog("Close Hide 2");
                 clientSocket_OLS.Send(StringToByteArray("close_2"));
                 Thread.Sleep(100);
             }
             catch (Exception error)
             {
                 logger.WriteErrorLog("Close Hide 2 Error! " + error.ToString());
             }*/
        }
        private void timer_Mouse_Point_Tick(object sender, EventArgs e)
        {
            now_delay = 0;
        }
        private void timer_Open_Hide_Tick(object sender, EventArgs e)
        {
            if (open_hide_1 && open_hide_1 != open_hide_1_Old)
            {
                button_Open_Hide_Click(sender, e);
                open_hide_1_Old = open_hide_1;
            }
            else if (!open_hide_1 && open_hide_1 != open_hide_1_Old)
            {
                button_Close_Hide_Click(sender, e);
                open_hide_1_Old = open_hide_1;
            }
            else if (open_hide_2 && open_hide_2 != open_hide_2_Old)
            {
                button_Open_Hide_2_Click(sender, e);
                open_hide_2_Old = open_hide_2;
            }
            else if (!open_hide_2 && open_hide_2 != open_hide_2_Old)
            {
                button_Close_Hide_2_Click(sender, e);
                open_hide_2_Old = open_hide_2;
            }
        }
        private void timer_connect_client_Tick(object sender, EventArgs e)
        {
            /* if (!bgWorkerServerRun.IsBusy)
             {
                 bgWorkerServerRun.RunWorkerAsync();
             }
            */
        }
        private void bgWorkerServerRun_DoWork(object sender, DoWorkEventArgs e)
        {/*
            try
            {
                if (!connect_OLS_client)
                {
                    //連線成功
                    clientSocket_OLS = Socketserver_OLS.Accept();
                    connect_OLS_client = true;
                    UpdateTextboxAdd("Client connect ip:" + IPAddress.Parse(((IPEndPoint)clientSocket_OLS.RemoteEndPoint).Address.ToString()) + Environment.NewLine, textBox_Server_Receive);
                    //傳送預設資料
                    string send_data_str = get_socket_send_data();
                    byte[] send_data = new byte[send_data_str.Length];
                    for (int i = 0; i < send_data_str.Length; i++)
                        send_data[i] = Convert.ToByte(send_data_str[i]);
                    logger.WriteLog("Connect client : " + IPAddress.Parse(((IPEndPoint)clientSocket_OLS.RemoteEndPoint).Address.ToString()) + Environment.NewLine);
                    clientSocket_OLS.Send(send_data);
                }
                else
                {
                    try
                    {
                        byte[] Receive_data = new byte[1024];
                        string receive_data = "";
                        //通過clientSocket接收資料
                        int receiveNumber = clientSocket_OLS.Receive(Receive_data);
                        for (int i = 0; i < 1024; i++)
                        {
                            if (Receive_data[i] == 0)
                                break;
                            else
                            {
                                receive_data += Convert.ToString(Convert.ToChar(Receive_data[i]));
                            }
                        }
                        if (receive_data == "hand measurement on")
                        {
                            UpdateTextboxAdd("進入手動量測模式" + "\r\n", textBox_Server_Receive);
                            is_hand_measurement = true;
                        }
                        //從cover_and_init接收資料
                        UpdateTextboxAdd("OLS Recive:" + receive_data + "\r\n", textBox_Server_Receive);

                        logger.WriteLog("OLS Recive:" + receive_data);

                    }
                    catch (Exception ex)
                    {
                        logger.WriteErrorLog(ex.ToString());
                        int aaa = ex.HResult;
                        if (aaa == -2147467259)
                        {
                            clientSocket_OLS.Shutdown(SocketShutdown.Both);
                            clientSocket_OLS.Close();
                            connect_OLS_client = false;
                            logger.WriteErrorLog("OLS Disconnect!");
                        }
                    }
                }
            }
            catch (Exception error)
            {
                logger.WriteErrorLog("OLS error" + error.ToString());
            }*/
        }
        private void button_Send_Client_Click(object sender, EventArgs e)
        {
            /* try
             {
                 string send_ss = textBox_Server_Send.Text;
                 byte[] send_data = new byte[send_ss.Length];
                 for (int i = 0; i < send_ss.Length; i++)
                     send_data[i] = Convert.ToByte(send_ss[i]);
                 clientSocket_OLS.Send(send_data);
                 textBox_Server_Receive.Text += "Send:" + send_ss + Environment.NewLine;
                 timer_Server.Enabled = true;
             }
             catch (Exception error)
             {
                 MessageBox.Show(error.ToString());
             }*/
        }
        private void button_Start_Server_Click(object sender, EventArgs e)
        {
            /* try
             {
                 logger.WriteLog("Create OLS Server");
                 string ip_address = comboBox_IP.Text;
                 IPAddress ip = IPAddress.Parse(ip_address);
                 int port = Convert.ToInt32(textBox_Server_Port.Text);
                 Socketserver_OLS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                 Socketserver_OLS.Bind(new IPEndPoint(ip, port));  //繫結IP地址：埠
                 Socketserver_OLS.Listen(10);    //設定最多10個排隊連線請求
                 textBox_Server_Receive.Text += "socket start" + Environment.NewLine;
                 timer_Server.Enabled = true;
                 timer_connect_client.Enabled = true;
                 timer_Open_Hide.Enabled = true;
                 logger.WriteLog("Create OLS Server Successful");
             }
             catch (Exception error)
             {
                 logger.WriteErrorLog("Create OLS Server Fail ! " + error.ToString());
             }
            */

        }
        private void button_auto_click_Sp1_Click(object sender, EventArgs e)
        {
            /* Thread.Sleep(1000);
             string send_data_str = get_socket_send_data();
             //clientSocket_OLS.Send(StringToByteArray(send_data_str));
             //Thread.Sleep(100);
             clientSocket_OLS.Send(StringToByteArray("SP1"));
             Thread.Sleep(100);*/

        }
        private void button_auto_click_Sp2_Click(object sender, EventArgs e)
        {
            /* string send_data_str = get_socket_send_data();
             //clientSocket_OLS.Send(StringToByteArray(send_data_str));
             //Thread.Sleep(100);
             clientSocket_OLS.Send(StringToByteArray("SP2"));
             Thread.Sleep(100);*/
        }
        private void button_auto_click_Sp3_Click(object sender, EventArgs e)
        {
            /*string send_data_str = get_socket_send_data();
            //clientSocket_OLS.Send(StringToByteArray(send_data_str));
            //Thread.Sleep(100);
            clientSocket_OLS.Send(StringToByteArray("SP3"));
            Thread.Sleep(100);*/
        }
        #endregion

        private void button_auto_click_Sp5_Click(object sender, EventArgs e)
        {
            /* string send_data_str = get_socket_send_data();
             //clientSocket_OLS.Send(StringToByteArray(send_data_str));
             //Thread.Sleep(100);
             clientSocket_OLS.Send(StringToByteArray("SP5"));
             Thread.Sleep(100);*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cal_File_Address();
        }

        private void textBox_Mesument_45_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (e.KeyCode == Keys.Enter && textBox.Text != "")
            {
                try
                {
                    double value = Math.Round(Convert.ToDouble(textBox.Text) * Math.Sqrt(2), 5);
                    textBox.Text = value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }

        }

        private void button_hb_on_Click(object sender, EventArgs e)
        {
            /* string send_data_str = get_socket_send_data();
              is_hand_measurement = false;
               //clientSocket_OLS.Send(StringToByteArray(send_data_str));
              Thread.Sleep(100);
              clientSocket_OLS.Send(StringToByteArray("open_hb"));
              Thread.Sleep(100);*/
        }

        private void numericUpDown_AOI_save_idx1_ValueChanged(object sender, EventArgs e)
        {
            //設定 AOI 存圖 索引
            if (AOI_Measurement != null)
            {
                AOI_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                AOI_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                AOI_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                AOI_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                AOI_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                AOI_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;
            }
            if (Hand_Measurement != null)
            {
                Hand_Measurement.save_AOI_result_idx_1 = (int)numericUpDown_AOI_save_idx1.Value;
                Hand_Measurement.save_AOI_result_idx_2 = (int)numericUpDown_AOI_save_idx2.Value;
                Hand_Measurement.save_AOI_result_idx_3 = (int)numericUpDown_AOI_save_idx3.Value;
                Hand_Measurement.manual_save_AOI_result_idx_1 = (int)numericUpDown_manual_save_idx1.Value;
                Hand_Measurement.manual_save_AOI_result_idx_2 = (int)numericUpDown_manual_save_idx2.Value;
                Hand_Measurement.manual_save_AOI_result_idx_3 = (int)numericUpDown_manual_save_idx3.Value;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Receive_SetRecipe(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Receive_Init();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                Receive_Mode("Top");
            }
            else
            {
                Receive_Mode("Side");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Receive_Start(18, "12345678", 1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Receive_InPos((int)numericUpDown1.Value);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Receive_Stop("0000");
        }

        private void button_update_value_Click(object sender, EventArgs e)
        {
            // string send_data_str = get_socket_send_data();
            //  clientSocket_OLS.Send(StringToByteArray(send_data_str));
        }


        #region AOI 測量用



        private void button8_Click(object sender, EventArgs e)
        {

            //    var window =   new SPIL_TCPSimulator.MainWindow();
            CB_RecipeList.SelectedIndex = 3;

            // if (hostCommunication == null)
            //     hostCommunication = new HostCommunication("127.0.0.1", 1234);
            // else
            //     hostCommunication.Open();

        }

        private void HostException(Exception exception)
        {
            MessageBox.Show($"Host Communication Error : {exception} ");
            hostCommunication.Open();
        }

        private void btn_AOIOpenImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP files (*.bmp)|*.bmp|JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png";
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                sharpnessImage = new Bitmap(dlg.FileName);



                //     var cogGM = new CogGapCaliper { MethodName = MethodName.GapMeansure };

                //     cogGM.EditParameter(image);
            }
        }
        private void listBox_SharpnessAlgorithmList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (sharpnessImage == null) throw new Exception($"Image not exist");
                int index = listBox_SharpnessAlgorithmList.SelectedIndex;
                //AOIParams  與 UIListbox 的順序一致  所以直接拿位置
                var algorithmItem = sPILRecipe.ClarityParams[index];
                //參數塞到  aoIFlow.CogAOIMethods 對應的方法
                sharpnessFlow.CogMethods[index].method.RunParams = algorithmItem;
                sharpnessFlow.CogMethods[index].method.EditParameter(sharpnessImage);
                //參數寫回  sPILRecipe.AOIParams
                algorithmItem = sharpnessFlow.CogMethods[index].method.RunParams;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void btn_RecipeSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = tBx_RecipeName.Text;

                string path = $"{systemPath}\\Recipe\\{name}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                else
                {
                    var result = MessageBox.Show("Do you want to replace the existing file?", "", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.Cancel) return;
                }



                SaveRecipe(sPILRecipe, path);
                MessageBox.Show("存檔完成");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }



        }

        private void CB_RecipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string name = CB_RecipeList.SelectedItem.ToString();
                string path = $"{systemPath}\\Recipe\\{name}";
                LoadRecipe(path);
                MessageBox.Show("讀取完成");
                groupBox18.Enabled = true;
                groupBox17.Enabled = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void LoadRecipe(string path)
        {

            //讀取 料號 實際Cognex參數存在這
            sPILRecipe.Load(path);
            UpdateTextbox(new DirectoryInfo(path).Name, tBx_RecipeName);

            //        UpdateTextbox(machineSetting.AOIVppPath, tbx_AOIPath);
            //        UpdateTextbox(machineSetting.SharpVppPath, tbx_SharpPath);
            //tBx_RecipeName.Text = new DirectoryInfo(path).Name;
            //tbx_AOIPath.Text = machineSetting.AOIVppPath;
            //tbx_SharpPath.Text = machineSetting.SharpVppPath;
            aoIFlow.SetMethodParam(sPILRecipe.AOIParams);
            sharpnessFlow.SetMethodParam(sPILRecipe.ClarityParams);


        }
        private void SaveRecipe(SPILRecipe recipe, string path)
        {
            recipe.Save(path);
        }

        private void btn_ReadAOIVPP_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            listBox_AOIAlgorithmList.Items.Clear();
            dlg.Filter = "vpp |*.vpp";
            dlg.FileName = machineSetting.AOIVppPath;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                tbx_AOIPath.Text = dlg.FileName;
                //  toolBlock = CogSerializer.LoadObjectFromFile(dlg.FileName) as CogToolBlock;


                machineSetting.AOIVppPath = tbx_AOIPath.Text;

                machineSetting.Save($"{systemPath}\\machineConfig.cfg");
            }
        }

        private void btn_ReadSharpVPP_Click(object sender, EventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            listBox_AOIAlgorithmList.Items.Clear();
            dlg.Filter = "vpp |*.vpp";
            dlg.FileName = machineSetting.SharpVppPath;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                tbx_SharpPath.Text = dlg.FileName;
                //  toolBlock = CogSerializer.LoadObjectFromFile(dlg.FileName) as CogToolBlock;


                machineSetting.SharpVppPath = tbx_SharpPath.Text;
                machineSetting.Save($"{systemPath}\\machineConfig.cfg");
            }
        }

        private void CB_RecipeList_Click(object sender, EventArgs e)
        {
            string folderPath = $"{systemPath}\\Recipe"; // 資料夾路徑

            // 取得資料夾內所有資料夾的路徑
            string[] subDirectories = Directory.GetDirectories(folderPath);
            CB_RecipeList.Items.Clear();
            var dirNames = subDirectories.Select(d => new DirectoryInfo(d).Name);
            foreach (var item in dirNames)
            {
                CB_RecipeList.Items.Add(item);
            }

        }


        private async void btn_AOITesting_Click(object sender, EventArgs e)
        {
            try
            {


                /* Bitmap img1 = new Bitmap(txB_RecipePicName1.Text);
                 Bitmap img2 = new Bitmap(txB_RecipePicName2.Text);
                 Bitmap img3 = new Bitmap(txB_RecipePicName3.Text);
                 var cord = aoIFlow.Measurment(img1, img2, img3, out double distance_CuNi, out double distance_Cu);*/



                cogRcdDisp_Distance1.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.Touch;
                cogRcdDisp_Distance2.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.Touch;
                cogRcdDisp_Distance3.MouseMode = Cognex.VisionPro.Display.CogDisplayMouseModeConstants.Touch;

                cogRcdDisp_Distance1.AutoFit = true;
                cogRcdDisp_Distance2.AutoFit = true;
                cogRcdDisp_Distance3.AutoFit = true;
                /*    
                   cogRcdDisp_Distance1.Record = cord.SubRecords["CogFixtureTool1.OutputImage"];
                   cogRcdDisp_Distance2.Record = cord.SubRecords["CogFixtureTool2.OutputImage"];
                   cogRcdDisp_Distance3.Record = cord.SubRecords["CogFixtureTool3.OutputImage"];

                   img1.Dispose();
                   img2.Dispose();
                   img3.Dispose();
                
                 */

                AOI_Measurement.ShowRecord += UpdateAOIRecord;
                AOI_Measurement.MeasureToolBlock = aoIFlow.MeasureToolBlock;




                var result = AOI_Calculate(AOI_Measurement, txB_RecipePicName1.Text, txB_RecipePicName2.Text, txB_RecipePicName3.Text, false);
                tBx_CuNiValue.Text = result.cuNi.ToString("0.000");
                tBx_CuValue.Text = result.cu.ToString("0.000");
                await Task.Delay(1000);



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                AOI_Measurement.ShowRecord -= UpdateAOIRecord;
            }
        }

        private void btn_AOIOpenImage1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP files & JPG files |*.bmp;*.jpg|PNG files (*.png)|*.png";
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                aoiImage1 = new Bitmap(dlg.FileName);


                txB_RecipePicName1.Text = dlg.FileName;


            }
        }



        private void btn_AOIOpenImage2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP files & JPG files |*.bmp;*.jpg|PNG files (*.png)|*.png";
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                aoiImage2 = new Bitmap(dlg.FileName);

                txB_RecipePicName2.Text = dlg.FileName;


            }
        }

        private void btn_AOIOpenImage3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP files & JPG files |*.bmp;*.jpg|PNG files (*.png)|*.png";
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                aoiImage3 = new Bitmap(dlg.FileName);


                txB_RecipePicName3.Text = dlg.FileName;


            }
        }
        #endregion


        private void btn_OpenSharpnessImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP files & JPG files |*.bmp;*.jpg|PNG files (*.png)|*.png";
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {// 載入圖片

                sharpnessImage = new Bitmap(dlg.FileName);

                pBox_SharpnessPic.Image = sharpnessImage;
                pBox_SharpnessPic.SizeMode = PictureBoxSizeMode.Zoom;

                txB_SharpnessPicName.Text = dlg.FileName;

                //     var cogGM = new CogGapCaliper { MethodName = MethodName.GapMeansure };

                //     cogGM.EditParameter(image);
            }
        }
        private void btn_SharpnessRun_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap img1 = new Bitmap(txB_SharpnessPicName.Text);
                //  cogRecordDisplay2.Size = new System.Drawing.Size(img1.Width, img1.Height);
                var sharpResult = sharpnessFlow.Measurment(img1);

                if (sharpResult.result == null) throw new Exception(sharpnessFlow.ResultMessage);

                tbx_SearchScore1.Text = sharpResult.result.SearchScore1.ToString("0.00000");
                tbx_SearchScore2.Text = sharpResult.result.SearchScore2.ToString("0.00000");

                tbx_SharpnessScore1.Text = sharpResult.result.Score1.ToString("0.00000000");
                tbx_SharpnessScore2.Text = sharpResult.result.Score2.ToString("0.00000000");
                tbx_SharpnessScore3.Text = sharpResult.result.Score3.ToString("0.00000000");

                cogRecordDisplay2.Record = sharpResult.cogRecord;

                img1.Dispose();



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }




        }

        private void listBox_AlgorithmList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (txB_RecipePicName1.Text == "" || txB_RecipePicName2.Text == "" || txB_RecipePicName3.Text == "")
                    throw new Exception("picture  not exist");
                //   if (sharpnessImage == null) throw new Exception($"Image not exist");
                Bitmap img1 = new Bitmap(txB_RecipePicName1.Text);
                Bitmap img2 = new Bitmap(txB_RecipePicName2.Text);
                Bitmap img3 = new Bitmap(txB_RecipePicName3.Text);

                //先跑過一次 把圖片都吃進去 ， 再把輸入的圖片拿出來
                aoIFlow.Measurment(img1, img2, img3, out double distance_CuNi, out double distance_Cu);
                var inputImage = aoIFlow.RunningToolInputImage(machineSetting.AOIAlgorithms[listBox_AOIAlgorithmList.SelectedIndex].Name);

                //   var select = listBox_AOIAlgorithmList.SelectedItem;
                //AOIParams  與 UIListbox 的順序一致  所以直接拿位置
                CogParameter algorithmItem = sPILRecipe.AOIParams[listBox_AOIAlgorithmList.SelectedIndex];

                //參數塞到  aoIFlow.CogAOIMethods 對應的方法
                aoIFlow.CogMethods[listBox_AOIAlgorithmList.SelectedIndex].method.RunParams = algorithmItem;
                aoIFlow.CogMethods[listBox_AOIAlgorithmList.SelectedIndex].method.EditParameter(inputImage);
                //參數寫回  sPILRecipe.AOIParams
                algorithmItem = aoIFlow.CogMethods[listBox_AOIAlgorithmList.SelectedIndex].method.RunParams;

                img1.Dispose();
                img2.Dispose();
                img3.Dispose();
                //   var a  = aoiImage.ImageToBytes(aoiImage.RawFormat);
                /*   var tool = toolBlock.Tools[algorithmItem.Name];


                   algorithmItem.CogAOIMethod.SetCogToolParameter(tool);
                   algorithmItem.CogAOIMethod.EditParameter(aoiImage);

                   tool = algorithmItem.CogAOIMethod.GetCogTool();
                */


                /* switch (algorithmItem.CogMethodtype) {

                     case MethodType.CogSearchMaxTool:
                         CogSearchMax gapCaliper = algorithmItem.CogAOIMethod as CogSearchMax;

                         gapCaliper.EditParameter(aoiImage);
                         break;
                     case MethodType.CogImageConvertTool:
                         CogImageConverter imageConvert = algorithmItem.CogAOIMethod as CogImageConverter;
                         imageConvert.EditParameter(aoiImage);
                         break;
                     case MethodType.CogFindEllipseTool:
                         CogEllipseCaliper ellipseCaliper = algorithmItem.CogAOIMethod as CogEllipseCaliper;

                         ellipseCaliper.EditParameter(aoiImage);
                         break;
                     default:
                         break;
                 }*/
                //  CogSerializer.SaveObjectToFile(toolBlock, "D:\\test-2.vpp");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void listBox_SharpnessAlgorithmList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (txB_SharpnessPicName.Text == "") throw new Exception($"Image not exist");
                Bitmap img1 = new Bitmap(txB_SharpnessPicName.Text);


                //先跑過一次 把圖片都吃進去 ， 再把輸入的圖片拿出來
                sharpnessFlow.Measurment(img1);
                var inputImage = sharpnessFlow.RunningToolInputImage(machineSetting.SharpAlgorithms[listBox_SharpnessAlgorithmList.SelectedIndex].Name);

                //   var select = listBox_AOIAlgorithmList.SelectedItem;
                //AOIParams  與 UIListbox 的順序一致  所以直接拿位置
                CogParameter algorithmItem = sPILRecipe.ClarityParams[listBox_SharpnessAlgorithmList.SelectedIndex];

                //參數塞到  aoIFlow.CogAOIMethods 對應的方法
                sharpnessFlow.CogMethods[listBox_SharpnessAlgorithmList.SelectedIndex].method.RunParams = algorithmItem;
                sharpnessFlow.CogMethods[listBox_SharpnessAlgorithmList.SelectedIndex].method.EditParameter(inputImage);
                //參數寫回  sPILRecipe.AOIParams
                algorithmItem = sharpnessFlow.CogMethods[listBox_SharpnessAlgorithmList.SelectedIndex].method.RunParams;

                img1.Dispose();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }



        private async void btn_SharpnessMultRun_Click(object sender, EventArgs e)
        {
            try
            {


                // 建立 FolderBrowserDialog 物件
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                dataGrid_Sharpness.Rows.Clear();
                // 設定對話方塊的標題
                folderBrowserDialog.Description = "請選取資料夾";
                var CurrentDirectory = Directory.GetCurrentDirectory() + "\\dir.txt";

                if (File.Exists(CurrentDirectory))
                    folderBrowserDialog.SelectedPath = File.ReadAllText(CurrentDirectory);

                // 顯示對話方塊並等待使用者選擇資料夾
                DialogResult result = folderBrowserDialog.ShowDialog();
                File.WriteAllText(CurrentDirectory, folderBrowserDialog.SelectedPath);
                // 檢查使用者是否選擇了資料夾
                if (result == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;

                    // 取得資料夾中的所有檔案
                    string[] files = Directory.GetFiles(folderPath);
                    List<SharpnessResult> sharpnessResults = new List<SharpnessResult>();
                    List<Bitmap> images = new List<Bitmap>();
                    List<string> imageNames = new List<string>();

                    // 遍歷每個檔案，檢查是否為影像檔
                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file).ToLower();
                        string name = Path.GetFileName(file);
                        // 檢查副檔名是否為影像檔（可根據需求調整）
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                        {

                            images.Add(new Bitmap(file));
                            imageNames.Add(file);
                            /* using (Bitmap img1 = new Bitmap(file))
                             {
                                 SharpnessResult sharpResult = null;
                                 await Task.Run(async () =>
                              {

                                  sharpResult = await sharpnessFlow.Measurment(img1);
                                  sharpnessResults.Add(sharpResult);
                              });

                                 if (sharpResult == null)
                                     dataGrid_Sharpness.Rows.Add(name, -1, -1, -1, -1, -1);
                                 else
                                 {
                                     dataGrid_Sharpness.Rows.Add(name, sharpResult.SearchScore1.ToString("0.00000"), sharpResult.SearchScore2.ToString("0.00000"),
                                         sharpResult.Score1.ToString("0.00000000"), sharpResult.Score2.ToString("0.00000000"), sharpResult.Score3.ToString("0.00000000"));
                                     cogRecordDisplay2.Record = sharpResult.CogRecord;
                                 }
                             }*/
                        }
                    }

                    sharpnessFlow.WriteCogResult += UpdateDataGridView;

                    //   sharpnessFlow.CogResult += UpdateDataGridView;
                    sharpnessFlow.MethodAssignTool();
                    var imagesIndex = await Task.Run(() => sharpnessFlow.SharpnessAnalyzeAsync(images));


                    aoiImage1 = images[imagesIndex.Image1Index];
                    aoiImage2 = images[imagesIndex.Image2Index];
                    aoiImage3 = images[imagesIndex.Image3Index];

                    txB_RecipePicName1.Text = imageNames[imagesIndex.Image1Index];
                    txB_RecipePicName2.Text = imageNames[imagesIndex.Image2Index];
                    txB_RecipePicName3.Text = imageNames[imagesIndex.Image3Index];
                    // MessageBox.Show($"清晰度計算完成: {imageNames[imagesIndex.Image1Index]}  ， { imageNames[imagesIndex.Image2Index]}  ，{imageNames[imagesIndex.Image3Index]}");

                    foreach (var image in images)
                        image.Dispose();


                    MessageBox.Show($"清晰度計算完成:  ");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

                sharpnessFlow.WriteCogResult -= UpdateDataGridView;
            }
        }

        private void UpdateAOIRecord(ICogRecord cogRecord1, ICogRecord cogRecord2, ICogRecord cogRecord3)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            int index = dataGrid_Sharpness.Rows.Count;

            if (dataGrid_Sharpness.InvokeRequired)
            {
                // 如果不在 UI 執行緒，則使用 Control.Invoke 方法在 UI 執行緒上執行程式碼
                dataGrid_Sharpness.Invoke(new Action<ICogRecord, ICogRecord, ICogRecord>(UpdateAOIRecord), new object[] { cogRecord1, cogRecord2, cogRecord3 });
            }
            else
            {

                // 在此範例中，將資料新增至 DataGridView
                cogRcdDisp_Distance1.Record = cogRecord1;
                cogRcdDisp_Distance2.Record = cogRecord2;
                cogRcdDisp_Distance3.Record = cogRecord3;

            }
        }
        private void UpdateDataGridView(SharpnessResult sharpResult, ICogRecord cogRecord)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            int index = dataGrid_Sharpness.Rows.Count;

            if (dataGrid_Sharpness.InvokeRequired)
            {
                // 如果不在 UI 執行緒，則使用 Control.Invoke 方法在 UI 執行緒上執行程式碼
                dataGrid_Sharpness.Invoke(new Action<SharpnessResult, ICogRecord>(UpdateDataGridView), new object[] { sharpResult, cogRecord });
            }
            else
            {

                // 在此範例中，將資料新增至 DataGridView
                if (sharpResult == null)
                    dataGrid_Sharpness.Rows.Add(index, -1, -1, -1, -1, -1);
                else
                {
                    dataGrid_Sharpness.Rows.Add(index, sharpResult.SearchScore1.ToString("0.00000"), sharpResult.SearchScore2.ToString("0.00000"),
                        sharpResult.Score1.ToString("0.00000000"), sharpResult.Score2.ToString("0.00000000"), sharpResult.Score3.ToString("0.00000000"));

                    cogRecordDisplay2.Record = cogRecord;
                    cogRecordDisplay2.Record.SubRecords.Clear();
                }
                //dataGrid_Sharpness.Rows.Add(sharpResult);
            }
        }
        private void ReciveException(Exception exception)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            MessageBox.Show(exception.Message);

        }

        private async void ReciveMessage(string receiveData)
        {
            try
            {
                int id = Thread.CurrentThread.ManagedThreadId;

                logger.WriteLog("Receive : " + receiveData);

                string[] re_data = Cal_Recive_Data(receiveData);

                if (re_data != null)
                {

                    if (re_data[0].Contains("YuanLi"))
                        Receive_YuanLi();
                    else if (re_data[0].Contains("Init"))
                        Receive_Init();
                    else if (re_data[0].Contains("SetRecipe"))
                        Receive_SetRecipe(re_data[1]);
                    else if (re_data[0].Contains("Mode"))
                        Receive_Mode(re_data[1]);
                    else if (re_data[0].Contains("Start"))
                        Receive_Start(Convert.ToInt32(re_data[1]), re_data[2], Convert.ToInt32(re_data[3]));
                    else if (re_data[0].Contains("InPos"))
                        Receive_InPos(Convert.ToInt32(re_data[1]));
                    else if (re_data[0].Contains("Stop"))
                        Receive_Stop(re_data[1]);
                    else if (re_data[0].Contains("RFID"))
                        Receive_RFID(re_data[1], re_data[2]);
                    else if (re_data[0].Contains("AOIRun"))
                        await Receive_AOI();


                    else
                        logger.WriteErrorLog("No Match Data!");
                }
                else
                    logger.WriteErrorLog("Motion Client Receive Error : " + receiveData);

            }
            catch (Exception ex)
            {
                ShowMessageBoxFromThread(ex.Message);

            }
        }
        private delegate void ShowMessageBoxDelegate(string message);

        private void ShowMessageBoxFromThread(string message)
        {
            // 檢查是否需要進行跨線程調用
            if (InvokeRequired)
            {
                // 使用委託在UI線程上執行顯示消息框的操作
                Invoke(new ShowMessageBoxDelegate(ShowMessageBoxFromThread), message);
            }
            else
            {
                // 在UI線程上顯示消息框
                MessageBox.Show(message);
            }
        }
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button_hb_off_Click(object sender, EventArgs e)
        {
            HB_off();
        }
        private void HB_off()
        {
            /* string send_data_str = get_socket_send_data();
             //clientSocket_OLS.Send(StringToByteArray(send_data_str));
             Thread.Sleep(100);
             clientSocket_OLS.Send(StringToByteArray("close_hb"));
             Thread.Sleep(100);*/

        }


    }



}
