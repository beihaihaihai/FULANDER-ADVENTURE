using UnityEngine;



public class DataDefinition : MonoBehaviour
{
    [SerializeField]private string _id;
    [SerializeField]private SaveType _saveType;

    public string ID
    {
        get
        {
            // 优先使用已存在的ID
            if (!string.IsNullOrEmpty(_id)) return _id;

            // 如果没有则生成新ID
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
