using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Storage
{
    public static class ConfigurationStorage
    {
        public static T Get<T>() where T : new()
        {
            var filePath = GetFilePath<T>();
            if (File.Exists(filePath))
            {
                var fileText = File.ReadAllText(filePath);
                var acc = JsonConvert.DeserializeObject<T>(fileText);
                return acc;
            }
            else
            {
                return new T();
            }
        }

        public static void Save<T>(T collection)
        {
            var filePath = GetFilePath<T>();
            var accJson = JsonConvert.SerializeObject(collection);
            File.WriteAllText(filePath, accJson);
        }

        private static string GetFilePath<T>()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(path, $"{typeof(T).Name}.xml");
            return filePath;
        }
    }
}
