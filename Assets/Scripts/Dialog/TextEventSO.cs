using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Event/TextEventSO")]
public class TextEventSO : ScriptableObject
{
    public UnityAction<string> TextEvent;

    public void RaiseTextEvent(string text)
    {
        TextEvent?.Invoke(text);
    }
}
