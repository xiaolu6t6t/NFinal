using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace NFinal.Compile
{
    public class GenConfig
    {
        public List<string> controllerFiles = new List<string>();
        public List<string> bllFiles = new List<string>();

        public static GenConfig Load(string fileName)
        {
            GenConfig config=null;
            if (File.Exists(fileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GenConfig));
                StreamReader sr = new StreamReader(fileName, System.Text.Encoding.UTF8);
                config = (GenConfig)ser.Deserialize(sr);
                sr.Close();
            }
            return config;
        }
    }
}