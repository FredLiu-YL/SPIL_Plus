using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.SearchMax;
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

    public class VisionProFlow
    {
        protected CogToolBlock measureToolBlock;
        protected AlgorithmDescribe[] algorithmDescribes;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aoiVppPath"> toolblock 的vpp </param>
        /// <param name="algorithmDescribes"> config內紀錄 可被編輯的tool</param>
        /// <param name="paramBeginID">存到硬碟用於區分檔案  避免讀取到同一個檔案</param>
        public VisionProFlow(string vppPath, AlgorithmDescribe[] algorithmDescribes, int paramBeginID)
        {
            this.algorithmDescribes = algorithmDescribes;
            //讀取vpp檔 獲得寫在vpp的完整流程
            measureToolBlock = CogSerializer.LoadObjectFromFile(vppPath) as CogToolBlock;
            measureToolBlock.GarbageCollectionEnabled = true;
            CogMethods = CreateMethod(algorithmDescribes);
            ToolBlockToMethodParam(paramBeginID);
        }

        /// <summary>
        /// cognex的演算法   
        /// </summary>   
        public (CogMethod method, int id, string name)[] CogMethods { get; }

        /// <summary>
        /// 獲得 tool 參數
        /// </summary>
        private void ToolBlockToMethodParam(int id)
        {

            try
            {
                foreach (var item in CogMethods)
                {

                    var tool = measureToolBlock.Tools[item.name];
                    tool.Name = item.name;
                    item.method.SetCogToolParameter(tool);
                    item.method.RunParams.Id = id;
                    id++;
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"ToolBlock Get Parameter  Fail : {ex.Message}");
            }

        }
        private void ToolBlockToMethodParam((CogMethod method, int id, string name)[] cogMethods)
        {

            try
            {
                foreach (var item in cogMethods)
                {

                    var tool = measureToolBlock.Tools[item.name];

                    item.method.SetCogToolParameter(tool);

                }
            }
            catch (Exception ex)
            {

                throw new Exception($"ToolBlock Get Parameter  Fail : {ex.Message}");
            }

        }
        /// <summary>
        /// 找到對應名稱的Tool 輸入圖片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ICogImage RunningToolInputImage(string name)
        {
            try
            {


                //從 CogMethods 裡面的名字去尋找 對應的tool

                var describe = algorithmDescribes.Where(a => a.Name == name).FirstOrDefault();

                var tool = measureToolBlock.Tools[name];

                ICogImage cogImage = null;
                switch (describe.CogMethodtype)
                {

                    case MethodType.CogSearchMaxTool:
                        var max = tool as CogSearchMaxTool;
                        cogImage = max.InputImage;

                        break;
                    case MethodType.CogImageConvertTool:

                        var cvr = tool as CogImageConvertTool;
                        cogImage = cvr.InputImage;
                        break;
                    case MethodType.CogFindEllipseTool:

                        var ecp = tool as CogFindEllipseTool;
                        cogImage = ecp.InputImage;
                        break;

                    case MethodType.CogImageSharpnessTool:

                        var cis = tool as CogImageSharpnessTool;
                        cogImage = cis.InputImage;
                        break;
                    default:
                        break;

                }
                return cogImage;
            }
            catch (Exception ex)
            {

                throw new Exception($"ToolBlock Get Parameter  Fail : {ex.Message}");
            }

        }

        public void MethodAssignTool()
        {
            try
            {
                foreach (var method in CogMethods)
                {
                    var tool = method.method.GetCogTool();

                    tool.Name = method.name;//Tool 會沒有名字 所以重新給名字
                    //判斷 Icogtool 的實際工具種類
                    MethodType type = DecideType(tool);

                    //將元利實體化的tool參數  塞回vpp流程裡面的工具  
                    switch (type)
                    {
                        case MethodType.CogSearchMaxTool:
                            CogSearchMaxTool toolBlockMaxTool = measureToolBlock.Tools[method.name] as CogSearchMaxTool;
                            CogSearchMaxTool methodTool = tool as CogSearchMaxTool;
                            toolBlockMaxTool.RunParams = methodTool.RunParams;
                            toolBlockMaxTool.Pattern = methodTool.Pattern;
                            toolBlockMaxTool.SearchRegion = methodTool.SearchRegion;

                            break;
                        case MethodType.CogImageConvertTool:
                            CogImageConvertTool toolBlockConvertTool = measureToolBlock.Tools[method.name] as CogImageConvertTool;
                            CogImageConvertTool methodConvertTool = tool as CogImageConvertTool;
                            toolBlockConvertTool.RunParams = methodConvertTool.RunParams;
                            toolBlockConvertTool.Region = methodConvertTool.Region;
                            break;
                        case MethodType.CogFindEllipseTool:
                            CogFindEllipseTool toolBlockFindEllipseTool = measureToolBlock.Tools[method.name] as CogFindEllipseTool;
                            CogFindEllipseTool methodFindEllipseTool = tool as CogFindEllipseTool;
                            toolBlockFindEllipseTool.RunParams = methodFindEllipseTool.RunParams;
                            
                            break;
                        case MethodType.CogImageSharpnessTool:
                         
                            CogImageSharpnessTool toolBlockFindSharpnessTool = measureToolBlock.Tools[method.name] as CogImageSharpnessTool;
                           
                            CogImageSharpnessTool methodFindSharpnessTool = tool as CogImageSharpnessTool;
                            toolBlockFindSharpnessTool.RunParams = methodFindSharpnessTool.RunParams;
                            toolBlockFindSharpnessTool.Region = methodFindSharpnessTool.Region;
                            break;
                        case MethodType.Error:
                            break;
                        default:
                            break;
                    }


                }

            }
            catch (Exception ex)
            {

                throw new Exception($"ToolBlock Get Parameter  Fail : {ex.Message}");
            }

        }


        public void SetMethodParam(IEnumerable<CogParameter> cogParameters)
        {
            var paramArr = cogParameters.ToArray();
            for (int i = 0; i < cogParameters.Count(); i++)
            {
                CogMethods[i].method.RunParams = paramArr[i];
            }
            MethodAssignTool();

        }


        //實體化Vision pro 方法
        private (CogMethod method, int id, string name)[] CreateMethod(AlgorithmDescribe[] algorithmDescribes)
        {
            try
            {
                List<(CogMethod method, int id, string name)> coglist = new List<(CogMethod method, int id, string name)>();
                foreach (var item in algorithmDescribes)
                {


                    switch (item.CogMethodtype)
                    {

                        case MethodType.CogSearchMaxTool:
                            
                            coglist.Add((new CogSearchMax(item.Name), Convert.ToInt32(item.Id), item.Name));


                            break;
                        case MethodType.CogImageConvertTool:
                            coglist.Add((new CogImageConverter(item.Name), Convert.ToInt32(item.Id), item.Name));

                            break;
                        case MethodType.CogFindEllipseTool:
                            coglist.Add((new CogEllipseCaliper(item.Name), Convert.ToInt32(item.Id), item.Name));
                            break;
                        case MethodType.CogImageSharpnessTool:
                            coglist.Add((new CogFindImageSharpness(item.Name), Convert.ToInt32(item.Id), item.Name));


                            break;
                        default:
                            break;

                    }
                }
                return coglist.ToArray();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private MethodType DecideType(ICogTool tool)
        {
 
            if (tool is CogSearchMaxTool)
                return MethodType.CogSearchMaxTool;
            else if (tool is CogImageConvertTool)
                return MethodType.CogImageConvertTool;
            else if (tool is CogFindEllipseTool)
                return MethodType.CogFindEllipseTool;
            else if (tool is CogImageSharpnessTool)
                return MethodType.CogImageSharpnessTool;
            return MethodType.Error;
        }
    }


}
