using UnityEngine;

public class Interactive : MonoBehaviour
{
    protected bool wasInRange = false;
    protected bool playerIsInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = false;
        }
    }
}
