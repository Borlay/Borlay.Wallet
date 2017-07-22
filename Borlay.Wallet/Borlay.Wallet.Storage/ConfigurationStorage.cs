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

        public static void Save<T>(T collection) where T : new()
        {
            BackUp<T>();
            var filePath = GetFilePath<T>();
            var accJson = JsonConvert.SerializeObject(collection);
            File.WriteAllText(filePath, accJson);
        }

        public static void BackUp<T>() where T: new()
        {
            var filePath = GetFilePath<T>();
            if (File.Exists(filePath))
            {
                var fileText = File.ReadAllText(filePath);
                var acc = JsonConvert.DeserializeObject<T>(fileText);
                if (acc == null)
                    throw new Exception("cannot backup");

                var backUpFilePath = GetFilePath<T>(true);
                File.WriteAllText(backUpFilePath, fileText);
            }
        }

        private static string GetFilePath<T>(bool forBackup = false)
        {
            var appPath = GetDirectory(forBackup);
            var fileName = GetFileName<T>(forBackup);
            var filePath = Path.Combine(appPath, fileName);
            return filePath;
        }

        private static string GetFileName<T>(bool forBackup =false)
        {
            if (forBackup)
                return $"{typeof(T).Name}_{DateTime.Now.Ticks}.json";
            else
                return $"{typeof(T).Name}.json";
        }

        public static string GetDirectory(bool forBackup = false)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appPath = Path.Combine(path, "Borlay\\Wallet");

            if (forBackup)
                appPath = Path.Combine(appPath, "Backup");

            if (!Directory.Exists(appPath))
                Directory.CreateDirectory(appPath);

            return appPath;
        }
    }
}
