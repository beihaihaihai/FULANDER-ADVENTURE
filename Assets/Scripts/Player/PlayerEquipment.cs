using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquipment : MonoBehaviour
{
    public Item currentItem;

    private Rigidbody2D rb;
    private PlayerMove playerMove;
    private PlayerDie playDie;
    private bool canUseEquipment; //�Ƿ����ʹ�õ��� 
    
    //ѻ����
    public float Force = 8f; //��������

    //Ԫ���ܲ�
    private float OriginalSpeed; //ԭʼ�ƶ��ٶ�
    public float FastTime = 10f; //����ʱ��
    public float FastSpeed = 6.5f; //���ٺ���ٶ�

    //������ż
    public float duanWuTime = 5f; //������ż����ʱ��
    public GameObject duanWuDoll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMove = GetComponent<PlayerMove>();
        playDie = GetComponent<PlayerDie>();
        OriginalSpeed = playerMove.moveSpeed; //��¼ԭʼ�ƶ��ٶ�
    }

    private void OnEnable()
    {
        canUseEquipment = true;
        playerMove.moveSpeed = OriginalSpeed; //�����ƶ��ٶ�
        if(!playDie.gameObject.activeSelf)
        {
            playDie.gameObject.SetActive(true);
        }
        duanWuDoll.SetActive(false); //ȷ��������ż��ʼ״̬Ϊ������
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
        //switch-case����д�����߼�
        switch (itemName)
        {
            case "ѻ����":
                Debug.Log("������Ծ���ߣ�Yayushan");
                StartCoroutine(Yayushan());
                break;
            case "�ܲ�Ԫ��":
                Debug.Log("�������ߣ�CarrotYuanXiao");
                StartCoroutine(CarrotYuanXiao());
                break;
            case "������ż":
                Debug.Log("�������ߣ�������ż");
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

        yield return new WaitForSeconds(0.5f); //0.5����ȴ
        yield return new WaitUntil(() => playerMove.GetIsGround);

        canUseEquipment = true;
    }

    private IEnumerator CarrotYuanXiao()
    {
        canUseEquipment = false;

        playerMove.moveSpeed = FastSpeed; //����

        yield return new WaitForSeconds(FastTime); //��������ʱ��

        playerMove.moveSpeed = OriginalSpeed;
        
        yield return new WaitForSeconds(10f); //10����ȴ

        canUseEquipment = true;
    }

    private IEnumerator DuanWuDoll()
    {
        canUseEquipment = false;

        duanWuDoll.SetActive(true); //���������ż
        playDie.enabled = false;

        yield return new WaitForSeconds(duanWuTime); //����ʱ��

        playDie.enabled = true;
        duanWuDoll.SetActive(false); //�رն�����ż

        yield return new WaitForSeconds(30f); //30����ȴ
        
        canUseEquipment = true;
    }
}
