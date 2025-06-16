using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class ItemSaveData
{
    // 需要持久化的字段（已排除itemImage）
    public int itemHeld;
    [TextArea] public string itemInfo;
    public bool Equip;
}

// 序列化包装器
[System.Serializable]
public class ItemDatabase
{
    [System.Serializable]public class SerializedItem

    {
        public string key;       // itemName
        public ItemSaveData value; // 对应数据
    }

    public List<SerializedItem> items = new List<SerializedItem>();
}

[System.Serializable]
public class BoolListWrapper
{
    public List<bool> boolList;
}


[DefaultExecutionOrder(-100)]
public class GameSaveAndLoad : MonoBehaviour
{
    public Inventory myBag;
    private string gameDataPath => Path.Combine(Application.persistentDataPath, "GameData");
    private string saveDataPath => Path.Combine(Application.persistentDataPath, "SaveData");
    private string myBagPath => Path.Combine(Application.persistentDataPath, "myBag.json");
    private string scenePath => Path.Combine(Application.persistentDataPath, "savedScene.json");
    private string posPath => Path.Combine(Application.persistentDataPath, "CharacterPos.json");
    private string itemInScenePaht => Path.Combine(Application.persistentDataPath, "itemInScene.json");

    public static GameSaveAndLoad instance;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    private BoolListWrapper wrapper = new BoolListWrapper();
    public List<bool> itemBoolsList;

    [Header("EventListener")]
    public VoidEventSO saveGameEvent;
    public VoidEventSO loadGameEvent;
    public VoidEventSO NewGameEvent;

    [Header("Broadcast")]
    public TextEventSO textEventSO;

    private void Awake()
    {
        //创建SaveData文件夹
        if (!Directory.Exists(saveDataPath))
        {
            Directory.CreateDirectory(saveDataPath);
        }
        //创建GameData文件夹
        if (!Directory.Exists(gameDataPath))
        {
            Directory.CreateDirectory(gameDataPath);
        }
        //创建单例
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }

