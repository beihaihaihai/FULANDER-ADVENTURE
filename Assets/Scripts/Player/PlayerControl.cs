using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("EventListener")]
    public SceneLoadEventSO loadEvent;
    public VoidEventSO afterSceneLoadedEvent;

    private Rigidbody2D rb;
    private PlayerMove playerMove;
    private PlayerJump playerJump;
    private PlayerDash playerDash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMove>();
        playerJump = GetComponent<PlayerJump>();
        playerDash = GetComponent<PlayerDash>();
    }

    private void OnEnable()
    {
        loadEvent.LoadRequestEvent += OnLoadRequest;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        loadEvent.LoadRequestEvent -= OnLoadRequest;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    private void OnLoadRequest(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        Debug.Log("load.........................");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        playerMove.enabled = false;
        playerJump.enabled = false;
        playerDash.enabled = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        playerMove.enabled = true;
        playerJump.enabled = true;
        playerDash.enabled = true;
    }
}
