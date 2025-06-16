using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitialLoad : MonoBehaviour
{
    public AssetReference initialScene;

    private void Awake()
    {
        Addressables.LoadSceneAsync(initialScene);
    }
}