        saveData = new Data();
    }

    private void OnEnable()
    {
        saveGameEvent.OnEventRaised += Save;
        loadGameEvent.OnEventRaised += Load;
        NewGameEvent.OnEventRaised += NewGame;
    }

    private void OnDisable()
    {
        saveGameEvent.OnEventRaised -= Save;
        loadGameEvent.OnEventRaised -= Load;
        NewGameEvent.OnEventRaised -= NewGame;
    }

    private void NewGame()
    {
        //新游戏开始时，清空所有保存数据
        if (File.Exists(myBagPath)) { File.Delete(myBagPath); }
        if (File.Exists(scenePath)) { File.Delete(scenePath); }
        if (File.Exists(posPath)) { File.Delete(posPath); }
        if (Directory.Exists(saveDataPath))
        {
            string[] files = Directory.GetFiles(saveDataPath, ".", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log($"已删除文件: {file}");
            }
        }
        if (Directory.Exists(gameDataPath))
        {
            string[] files = Directory.GetFiles(gameDataPath, ".", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log($"已删除文件: {file}");
            }
        }

        //新建saveData里面的对话文件
        TextAsset[] textAssets = Main.GetAllPlotText();
        foreach(TextAsset file in textAssets)
        {
            File.WriteAllText(Path.Combine(saveDataPath, file.name + ".csv"), file.text);
        }

        //新建空的myBag.json
        ItemDatabase db = new ItemDatabase();
        string json = JsonUtility.ToJson(db, true);
        File.WriteAllText(myBagPath, json);

        //新建新的可拾取道具状态表
        var itemInSceneList = Main.GetItemInSceneList();
        int cnt = itemInSceneList.Count;
        Debug.Log("道具数量为" + cnt);
        
        wrapper.boolList = new List<bool>(cnt);
        for(int i = 0; i < cnt; i++)
        {
            wrapper.boolList.Add(true);
        }
        itemBoolsList = wrapper.boolList;
        json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(itemInScenePaht, json);
        Debug.Log(json);


        //读取
        LoadGame();

        //保存初始状态
        Save();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Load();
        }
    }

    public void RegisterSaveData(ISaveable saveable)
    {
        if(!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }
    public void UnRegisterSaveData(ISaveable saveable)
    {
        

        saveableList.Remove(saveable);
    }

    //保存和加载方法
    public void Save()
    {
        ISaveable sceneSaveable = null; 
        foreach (var saveable in saveableList)
        {
            if(saveable.GetDataID().SaveType != SaveType.None)
            {
                saveable.GetSaveData(saveData);
            }
            else
            {
                sceneSaveable = saveable;
            }
        }

        if(sceneSaveable != null)
        {
            sceneSaveable.GetSaveData(saveData);
        }

        SaveGame();

        StartCoroutine(SavedTips());
    }
    public void Load()
    {
        //加载道具状态
        if (!File.Exists(itemInScenePaht)) return;
        string json = File.ReadAllText(itemInScenePaht);
        wrapper = JsonUtility.FromJson<BoolListWrapper>(json);
        itemBoolsList = wrapper.boolList;


        ISaveable sceneSaveable = null;
        foreach (var saveable in saveableList)
        {
            if(saveable.GetDataID().SaveType == SaveType.None)
            {
                sceneSaveable = saveable;
            }
        }

        if (sceneSaveable != null)
        {
            sceneSaveable.LoadData(saveData);
        }

        foreach (var saveable in saveableList)
        {
            if(saveable.GetDataID().SaveType != SaveType.None)
            {
                saveable.LoadData(saveData);
            }
        }


        LoadGame();
    }


    //关于对话文件和背包文件的保存和加载
    public void SaveGame()
    {
        //转移gameDataPath里面的csv文件到saveDataPath里面
        if (Directory.Exists(gameDataPath))
        {
            string[] files = Directory.GetFiles(gameDataPath);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(saveDataPath, fileName);
                File.Copy(file, destFile, true);
                Debug.Log($"File {fileName} copied to {saveDataPath}");
            }
        }
        else
        {
            Debug.LogWarning("GameData folder does not exist.");
        }



        //将背包数据保存到JSON文件
        ItemDatabase db = new ItemDatabase();
        foreach(Item item in myBag.itemList)
        {
            Debug.Log(item.itemName);

            db.items.Add(new ItemDatabase.SerializedItem
            {
                key = item.itemName,
                value = new ItemSaveData
                {
                    itemHeld = item.itemHeld,
                    itemInfo = item.itemInfo,
                    Equip = item.Equip
                }
            });
        }
        string json = JsonUtility.ToJson(db, true);

        File.WriteAllText(myBagPath, json);

        //保存道具状态
        wrapper.boolList = itemBoolsList;
        json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(itemInScenePaht, json);
    }

    public void LoadGame()
    {
        //转移saveDataPath里面的csv文件到gameDataPath里面
        if (Directory.Exists(saveDataPath))
        {
            string[] files = Directory.GetFiles(saveDataPath);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(gameDataPath, fileName);
                File.Copy(file, destFile, true);
                Debug.Log($"File {fileName} copied to {gameDataPath}");
            }
        }
        else
        {
            Debug.LogWarning("SaveData folder does not exist.");
        }

        //加载背包数据
        if (!File.Exists(myBagPath)) return;
        string json = File.ReadAllText(myBagPath);
        ItemDatabase db = JsonUtility.FromJson<ItemDatabase>(json);

        // 转换为字典便于查找
        Dictionary<string, ItemSaveData> itemDict = db.items.ToDictionary(x => x.key, x => x.value);

        foreach (var item in itemDict)
        {
            Debug.Log($"Item: {item.Key}, Held: {item.Value.itemHeld}, Info: {item.Value.itemInfo}, Equip: {item.Value.Equip}");
        }

        //清空背包中的物品
        myBag.itemList.Clear();

        foreach(var item in itemDict)
        {
            // 创建新的Item实例
            Item missionItem = Main.GetMissionItem(item.Key);
            missionItem.itemHeld = item.Value.itemHeld;
            // 添加到背包中
            myBag.itemList.Add(missionItem);
        }

        //foreach (Item item in myBag.itemList)
        //{
        //    if (itemDict.TryGetValue(item.itemName, out ItemSaveData data))
        //    {
        //        item.itemHeld = data.itemHeld;
        //        item.itemInfo = data.itemInfo ?? "";
        //        item.Equip = data.Equip;
        //    }
        //}
    }

    private IEnumerator SavedTips()
    {
        textEventSO.RaiseTextEvent("游戏保存成功！按R来读取");
        yield return new WaitForSeconds(2f);
        textEventSO.RaiseTextEvent(""); // 清除提示文本
    }

    public void UpdateitemBoolsList(int idx)
    {
        itemBoolsList[idx] = false;
    }

    public List<bool> GetItemBoolsList()
    {
        return itemBoolsList;
    }
}
