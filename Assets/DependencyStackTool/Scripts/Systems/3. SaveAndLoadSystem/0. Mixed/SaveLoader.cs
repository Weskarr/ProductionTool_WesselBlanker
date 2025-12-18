
// [Summary] (By Wessel)
//
// This is a prototype script for loading saves.
//

using System;
using System.IO;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class SaveLoader : MonoBehaviour
    {
        // File Path Handler
        private FilePathHandler _filePathHandler => FilePathHandler.Instance;

        // Events
        public event Action<NodeStackData> OnSaveLoadedSuccesfully;
        public event Action<string> OnSaveLoadedFailed;


        // ----------------- Functions -----------------


        public void LoadSave(string saveFileName)
        {
            string path = TryGetSaveFilePath(saveFileName);

            // Assume failure.
            bool isLoaded = false;
            NodeStackData data = null;

            if (path != null)
                data = TryGetSaveFile(path);

            // Proof of success.
            if (data != null)
                isLoaded = true;

            BroadcastResult(isLoaded, data, saveFileName);
        }

        private void BroadcastResult(bool isLoaded, NodeStackData data, string saveFileName)
        {
            if (isLoaded)
                OnSaveLoadedSuccesfully?.Invoke(data);
            else
                OnSaveLoadedFailed?.Invoke(saveFileName);
        }

        private string TryGetSaveFilePath(string saveFileName)
        {
            // No file paths, null.
            if (_filePathHandler == null)
                return null;

            // Save file name is null or empty, null.
            if (string.IsNullOrWhiteSpace(saveFileName))
                return null;

            // Okay can now safely create path to the saved file.
            return Path.Combine(_filePathHandler.GetSavesFolder, saveFileName + _filePathHandler.GetSaveFileExtension);
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