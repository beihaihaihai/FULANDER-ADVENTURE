using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGround;
    private PlayerDash playerDash;
    public float moveControl;

    public float moveSpeed = 5f;

    public bool GetIsGround
    {
        get { return isGround; }
        set { isGround = value; }
    }

    public Vector3 pos
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDash = GetComponent<PlayerDash>();

    }

    private void Update()
    {
        if (playerDash.GetIsDash)
        {
            return;
        }

        // Move
        moveControl = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveControl * moveSpeed, rb.linearVelocity.y);

        // Flip��ɫ����
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }


    //��������ɫ�Ų��Ƿ�Ӵ����棬Ȼ����ĸ������isGround����
    public void HandleFeetEvent(bool b)
    {
        isGround = b;
        //Debug.Log("isGround: " + isGround);
    }
}

