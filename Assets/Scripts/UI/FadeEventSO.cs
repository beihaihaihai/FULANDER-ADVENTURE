using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float, bool> OnEventRaise;

    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black, duration, true);
    }

    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear, duration, false);
    }

    public void RaiseEvent(Color target, float duration, bool fadeIn)
    {
        OnEventRaise?.Invoke(target, duration, fadeIn);
    }
}
