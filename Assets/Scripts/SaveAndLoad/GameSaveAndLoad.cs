using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class ItemSaveData
{
    // ��Ҫ�־û����ֶΣ����ų�itemImage��
    public int itemHeld;
    [TextArea] public string itemInfo;
    public bool Equip;
}

// ���л���װ��
[System.Serializable]
public class ItemDatabase
{
    [System.Serializable]public class SerializedItem

    {
        public string key;       // itemName
        public ItemSaveData value; // ��Ӧ����
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
        //����SaveData�ļ���
        if (!Directory.Exists(saveDataPath))
        {
            Directory.CreateDirectory(saveDataPath);
        }
        //����GameData�ļ���
        if (!Directory.Exists(gameDataPath))
        {
            Directory.CreateDirectory(gameDataPath);
        }
        //��������
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
        //����Ϸ��ʼʱ��������б�������
        if (File.Exists(myBagPath)) { File.Delete(myBagPath); }
        if (File.Exists(scenePath)) { File.Delete(scenePath); }
        if (File.Exists(posPath)) { File.Delete(posPath); }
        if (Directory.Exists(saveDataPath))
        {
            string[] files = Directory.GetFiles(saveDataPath, ".", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log($"��ɾ���ļ�: {file}");
            }
        }
        if (Directory.Exists(gameDataPath))
        {
            string[] files = Directory.GetFiles(gameDataPath, ".", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log($"��ɾ���ļ�: {file}");
            }
        }

        //�½�saveData����ĶԻ��ļ�
        TextAsset[] textAssets = Main.GetAllPlotText();
        foreach(TextAsset file in textAssets)
        {
            File.WriteAllText(Path.Combine(saveDataPath, file.name + ".csv"), file.text);
        }

        //�½��յ�myBag.json
        ItemDatabase db = new ItemDatabase();
        string json = JsonUtility.ToJson(db, true);
        File.WriteAllText(myBagPath, json);

        //�½��µĿ�ʰȡ����״̬��
        var itemInSceneList = Main.GetItemInSceneList();
        int cnt = itemInSceneList.Count;
        Debug.Log("��������Ϊ" + cnt);
        
        wrapper.boolList = new List<bool>(cnt);
        for(int i = 0; i < cnt; i++)
        {
            wrapper.boolList.Add(true);
        }
        itemBoolsList = wrapper.boolList;
        json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(itemInScenePaht, json);
        Debug.Log(json);


        //��ȡ
        LoadGame();

        //�����ʼ״̬
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

    //����ͼ��ط���
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
        //���ص���״̬
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


    //���ڶԻ��ļ��ͱ����ļ��ı���ͼ���
    public void SaveGame()
    {
        //ת��gameDataPath�����csv�ļ���saveDataPath����
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



        //���������ݱ��浽JSON�ļ�
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

        //�������״̬
        wrapper.boolList = itemBoolsList;
        json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(itemInScenePaht, json);
    }

    public void LoadGame()
    {
        //ת��saveDataPath�����csv�ļ���gameDataPath����
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

        //���ر�������
        if (!File.Exists(myBagPath)) return;
        string json = File.ReadAllText(myBagPath);
        ItemDatabase db = JsonUtility.FromJson<ItemDatabase>(json);

        // ת��Ϊ�ֵ���ڲ���
        Dictionary<string, ItemSaveData> itemDict = db.items.ToDictionary(x => x.key, x => x.value);

        foreach (var item in itemDict)
        {
            Debug.Log($"Item: {item.Key}, Held: {item.Value.itemHeld}, Info: {item.Value.itemInfo}, Equip: {item.Value.Equip}");
        }

        //��ձ����е���Ʒ
        myBag.itemList.Clear();

        foreach(var item in itemDict)
        {
            // �����µ�Itemʵ��
            Item missionItem = Main.GetMissionItem(item.Key);
            missionItem.itemHeld = item.Value.itemHeld;
            // ��ӵ�������
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
        textEventSO.RaiseTextEvent("��Ϸ����ɹ�����R����ȡ");
        yield return new WaitForSeconds(2f);
        textEventSO.RaiseTextEvent(""); // �����ʾ�ı�
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
