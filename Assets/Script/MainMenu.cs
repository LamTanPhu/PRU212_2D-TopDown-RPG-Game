using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Map-1.1");
    }

    public void ExitGame()
    {
        Application.Quit();
		Debug.Log("Game is exiting");
	}
}
