using UnityEngine;

public class Teleport : MonoBehaviour
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToLoad;
    public Vector3 spawnPosition;

    public void TeleportToScene()
    {
        Debug.Log("teleport!!");
        if (loadEventSO != null)
        {
            loadEventSO.RaiseLoadRequestEvent(sceneToLoad, spawnPosition, true);
        }
        else
        {
            Debug.LogWarning("LoadEventSO is not assigned in the Teleport script.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TeleportToScene();
        }
    }
}
