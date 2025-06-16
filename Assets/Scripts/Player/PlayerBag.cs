using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public GameObject myBag;
    private bool isOpen;


    // Update is called once per frame
    void Update()
    {
        OpenMyBag();
    }

    private void OpenMyBag()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            myBag.SetActive(isOpen);
            Main.RefreshItem();
        }
    }
}
