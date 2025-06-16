using UnityEngine;

public class CanTalk : Interactive
{

    [SerializeField]public string plotName;

    // Update is called once per frame
    void Update()
    {
        if(playerIsInRange && Input.GetKeyDown(KeyCode.W))
        {
            DialogSystem.LoadText(plotName);
            DialogSystem.OpenChatBox();
        }
    }

}
