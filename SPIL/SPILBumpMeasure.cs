using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using YuanLi_Logger;
using System.IO;
using Cognex.VisionPro.Display;

namespace SPIL
{
    class SPILBumpMeasure
    {
        CogToolBlock MeasureToolBlock;
        Logger logger = new Logger("AOI");
        public CogRecordDisplay cogRecord_save_result_img;
        //toolblock存圖索引
        public int save_AOI_result_idx_1 = 0;
        public int save_AOI_result_idx_2 = 0;
        public int save_AOI_result_idx_3 = 0;
        public int manual_save_AOI_result_idx_1 = 0;
        public int manual_save_AOI_result_idx_2 = 0;
        public int manual_save_AOI_result_idx_3 = 0;
        //顯示圖片用的cogdisplay元件
        public CogDisplay CogDisplay_result_1;
        public CogDisplay CogDisplay_result_2;
        public CogDisplay CogDisplay_result_3;
        public SPILBumpMeasure(string Vision_Pro_Tool_Block_Address)
        {
            logger.Write_Logger("Load AOI File");
            try
            {
                MeasureToolBlock = CogSerializer.LoadObjectFromFile(Vision_Pro_Tool_Block_Address) as CogToolBlock;
            }
            catch(Exception ex)
            {
                logger.Write_Error_Logger(ex.ToString());
            }
        }

        //public double Measurment(Bitmap Input_Image)
        //{
        //    try
        //    {
        //        logger.Write_Logger("Measurement Image! bitmap");
        //        CogImage8Grey cogImage8Grey = new CogImage8Grey(Input_Image);
        //        MeasureToolBlock.Inputs["InputImage"].Value = cogImage8Grey;
        //        MeasureToolBlock.Run();
        //        double distance = (double)MeasureToolBlock.Outputs["Distance"].Value;
        //        CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
        //        if (vision_pro_run_result != CogToolResultConstants.Accept)
        //            distance = 0;
        //        logger.Write_Logger("Run Result : " + Convert.ToString(vision_pro_run_result));
        //        logger.Write_Logger("Measurement Distance : " + Convert.ToString(distance));
        //        return distance;
        //    }
        //    catch (Exception error)
        //    {
        //        logger.Write_Error_Logger("Measurement Error! " + error.ToString());
        //        return -1;
        //    }
        //}
        //public double Measurment(string Input_Image_Address)
        //{
        //    try
        //    {
        //        logger.Write_Logger("Measurement Image Address! one");
        //        Bitmap Input_Image = new Bitmap(Input_Image_Address);
        //        CogImage8Grey cogImage8Grey = new CogImage8Grey(Input_Image);
        //        MeasureToolBlock.Inputs["Image"].Value = cogImage8Grey;
        //        MeasureToolBlock.Run();
        //        double distance = (double)MeasureToolBlock.Outputs["Distance"].Value;
        //        CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
        //        logger.Write_Logger("Run Result : " + Convert.ToString(vision_pro_run_result));
        //        if (vision_pro_run_result != CogToolResultConstants.Accept)
        //        {
        //            logger.Write_Error_Logger("Run Result : " + Convert.ToString(MeasureToolBlock.RunStatus.Message));
        //        }
        //        logger.Write_Logger("Measurement Distance : " + Convert.ToString(distance));
        //        return distance;
        //    }
        //    catch (Exception error)
        //    {
        //        logger.Write_Error_Logger("Measurement Error! " + error.ToString());
        //        return -1;
        //    }
        //}

