using System;
using System.IO;
using Newtonsoft.Json;

namespace SharpWcf.Configuration
{
    public class ConfigLoader
    {
        public static T LoadConfig<T>(string fileName)
        {
            var localFileName = fileName + ".local";

            if (File.Exists(localFileName))
            {
                fileName = localFileName;
            }

            if (!File.Exists(fileName))
                throw new ApplicationException("Unable to find configuration: " + fileName);

            var config = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));

            return config;
        }
    }
}