
// [Summary] (By Wessel)
//
// This script is the center of all folder file paths (& extension),
// Other scripts may freely read this information and use them accordingly.
//

using System.IO;
using UnityEngine;

public class FilePathHandler : MonoBehaviour
{
    // Bad: Move this to a data script!
    [SerializeField] private string _saveFileExtension = ".json";
    [SerializeField] private string _savesFolderName = "Saves";
    //[SerializeField] private string _exportsFolderName = "Exports";

    // Singleton reference
    public static FilePathHandler Instance { get; private set; }

    // Avaliable Paths
    [SerializeField] private string _baseFolderPath;
    [SerializeField] private string _savesFolderPath;
    //private string _exportsFolderPath;


    // ----------------- Functions -----------------


    #region Getters

    public string GetSaveFileExtension => _saveFileExtension;
    public string GetBaseFolder => _baseFolderPath;
    public string GetSavesFolder => _savesFolderPath;
    //public string GetExportsFolder => _exportsFolderPath;

    #endregion

    #region Awake Functions

    private void Awake()
    {
        Singleton();

#if UNITY_EDITOR || UNITY_STANDALONE
        SetupBaseFolder();
        _savesFolderPath = SetupFolder(_savesFolderName);
        //_exportsFolderPath = SetupFolder(_exportsFolderName);
#endif
    }

    private void Singleton()
    {
        // Sets up singleton logic.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes.
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances.
        }
    }

    #endregion

    #region OnEnable Functions

    private void SetupBaseFolder()
    {
#if UNITY_EDITOR
        // In editor assets folder.
        _baseFolderPath = Application.dataPath;
#else
        // Get the folder containing the executable.
        _baseFolderPath = Path.GetDirectoryName(Application.dataPath);
#endif
    }

    private string SetupFolder(string name)
    {
        string folder = Path.Combine(_baseFolderPath, name);

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        return folder;
    }

    #endregion
}