using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using SPIL.model;
using System;
using System.Collections.Generic;
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
        public SharpnessFlow(string aoiVppPath, AlgorithmDescribe[] algorithmDescribes, Logger logger) : base(aoiVppPath, algorithmDescribes, 201)
        {


        }

        public void Measurment(string Input_Image_Address1)
        {
        }
    }
}
