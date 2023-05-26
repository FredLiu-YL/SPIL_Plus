using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using SPIL.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.ImageProcess;
using YuanliCore.ImageProcess.Caliper;
using YuanliCore.ImageProcess.Match;

namespace SPIL.Model
{
    public class SharpnessFlow : VisionProFlow
    {
        private Logger logger;
        public SharpnessFlow(string aoiVppPath, AlgorithmDescribe[] algorithmDescribes, Logger logger) : base(aoiVppPath, algorithmDescribes, 201)
        {

            this.logger = logger;
        }

        public void Measurment(Bitmap img1)
        {
            try {
                logger.WriteLog("Measurement for two images!");

                measureToolBlock.Inputs["Input"].Value = new CogImage24PlanarColor(img1);
             
                measureToolBlock.Run();
              var  distance_CuNi = (double)measureToolBlock.Outputs["Results_Item_0_Score"].Value;
           var     distance_Cu = (double)measureToolBlock.Outputs["Results_Item_0_Score1"].Value;
                var s0 = (double)measureToolBlock.Outputs["Score"].Value;
                var s1 = (double)measureToolBlock.Outputs["Score1"].Value;
                var s2 = (double)measureToolBlock.Outputs["Score2"].Value;
                CogToolResultConstants vision_pro_run_result = measureToolBlock.RunStatus.Result;
                logger.WriteLog("Run Result : " + Convert.ToString(vision_pro_run_result));


                /*
                if (vision_pro_run_result != CogToolResultConstants.Accept) {
                    logger.WriteErrorLog("Run Result : " + Convert.ToString(measureToolBlock.RunStatus.Message));
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
                else {
                    Save_Toolblock_result_img(Input_Image_Address1, Input_Image_Address2, Input_Image_Address3, is_maunal);
                    logger.WriteLog("Measurement Cu+Ni : " + Convert.ToString(distance_CuNi) + " Cu : " + Convert.ToString(distance_Cu));
                }*/
          
            
                if (vision_pro_run_result != CogToolResultConstants.Accept) throw new Exception($" { measureToolBlock.RunStatus.Message}");


            }
            catch (Exception error) 
            {
                logger.WriteErrorLog("Measurement Error! " + error.ToString());
             

            }
        }
    }
}
