using UnityEngine;

public class UIControllor : MonoBehaviour
{
    public GameObject newGameButton;


    //private void OnEnable()
    //{
    //    Debug.Log("选中newGameButton");
    //    EventSystem.current.SetSelectedGameObject(newGameButton);
    //}

    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
    
}

