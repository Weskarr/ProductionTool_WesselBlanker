
using UnityEngine;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class SaveVisual : MonoBehaviour
{
    [Header("Visual Text")]
    [SerializeField] private TextMeshProUGUI _saveNameText;

    [Header("Input UI")]
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _deleteButton;

    // Data
    private string _savePath;

    // Events
    public event Action<string> OnLoadTriggered;
    public event Action<string> OnDeleteTriggered;


    // ----------------- Functions -----------------


    #region Setter Functions

    public void SetSavePath(string path)
    {
        _savePath = path;
        _saveNameText.text = Path.GetFileNameWithoutExtension(path);
    }

    #endregion

    #region Trigger Functions

    public void LoadButtonTrigger() => OnLoadTriggered?.Invoke(_savePath);
    public void DeleteButtonTrigger() => OnDeleteTriggered?.Invoke(_savePath);

    #endregion
    
}
