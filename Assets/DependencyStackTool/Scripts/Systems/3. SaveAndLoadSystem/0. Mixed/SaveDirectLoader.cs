
// [Summary] (By Wessel)
//
// This is a prototype script for loading saves.
//

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class SaveDirectLoader : MonoBehaviour
    {
        // Library Plugins
        [DllImport("__Internal")] private static extern void UploadFile();

        // Events
        public event Action<NodeStackData> OnSaveLoadedSuccesfully;
        public event Action OnSaveLoadedFailed;


        // ----------------- Functions -----------------


        public void RequestLoad()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                UploadFile();
            #else
                Debug.LogWarning("RequestLoad() called outside WebGL. No file picker available.");
            #endif
        }

        public void OnFileLoaded(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                OnSaveLoadedFailed?.Invoke();
                return;
            }

            NodeStackData data = JsonUtility.FromJson<NodeStackData>(json);

            if (data == null)
            {
                OnSaveLoadedFailed?.Invoke();
                return;
            }

            OnSaveLoadedSuccesfully?.Invoke(data);
        }
    }
}