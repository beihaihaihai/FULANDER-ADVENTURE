using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Broadcast")]
    public VoidEventSO SaveGameEvent;


    private bool playerIsInRange = false;
    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("±£¥Ê”Œœ∑");
            SaveGameEvent.RaiseEvent();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered the save point range.");
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("out point range");
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = false;
        }
    }

}
