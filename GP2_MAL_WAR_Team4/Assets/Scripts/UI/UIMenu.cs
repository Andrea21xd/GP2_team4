using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _optionCanvas;

    [Header("=== Scene Loader Name ===")]
    public int sceneLoader;

    



    private void MenuControll()
    {

            if (!_pauseCanvas.activeSelf) 
            {
                OpenPause();
            }
            else 
            {
                ClosePause();
                CloseOptions();
            }
        
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
        PlayerPrefs.SetInt("Start", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneLoader);  // TODO ADD NEXT SCENE 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Resume()
    {
        _optionCanvas.SetActive(false);
        if (_pauseCanvas == null) { return; }
        _pauseCanvas.SetActive(false);
        
    }

    public void OpenOptions()
    {
        _optionCanvas.SetActive(true);
        if (_pauseCanvas == null) { return; }
        _pauseCanvas.SetActive(false);
    }

    public void CloseOptions()
    {
        _optionCanvas.SetActive(false);
        if (_pauseCanvas == null) { return; }
        _pauseCanvas.SetActive(true);
    }

    public void OpenPause()
    {
        _pauseCanvas.SetActive(true);
    }

    public void ClosePause()
    {
        _pauseCanvas.SetActive(false);
        _optionCanvas.SetActive(false);
    }

    


    public void ExitGame()
    {
        Debug.Log("Closes the application");
        Application.Quit();
    }

}
