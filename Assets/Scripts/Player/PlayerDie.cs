using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [Header("Broadcast")]
    public VoidEventSO loadGameEvent;


    //���������ж���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ����������״̬���
        if (!this.enabled) return;

        GameObject other = collision.gameObject;
        if(other.tag == "DiedLine")
        {
            Debug.Log("�����������ȡ����浵");
            Debug.Log(this.gameObject);
            //�������ȡ����浵
            loadGameEvent.RaiseEvent();
        }
    }
}
