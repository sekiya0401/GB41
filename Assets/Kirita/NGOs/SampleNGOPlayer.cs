using MS.Extensions;
using System;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SampleNGOPlayer : NetworkBehaviour
{
    [SerializeField]
    private PlayerInput m_PlayerInput;
    [SerializeField]
    private TextMeshPro m_StateField;
    [SerializeField, Range(0f, 100f)]
    private float m_Speed = 3f;
    [SerializeField]
    private GameObject m_Avatar;
    private CharacterController m_CharacterController;
    private Vector2 m_MoveInputValue;
    private NetworkVariable<int> m_Count = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 自分のプレイヤー以外はInput無効化
        if (!IsOwner)
        {
            m_PlayerInput.enabled = false;
        }
        else
        {
            // 自分の入力のみ有効
            m_PlayerInput.enabled = true;
        }

        UnityEngine.Random.InitState((int)OwnerClientId);
        m_Avatar.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();

        m_StateField.color = UnityEngine.Random.ColorHSV();

        SetStateField();
    }

    private void OnEnable()
    {
        m_PlayerInput.actions["Move"].AddAllPhaseCallbacks(OnMove);
        m_PlayerInput.actions["Skill"].AddPhaseCallbacks(OnCount, InputActionExtensions.PHASE.STARTED);

        m_Count.OnValueChanged += OnCountChanged;
    }

    private void OnDisable()
    {
        m_PlayerInput.actions["Move"].RemoveAllPhaseCallbacks(OnMove);
        m_PlayerInput.actions["Skill"].RemovePhaseCallbacks(OnCount, InputActionExtensions.PHASE.STARTED);

        m_Count.OnValueChanged -= OnCountChanged;
    }

    private void OnCountChanged(int previousValue, int newValue)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"previousValue: {previousValue}");
        sb.AppendLine($"newValue: {newValue}");
        Debug.Log(sb.ToString());

        SetStateField();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_MoveInputValue = context.ReadValue<Vector2>();
    }

    private void OnCount(InputAction.CallbackContext context)
    {
        m_Count.Value++;
    }

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        m_CharacterController?.Move(m_MoveInputValue * m_Speed * Time.deltaTime);
    }

    private void SetStateField()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"ID: {OwnerClientId}");
        sb.AppendLine($"Count: {m_Count.Value}");

        m_StateField.text = sb.ToString();
    }
}
