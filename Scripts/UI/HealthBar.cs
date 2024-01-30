using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public void SetMax(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void Set(int health)
    {
        slider.value = health;
    }
}
