using UnityEngine;



public class DataDefinition : MonoBehaviour
{
    [SerializeField]private string _id;
    [SerializeField]private SaveType _saveType;

    public string ID
    {
        get
        {
            // ����ʹ���Ѵ��ڵ�ID
            if (!string.IsNullOrEmpty(_id)) return _id;

            // ���û����������ID
            if (string.IsNullOrEmpty(_id))
            {
                _id = gameObject.name;
            }

            return _id;
        }
    }
    
    public SaveType SaveType
    {
        get => _saveType;
        set => _saveType = value;
    }


    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            //ID = System.Guid.NewGuid().ToString();
            _id = gameObject.name;
        }
    }
}
