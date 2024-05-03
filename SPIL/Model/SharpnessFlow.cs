using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.SearchMax;
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

        private CogToolBlock toolBlock2;
        private CogToolBlock toolBlock3;
        private CogToolBlock toolBlock4;
        private CogToolBlock toolBlock5;

        public event Action<string> WriteLog;
        public event Action<SharpnessResult, ICogRecord> WriteCogResult;


        // public event Action<ICogRecord >  CogResult;
        public string ResultMessage { get; private set; }

        public SharpnessFlow(string aoiVppPath, AlgorithmDescribe[] algorithmDescribes) : base(aoiVppPath, algorithmDescribes, 201)
        {


        }
        public void DuplicateTool()
        {
            if (toolBlock2 != null)
            {
                toolBlock2.Dispose();
                toolBlock3.Dispose();
                toolBlock4.Dispose();
                toolBlock5.Dispose();
            }
            toolBlock2 = CogSerializer.DeepCopyObject(measureToolBlock) as CogToolBlock;
            toolBlock3 = CogSerializer.DeepCopyObject(measureToolBlock) as CogToolBlock;
            toolBlock4 = CogSerializer.DeepCopyObject(measureToolBlock) as CogToolBlock;
            toolBlock5 = CogSerializer.DeepCopyObject(measureToolBlock) as CogToolBlock;

            var tbtool2 = toolBlock2.Tools[0] as CogSearchMaxTool;
            var tbtool5 = toolBlock2.Tools[5] as CogImageSharpnessTool;

            WriteLog?.Invoke($"toolBlock2 SearchMaxTool: { tbtool2.RunParams.AcceptThreshold }");
            WriteLog?.Invoke($"toolBlock2 SharpnessTool:{ tbtool5.RunParams.Mode }  { tbtool5.RunParams.GradientEnergyLowPassSmoothing }");
        }

        public (int Image1Index, int Image2Index, int Image3Index) SharpnessAnalyzeAsync(IEnumerable<Bitmap> bitmaps,int sharpImageIncludeNumber , bool isMulit )
        {

            //   MethodAssignTool();
            List<SharpnessResult> results = new List<SharpnessResult>();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            var sec = stopwatch.ElapsedMilliseconds;


            if (isMulit)
            {
                //計算清晰度(多執行續)
                var bmps = bitmaps.ToArray();
                results = Mulitmeasure(bmps);
            }
            else
            {
                //計算清晰度
                foreach (var bmp in bitmaps)
                {
                    var sharpnessResult = Measurement(bmp);

                    results.Add(sharpnessResult.result);
                    WriteCogResult?.Invoke(sharpnessResult.result, sharpnessResult.cogRecord);

                }
            }


            //0821 新增  改先刪除NULL的資料
            //var deleteNull = results.Where(r => r != null).ToArray();

            //排序後找出最大20張圖
            var searchScore1s = results.Select((result, index) => (result, index)).Where(r => r.result != null).OrderByDescending(o => o.result.SearchScore1).Take(sharpImageIncludeNumber).ToList();
            if (searchScore1s.Count < 20) throw new Exception("Sharpness Analyze Error");
            //  var image1 = searchScore1s.OrderByDescending(o => o.result.Score1).First();

            var image2 = searchScore1s.OrderByDescending(o => o.result.Score2).First();

            //Score2  最高那張號碼之前的所有圖片，找Score1最高分
            //    var searchImage2 = results.Take(image2.index+1);

            var image2Index = searchScore1s.IndexOf(image2);

            var searchImage2 = searchScore1s.Take(image2Index + 1);

            if (searchImage2.Where(r => r.result != null).Count() == 0) throw new Exception("Image2 Sharpness Analyze Error");
            var image1 = searchImage2.Select((result) => (result)).Where(r => r.result != null).OrderByDescending(o => o.result.Score1).First();

            // var image1 = searchImage2.Select((result, index) => (result, index)).Where(r => r.result != null).OrderByDescending(o => o.result.Score1).First();

            //       var searchScore2s = results.Select((r, i) => (r.SearchScore2, i)).OrderBy(o => o.SearchScore2).Take(10);
            //排序後找出最大20張圖
            var searchScore12s = results.Select((result, index) => (result, index)).Where(r => r.result != null).OrderByDescending(o => o.result.SearchScore2).Take(sharpImageIncludeNumber).ToList();
            var image3 = searchScore12s.OrderByDescending(o => o.result.Score3).First();


            //      var image2 = searchScore2s.Max(s => s.SearchScore2);
            //     var image1 = searchScore1s.Max(s => s.SearchScore1);

            return (image1.index, image2.index, image3.index);

        }



        /// <summary>
        /// 量測
        /// </summary>
        /// <param name="img1"></param>
        /// <returns></returns>
        public (SharpnessResult result, ICogRecord cogRecord) Measurement(Bitmap img1)
        {
            try
            {

                var cogImg1 = new CogImage24PlanarColor(img1);
      //          WriteLog?.Invoke($"Sharpness start ");

                measureToolBlock.Inputs["Input"].Value = cogImg1;

                measureToolBlock.Run();
                //         cogImg1.Dispose();

                var r1 = (double)measureToolBlock.Outputs["Results_Item_0_Score"].Value;
                var r2 = (double)measureToolBlock.Outputs["Results_Item_0_Score1"].Value;

                var s0 = (double)measureToolBlock.Outputs["Score"].Value;
                var s1 = (double)measureToolBlock.Outputs["Score1"].Value;
                var s2 = (double)measureToolBlock.Outputs["Score2"].Value;
                CogToolResultConstants vision_pro_run_result = measureToolBlock.RunStatus.Result;

                ResultMessage = $"Run Result : {vision_pro_run_result}   : { measureToolBlock.RunStatus.Message} ";
                WriteLog?.Invoke(ResultMessage);
                ICogRecord cogRecord = measureToolBlock.CreateLastRunRecord().SubRecords[0];
                if (vision_pro_run_result != CogToolResultConstants.Accept) return (null, null);
                SharpnessResult result = new SharpnessResult(r1, r2, s0, s1, s2);

                return (result, cogRecord);

            }
            catch (Exception error)
            {

                throw error;

            }
        }
        public (SharpnessResult result, ICogRecord cogRecord) Measurement(CogToolBlock measureToolBlock, Bitmap img1)
        {
            try
            {

                var cogImg1 = new CogImage24PlanarColor(img1);
       //         WriteLog?.Invoke($"Sharpness start ");

                measureToolBlock.Inputs["Input"].Value = cogImg1;
                measureToolBlock.Run();
                //         cogImg1.Dispose();

                var r1 = (double)measureToolBlock.Outputs["Results_Item_0_Score"].Value;
                var r2 = (double)measureToolBlock.Outputs["Results_Item_0_Score1"].Value;

                var s0 = (double)measureToolBlock.Outputs["Score"].Value;
                var s1 = (double)measureToolBlock.Outputs["Score1"].Value;
                var s2 = (double)measureToolBlock.Outputs["Score2"].Value;
                CogToolResultConstants vision_pro_run_result = measureToolBlock.RunStatus.Result;

                ResultMessage = $"Run Result : {vision_pro_run_result}   : { measureToolBlock.RunStatus.Message} ";
    //            WriteLog?.Invoke(ResultMessage);
                ICogRecord cogRecord = measureToolBlock.CreateLastRunRecord().SubRecords[0];
                if (vision_pro_run_result != CogToolResultConstants.Accept) return (null, null);
                SharpnessResult result = new SharpnessResult(r1, r2, s0, s1, s2);

                return (result, cogRecord);

            }
            catch (Exception error)
            {

                throw error;

            }
        }


        private List<SharpnessResult> Mulitmeasure(Bitmap[] bmps)
        {
            List<SharpnessResult> results = new List<SharpnessResult>();
            Task<(SharpnessResult result, ICogRecord cogRecord)> ms1 = null;
            Task<(SharpnessResult result, ICogRecord cogRecord)> ms2 = null;
            Task<(SharpnessResult result, ICogRecord cogRecord)> ms3 = null;
            Task<(SharpnessResult result, ICogRecord cogRecord)> ms4 = null;
            Task<(SharpnessResult result, ICogRecord cogRecord)> ms5 = null;


            for (int i = 0; i < bmps.Length; i += 5)
            {

                ms1 = Task.Run(() =>
                {
                    return Measurement(measureToolBlock, bmps[i]);
                });

                if (i + 1 < bmps.Length)
                {
                    ms2 = Task.Run(() =>
                    {
                        return Measurement(toolBlock2, bmps[i + 1]);
                    });
                }

                if (i + 2 < bmps.Length)
                {
                    ms3 = Task.Run(() =>
                    {
                        return Measurement(toolBlock3, bmps[i + 2]);
                    });
                }
                if (i + 3 < bmps.Length)
                {
                    ms4 = Task.Run(() =>
                    {
                        return Measurement(toolBlock4, bmps[i + 3]);
                    });
                }
                if (i + 4 < bmps.Length)
                {
                    ms5 = Task.Run(() =>
                    {
                        return Measurement(toolBlock5, bmps[i + 4]);
                    });
                }


                var r1 = ms1.Result;
                results.Add(r1.result);

                if (i + 1 < bmps.Length)
                {
                    var r2 = ms2.Result;
                    results.Add(r2.result);
                }


                if (i + 2 < bmps.Length)
                {
                    var r3 = ms3.Result;
                    results.Add(r3.result);
                }
                if (i + 3 < bmps.Length)
                {
                    var r4 = ms4.Result;
                    results.Add(r4.result);
                }


                if (i + 4 < bmps.Length)
                {
                    var r5 = ms5.Result;
                    results.Add(r5.result);
                }

       
                WriteCogResult?.Invoke(r1.result, r1.cogRecord);
            }

            return results;
        }

        /// <summary>
        /// 帶Cog 結果的量測
        /// </summary>
        /// <param name="img1"></param>
        /// <returns></returns>
        //public SharpnessResult MeasurmentRecord(Bitmap img1)
        //{
        //    SharpnessResult result = Measurment(img1);

        //    ICogRecord cogRecord = measureToolBlock.CreateLastRunRecord().SubRecords[0];

        //    if (result == null) return null;
        //    SharpnessResult sharpnessResult = new SharpnessResult(result.SearchScore1, result.SearchScore2,
        //        result.Score1, result.Score2, result.Score3);
        // //   CogResult.Invoke(cogRecord);

        //    return sharpnessResult;
        //}
    }


    public class SharpnessResult
    {
        public SharpnessResult(double searchScore1, double searchScore2, double score1, double score2, double score3)
        {
            SearchScore1 = searchScore1;
            SearchScore2 = searchScore2;
            Score1 = score1;
            Score2 = score2;
            Score3 = score3;
            //   CogRecord = cogRecord;
        }

        public double SearchScore1 { get; }
        public double SearchScore2 { get; }
        public double Score1 { get; }
        public double Score2 { get; }
        public double Score3 { get; }
        //  public ICogRecord CogRecord { get; }

    }
}
