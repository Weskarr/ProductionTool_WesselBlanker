
// [Summary] (By Wessel)
//
// This is a prototype script for making saves.
//

using System;
using System.IO;
using UnityEngine;

// Bad: ToolMaster -> Here
using NodeStackSystem;

namespace SaveAndLoadSystem
{
    public class SaveCreator : MonoBehaviour
    {
        // File Path Handler
        private FilePathHandler _filePathHandler => FilePathHandler.Instance;

        // Events
        public event Action<string> OnSaveCreated;


        // ----------------- Functions -----------------


        public void CreateSave(NodeStackManager nodeStackManager, string saveFileName)
        {
            NodeStackStyleData style = nodeStackManager.GetCurrentNodeStackStyle();
            string order = nodeStackManager.GetCurrentLayoutNodeDataOrder();
            LayoutNodeData[] array = nodeStackManager.GetCurrentLayoutNodeDataArray();

            NodeStackData data = CreateSaveData(style, order, array);
            string path = TryMakeSaveFilePath(saveFileName);

            // Assume failure.
            bool isCreated = false;

            // Proof of succes.
            if (data != null && path != null)
            {
                isCreated = true;
                CreateSaveFile(data, path);
            }

            BroadcastResult(isCreated, saveFileName);
        }

        private void BroadcastResult(bool isCreated, string saveFileName)
        {
            if (true)
                OnSaveCreated?.Invoke(saveFileName);
        }

        private void CreateSaveFile(NodeStackData nodeStackData, string savePath)
        {
            string dataJson = JsonUtility.ToJson(nodeStackData, true);
            File.WriteAllText(savePath, dataJson);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        private string TryMakeSaveFilePath(string saveFileName)
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

        private NodeStackData CreateSaveData(NodeStackStyleData nodeStackStyle, string nodeLayoutOrder, LayoutNodeData[] nodeDataArray)
        {
            if (nodeStackStyle == null)
            {
                // Switch to an alternative default?

                Debug.Log("CreateSaveData, nodeStackStyle is null!");
            }

            if (nodeLayoutOrder == null || nodeLayoutOrder == null)
            {
                // Switch to an alternative default?

                Debug.Log("CreateSaveData, nodeLayoutOrder or nodeDataArray is null!");
            }

            NodeStackData data = new()
            {
                NodeStackStyle = nodeStackStyle,
                NodeLayoutDataOrder = nodeLayoutOrder,
                NodeLayoutDataArray = nodeDataArray
            };

            return data;
        }
    }
}