using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPIL
{
    public class ManualPeakImage
    {
        private bool isCancel = false;

        public ManualPeakImage()
        {



        }

        /// <summary>
        /// 等待資料夾有三張圖片
        /// </summary>
        /// <param name="folder_info"></param>
        /// <returns></returns>
        public async Task<List<string>> WaitForImage(DirectoryInfo folder_info)
        {

            int imagecount = 0;
            isCancel = false;
            //  List<Bitmap> bmpList = new List<Bitmap>();
            List<string> bmpList = new List<string>();
    //        FileInfo[] jpgFiles = new FileInfo[] { };
            FileInfo[] bmpFiles = new FileInfo[] { };

            await Task.Run(async () =>
             {


                 while (imagecount < 3 && !isCancel)
                 {
                     await Task.Delay(500);
     //                jpgFiles = folder_info.GetFiles("*.jpg");
                     bmpFiles = folder_info.GetFiles("*COLOR2D*");
               //      "COLOR2D"
                  //   imagecount = jpgFiles.Length + bmpFiles.Length;
                     imagecount =   bmpFiles.Length;
                 }



             });


            if (isCancel)
            {

                return bmpList;
            }

            /*
            List<Bitmap> tempBmpList = new List<Bitmap>();
            foreach (var item in jpgFiles)
            {
                tempBmpList.Add(new Bitmap(item.FullName));
            }
            foreach (var item in bmpFiles)
            {
                tempBmpList.Add(new Bitmap(item.FullName));
            }

            //複製到記憶體  釋放BMP磁碟檔案
            foreach(var item in tempBmpList)
            {
                bmpList.Add(new Bitmap(item));
                item.Dispose();
            }*/

         //   foreach (var item in jpgFiles)
         //       bmpList.Add(item.FullName);
        
            foreach (var item in bmpFiles)
                bmpList.Add(item.FullName);
       

            return bmpList;

        }
        /// <summary>
        /// 清除資料夾內 圖片檔案
        /// </summary>
        /// <param name="folder_info"></param>
        public void DelDirectoryImage(DirectoryInfo folder_info)
        {
            FileInfo[] jpgFiles = new FileInfo[] { };
            FileInfo[] bmpFiles = new FileInfo[] { };
            jpgFiles = folder_info.GetFiles("*.jpg");
            bmpFiles = folder_info.GetFiles("*.bmp");

            foreach (var item in jpgFiles)
            {
                item.Delete();
            }
            foreach (var item in bmpFiles)
            {
                item.Delete();
            }
        }
        /// <summary>
        /// 取消  資料夾等待三張圖片
        /// </summary>
        public void CancelWait()
        {
            isCancel = true;

        }
    }
}
