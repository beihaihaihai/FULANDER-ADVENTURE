using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    public Item currentItem;

    private Rigidbody2D rb;
    private PlayerMove playerMove;
    private PlayerDie playDie;
    private bool canUseEquipment; //是否可以使用道具 
    
    //鸦羽扇
    public float Force = 8f; //上升力度

    //元宵萝卜
    private float OriginalSpeed; //原始移动速度
    public float FastTime = 10f; //加速时间
    public float FastSpeed = 6.5f; //加速后的速度

    //端午人偶
    public float duanWuTime = 5f; //端午人偶持续时间
    public GameObject duanWuDoll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMove>();
        playDie = GetComponent<PlayerDie>();
        OriginalSpeed = playerMove.moveSpeed; //记录原始移动速度
    }

    private void OnEnable()
    {
        canUseEquipment = true;
        playerMove.moveSpeed = OriginalSpeed; //重置移动速度
        if(!playDie.gameObject.activeSelf)
        {
            playDie.gameObject.SetActive(true);
        }
        duanWuDoll.SetActive(false); //确保端午人偶初始状态为不激活
        playDie.enabled = true;
    }

    private void Update()
    {
        currentItem = Main.GetEquipment();
        if(currentItem != null && Keyboard.current.jKey.wasPressedThisFrame && canUseEquipment)
        {
            UseEquipment(currentItem.itemName);
        }
    }

    private void UseEquipment(string itemName)
    {
        //switch-case来编写道具逻辑
        switch (itemName)
        {
            case "鸦羽扇":
                Debug.Log("启动跳跃道具：Yayushan");
                StartCoroutine(Yayushan());
                break;
            case "萝卜元宵":
                Debug.Log("启动道具：CarrotYuanXiao");
                StartCoroutine(CarrotYuanXiao());
                break;
            case "端午人偶":
                Debug.Log("启动道具：端午人偶");
                StartCoroutine(DuanWuDoll());
                break;
            default:
                break;
        }
    }



    private IEnumerator Yayushan()
    {
        canUseEquipment = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Force);

        yield return new WaitForSeconds(0.5f); //0.5秒冷却
        yield return new WaitUntil(() => playerMove.GetIsGround);

        canUseEquipment = true;
    }

    private IEnumerator CarrotYuanXiao()
    {
        canUseEquipment = false;

        playerMove.moveSpeed = FastSpeed; //加速

        yield return new WaitForSeconds(FastTime); //持续加速时间

        playerMove.moveSpeed = OriginalSpeed;
        
        yield return new WaitForSeconds(10f); //10秒冷却

        canUseEquipment = true;
    }

    private IEnumerator DuanWuDoll()
    {
        canUseEquipment = false;

        duanWuDoll.SetActive(true); //激活端午人偶
        playDie.enabled = false;

        yield return new WaitForSeconds(duanWuTime); //持续时间

        playDie.enabled = true;
        duanWuDoll.SetActive(false); //关闭端午人偶

        yield return new WaitForSeconds(30f); //30秒冷却
        
        canUseEquipment = true;
    }
}
