using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemInScene
{
    public int index;
    public Vector3 itemPos;
    public GameObject itemPrefab;
}

public class Main : MonoBehaviour
{
    [Header("Items")]
    public List<Item> items;
    public List<string> itemName;
    public List<ItemInScene> itemPosition;

    [Header("plotTexts")]
    public List<TextAsset> plotText;
    public List<string> plotTextName;

    [Header("Sprites")]
    public List<Sprite> sprites;
    public List<string> indexList;

    [Header("配置背包管理")]
    public Inventory myBag;
    public GameObject slotGrid;
    public Slot slotPrefab;
    public TMP_Text itemInformation;
    public Image currentSlot;
    


    static Main instance;
    static Dictionary<string, Sprite> IMAGE_DIC = new Dictionary<string, Sprite>();
    static Dictionary<string, TextAsset> TEXT_DIC = new Dictionary<string, TextAsset>(); 
    static Dictionary<string, Item> ITEM_DIC = new Dictionary<string, Item>();

    [SerializeField]private Item currentEquipItem;

    private void Awake()
    {
        currentEquipItem = null;

        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        //初始化立绘列表
        for (int i = 0; i < sprites.Count; i++)
        {
            IMAGE_DIC.Add(indexList[i], sprites[i]);
        }

        //初始化对话资源
        for (int i = 0; i < plotText.Count; i++)
        {
            TEXT_DIC.Add(plotTextName[i], plotText[i]);
        }
        //初始化道具资源
        for (int i = 0; i < items.Count; i++)
        {
            ITEM_DIC.Add(itemName[i], items[i]);
        }
    }

    public static List<ItemInScene> GetItemInSceneList()
    {
        return instance.itemPosition;
    }

    //获取立绘资源
    public static Sprite GetSprite(string name)
    {
        if (IMAGE_DIC.ContainsKey(name))
        {
            return IMAGE_DIC[name];
        }
        return null;
    }

    //获取对话内容资源
    public static TextAsset GetPlotText(string name)
    {
        if (TEXT_DIC.ContainsKey(name))
        {
            return TEXT_DIC[name];
        }
        return null;
    }
    public static TextAsset[] GetAllPlotText()
    {
        TextAsset[] textAssets = new TextAsset[TEXT_DIC.Count];
        TEXT_DIC.Values.CopyTo(textAssets, 0);
        return textAssets;
    }

    //获取道具资源
    public static Item GetMissionItem(string name)
    {
        if (ITEM_DIC.ContainsKey(name))
        {
            return ITEM_DIC[name];
        }
        return null;
    }

    //------更新背包GUI的显示--------
    //获取到新物品
    public static void CreateNewItem(Item item)
    {
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform);
        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        newItem.slotNum.text = item.itemHeld.ToString();
        //BAG_DIC.Add(item.itemName, newItem);
    }

    //根据myBag里的物品更新背包GUI界面（打开背包或者获取物品时更新）
    public static void RefreshItem()
    {
        //销毁原有显示
        for(int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if(instance.slotGrid.transform.childCount == 0) { break; }
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
        }
        //获取myBag物品并显示
        for(int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            CreateNewItem(instance.myBag.itemList[i]);
        }
    }
    
    //更新物品描述信息
    public static void UpdateItemInfo(string ItemDescription)
    {
        instance.itemInformation.text = ItemDescription;
    }

    //装备可装备的物品
    public static void EquipItem(Item item)
    {
        instance.currentSlot.sprite = item.itemImage;

        Color color = instance.currentSlot.color;
        color.a = 1f;
        instance.currentSlot.color = color;

        instance.currentEquipItem = item;
    }

    //获取装备信息
    public static Item GetEquipment()
    {
        return instance.currentEquipItem;
    }
}
