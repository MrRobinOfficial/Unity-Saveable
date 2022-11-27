#define AUTO_SPAWN
#undef AUTO_SPAWN

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Saveable.SaveableExtensions;
using static uJSON.uJSONExtensions;

namespace Saveable
{
    [AddComponentMenu("Tools/Saveables/Saveable Manager"), DisallowMultipleComponent]
    public sealed class SaveableManager : MonoBehaviour
    {
#if AUTO_SPAWN
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (SaveableManager.Instance != null)
                return;

            var obj = new GameObject("Saveable Manager [Runtime]");

            obj.transform.hideFlags = HideFlags.HideInInspector;
            obj.AddComponent<SaveableManager>();
        }
#endif

        public static SaveableManager Instance { get; private set; } = null;

        public const string DefaultDirectory = "Saves\\";
        public const string DefaultFilePath = "gamesave.dat";

        [field: SerializeField] public string FilePath { get; set; } = DefaultFilePath;
        [field: SerializeField] public bool AllowAutoSave { get; set; } = false;
        [field: SerializeField] public bool IncludeInactiveObjects { get; set; } = true;

        public string GetDirectoryPath() => Path.Combine(Application.streamingAssetsPath, DefaultDirectory);

        public string GetFullPath()
        {
            if (string.IsNullOrEmpty(FilePath))
                FilePath = DefaultFilePath;

            return Path.Combine(GetDirectoryPath(), FilePath);
        }

        public string GetFullAutoPath() => Path.Combine(GetDirectoryPath(), "Autosave.bak");

        public IReadOnlyDictionary<Guid, IReadOnlyDictionary<int, JObject>> Storage => _storage;

        private Dictionary<Guid, IReadOnlyDictionary<int, JObject>> _storage = new();

        private void Awake()
        {
            transform.hideFlags = HideFlags.HideInInspector;

            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Reload Snapshot", isValidateFunction: false, priority: 100050)]
        public void ReloadSnapshot()
        {
#if DEBUG
            Debug.Log(message: "<color=green>Reloading snapshot...</color>", this);
#endif

            foreach (var obj in FindObjectsOfType<SaveableObject>(IncludeInactiveObjects))
            {
                if (!obj.TryGetGuid(out var guid))
                    continue;

                if (_storage.TryGetValue(guid, out IReadOnlyDictionary<int, JObject> collection))
                    obj.LoadAllComponents(collection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Save Snapshot", isValidateFunction: false, priority: 100050)]
        public void SaveSnapshot()
        {
            //TODO: Make this async method!
            // Scenario: If saving takes more time than destroying the scene!

#if DEBUG
            Debug.Log(message: "<color=green>Saving snapshot...</color>", this);
#endif

            foreach (var obj in FindObjectsOfType<SaveableObject>(IncludeInactiveObjects))
                HandleObject(obj);

            if (AllowAutoSave)
                WriteFile(GetFullAutoPath(), string.Empty);

            void HandleObject(SaveableObject obj)
            {
                if (!obj.TryGetGuid(out var guid))
                    return;

                _storage[guid] = obj.SaveAllComponents();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Delete Snapshot", isValidateFunction: false, priority: 100050)]
        public void DeleteSnapshot() => _storage.Clear();

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Fetch Snapshot", isValidateFunction: false, priority: 1000150)]
        public void FetchSnapshot()
        {
            var path = GetFullPath();

            if (!File.Exists(path))
                return;

            try
            {
                using var stream = new FileStream(path, FileMode.Open);
                using var reader = new StreamReader(stream);

                var bytes = default(byte[]);

                using var memstream = new MemoryStream();

                reader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();

                var json = EncrypterAES.DecryptStringFromBytes_Aes(bytes);
                _storage = DeserializeObject<Dictionary<Guid, IReadOnlyDictionary<int, JObject>>>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Push Snapshot", isValidateFunction: false, priority: 1000150)]
        public void PushSnapshot()
        {
            var filePath = GetFullPath();

            try
            {
                var directory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
                using var writer = new StreamWriter(stream);

                var json = SerializeObject(_storage);
                var byteArray = EncrypterAES.EncryptStringToBytes_Aes(json);

                writer.BaseStream.Write(byteArray, offset: 0, byteArray.Length);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/SaveAndPush Snapshot", isValidateFunction: false, priority: 1000300)]
        public void SaveAndPushSnapshot()
        {
            SaveSnapshot();
            PushSnapshot();
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/FetchAndReload Snapshot", isValidateFunction: false, priority: 1000300)]
        public void FetchAndReloadSnapshot()
        {
            FetchSnapshot();
            ReloadSnapshot();
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Create Save", isValidateFunction: false, priority: 1000500)]
        public void CreateSave()
        {
            var filePath = GetFullPath();

            try
            {
                var directory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var fileName = IterateFileName(filePath);
                var newPath = Path.Combine(directory, fileName);
                File.Create(newPath).Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Delete Save", isValidateFunction: false, priority: 1000500)]
        public void DeleteSave()
        {
            var filePath = GetFullPath();

            try
            {
                if (!File.Exists(filePath))
                    return;

                var metaFilePath = $"{filePath}.meta";

                if (File.Exists(metaFilePath))
                    File.Delete(metaFilePath);

                File.Delete(filePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Manager/Clear Save", isValidateFunction: false, priority: 1000500)]
        public void ClearSave()
        {
            var filePath = GetFullPath();

            try
            {
                if (!File.Exists(filePath))
                    return;

                using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
                stream.SetLength(value: 0);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

#region Callbacks

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var obj = new GameObject(name: "Saveable Handler [Runtime]");
            obj.AddComponent<SaveableHandler>();

            print(nameof(OnSceneLoaded));
        }

#endregion

        [System.Serializable]
        readonly struct ReadOnlyData
        {
            public readonly string name;
            public readonly int age;

            public ReadOnlyData(string name, int age)
            {
                this.name = name;
                this.age = age;
            }

            public override string ToString() => $"{name}: {age} years old";
        }

        [ContextMenu(nameof(HelloWorld))]
        private void HelloWorld()
        {
            var ctx = new ReadOnlyData();

            var json = SerializeObject(ctx);
            var data = DeserializeObject<ReadOnlyData>(json);

            print(data.ToString());
        }

        [ContextMenu(nameof(Temp))]
        private void Temp() => SceneManager.LoadScene(sceneBuildIndex: 0);

        [ContextMenu(nameof(Temp1))]
        private void Temp1() => SceneManager.LoadScene(sceneBuildIndex: 0, LoadSceneMode.Additive);
    }
}