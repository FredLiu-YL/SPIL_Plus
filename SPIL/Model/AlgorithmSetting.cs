﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.ImageProcess;
using YuanliCore.Interface;

namespace SPIL.model
{
    public class SPILRecipe : AbstractRecipe
    {

        public SPILRecipe(AlgorithmDescribe[] aOIAlgorithms, AlgorithmDescribe[] sharpAlgorithms)
        {
            AOIParams = new List<CogParameter>();
            AOIParams2 = new List<CogParameter>();
            //foreach (var algorithms in aOIAlgorithms) {
            //    AOIParams.Add(null);
            //}

        }
      
        public double Mesument_0offset { get; set; }
        public double Mesument_CuNioffset { get; set; }
        public double Mesument_Cuoffset { get; set; }
 
        public AOIFunction AOIAlgorithmFunction { get; set; }


        /// <summary>
        /// 清晰度演算法
        /// </summary>
        public List<CogParameter> ClarityParams { get; set; }

        /// <summary>
        /// AOI 算法 (圓形)
        /// </summary>
        public List<CogParameter> AOIParams { get; set; }


        /// <summary>
        /// AOI 算法(八角)
        /// </summary>
        public List<CogParameter> AOIParams2 { get; set; }


        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做存檔功能
        /// </summary>
        /// <param name="Path"></param>
        public  void RecipeSave(string path)
        {
            //刪除所有Vistiontool 的檔案避免 id重複 寫錯，或是 原先檔案數量5個  後來變更成3個  讀檔會錯誤
            string[] files = Directory.GetFiles(path, "*VsTool_*");
            foreach (string file in files) {
                if (file.Contains("VsTool")) // 如果文件名包含 "VSP"
                {

                    File.Delete(file); // 删除该文件
                }
            }

            //RunParams  id 在100-199  用於檔案區隔
            if (AOIParams != null) {
                foreach (CogParameter param in AOIParams) {
                    param.Save(path);
                }
            }
            //RunParams  id 在301-399  用於檔案區隔
            if (AOIParams2 != null)
            {
                foreach (CogParameter param in AOIParams2)
                {
                    param.Save(path);
                }
            }
            //RunParams  id 在200-299以下 用於檔案區隔
            if (ClarityParams != null) {
                foreach (CogParameter param in ClarityParams) {
                    param.Save(path);
                }
            }


            base.Save(path + "\\Recipe.json");
        }
        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做讀檔功能
        /// </summary>
        /// <param name="Path"></param>
        public void Load(string path)
        {
            if (ClarityParams == null)
                ClarityParams = new List<CogParameter>();

            if (AOIParams == null)
                AOIParams = new List<CogParameter>();

            if (AOIParams2 == null)
                AOIParams2 = new List<CogParameter>();
            //不知道 直接清除會不會有問題  可能要Dispose
            AOIParams.Clear();
            AOIParams2.Clear();
            ClarityParams.Clear();
            //想不到好方法做序列化 ， 如果需要修改 就要用JsonConvert 把不能序列化的屬性都改掉  這樣就能正常做load
            var mRecipe = AbstractRecipe.Load<SPILRecipe>($"{path}\\Recipe.json");

            //未來新增不同屬性  這裡都要不斷新增

            AOIAlgorithmFunction = mRecipe.AOIAlgorithmFunction ;
            Mesument_0offset = mRecipe.Mesument_0offset;
            Mesument_CuNioffset = mRecipe.Mesument_CuNioffset;
            Mesument_Cuoffset = mRecipe.Mesument_Cuoffset;
            //SharpVppPath = mRecipe.SharpVppPath;


            // AOI圓形
            string[] algorithmfiles = Directory.GetFiles(path, "*VsTool_1*");
            foreach (var file in algorithmfiles) {

                string fileName = Path.GetFileName(file);

                string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);
                if (id[0] == "0") continue; // 0 是定位用的樣本 所以排除
                CogParameter param = CogParameter.Load(path, Convert.ToInt32(id[0]));
                AOIParams.Add(param);


            }
            // AOI 八角形
            string[] algorithmfiles2 = Directory.GetFiles(path, "*VsTool_3*");
            foreach (var file in algorithmfiles2)
            {

                string fileName = Path.GetFileName(file);

                string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);
                if (id[0] == "0") continue; // 0 是定位用的樣本 所以排除
                CogParameter param = CogParameter.Load(path, Convert.ToInt32(id[0]));
                AOIParams2.Add(param);


            }
            //清晰度
            string[] clarityfiles = Directory.GetFiles(path, "*VsTool_2*");
            foreach (var file in clarityfiles) {
                string fileName = Path.GetFileName(file);

                string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);
                if (id[0] == "0") continue; // 0 是定位用的樣本 所以排除
                CogParameter param = CogParameter.Load(path, Convert.ToInt32(id[0]));
                ClarityParams.Add(param);


            }
        }


    }
    // 自訂的演算法顯示
    public class AlgorithmDescribe
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public MethodType CogMethodtype { get; set; }

        /// <summary>
        /// cognex的演算法   因無法json序列化 所以忽略存檔 ，另外處理
        /// </summary>
       /* [JsonIgnore]      
        public CogMethod CogAOIMethod { get; set; }*/

        public AlgorithmDescribe(string id, string name, MethodType methodType)
        {
            Id = id;
            Name = name;
            CogMethodtype = methodType;

            //      CogSearchMaxTool;
            //      CogImageConvertTool asd;

        }

        public override string ToString()
        {
            return Id + " | " + Name;
        }
    }

    public enum MethodType
    {
        CogSearchMaxTool,
        CogImageConvertTool,
        CogFindEllipseTool,
        CogImageSharpnessTool,
        CogFindLineTool,
        Error,

    }

    public enum AOIFunction
    {
        Circle,
        Octagon
    }
}
