using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    public Image slotImage;
    public TMP_Text slotNum;

    [Header("Double Click Settings")]
    public float doubleClickTimeThreshold = 0.3f; // 双击判定时间阈值（秒）
    private Coroutine _doubleClickCoroutine;

    public void ItemOnClick()
    {
        Main.UpdateItemInfo(slotItem.itemInfo);

        //如果是可装备道具，双击装备
        if (slotItem.Equip)
        {
            if(_doubleClickCoroutine == null)
            {
                _doubleClickCoroutine = StartCoroutine(DoubleClickListener());
            }
            else
            {
                StopCoroutine(_doubleClickCoroutine);
                _doubleClickCoroutine = null;
                Main.EquipItem(slotItem);
            }
        }
    }
    private IEnumerator DoubleClickListener()
    {
        yield return new WaitForSeconds(doubleClickTimeThreshold);
        _doubleClickCoroutine = null;
    }

}
