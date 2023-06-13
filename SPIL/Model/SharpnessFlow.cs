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

        public event Action<string> WriteLog;

        public string ResultMessage { get; private set; }

        public SharpnessFlow(string aoiVppPath, AlgorithmDescribe[] algorithmDescribes) : base(aoiVppPath, algorithmDescribes, 201)
        {


        }


        public async Task SharpnessAsync(IEnumerable<Bitmap> bitmaps)
        {
            List<SharpnessResult> results = new List<SharpnessResult>();


            foreach (var bmp in bitmaps)
            {
                SharpnessResult sharpnessResult = await Measurment(bmp);

                results.Add(sharpnessResult);

            }

           
            
           var searchScore1s = results.Select((r ,i) => (r.SearchScore1,i)).OrderBy(o=>o.SearchScore1);
            var searchScore2s = results.Select((r, i) => (r.SearchScore2, i)).OrderBy(o => o.SearchScore2);
            List<int> max10 = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                searchScore1s.Max();
            }






        }

        public async Task PeakSharpnessAsync(IEnumerable<Bitmap> bitmaps)
        {
            List<SharpnessResult> results = new List<SharpnessResult>();


            foreach (var bmp in bitmaps)
            {
                SharpnessResult sharpnessResult = await Measurment(bmp);

                results.Add(sharpnessResult);

            }



        }



        public async Task<SharpnessResult> Measurment(Bitmap img1)
        {
            try
            {
                WriteLog?.Invoke($"Sharpness start ");

                measureToolBlock.Inputs["Input"].Value = new CogImage24PlanarColor(img1);

                measureToolBlock.Run();


                var r1 = (double)measureToolBlock.Outputs["Results_Item_0_Score"].Value;
                var r2 = (double)measureToolBlock.Outputs["Results_Item_0_Score1"].Value;

                var s0 = (double)measureToolBlock.Outputs["Score"].Value;
                var s1 = (double)measureToolBlock.Outputs["Score1"].Value;
                var s2 = (double)measureToolBlock.Outputs["Score2"].Value;
                CogToolResultConstants vision_pro_run_result = measureToolBlock.RunStatus.Result;

                ResultMessage = $"Run Result : {vision_pro_run_result}   : { measureToolBlock.RunStatus.Message} ";
                WriteLog?.Invoke(ResultMessage);

                ICogRecord cogRecord = measureToolBlock.CreateLastRunRecord().SubRecords[0];


                if (vision_pro_run_result != CogToolResultConstants.Accept) return null;

                SharpnessResult result = new SharpnessResult(r1, r2, s0, s1, s2, cogRecord);
                return result;

            }
            catch (Exception error)
            {

                throw error;

            }
        }
    }


    public class SharpnessResult
    {
        public SharpnessResult(double searchScore1, double searchScore2, double score1, double score2, double score3, ICogRecord cogRecord)
        {
            SearchScore1 = searchScore1;
            SearchScore2 = searchScore2;
            Score1 = score1;
            Score2 = score2;
            Score3 = score3;
            CogRecord = cogRecord;
        }

        public double SearchScore1 { get; }
        public double SearchScore2 { get; }
        public double Score1 { get; }
        public double Score2 { get; }
        public double Score3 { get; }
        public ICogRecord CogRecord { get; }

    }
}
