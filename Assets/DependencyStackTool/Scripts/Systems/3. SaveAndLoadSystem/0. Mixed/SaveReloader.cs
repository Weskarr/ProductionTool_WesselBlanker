
// [Summary] (By Wessel)
//
// This is a prototype script for reloading saves.
//

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class SaveReloader : MonoBehaviour
    {
        // File Path Handler
        private FilePathHandler _filePathHandler => FilePathHandler.Instance;

        // Collection of saves
        private HashSet<string> _saveFileNames = new();

        // Events
        public event Action OnSaveReloaded;


        // ----------------- Functions -----------------


        public HashSet<string> GetSaveFilesNames()
        {
            // Always reload files for now.
            ReloadSaves();

            // Return a hashset of all the available save file names.
            return _saveFileNames;
        }

        private void ReloadSaves()
        {
            // No file paths, null.
            if (_filePathHandler == null)
                return;

            // Clear all save file names hashset.
            _saveFileNames.Clear();

            // Get the save folder & all save file paths in it.
            string savesFolder = _filePathHandler.GetSavesFolder;
            string[] saveFilePaths = Directory.GetFiles(savesFolder, "*" + _filePathHandler.GetSaveFileExtension);

            // Check if they are valid save files then add their names to a hashset.
            foreach (string saveFilePath in saveFilePaths)
            {
                // Try to load the save file.
                NodeStackData data = TryGetSaveFile(saveFilePath);

                // Check if it is loadable.
                if (data == null)
                    continue;

                // Just add the name to the file names hashset.
                string fileName = Path.GetFileNameWithoutExtension(saveFilePath);
                _saveFileNames.Add(fileName);
            }

            BroadcastResult();
        }

        private void BroadcastResult()
        {
            OnSaveReloaded?.Invoke();
        }

        private NodeStackData TryGetSaveFile(string path)
        {
            // No save file exists to load, null.
            if (!File.Exists(path))
                return null;

            // Actually load the save file.
            string filetoLoad = File.ReadAllText(path);

            // Loaded save file name is null or empty, null.
            if (string.IsNullOrWhiteSpace(filetoLoad))
                return null;

            // Attempt to parse into saved class.
            NodeStackData data = JsonUtility.FromJson<NodeStackData>(filetoLoad);

            // Failed to parse into the saved class, null.
            if (data == null)
                return null;

            // Optional: check if required fields exist?
            // And if not, perhaps attempt repairs?
            // Lets assume it's all good for now!

            // Loaded save file class succesfully.
            return data;
        }
    }
}