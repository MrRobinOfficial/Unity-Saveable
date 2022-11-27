using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using uJSON.Converters;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Saveable
{
    public static class SaveableExtensions
    {
        [System.Serializable]
        public readonly struct ReadonlyVector3
        {
            public readonly float x;
            public readonly float y;
            public readonly float z;

            public ReadonlyVector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static implicit operator ReadonlyVector3(Vector3 a) => new(a.x, a.y, a.z);

            public static implicit operator Vector3(ReadonlyVector3 a) => new(a.x, a.y, a.z);

            public override string ToString() => string.Format("{0:0.00}{1:0.00}{2:0.00}", x, y, z);
        }

        public static bool TryGetGuid(this SaveableObject obj, out System.Guid guid)
        {
            guid = System.Guid.Empty;

            if (string.IsNullOrEmpty(obj.ID))
                return false;

            return System.Guid.TryParse(obj.ID, out guid);
        }

        public static string GetAutoPropertyName(string propName) => 
            string.Format("<{0}>k__BackingField", propName);

        public static bool IsDirectory(string path) => 
            string.IsNullOrEmpty(Path.GetFileName(path)) || Directory.Exists(path);

        public static async UniTask WriteFile(string filePath, string contents) => 
            await UniTask.CompletedTask;

        public static async UniTask<string> ReadFile(string filePath) => 
            await new UniTask<string>(string.Empty);

        public static string IterateFileName(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            var info = new FileInfo(filePath);
            var ext = info.Extension;
            var name = info.FullName.Substring(startIndex: 0, info.FullName.Length - ext.Length);

            int i = 2;

            while (File.Exists(path: $"{name}_{i:00}{ext}"))
                i++;

            return $"{name}_{i:00}{ext}";
        }

        public static string GetJsonFromSave(this SaveableManager manager) => string.Empty;

        public static void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            System.Diagnostics.Process.Start(filePath);
        }

        public static void ShowFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            System.Diagnostics.Process.Start(fileName: "explorer.exe", filePath);
        }

        //        public bool GetJsonFromSave(out string json)
        //        {
        //            json = string.Empty;
        //            var path = GetFullPath();

        //            if (!File.Exists(path))
        //                return false;

        //            try
        //            {
        //                using var stream = new FileStream(path, FileMode.Open);
        //                using var reader = new StreamReader(stream);

        //                var bytes = default(byte[]);

        //                using var memstream = new MemoryStream();

        //                reader.BaseStream.CopyTo(memstream);
        //                bytes = memstream.ToArray();

        //                var settings = new Newtonsoft.Json.JsonSerializerSettings
        //                {
        //                    Formatting = Newtonsoft.Json.Formatting.Indented,
        //                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
        //                };

        //                var data = EncrypterAES.DecryptStringFromBytes_Aes(bytes);

        //                if (data == null)
        //                    return false;

        //                var collection = DeserializeObject<Dictionary<Guid, IReadOnlyDictionary<int, object>>>(data);

        //                json = SerializeObject(collection, settings);
        //            }
        //            catch (System.Exception ex)
        //            {
        //                Debug.LogException(ex);
        //            }

        //#if UNITY_EDITOR
        //            UnityEditor.AssetDatabase.Refresh();
        //#endif

        //            return true;
        //        }
    }
}