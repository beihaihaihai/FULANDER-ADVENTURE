using UnityEngine;
using UnityEngine.Events;



[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    public void RaiseLoadRequestEvent(GameSceneSO location, Vector3 pos, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(location, pos, fadeScreen);
    }
}