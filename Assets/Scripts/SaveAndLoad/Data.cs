using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Data
{
    private string scenePath = Path.Combine(Application.persistentDataPath, "savedScene.json");
    private string posPath = Path.Combine(Application.persistentDataPath, "CharacterPos.json");

    public Dictionary<string, Vector3> characterPosDict = new Dictionary<string, Vector3>();
    public Dictionary<string, bool> characterExistDict = new Dictionary<string, bool>();
    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();

    //public string sceneToSave;

    //Data类的可序列化场景子类
    public class GameSceneData
    {
        public string sceneAssetGUID;
        public SceneType sceneType;
        public GameSceneData(GameSceneSO scene)
        {
            sceneType = scene.sceneType;
            sceneAssetGUID = scene.sceneReference.AssetGUID;
        }
    }

    // SerializableVector3类用于序列化Vector3
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class PositionEntry
    {
        public string key;
        public bool exsit;
        public SerializableVector3 value;
    }

    [System.Serializable]
    public class PositionDataWrapper
    {
        public PositionEntry[] entries;
    }

    public void SavePosition()
    {
        PositionDataWrapper wrapper = new PositionDataWrapper();
        wrapper.entries = characterPosDict.Select(pair => new PositionEntry
        {
            key = pair.Key,
            value = new SerializableVector3(pair.Value)
        }).ToArray();
        // 添加exsit信息
        foreach (var entry in wrapper.entries)
        {
            entry.exsit = characterExistDict.ContainsKey(entry.key) && characterExistDict[entry.key];
        }

        string json = JsonUtility.ToJson(wrapper, true);

        Debug.Log(json);
        File.WriteAllText(posPath, json);

    }

    public void LoadPosition()
    {
        if (!File.Exists(posPath))
        {
            Debug.LogWarning("存档文件不存在");
            return;
        }
        string json = File.ReadAllText(posPath);

        PositionDataWrapper wrapper = JsonUtility.FromJson<PositionDataWrapper>(json);

        characterPosDict.Clear();
        foreach (var entry in wrapper.entries)
        {
            characterExistDict[entry.key] = entry.exsit;
            characterPosDict[entry.key] = entry.value.ToVector3();
        }

    }


    //保存游戏场景数据到JSON文件
    public void SaveGameScene(GameSceneSO savedScene)
    {
        GameSceneData sceneData = new GameSceneData(savedScene);
        string json = JsonUtility.ToJson(sceneData, true);
        File.WriteAllText(scenePath, json);

    }

    // 获取保存的游戏场景数据
    public GameSceneSO GetSavedScene()
    {

        if (!File.Exists(scenePath))
        {
            Debug.LogWarning($"Save file not found at: {scenePath}");
            return null;
        }

        string json = File.ReadAllText(scenePath);
        GameSceneData data = JsonUtility.FromJson<GameSceneData>(json);

        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        newScene.sceneType = data.sceneType;
        newScene.sceneReference = new AssetReference(data.sceneAssetGUID);

        return newScene;
    }
    

} 
