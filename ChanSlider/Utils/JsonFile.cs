using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Utils
{
    class JsonFile
    {
        private string filePath;

        public static T Get<T>(string filePath)
            where T : JsonFile, new()
        {
            T obj;
            if (!File.Exists(filePath))
            {
                obj = new T() { filePath = filePath };
                obj.SaveJson();
                return obj;
            }

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    obj = new Newtonsoft.Json.JsonSerializer().Deserialize<T>(jsonReader);
            }
            catch
            {
                obj = new T();
            }

            obj.filePath = filePath;
            return obj;
        }

        public void SaveJson()
        {
            if (filePath == null)
                return;

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var streamWriter = new StreamWriter(fileStream);
            new Newtonsoft.Json.JsonSerializer() { Formatting = Newtonsoft.Json.Formatting.Indented }
            .Serialize(streamWriter, this, GetType());
        }
    }
}
