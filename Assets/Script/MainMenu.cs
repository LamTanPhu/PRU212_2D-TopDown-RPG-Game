using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameObject mainmenu;
    public GameObject guide;
    public GameObject story;

    public Button nextGuideButton;
    public Button nextStoryButton;

    void Start()
    {
        nextGuideButton.onClick.AddListener(StartGuide);
        nextStoryButton.onClick.AddListener(StartStory);
    }
    public void StartGuide()
    {        
        guide.SetActive(true);
        mainmenu.SetActive(false);
        story.SetActive(false);
    }

    public void StartStory()
    {
        mainmenu.SetActive(false);
        guide.SetActive(false);
        story.SetActive(true);
    }

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
