﻿using Cognex.VisionPro;
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
    /// <summary>
    /// AOI 計算 流程
    /// </summary>
    public class AOIFlow
    {
        private CogToolBlock measureToolBlock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aoiVppPath"> toolblock 的vpp </param>
        /// <param name="algorithmDescribes"> config內紀錄 可被編輯的tool</param>
        public AOIFlow(string aoiVppPath, AlgorithmDescribe[] algorithmDescribes)
        {
            //讀取vpp檔 獲得寫在vpp的完整流程
            measureToolBlock = CogSerializer.LoadObjectFromFile(aoiVppPath) as CogToolBlock;

            CogAOIMethods = CreateMethod(algorithmDescribes);
            ToolBlockToMethodParam();
        }

        /// <summary>
        /// cognex的演算法   
        /// </summary>   
        public (CogMethod method, int id, string name)[] CogAOIMethods { get; }

        /// <summary>
        /// 獲得 tool 參數
        /// </summary>
        private void ToolBlockToMethodParam()
        {
            int id = 101;
            try {
                foreach (var item in CogAOIMethods) {

                    var tool = measureToolBlock.Tools[item.name];
                    item.method.SetCogToolParameter(tool);
                    item.method.RunParams.Id = id;
                    id++;
                }
            }
            catch (Exception ex) {

                throw  new Exception($"ToolBlock Get Parameter  Fail : {ex.Message}" );
            }

        }

        public void SetMethodParam(IEnumerable<CogParameter> cogParameters)
        {
            var paramArr = cogParameters.ToArray();
            for (int i = 0; i < cogParameters.Count(); i++) {
                CogAOIMethods[i].method.RunParams = paramArr[i];
            }

        }


        //實體化Vision pro 方法
        private (CogMethod method, int id, string name)[] CreateMethod(AlgorithmDescribe[] algorithmDescribes)
        {
            try {
                List<(CogMethod method, int id, string name)> coglist = new List<(CogMethod method, int id, string name)>();
                foreach (var item in algorithmDescribes) {


                    switch (item.CogMethodtype) {

                        case MethodType.CogSearchMaxTool:
                            coglist.Add((new CogSearchMax(), Convert.ToInt32(item.Id), item.Name));


                            break;
                        case MethodType.CogImageConvertTool:
                            coglist.Add((new CogImageConverter(), Convert.ToInt32(item.Id), item.Name));

                            break;
                        case MethodType.CogFindEllipseTool:
                            coglist.Add((new CogEllipseCaliper(), Convert.ToInt32(item.Id), item.Name));


                            break;
                        default:
                            break;

                    }
                }
                return coglist.ToArray();
            }
            catch (Exception ex) {

                throw ex;
            }
        }
    }
}
