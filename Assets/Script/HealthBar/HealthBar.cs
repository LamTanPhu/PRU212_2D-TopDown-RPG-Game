using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image CurrentHealthBar;
    [SerializeField] private Image TotalhealthBar;
    [SerializeField] private PlayerHealth PlayerhealthBar;

    private void Start()
    {
        if (PlayerhealthBar == null)
        {
            Debug.LogError("HealthBar: PlayerHealth reference is missing! Assign it in the Inspector.");
            return;
        }
        TotalhealthBar.fillAmount = PlayerhealthBar.CurrentHealth / 10;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (PlayerhealthBar == null)
        {
            Debug.LogError("HealthBar: PlayerHealth reference is missing! Assign it in the Inspector.");
            return;
        }
        CurrentHealthBar.fillAmount = PlayerhealthBar.CurrentHealth / 10;
        Debug.Log($"Updating Health Bar: {PlayerhealthBar.CurrentHealth}");
    }
}