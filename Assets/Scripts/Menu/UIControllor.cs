using UnityEngine;

public class UIControllor : MonoBehaviour
{
    public GameObject newGameButton;


    //private void OnEnable()
    //{
    //    Debug.Log("ѡ��newGameButton");
    //    EventSystem.current.SetSelectedGameObject(newGameButton);
    //}

    public void ExitGame()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();
    }
    
}

