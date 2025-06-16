using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [Header("Broadcast")]
    public VoidEventSO loadGameEvent;


    //遇到死亡判断线
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 添加组件启用状态检查
        if (!this.enabled) return;

        GameObject other = collision.gameObject;
        if(other.tag == "DiedLine")
        {
            Debug.Log("玩家死亡，读取最近存档");
            Debug.Log(this.gameObject);
            //死亡后读取最近存档
            loadGameEvent.RaiseEvent();
        }
    }
}
