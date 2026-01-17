using Character;
using DI;
using UnityEngine;
using UnityEngine.UI;


[Priority(100)]
public class BloodScreenController : MonoBehaviour
{
    [SerializeField]
    private Image _bloodScreen;

    [Inject]
    private PlayerProxy _characterFacade;

    public void Init()
    {
        this.Inject();

        _bloodScreen.color = new Color(1,1,1,0);
        _characterFacade.AddOnHealthChange(OnBloodScreenChange);
    }

    private void OnBloodScreenChange(int health) => _bloodScreen.color = new Color(1, 1, 1, 1 - (health / 100f));
}
