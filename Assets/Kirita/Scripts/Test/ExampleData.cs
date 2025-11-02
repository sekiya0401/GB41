using UnityEngine;

[CreateAssetMenu(fileName = "ExampleData", menuName = "Scriptable Objects/Example/ExampleData")]
public class ExampleData : JsonSerializableScriptableObject
{
    [SerializeField]
    private ExampleState m_State;
    public string m_DataName = "Default";
}
