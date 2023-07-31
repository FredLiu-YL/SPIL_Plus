using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System.IO;
using Cognex.VisionPro.Display;
using SPIL.model;

namespace SPIL
{
    public class SPILBumpMeasure
    {
        Logger logger = new Logger("AOI");


    


        public SPILBumpMeasure(string Vision_Pro_Tool_Block_Address)
        {
            logger.WriteLog("Load AOI File");
            try
            {
                MeasureToolBlock = CogSerializer.LoadObjectFromFile(Vision_Pro_Tool_Block_Address) as CogToolBlock;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLog(ex.ToString());
            }
        }
        public SPILBumpMeasure(CogToolBlock cogToolBlock)
        {
            logger.WriteLog("Load AOI CogToolBlock");
            try
            {
                MeasureToolBlock = cogToolBlock;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLog(ex.ToString());
            }
        }

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
        public CogToolBlock MeasureToolBlock { get; set; }
        public event Action<ICogRecord, ICogRecord, ICogRecord> ShowRecord;


        //public double Measurment(Bitmap Input_Image)
        //{
        //    try
        //    {
        //        logger.WriteLog("Measurement Image! bitmap");
        //        CogImage8Grey cogImage8Grey = new CogImage8Grey(Input_Image);
        //        MeasureToolBlock.Inputs["InputImage"].Value = cogImage8Grey;
        //        MeasureToolBlock.Run();
        //        double distance = (double)MeasureToolBlock.Outputs["Distance"].Value;
        //        CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
        //        if (vision_pro_run_result != CogToolResultConstants.Accept)
        //            distance = 0;
        //        logger.WriteLog("Run Result : " + Convert.ToString(vision_pro_run_result));
        //        logger.WriteLog("Measurement Distance : " + Convert.ToString(distance));
        //        return distance;
        //    }
        //    catch (Exception error)
        //    {
        //        logger.WriteErrorLog("Measurement Error! " + error.ToString());
        //        return -1;
        //    }
        //}
        //public double Measurment(string Input_Image_Address)
        //{
        //    try
        //    {
        //        logger.WriteLog("Measurement Image Address! one");
        //        Bitmap Input_Image = new Bitmap(Input_Image_Address);
        //        CogImage8Grey cogImage8Grey = new CogImage8Grey(Input_Image);
        //        MeasureToolBlock.Inputs["Image"].Value = cogImage8Grey;
        //        MeasureToolBlock.Run();
        //        double distance = (double)MeasureToolBlock.Outputs["Distance"].Value;
        //        CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
        //        logger.WriteLog("Run Result : " + Convert.ToString(vision_pro_run_result));
        //        if (vision_pro_run_result != CogToolResultConstants.Accept)
        //        {
        //            logger.WriteErrorLog("Run Result : " + Convert.ToString(MeasureToolBlock.RunStatus.Message));
        //        }
        //        logger.WriteLog("Measurement Distance : " + Convert.ToString(distance));
        //        return distance;
        //    }
        //    catch (Exception error)
        //    {
        //        logger.WriteErrorLog("Measurement Error! " + error.ToString());
        //        return -1;
        //    }
        //}

