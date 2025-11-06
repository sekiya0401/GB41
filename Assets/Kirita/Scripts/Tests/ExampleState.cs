using UnityEngine;
using MS.SO.Variable;

[CreateAssetMenu(fileName = "ExampleState", menuName = "Scriptable Objects/Example/ExampleState")]
public class ExampleState : JsonSerializableScriptableObject
{
    public IntVariable m_Health;
    public FloatVariable m_Stamina;
}
