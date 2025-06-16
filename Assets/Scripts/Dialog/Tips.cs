using UnityEngine;

public class Tips : Interactive
{
    public string tipsContext;

    [Header("Broadcast")]
    public TextEventSO textEventSO;


    private void Update()
    {
        if(playerIsInRange != wasInRange)
        {
            if (playerIsInRange)
            {
                textEventSO.RaiseTextEvent(tipsContext);
            }
            else
            {
                textEventSO.RaiseTextEvent(""); // Clear the text when not in range
            }
        }
        wasInRange = playerIsInRange;
    }

}
