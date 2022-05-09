using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace SPIL
{
    class Variable_Data
    {
        public byte[] IP_Address = new byte[4];
        public int IP_Port;
        public string OLS_Name;
        public string Windows_Name;
        public string Vision_Pro_File;
        public string Save_File_Path_1;
        public string Save_File_Path_2;
        public string Save_File_Path_3;
        public string Save_File_Name;
        public string Sample_Excel_File;
        public int Recipe_Name_Locate_Num;
        public double Degree_Ratio = 1.0;
        public bool Auto_Start = false;
        public string OLS_Folder;
        public int Cover_Start_X1;
        public int Cover_Start_Y1;
        public int Cover_End_X1;
        public int Cover_End_Y1;
        public int Cover_Start_X2;
        public int Cover_Start_Y2;
        public int Cover_End_X2;
        public int Cover_End_Y2;
        public bool Initial_Step_1 = false;
        public int Initial_Step_1_X;
        public int Initial_Step_1_Y;
        public bool Initial_Step_2 = false;
        public int Initial_Step_2_X;
        public int Initial_Step_2_Y;
        public bool Initial_Step_3 = false;
        public int Initial_Step_3_X;
        public int Initial_Step_3_Y;
        public bool Initial_Step_4 = false;
        public int Initial_Step_4_Delay_Time;
        public bool Initial_Step_5 = false;
        public int Initial_Step_5_X;
        public int Initial_Step_5_Y;
        public bool Initial_Step_6 = false;
        public int Initial_Step_6_Delay_Time;
        //移動檔案設定
        public bool Need_Move_bmp = false;  
        public bool Need_Move_poir = false;
        public bool Need_Move_xlsx = false;
        public bool Need_Move_csv = false;
        public int delete_data_setting = -1; //刪除資料夾設定時間
        public string[] Degree_height_A = new string[1];//量測輪廓高度欄位
        public string[] Degree_Num = new string[1];//量測輪廓高度欄位

        //存AOI圖片索引
        public int AOI_save_idx_1 = 0;
        public int AOI_save_idx_2 = 0;
        public int AOI_save_idx_3 = 0;
        public int manual_save_idx_1 = 0;
        public int manual_save_idx_2 = 0;
        public int manual_save_idx_3 = 0;
        //手動量測button座標,長寬
        public int hand_measurement_X = 0;
        public int hand_measurement_Y = 0;
        public int hand_measurement_H = 0;
        public int hand_measurement_W = 0;
        public Variable_Data(string Xml_File_Address)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Xml_File_Address);//載入xml檔
                                            //
                XmlNode Fist_Node = xmlDoc.SelectSingleNode("SPIL_Program_Setup");
                //顯示根目錄(A層)下第一層(B層)的所有屬性值
                XmlNodeList Second_Node = Fist_Node.ChildNodes;
                //
                foreach (XmlNode Second_Node_Each in Second_Node)

                {
                    XmlElement Second_Node_XmlElement = (XmlElement)Second_Node_Each;
                    String Second_Node_Data = Second_Node_XmlElement.GetAttribute("Setup_Part");
                    //顯示根目錄(B層)下第一層(C層)的所有屬性值
                    XmlNodeList Third_Node = Second_Node_XmlElement.ChildNodes;
                    foreach (XmlNode Third_Node_Each in Third_Node)

                    {
                        XmlElement Third_Node_Each_XmlElement = (XmlElement)Third_Node_Each;
                        String Third_Node_Data_1 = Third_Node_Each_XmlElement.GetAttribute("Setup");
                        if (Second_Node_Data == "Motion Server")
                        {
                            if (Third_Node_Data_1 == "IP_Address_1")
                                IP_Address[0] = Convert.ToByte(Convert.ToString(Third_Node_Each_XmlElement.InnerText));
                            else if (Third_Node_Data_1 == "IP_Address_2")
                                IP_Address[1] = Convert.ToByte(Convert.ToString(Third_Node_Each_XmlElement.InnerText));
                            else if (Third_Node_Data_1 == "IP_Address_3")
                                IP_Address[2] = Convert.ToByte(Convert.ToString(Third_Node_Each_XmlElement.InnerText));
                            else if (Third_Node_Data_1 == "IP_Address_4")
                                IP_Address[3] = Convert.ToByte(Convert.ToString(Third_Node_Each_XmlElement.InnerText));
                            else if (Third_Node_Data_1 == "IP_Port")
                                IP_Port = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Auto_Start")
                                Auto_Start = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                        }
                        else if (Second_Node_Data == "AOI")
                        {
                            if (Third_Node_Data_1 == "Vision_Pro_File")
                                Vision_Pro_File = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                        }
                        else if (Second_Node_Data == "OLS")
                        {
                            if (Third_Node_Data_1 == "OLS_Name")
                                OLS_Name = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Windows_Name")
                                Windows_Name = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "OLS_Folder")
                                OLS_Folder = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_Start_X1")
                                Cover_Start_X1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_Start_Y1")
                                Cover_Start_Y1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_End_X1")
                                Cover_End_X1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_End_Y1")
                                Cover_End_Y1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_Start_X2")
                                Cover_Start_X2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_Start_Y2")
                                Cover_Start_Y2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_End_X2")
                                Cover_End_X2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Cover_End_Y2")
                                Cover_End_Y2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_1")
                                Initial_Step_1 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_1_X")
                                Initial_Step_1_X = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_1_Y")
                                Initial_Step_1_Y = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_2")
                                Initial_Step_2 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_2_X")
                                Initial_Step_2_X = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_2_Y")
                                Initial_Step_2_Y = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_3")
                                Initial_Step_3 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_3_X")
                                Initial_Step_3_X = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_3_Y")
                                Initial_Step_3_Y = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_4")
                                Initial_Step_4 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_4_Delay_Time")
                                Initial_Step_4_Delay_Time = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_5")
                                Initial_Step_5 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_5_X")
                                Initial_Step_5_X = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_5_Y")
                                Initial_Step_5_Y = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_6")
                                Initial_Step_6 = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Initial_Step_6_Delay_Time")
                                Initial_Step_6_Delay_Time = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Need_Move_bmp")
                                Need_Move_bmp = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Need_Move_poir")
                                Need_Move_poir = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Need_Move_xlsx")
                                Need_Move_xlsx = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Need_Move_csv")
                                Need_Move_csv = Convert.ToBoolean(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "0_Degree_height_A")
                                Degree_height_A[0] = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "0_degree_height_Num")
                                Degree_Num[0] = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "AOI_save_idx_1")
                                AOI_save_idx_1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "AOI_save_idx_2")
                                AOI_save_idx_2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "AOI_save_idx_3")
                                AOI_save_idx_3 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "manual_save_idx_1")
                                manual_save_idx_1 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "manual_save_idx_2")
                                manual_save_idx_2 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "manual_save_idx_3")
                                manual_save_idx_3 = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "hand_measurement_X")
                                hand_measurement_X = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "hand_measurement_Y")
                                hand_measurement_Y = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "hand_measurement_H")
                                hand_measurement_H = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "hand_measurement_W")
                                hand_measurement_W = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                        }
                        else if (Second_Node_Data == "Delete Data")
                        {
                            if (Third_Node_Data_1 == "Delete_Time_Setting")
                            {
                                delete_data_setting = Convert.ToInt32(Third_Node_Each_XmlElement.InnerText);
                            }
                        }
                        else if (Second_Node_Data == "Excel File")
                        {
                            if (Third_Node_Data_1 == "Save_File_Path_1")
                                Save_File_Path_1 = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Save_File_Path_2")
                                Save_File_Path_2 = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Save_File_Path_3")
                                Save_File_Path_3 = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Save_File_Name")
                                Save_File_Name = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Sample_Excel_File")
                                Sample_Excel_File = Convert.ToString(Third_Node_Each_XmlElement.InnerText);
                            else if (Third_Node_Data_1 == "Degree_Ratio")
                                Degree_Ratio = Convert.ToDouble(Third_Node_Each_XmlElement.InnerText);
                        }
                    }
                }
            }
            catch(Exception Error)
            {
                MessageBox.Show(Convert.ToString(Error));
            }
        }
    }
}
