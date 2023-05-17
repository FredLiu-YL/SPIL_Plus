using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ImageProcessing;
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
    public class AlgorithmSetting: AbstractRecipe
    {
        /// <summary>
        /// 清晰度演算法
        /// </summary>
        public List< AlgorithmDescribe> ClarityAlgorithms { get; set; }

        /// <summary>
        /// AOI 驗算法
        /// </summary>
        public List<AlgorithmDescribe> AlgorithmDescribes { get; set; }


        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做存檔功能
        /// </summary>
        /// <param name="Path"></param>
        public new void Save(string path)
        {
            //刪除所有Vistiontool 的檔案避免 id重複 寫錯，或是 原先檔案數量5個  後來變更成3個  讀檔會錯誤
            string[] files = Directory.GetFiles(path, "*VsTool_*");
            foreach (string file in files) {
                if (file.Contains("VsTool")) // 如果文件名包含 "VSP"
                {
                  
                    File.Delete(file); // 删除该文件
                }
            }

         
            foreach (AlgorithmDescribe param in ClarityAlgorithms) {
                param.CogAOIMethod.RunParams.Save(path);
            }
            base.Save(path + "\\Recipe.json");
        }
        /// <summary>
        /// 因某些元件無法被正常序列化 所以另外做讀檔功能
        /// </summary>
        /// <param name="Path"></param>
        public void Load(string path)
        {

            ClarityAlgorithms.Clear();
            AlgorithmDescribes.Clear();
            //想不到好方法做序列化 ， 如果需要修改 就要用JsonConvert 把不能序列化的屬性都改掉  這樣就能正常做load
            var mRecipe = AbstractRecipe.Load<AlgorithmSetting>($"{path}\\Recipe.json");
            
            //未來新增不同屬性  這裡都要不斷新增
            ClarityAlgorithms = mRecipe.ClarityAlgorithms; 
            AlgorithmDescribes = mRecipe.AlgorithmDescribes;

            string[] files = Directory.GetFiles(path, "*VsTool_*");

            foreach (var file in files) {
                string fileName = Path.GetFileName(file);

                string[] id = fileName.Split(new string[] { "VsTool_", ".tool" }, StringSplitOptions.RemoveEmptyEntries);
                if (id[0] == "0") continue; // 0 是定位用的樣本 所以排除
                CogParameter param = CogParameter.Load(path, Convert.ToInt32(id[0]));

              

            }

        }


    }
    // 自訂的演算法顯示
    public class AlgorithmDescribe
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public MethodType CogMethodtype  { get; set; }

       
        public CogMethod CogAOIMethod { get; set; }

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


    }
}
