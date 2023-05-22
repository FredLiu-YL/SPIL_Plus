﻿using Newtonsoft.Json;
using SPIL.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIL.Model
{
    public class MachineSetting
    {
        public MachineSetting()
        {
            AOIAlgorithms = new AlgorithmDescribe[]
                     {
                         new AlgorithmDescribe("001", "CogSearchMaxTool1", MethodType.CogSearchMaxTool) ,
                         new AlgorithmDescribe("002", "CogFindEllipseTool2", MethodType.CogFindEllipseTool) ,
                         new AlgorithmDescribe("003", "CogImageConvertTool1", MethodType.CogImageConvertTool)

                     };
            SharpAlgorithms = new AlgorithmDescribe[]
                    {
                         new AlgorithmDescribe("001", "CogSearchMaxTool1", MethodType.CogSearchMaxTool) ,
                         new AlgorithmDescribe("002", "CogFindEllipseTool2", MethodType.CogFindEllipseTool) ,
                         new AlgorithmDescribe("003", "CogImageConvertTool1", MethodType.CogImageConvertTool)

                    };

        }


        public string AOIVppPath { get; set; }
        public string SharpVppPath { get; set; }

        public AlgorithmDescribe[] AOIAlgorithms { get; set; }
        public AlgorithmDescribe[] SharpAlgorithms { get; set; }
        public static MachineSetting Load(string filename)  
        {
            string extension = Path.GetExtension(filename);
            if (!File.Exists(filename)) throw new FileNotFoundException($"Not found recipe file", filename);

            try {
                string dirPath = new DirectoryInfo(filename).FullName;
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    TypeNameHandling = TypeNameHandling.Auto
                };

                using (FileStream fs = File.Open(Path.GetFullPath(filename), FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                using (JsonReader jr = new JsonTextReader(sr)) {
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    var recipe = serializer.Deserialize<MachineSetting>(jr);
                  
                    return recipe;
                }
            }
            catch (JsonReaderException) {
                throw;
            }
        }
        public void Save(string fileName)
        {
            try {
                string fileFullPath = Path.GetFullPath(fileName);
                string dirFullPath = Path.GetDirectoryName(fileFullPath);

                DirectoryInfo dir = new DirectoryInfo(dirFullPath);
                if (!dir.Exists) throw new DirectoryNotFoundException($"Directory not exists {dir.FullName}");

             

                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Error,
                    TypeNameHandling = TypeNameHandling.All
                };

                using (FileStream fs = File.Open(fileName, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw)) {
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    serializer.Serialize(jw, this);
                }

            
            }
            catch (Exception ex) {
                throw new InvalidOperationException($"Save recipe failed.", ex);
            }
        }

    }


}
