using UnityEngine;
using UnityEngine.UI;

public class LifeMonitor : MonoBehaviour
{
    [SerializeField] private Image[] _heartContainers;

    private void OnEnable()
    {
        Player.OnPlayerDamage += DecreaseLife;
    }

    private void OnDisable()
    {
        Player.OnPlayerDamage -= DecreaseLife;
    }

    private void DecreaseLife(int currentLife)
    {
        if (currentLife >= 0)
        {
            _heartContainers[currentLife].enabled = false;
        }
    }
}