        public bool Measurment(string Input_Image_Address1, string Input_Image_Address2, string Input_Image_Address3, bool is_maunal, out double distance_CuNi, out double distance_Cu)
        {
            try
            {
                logger.WriteLog("Measurement for two images!");
                Bitmap img1 = new Bitmap(Input_Image_Address1);
                Bitmap img2 = new Bitmap(Input_Image_Address2);
                Bitmap img3 = new Bitmap(Input_Image_Address3);

                var cogImg1 = new CogImage24PlanarColor(img1);
                var cogImg2 = new CogImage24PlanarColor(img2);
                var cogImg3 = new CogImage24PlanarColor(img3);
                if (MeasureToolBlock == null) throw new Exception("Recipe not read");
                MeasureToolBlock.GarbageCollectionEnabled = true;
                if (is_maunal)
                {
                    MeasureToolBlock.Inputs["Input"].Value = cogImg1;
                    MeasureToolBlock.Inputs["Input1"].Value = cogImg2;
                    MeasureToolBlock.Inputs["Input2"].Value = cogImg3;
                }
                else
                {

                    MeasureToolBlock.Inputs["Input"].Value = cogImg1;
                    MeasureToolBlock.Inputs["Input1"].Value = cogImg2;
                    MeasureToolBlock.Inputs["Input2"].Value = cogImg3;

                }

                MeasureToolBlock.Run();

                distance_CuNi = (double)MeasureToolBlock.Outputs["Distance"].Value;
                distance_Cu = (double)MeasureToolBlock.Outputs["Distance1"].Value;


                CogToolResultConstants vision_pro_run_result = MeasureToolBlock.RunStatus.Result;
                logger.WriteLog("Run Result : " + Convert.ToString(vision_pro_run_result));



                if (vision_pro_run_result != CogToolResultConstants.Accept)
                {
                    logger.WriteErrorLog("Run Result : " + Convert.ToString(MeasureToolBlock.RunStatus.Message));
                    CogImage24PlanarColor error_img = new CogImage24PlanarColor(new Bitmap("X.png"));
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
                    //存圖有問題  如果有需要就另外寫
                   // Save_Toolblock_result_img(Input_Image_Address1, Input_Image_Address2, Input_Image_Address3, is_maunal);
                  
                    var cord = MeasureToolBlock.CreateLastRunRecord();
                    cogRecord_save_result_img.Record = cord.SubRecords["CogFixtureTool1.OutputImage"];
                    var result_img1 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
                    cogRecord_save_result_img.Record = cord.SubRecords["CogFixtureTool2.OutputImage"];
                    var result_img2 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
                    cogRecord_save_result_img.Record = cord.SubRecords["CogFixtureTool3.OutputImage"];
                    var result_img3 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
                   
                    CogDisplay_result_1.Image = new CogImage24PlanarColor(result_img1);
                    CogDisplay_result_1.Fit(true);
                    CogDisplay_result_2.Image = new CogImage24PlanarColor(result_img2);
                    CogDisplay_result_2.Fit(true);
                    CogDisplay_result_3.Image = new CogImage24PlanarColor(result_img3);
                    CogDisplay_result_3.Fit(true);
                    ShowRecord?.Invoke(cord.SubRecords["CogFixtureTool1.OutputImage"], cord.SubRecords["CogFixtureTool2.OutputImage"], cord.SubRecords["CogFixtureTool3.OutputImage"]);
                    logger.WriteLog("Measurement Cu+Ni : " + Convert.ToString(distance_CuNi) + " Cu : " + Convert.ToString(distance_Cu));

                    result_img1.Dispose();
                    result_img2.Dispose();
                    result_img3.Dispose();
                }
                cogImg1.Dispose();
                cogImg2.Dispose();
                cogImg3.Dispose();

                img1.Dispose();
                img2.Dispose();
                img3.Dispose();

                if (vision_pro_run_result != CogToolResultConstants.Accept)
                    return false;
                return true;
            }
            catch (Exception error)
            {
                logger.WriteErrorLog("Measurement Error! " + error.ToString());
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
            logger.WriteLog("save AOI idx:" + idx1);
            string save_result_name = Path.ChangeExtension(Input_Image_Address1, null);
            Bitmap save_result_img1 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            save_result_img1.Save(save_result_name + "_AOI.bmp");

            cogRecord_save_result_img.Record = MeasureToolBlock.CreateLastRunRecord().SubRecords[idx2];
            logger.WriteLog("save AOI idx:" + idx2);
            save_result_name = Path.ChangeExtension(Input_Image_Address2, null);
            Bitmap save_result_img2 = (Bitmap)cogRecord_save_result_img.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            save_result_img2.Save(save_result_name + "_AOI.bmp");

            cogRecord_save_result_img.Record = MeasureToolBlock.CreateLastRunRecord().SubRecords[idx3];
            logger.WriteLog("save AOI idx:" + idx3);
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
