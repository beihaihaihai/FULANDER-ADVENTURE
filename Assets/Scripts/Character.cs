using UnityEngine;

[DefaultExecutionOrder(-10)]
public class Character : MonoBehaviour, ISaveable
{
    public bool exist = true;
    private void Awake()
    {
        SetDataType(SaveType.CharacterPos);
    }
    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if(data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = transform.position;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, transform.position);
        }

        if (data.characterExistDict.ContainsKey(GetDataID().ID))
        {
            data.characterExistDict[GetDataID().ID] = exist;
        }
        else
        {       
            data.characterExistDict.Add(GetDataID().ID, exist);
        }


    }

    public void LoadData(Data data)
    {

        if (GetDataID().ID == "Player")
        {
            return;
        }

        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID];
        }
    }

    public void SetDataType(SaveType saveType)
    {
        GetDataID().SaveType = saveType;
    }
}
