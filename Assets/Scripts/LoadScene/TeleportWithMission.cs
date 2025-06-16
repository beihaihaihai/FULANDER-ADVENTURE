using UnityEngine;

public class TeleportWithMission : Interactive
{
    public TextEventSO textEventSO;
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToLoad;
    public Vector3 spawnPosition;

    [Header("关于任务判定的配置")]
    public Inventory myBag;
    public string missionItemName;

    private void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.W) && CheckMisssion(missionItemName))
        {
            
            TeleportToScene();
        }
    }

    public void TeleportToScene()
    {
        Debug.Log("teleport!!");
        if (loadEventSO != null)
        {
            textEventSO.RaiseTextEvent("");
            loadEventSO.RaiseLoadRequestEvent(sceneToLoad, spawnPosition, true);
        }
        else
        {
            Debug.LogWarning("LoadEventSO is not assigned in the Teleport script.");
        }
    }

    private bool CheckMisssion(string mission)
    {
        string[] cell = mission.Split('|');
        int num = int.Parse(cell[0]);
        string item = cell[1];

        //检查背包
        foreach (Item _item in myBag.itemList)
        {
            if (_item.itemName == item && _item.itemHeld >= num)
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
}
