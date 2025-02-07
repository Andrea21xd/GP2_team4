using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu1 : MonoBehaviour
{
    [SerializeField] public GameObject _optionCanvas;

    [Header("=== Scene Loader Name ===")]
    public int sceneLoader;

    



    private void MenuControll()
    {

            //if (!_pauseCanvas.activeSelf)
            //{
            //    OpenPause();
            //}
            //else 
            //{
            //    ClosePause();
            //    CloseOptions();
            //}
        
    }


    
    private void Update()
    {
        //if (InputsScript.instance.MenuOpenCloseInput)
        //{
        //    MenuControll();
        //}
    }
    



    public void StartGame()
    {
        SceneManager.LoadScene(sceneLoader);  // TODO ADD NEXT SCENE 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Resume()
    {
        
        _optionCanvas.SetActive(false);
    }

    public void OpenOptions()
    {
        _optionCanvas.SetActive(true);
       
    }

    public void CloseOptions()
    {
        _optionCanvas.SetActive(false);
       // _pauseCanvas.SetActive(true);
    }
    

    public void ExitGame()
    {
        Debug.Log("Closes the application");
        Application.Quit();
    }

}
