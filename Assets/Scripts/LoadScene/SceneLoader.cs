using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable
{
    public float fadeDuration = 1f; // Fade duration in seconds
    public Transform playerTrans;
    public Transform cameraTrans;
    public TMP_Text LoadingMes;
    public GameObject itemSet;
    public float offset;
    public GameObject TextLabel;

    //position
    public Vector3 playerFirstPosition;
    public Vector3 playerMenuPositon;

    [Header("�¼�����")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO NewGameEvent; 

    [Header("broadcast")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public TextEventSO textEventSO;


    [Header("Scenes")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;

    private GameSceneSO currentScene;
    private GameSceneSO sceneToLoad;
    private Vector3 po0;
    private bool fs0;

    private bool isLoading = false;
    [SerializeField]private int itemStartIndex;
    [SerializeField]private int itemEndIndex;
    [SerializeField]private string sceneName;
    private List<bool> itemBoolsList;

    private void Start()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, playerMenuPositon, true);
    }

    private void Update()
    {
        if(Keyboard.current.bKey.wasPressedThisFrame && currentScene != null && currentScene.sceneType != SceneType.Menu)
        {
            //�����ǰ�������ǲ˵��������򷵻ز˵�
            sceneToLoad = menuScene;
            loadEventSO.RaiseLoadRequestEvent(sceneToLoad, playerMenuPositon, true);
        }
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequest;
        NewGameEvent.OnEventRaised += NewGame;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequest;
        NewGameEvent.OnEventRaised -= NewGame;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, playerFirstPosition, true);
    }

    private void OnLoadRequest(GameSceneSO location, Vector3 pos, bool fadeScreen)
    {
        DestoryAllItems();
        if (TextLabel.activeInHierarchy)
        {
            TextLabel.SetActive(false);
        }

        if (isLoading)
        {
            return;
        }
        isLoading = true;

        sceneToLoad = location;
        po0 = pos;
        fs0 = fadeScreen;

        if (currentScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            //���û�е�ǰ������ֱ�Ӽ����³���
            StartCoroutine(LoadNewScene());
        }
    }


    //��������Э��
    private IEnumerator UnloadPreviousScene()
    {
        if(fs0 == true)
        {
            fadeEvent.FadeIn(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);

        //ж�ص�ǰ����
        yield return currentScene.sceneReference.UnLoadScene();

        //�ر�����
        playerTrans.gameObject.SetActive(false);

        //���¼����³���
        StartCoroutine(LoadNewScene());
    }

    private IEnumerator LoadNewScene()
    {
        //��Ů����
        StartCoroutine(DelayedFade());

        //�ƶ����ﵽָ��λ�ú��ټ����³���
        playerTrans.position = po0;

        if(sceneToLoad.sceneType == SceneType.Menu)
        {
            //nothing
        }
        else if(sceneToLoad.sceneType == SceneType.Location)
        {
            yield return new WaitUntil(() => playerTrans.position == po0 
                                        && cameraTrans.position.x >= po0.x - offset
                                        && cameraTrans.position.x <= po0.x + offset
                                        && cameraTrans.position.y >= po0.y - offset
                                        && cameraTrans.position.y <= po0.y + offset
                                        );
        }

        sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += (handle) =>
        {   
            currentScene = sceneToLoad;
            playerTrans.gameObject.SetActive(true);

            //���µ���״̬
            UpdateSceneIndex();

            var itemInSceneList = Main.GetItemInSceneList();
            itemBoolsList = GameSaveAndLoad.instance.GetItemBoolsList();
            for (int i = itemStartIndex; i < itemEndIndex; i++)
            {
                if (!itemBoolsList[i])
                {
                    continue;
                }
                else
                {
                    GameObject itemPrefab = Instantiate(itemInSceneList[i].itemPrefab, itemSet.transform);
                    itemPrefab.transform.position = itemInSceneList[i].itemPos;

                    ItemOnWorld itemOnWorld = itemPrefab.GetComponent<ItemOnWorld>();
                    itemOnWorld.UpdateIndex(itemInSceneList[i].index);
                }
            }

            isLoading = false;
            afterSceneLoadedEvent.RaiseEvent();
            StartCoroutine(NewSceneTips());
        };

        
    }

    private IEnumerator DelayedFade()
    {
        LoadingMes.gameObject.SetActive(true);
        yield return new WaitUntil(() => isLoading == false);
        yield return new WaitForSeconds(1f);
        LoadingMes.gameObject.SetActive(false);
        if (fs0)
        {
            fadeEvent.FadeOut(fadeDuration);
        }
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        data.SavePosition();

        data.SaveGameScene(currentScene);
    }

    public void LoadData(Data data)
    {
        data.LoadPosition();

        var playerID = playerTrans.GetComponent<DataDefinition>().ID;
        if(data.characterPosDict.ContainsKey(playerID))
        {
            po0 = data.characterPosDict[playerID];
            sceneToLoad = data.GetSavedScene();

            loadEventSO.RaiseLoadRequestEvent(sceneToLoad, po0, true);
        }
    }

    public void SetDataType(SaveType saveType)
    {
        GetDataID().SaveType = saveType;
    }

    private void UpdateSceneIndex()
    {
        var obj = GameObject.FindGameObjectWithTag("SceneIndex");
        if (obj == null)
        {
            Debug.Log("�Ҳ���SceneIndex");
            return;
        }
        sceneName = obj.GetComponent<SceneIndex>().sceneName;
        itemStartIndex = obj.GetComponent<SceneIndex>().itemStartIndex;
        itemEndIndex = obj.GetComponent<SceneIndex>().itemEndIndex;
    }

    private void DestoryAllItems()
    {
        Debug.Log("������������Ʒ");
        // ���ռ������Ӷ���
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in itemSet.transform)
        {
            children.Add(child.gameObject);
        }

        // �����������Ӷ���
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }

    private IEnumerator NewSceneTips()
    {
        Debug.Log("��ӡ����������");
        textEventSO.RaiseTextEvent(sceneName);
        yield return new WaitForSeconds(5f);
        textEventSO.RaiseTextEvent(""); // �����ʾ�ı�
    }
}
