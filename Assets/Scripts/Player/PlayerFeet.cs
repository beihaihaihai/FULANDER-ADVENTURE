using UnityEngine;


//----用来检测角色脚部是否接触地面，然后更改父对象的isGround属性----
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
