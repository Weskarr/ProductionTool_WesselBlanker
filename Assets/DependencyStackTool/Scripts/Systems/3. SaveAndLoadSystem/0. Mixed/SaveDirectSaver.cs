// [Summary] (By Wessel)
//
// This is a prototype script for making direct saves for WebGL.
//

using System;
using UnityEngine;
using System.Runtime.InteropServices;

// Bad: ToolMaster -> Here
using NodeStackSystem;

namespace SaveAndLoadSystem
{
    public class SaveDirectSaver : MonoBehaviour
    {
        // Library Plugins
        [DllImport("__Internal")] private static extern void DownloadFile(string filename, string content);

        // Events
        public event Action<string> OnDirectSaveSaved;


        // ----------------- Functions -----------------


        public void CreateSave(NodeStackManager nodeStackManager)
        {
            NodeStackStyleData style = nodeStackManager.GetCurrentNodeStackStyle();
            string order = nodeStackManager.GetCurrentLayoutNodeDataOrder();
            LayoutNodeData[] array = nodeStackManager.GetCurrentLayoutNodeDataArray();

            NodeStackData data = CreateSaveData(style, order, array);

            bool isCreated = false;
            string saveFileName = null;

            if (data != null)
            {
                saveFileName = GenerateDefaultFileName();
                CreateSaveFile(data, saveFileName);
                isCreated = true;
            }

            BroadcastResult(isCreated, saveFileName);
        }

        private string GenerateDefaultFileName()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return $"NodeStackSave_{timestamp}";
        }

        private void BroadcastResult(bool isCreated, string saveFileName)
        {
            if (isCreated)
                OnDirectSaveSaved?.Invoke(saveFileName);
        }

        private void CreateSaveFile(NodeStackData nodeStackData, string fileName)
        {
            string dataJson = JsonUtility.ToJson(nodeStackData, true);
            #if UNITY_WEBGL && !UNITY_EDITOR
                DownloadFile($"{fileName}.json", dataJson);
            #endif
        }

        private NodeStackData CreateSaveData
        (
            NodeStackStyleData nodeStackStyle,
            string nodeLayoutOrder,
            LayoutNodeData[] nodeDataArray
        )
        {
            if (nodeStackStyle == null)
                Debug.LogWarning("CreateSaveData: nodeStackStyle is null!");

            if (nodeLayoutOrder == null || nodeDataArray == null)
                Debug.LogWarning("CreateSaveData: nodeLayoutOrder or nodeDataArray is null!");

            return new NodeStackData
            {
                NodeStackStyle = nodeStackStyle,
                NodeLayoutDataOrder = nodeLayoutOrder,
                NodeLayoutDataArray = nodeDataArray
            };
        }
    }
}