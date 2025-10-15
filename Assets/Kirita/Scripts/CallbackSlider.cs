using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class CallbackSlider : MonoBehaviour
{
    private Slider m_Slider;
    private void Awake()
    {
        TryGetComponent(out m_Slider);
    }

    public void ChangedValue(short value) => m_Slider.value = value;
    public void ChangedValue(float value) => m_Slider.value = value;
    public void ChangedValue(int value) => m_Slider.value = value;

    public Slider Slider => m_Slider;
}
