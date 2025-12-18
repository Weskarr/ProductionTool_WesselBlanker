
// [Summary] (By Wessel)
//
// This is a prototype script for deleting saves.
//

using System;
using System.IO;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class SaveDeleter : MonoBehaviour
    {
        // File Path Handler
        private FilePathHandler _filePathHandler => FilePathHandler.Instance;

        // Events
        public event Action<string> OnSaveDeletedSuccesfully;
        public event Action<string> OnSaveDeletedFailed;


        // ----------------- Functions -----------------


        public void DeleteSave(string saveFileName)
        {
            string path = TryMakeSaveFilePath(saveFileName);

            // Assume failure.
            bool isDeleted = false;

            // Proof of success.
            if (path != null)
                isDeleted = TryDeleteSaveFile(path);

            BroadcastResult(isDeleted, saveFileName);
        }

        private void BroadcastResult(bool isDeleted, string saveFileName)
        {
            if (isDeleted)
                OnSaveDeletedSuccesfully?.Invoke(saveFileName);
            else
                OnSaveDeletedFailed?.Invoke(saveFileName);
        }

        private string TryMakeSaveFilePath(string saveFileName)
        {
            // No file path handler, null.
            if (_filePathHandler == null)
                return null;

            // Save file name is null or empty, null.
            if (string.IsNullOrWhiteSpace(saveFileName))
                return null;

            // Okay can now safely create path to the saved file.
            return Path.Combine(_filePathHandler.GetSavesFolder, saveFileName + _filePathHandler.GetSaveFileExtension);
        }

        private bool TryDeleteSaveFile(string path)
        {
            // No save file exists to delete, false.
            if (!File.Exists(path))
                return false;

            // Actually delete the existing save file.
            File.Delete(path);

            // Does the save file stil exist?
            if (!File.Exists(path))
            {
                // No, deleted save file succesfully.
                return true;
            }
            else
            {
                // Yes, failed to delete the save file.
                return false;
            }
        }
    }
}