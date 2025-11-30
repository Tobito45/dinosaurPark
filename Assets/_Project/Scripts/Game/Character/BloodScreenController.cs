using UnityEngine;
using UnityEngine.UI;

public class BloodScreenController : MonoBehaviour
{
    [SerializeField]
    private Image _bloodScreen;

    private void Awake()
    {
        _bloodScreen.color = new Color(1,1,1,0);
        GameClientsNerworkInfo.Singleton.OnPlayerSet += (pl) => pl.HealthController.OnHealthChange += OnBloodScreenChange;
    }

    private void OnBloodScreenChange(int health) => _bloodScreen.color = new Color(1, 1, 1, 1 - (health / 100f));
}
