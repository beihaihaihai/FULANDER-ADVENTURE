using UnityEngine;


//----��������ɫ�Ų��Ƿ�Ӵ����棬Ȼ����ĸ������isGround����----
public class PlayerFeet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMove playerMove = GetComponentInParent<PlayerMove>();
        if (playerMove != null)
        {
            //Debug.Log(other.tag);
            if(other.gameObject.tag == "Ground")
            {
                playerMove.HandleFeetEvent(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMove playerMove = GetComponentInParent<PlayerMove>();
        if (playerMove != null)
        {
            //Debug.Log(other.tag);
            if (other.gameObject.tag == "Ground")
            {
                playerMove.HandleFeetEvent(false);
            }
        }
    }


}
