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
        /// InputActionの全フェーズに同じコールバックを追加する
        /// </summary>
        /// <param name="inputAction">コールバック追加対象</param>
        /// <param name="action">コールバック</param>
        public static void AddAllPhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action)
        {
            inputAction.started += action;
            inputAction.performed += action;
            inputAction.canceled += action;
        }

        /// <summary>
        /// InputActionの全フェーズから同じコールバックを削除する
        /// </summary>
        /// <param name="inputAction">コールバック追加対象</param>
        /// <param name="action">コールバック</param>
        public static void RemoveAllPhaseCallbacks(this InputAction inputAction, Action<InputAction.CallbackContext> action)
        {
            inputAction.started -= action;
            inputAction.performed -= action;
            inputAction.canceled -= action;
        }

        /// <summary>
        /// 指定したフェーズにのみコールバックを追加する
        /// </summary>
        /// <param name="inputAction">コールバック追加対象</param>
        /// <param name="action">コールバック</param>
        /// <param name="phase">フェーズ</param>
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
        /// 指定したフェーズからのみコールバックを削除する
        /// </summary>
        /// <param name="inputAction">コールバック追加対象</param>
        /// <param name="action">コールバック</param>
        /// <param name="phase">フェーズ</param>
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