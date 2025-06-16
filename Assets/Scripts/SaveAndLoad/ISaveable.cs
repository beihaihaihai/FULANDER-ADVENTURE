
public interface ISaveable
{
    void SetDataType(SaveType saveType);
    DataDefinition GetDataID();
    void RegisterSaveData() => GameSaveAndLoad.instance.RegisterSaveData(this);
    void UnRegisterSaveData() => GameSaveAndLoad.instance.UnRegisterSaveData(this);
    void GetSaveData(Data data);
    void LoadData(Data data);
}
