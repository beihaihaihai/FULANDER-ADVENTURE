using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private enum AnimState
    {
        idle,
        run,
        jump,
        fall,
        dash
    }
    private AnimState state;
    private Animator anim;
    private PlayerMove playerMove;
    private PlayerJump playerJump;
    private PlayerDash playerDash;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
        playerJump = GetComponent<PlayerJump>();
        playerDash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        if (playerMove.moveControl != 0)
        {
            state = AnimState.run;
        }
        else
        {
            state = AnimState.idle;
        }

        if (playerJump.rb.linearVelocity.y > 0.1f && !playerMove.GetIsGround)
        {
            state = AnimState.jump;
        }
        if (playerJump.rb.linearVelocity.y < -0.1f && !playerMove.GetIsGround)
        {
            state = AnimState.fall;
        }
        if (playerDash.GetIsDash)
        {
            state = AnimState.dash;
        }

        anim.SetInteger("state", (int)state);
    }
}
