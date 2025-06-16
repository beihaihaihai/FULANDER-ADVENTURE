using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textLabel;
    public TMP_Text nameLabel;
    public SpriteRenderer SpriteLeft;
    public SpriteRenderer SpriteRight;

    [Header("textfile")]
    public GameObject ChatBox;
    public TextAsset textFile = null;

    [Header("Mybag")]
    public Inventory myBag;

    private int index;
    private List<string> textList;
    private Coroutine workCoroutine = null;
    private string CSVFileName;
    [SerializeField]private int startIndex = 0;
    [SerializeField] private int missionIndex = -1;

    static DialogSystem instance;

    //开对话框
    public static void OpenChatBox()
    {
        if (instance.ChatBox != null && !instance.ChatBox.activeSelf) // 避免重复打开
        {
            instance.ChatBox.SetActive(true);
        }
    }

    //根据对话人获取textFile
    public static void LoadText(string name)
    {
        Debug.Log("尝试loadText" + name);

        instance.textFile = Main.GetPlotText(name);
        string tempCSVFileName = name + ".csv";
        if(tempCSVFileName != instance.CSVFileName)
        {
            instance.missionIndex = -1;
        }
        instance.CSVFileName = name + ".csv";
    }

  
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        index = 0;
        textList = new List<string>();
        ChatBox.SetActive(false);
    }
    private void Update()
    {
        //若读到textFile则开启对话
        if (textFile != null && workCoroutine == null && ChatBox.activeSelf)
        {
            Debug.Log("开始读取并输出工作");
            index = 0;
            GetTextFromFile(textFile);
            workCoroutine = StartCoroutine(ShowText());
        }
    }

    //将更新后的CSV对话文件存储到本地
    private void SaveCSVToLocal()
    {
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "GameData")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "GameData"));
        }

        string filePath = Path.Combine(Application.persistentDataPath, "GameData", CSVFileName);
        Debug.Log($"{filePath}");

        string contentToWrite = string.Join("\r\n", textList);
        try
        {
            File.WriteAllText(filePath, contentToWrite);
            Debug.Log($"CSV successfully saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to write CSV to {filePath}\nError: {e.Message}");
        }
    }


    //读取csv，按每行分割
    private void GetTextFromFile(TextAsset file)
    {
        string[] textData;

        textList.Clear();
        string filePath = Path.Combine(Application.persistentDataPath, "GameData", CSVFileName);

        if(File.Exists(filePath))
        {
            string contents = File.ReadAllText(filePath);
            textData = contents.Split("\r\n");
        }
        else
        {
            textData = file.text.Split("\r\n");
        }

        foreach (var line in textData)
        {
            textList.Add(line);
        }
    }

    //解读csv文件的主要协程， ----这里要进行重构修改
    IEnumerator ShowText()
    {
        //检查是否有任务
        if(missionIndex != -1)
        {
            string mission = textList[missionIndex].Split(',')[7]; 
            if (CheckMisssion(mission))
            {
                ModifyFlag(missionIndex, "START");
                ModifyFlag(startIndex, "CONTINUE");
                missionIndex = -1;
                Debug.Log("任务完成");
            }
        }


        for (int i = 1; i < textList.Count; i++)
        {
            string[] cell = textList[i].Split(',');
            Debug.Log(cell[0]);

            if (int.Parse(cell[1]) == index)
            {
                if (cell[0] == "START")
                {
                    startIndex = i;
                    index++;
                }
                else if (cell[0] == "MISSION")
                {
                    Debug.Log("接取任务");
                    missionIndex = i;
                    Debug.Log($"mission = {i}");
                    break;
                }
                else if (cell[0] == "#")
                {
                    UpdateText(cell[2], cell[5]);
                    UpdateSprite(cell[2] + cell[3], cell[4]);
                    index = int.Parse(cell[6]);

                    if (cell[8] != "")
                    {
                        GetMissionItem(cell[8]);
                    }

                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
                    yield return null;
                }
                else if (cell[0] == "END")
                {
                    ModifyFlag(startIndex, "CONTINUE");
                    break;
                }
                else if (cell[0] == "CONTINUE")
                {
                    index = int.Parse(cell[6]);
                }
                else if (cell[0] == "BREAK")
                {
                    break;
                }
            }
        }
        SaveCSVToLocal();
        ClearSprite();
        ChatBox.SetActive(false);
        workCoroutine = null;
    }

    //更新每句话的立绘
    private void UpdateSprite(string name, string pos)
    {
        if(pos == "左")
        {
            SpriteLeft.sprite = Main.GetSprite(name);
        }
        else if(pos == "右")
        {
            SpriteRight.sprite = Main.GetSprite(name);
        }
    }

    //更新每句话的内容和说话人名字
    private void UpdateText(string name, string text)
    {
        //Debug.Log("text更新");
        nameLabel.text = name;
        textLabel.text = text;
    }

    private void ClearSprite()
    {
        SpriteLeft.sprite = null;
        SpriteRight.sprite = null;
    }

    private bool CheckMisssion(string mission)
    {
        string[] cell = mission.Split('|');
        int num = int.Parse(cell[0]);
        string item = cell[1];

        //检查背包
        foreach(Item _item in myBag.itemList)
        {
            if(_item.itemName == item && _item.itemHeld >= num)
            {
                if (_item.itemHeld == num)
                {
                    myBag.itemList.Remove(_item); //移除任务物品
                }
                else
                {
                    _item.itemHeld -= num; //减少任务物品数量
                }

                Main.RefreshItem();

                return true;
            }
        }

        return false;
    }

    //获取任务奖励
    private void GetMissionItem(string missionItem)
    {
        string[] cell = missionItem.Split('|');
        int num = int.Parse(cell[0]);
        string itemName = cell[1];

        Item item = Main.GetMissionItem(itemName);


        if (item == null)
        {
            Debug.LogError("任务物品不存在");
            return;
        }

        if (!myBag.itemList.Contains(item))
        {
            myBag.itemList.Add(item);
            item.itemHeld = num;
        }
        else
        {
            item.itemHeld += num;
        }

        Main.RefreshItem();
    }
    
    private void ModifyFlag(int pos, string Flag)
    {
        string[] parts = textList[pos].Split(',');
        parts[0] = Flag;
        textList[pos] = string.Join(",", parts);
    }

}
