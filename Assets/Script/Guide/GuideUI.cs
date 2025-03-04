using UnityEngine;

public class GuideUI : MonoBehaviour
{
    [SerializeField] private GameObject panel; // Assign your UI Panel in Inspector

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in " + gameObject.name);
        }
    }
}
