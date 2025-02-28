using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image CurrentHealthBar;
    [SerializeField] private Image TotalhealthBar;
    [SerializeField] private PlayerHealth PlayerhealthBar;

    private void Start()
    {
        TotalhealthBar.fillAmount = PlayerhealthBar.CurrentHealth / 10;
    }
    public void UpdateHealthBar()
    {
        CurrentHealthBar.fillAmount = PlayerhealthBar.CurrentHealth / 10;
    }

}