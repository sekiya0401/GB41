using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CallbackTextMeshProUGUI : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshProUGUI;
    private void Awake()
    {
        TryGetComponent(out m_TextMeshProUGUI);
    }
}
