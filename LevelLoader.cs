using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    void Awake() => Instance = this;

    public LevelData LoadLevel(int levelId)
    {
        string fileName = $"level_{levelId:D2}.json";
        string path = Path.Combine(Application.streamingAssetsPath, "Levels", fileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"Không tìm thấy file: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<LevelData>(json);
    }
}