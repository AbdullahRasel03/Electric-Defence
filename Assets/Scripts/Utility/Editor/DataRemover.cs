using System.IO;
using UnityEditor;
using UnityEngine;
public class DataRemover : MonoBehaviour
{

    [MenuItem("Utility/Clear All Data")]
    public static void DeleteFiles()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        PlayerPrefs.DeleteAll();
    }


}
