using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Systems
{
    public static class InputActionExtensions
    {
        [Flags]
        public enum PHASE
        {
            STARTED = 0,
            PERFORMED = 1 << 0,
            CANCELED = 1  << 1,
        }

        /// <summary>
        /// InputAction�̑S�t�F�[�Y�ɓ����R�[���o�b�N��ǉ�����
        /// </summary>
        /// <param name="inputAction">�R�[���o�b�N�ǉ��Ώ�</param>
        /// <param name="action">�R�[���o�b�N</param>
        public static void AddAllPhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action)
        {
            inputAction.started += action;
            inputAction.performed += action;
            inputAction.canceled += action;
        }

        /// <summary>
        /// InputAction�̑S�t�F�[�Y���瓯���R�[���o�b�N���폜����
        /// </summary>
        /// <param name="inputAction">�R�[���o�b�N�ǉ��Ώ�</param>
        /// <param name="action">�R�[���o�b�N</param>
        public static void RemoveAllPhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action)
        {
            inputAction.started -= action;
            inputAction.performed -= action;
            inputAction.canceled -= action;
        }

        /// <summary>
        /// �w�肵���t�F�[�Y�ɂ̂݃R�[���o�b�N��ǉ�����
        /// </summary>
        /// <param name="inputAction">�R�[���o�b�N�ǉ��Ώ�</param>
        /// <param name="action">�R�[���o�b�N</param>
        /// <param name="phase">�t�F�[�Y</param>
        public static void AddPhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action, PHASE phase)
        {
            if ((phase & PHASE.STARTED) == PHASE.STARTED)
            {
                inputAction.started += action;
            }
            if ((phase & PHASE.PERFORMED) == PHASE.PERFORMED)
            {
                inputAction.performed += action;
            }
            if ((phase & PHASE.CANCELED) == PHASE.CANCELED)
            {
                inputAction.canceled += action;
            }
        }

        /// <summary>
        /// �w�肵���t�F�[�Y����̂݃R�[���o�b�N���폜����
        /// </summary>
        /// <param name="inputAction">�R�[���o�b�N�ǉ��Ώ�</param>
        /// <param name="action">�R�[���o�b�N</param>
        /// <param name="phase">�t�F�[�Y</param>
        public static void RemovePhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action, PHASE phase)
        {
            if ((phase & PHASE.STARTED) == PHASE.STARTED)
            {
                inputAction.started -= action;
            }
            if ((phase & PHASE.PERFORMED) == PHASE.PERFORMED)
            {
                inputAction.performed -= action;
            }
            if ((phase & PHASE.CANCELED) == PHASE.CANCELED)
            {
                inputAction.canceled -= action;
            }
        }
    }
}