
// [Summary] (By Wessel)
//
// This is a prototype script for loading saves.
//

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SaveAndLoadSystem
{
    public class SaveDirectReset : MonoBehaviour
    {
        // Datas
        [SerializeField] private NodeStackData _nodeStackDataDefault;

        // Events
        public event Action<NodeStackData> OnSaveReset;


        // ----------------- Functions -----------------


        public void ResetSave()
        {
            _nodeStackDataDefault.NodeStackStyle.Reset();
            OnSaveReset?.Invoke(_nodeStackDataDefault);
        }
    }
}
