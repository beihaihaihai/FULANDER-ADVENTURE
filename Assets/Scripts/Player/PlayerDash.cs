using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    [SerializeField]private bool canDash;
    private bool isDashing;
    private float dashDirection;
    private Rigidbody2D rb;
    private PlayerMove playerMove;

    public float dashSpeed = 5f;
    public float dashTime = 0.5f;

    public bool GetIsDash
    {
        get { return isDashing; }
        set { isDashing = value; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMove>();
    }

    private void OnEnable()
    {
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        dashDirection = transform.localScale.x < 0 ? 1f : -1f;


        // Dash
        if (Input.GetKeyDown(KeyCode.K) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalScale = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        isDashing = false;

        rb.gravityScale = originalScale;

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => playerMove.GetIsGround);

        canDash = true;
    }
}
