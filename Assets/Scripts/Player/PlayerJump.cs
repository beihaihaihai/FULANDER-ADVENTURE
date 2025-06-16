using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public Rigidbody2D rb;
    private PlayerMove playerMove;
    private PlayerDash playerDash;
    private bool isJumping;
    private float jumpAddControllor;
    private bool canDoubleJump;

    public float jumpForce = 5f;
    public float jumpAddTime = 1f;


    public bool GetIsJump
    {
        get { return isJumping; }
        set {isJumping = value; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerDash = GetComponent<PlayerDash>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDash.GetIsDash)
        {
            return;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && playerMove.GetIsGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpAddControllor = 0f;
            canDoubleJump = true;
        }


        if (Input.GetButtonDown("Jump") && canDoubleJump && !playerMove.GetIsGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpAddControllor = 0f;
            canDoubleJump = false;
        }



        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if(isJumping)
        {
            if(jumpAddControllor < jumpAddTime)
            {
                rb.linearVelocity += new Vector2(0, -Physics2D.gravity.y * Time.deltaTime);
            }
            else
            {
                isJumping = false;
            }
            jumpAddControllor += Time.deltaTime;
        }



        if (!isJumping)
        {
            rb.linearVelocity -= new Vector2(0, -Physics2D.gravity.y * Time.deltaTime);
        }
    }
}