        public bool Measurment(string Input_Image_Address1, string Input_Image_Address2, string Input_Image_Address3, bool is_maunal, out double distance_CuNi, out double distance_Cu)
        {
            try
            {
                logger.Write_Logger("Measurement for two images!");
                Bitmap img1 = new Bitmap(Input_Image_Address1);
                Bitmap img2 = new Bitmap(Input_Image_Address2);
                Bitmap img3 = new Bitmap(Input_Image_Address3);
                if (is_maunal)
                {
                    MeasureToolBlock.Inputs["Image"].Value = new CogImage24PlanarColor(img1);
                    MeasureToolBlock.Inputs["Input"].Value = new CogImage24PlanarColor(img2);
                    MeasureToolBlock.Inputs["Input1"].Value = new CogImage24PlanarColor(img3);
                }
                else
                {
                    MeasureToolBlock.Inputs["Image"].Value = new CogImage24PlanarColor(img1);
                    MeasureToolBlock.Inputs["Input"].Value = new CogImage24PlanarColor(img2);
                    MeasureToolBlock.Inputs["Input1"].Value = new CogImage24PlanarColor(img3);
                }
                MeasureToolBlock.Run();
                distance_CuNi = (double)MeasureToolBlock.Outputs["Distance"].Value;
                distance_Cu = (double)MeasureToolBlock.Outputs["Distance1"].Value;
                CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
                logger.Write_Logger("Run Result : " + Convert.ToString(vision_pro_run_result));

                

                if (vision_pro_run_result != CogToolResultConstants.Accept)
                {
                    logger.Write_Error_Logger("Run Result : " + Convert.ToString(MeasureToolBlock.RunStatus.Message));
                    CogImage24PlanarColor error_img =new CogImage24PlanarColor(new Bitmap("X.png"));
                    CogDisplay_result_1.Image = new CogImage24PlanarColor(error_img);
                    CogDisplay_result_1.Fit(true);
                    CogDisplay_result_2.Image = new CogImage24PlanarColor(error_img);
                    CogDisplay_result_2.Fit(true);
                    CogDisplay_result_3.Image = new CogImage24PlanarColor(error_img);
                    CogDisplay_result_3.Fit(true);
                    distance_CuNi = -1;
                    distance_Cu = -1;
                }
                else
                {
                    Save_Toolblock_result_img(Input_Image_Address1, Input_Image_Address2, Input_Image_Address3, is_maunal);
                    logger.Write_Logger("Measurement Cu+Ni : " + Convert.ToString(distance_CuNi) + " Cu : " + Convert.ToString(distance_Cu));
                }
                img1.Dispose();
                img2.Dispose();
                img3.Dispose();
                if (vision_pro_run_result != CogToolResultConstants.Accept)
                    return false;
                return true;
            }
            catch (Exception error)
            {
                logger.Write_Error_Logger("Measurement Error! " + error.ToString());
                distance_CuNi = -1;
                distance_Cu = -1;
                return false;
            }
        }
        
        void Save_Toolblock_result_img(string Input_Image_Address1, string Input_Image_Address2, string Input_Image_Address3, bool is_maunal)
        {
            int idx1 = 0, idx2 = 0, idx3 = 0;
            if (is_maunal)
            {
                idx1 = manual_save_AOI_result_idx_1;
                idx2 = manual_save_AOI_result_idx_2;
                idx3 = manual_save_AOI_result_idx_3;
            }
            else
            {
                idx1 = save_AOI_result_idx_1;
                idx2 = save_AOI_result_idx_2;
                idx3 = save_AOI_result_idx_3;
            }
            // 存圖
            cogRecord_save_result_img.Record = MeasureToolBlock.CreateLastRunRecord().SubRecords[idx1];
            logger.Write_Logger("save AOI idx:" + idx1);
            string save_result_name = Path.ChangeExtension(Input_Image_Address1, null);
            Bitmap save_result_img1 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            save_result_img1.Save(save_result_name + "_AOI.bmp");

            cogRecord_save_result_img.Record = MeasureToolBlock.CreateLastRunRecord().SubRecords[idx2];
            logger.Write_Logger("save AOI idx:" + idx2);
            save_result_name = Path.ChangeExtension(Input_Image_Address2, null);
            Bitmap save_result_img2 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            save_result_img2.Save(save_result_name + "_AOI.bmp");

            cogRecord_save_result_img.Record = MeasureToolBlock.CreateLastRunRecord().SubRecords[idx3];
            logger.Write_Logger("save AOI idx:" + idx3);
            save_result_name = Path.ChangeExtension(Input_Image_Address3, null);
            Bitmap save_result_img3 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            save_result_img3.Save(save_result_name + "_AOI.bmp");

            CogDisplay_result_1.Image = new CogImage24PlanarColor(save_result_img1);
            CogDisplay_result_1.Fit(true);
            CogDisplay_result_2.Image = new CogImage24PlanarColor(save_result_img2);
            CogDisplay_result_2.Fit(true);
            CogDisplay_result_3.Image = new CogImage24PlanarColor(save_result_img3);
            CogDisplay_result_3.Fit(true);
        }
    }
}
