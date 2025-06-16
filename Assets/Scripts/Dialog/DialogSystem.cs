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

    //���Ի���
    public static void OpenChatBox()
    {
        if (instance.ChatBox != null && !instance.ChatBox.activeSelf) // �����ظ���
        {
            instance.ChatBox.SetActive(true);
        }
    }

    //���ݶԻ��˻�ȡtextFile
    public static void LoadText(string name)
    {
        Debug.Log("����loadText" + name);

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
        //������textFile�����Ի�
        if (textFile != null && workCoroutine == null && ChatBox.activeSelf)
        {
            Debug.Log("��ʼ��ȡ���������");
            index = 0;
            GetTextFromFile(textFile);
            workCoroutine = StartCoroutine(ShowText());
        }
    }

    //�����º��CSV�Ի��ļ��洢������
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


    //��ȡcsv����ÿ�зָ�
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

    //���csv�ļ�����ҪЭ�̣� ----����Ҫ�����ع��޸�
    IEnumerator ShowText()
    {
        //����Ƿ�������
        if(missionIndex != -1)
        {
            string mission = textList[missionIndex].Split(',')[7]; 
            if (CheckMisssion(mission))
            {
                ModifyFlag(missionIndex, "START");
                ModifyFlag(startIndex, "CONTINUE");
                missionIndex = -1;
                Debug.Log("�������");
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
                    Debug.Log("��ȡ����");
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

    //����ÿ�仰������
    private void UpdateSprite(string name, string pos)
    {
        if(pos == "��")
        {
            SpriteLeft.sprite = Main.GetSprite(name);
        }
        else if(pos == "��")
        {
            SpriteRight.sprite = Main.GetSprite(name);
        }
    }

    //����ÿ�仰�����ݺ�˵��������
    private void UpdateText(string name, string text)
    {
        //Debug.Log("text����");
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

        //��鱳��
        foreach(Item _item in myBag.itemList)
        {
            if(_item.itemName == item && _item.itemHeld >= num)
            {
                if (_item.itemHeld == num)
                {
                    myBag.itemList.Remove(_item); //�Ƴ�������Ʒ
                }
                else
                {
                    _item.itemHeld -= num; //����������Ʒ����
                }

                Main.RefreshItem();

                return true;
            }
        }

        return false;
    }

    //��ȡ������
    private void GetMissionItem(string missionItem)
    {
        string[] cell = missionItem.Split('|');
        int num = int.Parse(cell[0]);
        string itemName = cell[1];

        Item item = Main.GetMissionItem(itemName);


        if (item == null)
        {
            Debug.LogError("������Ʒ������");
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
