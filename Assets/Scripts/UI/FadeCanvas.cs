using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;

    [Header("EventListener")]
    public FadeEventSO fadeEvent;

    private void OnEnable()
    {
        fadeEvent.OnEventRaise += OnFadeEvent;
    }
    private void OnDisable()
    {
        fadeEvent.OnEventRaise -= OnFadeEvent;
    }

    private void OnFadeEvent(Color target, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
