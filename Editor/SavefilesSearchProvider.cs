using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Saveable.Editor
{
    public class SavefilesSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private List<FileInfo> infos = new();
        private SaveableManager manager;

        private const string META_EXTENSION = ".meta";

        public SavefilesSearchProvider(SaveableManager manager)
        {
            this.manager = manager;

            var directoryPath = manager.GetDirectoryPath();

            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var entries = Directory.GetFileSystemEntries(directoryPath,
                    searchPattern: "*", SearchOption.AllDirectories);

                foreach (var entry in entries)
                {
                    if (SaveableExtensions.IsDirectory(entry))
                        continue;

                    var extension = Path.GetExtension(entry);

                    if (extension.Equals(META_EXTENSION))
                        continue;

                    infos.Add(new FileInfo(entry));
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Save files"), level: 0));

            infos.Sort((x, y) => x.Name.CompareTo(y.Name));

            var groups = new List<string>();
            var dirLength = manager.GetDirectoryPath().Length;

            foreach (var info in infos)
            {
                var path = info.FullName.Substring(dirLength);
                var entryTitle = path.Split(Path.DirectorySeparatorChar);
                var groupName = string.Empty;

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    var item = entryTitle[i];

                    groupName += item;

                    if (!groups.Contains(groupName))
                    {
                        entries.Add(new SearchTreeGroupEntry(
                            new GUIContent(item), level: i + 1));

                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                var entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
                entry.level = entryTitle.Length;
                entry.userData = info;
                entries.Add(entry);
            }

            groups.Sort((x, y) => x.CompareTo(y));

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var info = (FileInfo)SearchTreeEntry.userData;

            if (info == null)
                return false;

            var path = info.FullName;
            manager.FilePath = path.Substring(manager.GetDirectoryPath().Length);

            return true;
        }
    }
}