using System;
using System.Collections.Generic;
using UnityEngine;
using MS.SO.Notification;

namespace MS.Games.UI
{
    public class BlinkView : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_BlinkIcon;
        [SerializeField]
        private NotificationInt m_MaxCount;
        [SerializeField]
        private NotificationInt m_Count;
        private Stack<GameObject> m_IconStack = new();

        private void OnEnable()
        {
            m_Count.ChangedValue += OnChangedCount;
        }

        private void OnDisable()
        {
            m_Count.ChangedValue -= OnChangedCount;
        }

        private void Start()
        {
            for (int i = 0; i < m_MaxCount.Value; i++)
            {
                CreateBlinkIcon();
            }
        }

        private void OnChangedCount(int oldVlue, int newValue)
        {
            if(oldVlue == newValue)
            {
                return;
            }

            int diff = Mathf.Abs(newValue - oldVlue);
            bool isIncrease = newValue - oldVlue > 0 ? true : false;

            for(int i = 0; i < diff ; i++)
            {
                if(isIncrease)
                {
                    CreateBlinkIcon();
                }
                else
                {
                    DeleteBlinkIcon();
                }
            }
        }

        private void CreateBlinkIcon()
        {
            m_IconStack.Push(Instantiate(m_BlinkIcon, transform));
        }

        private void DeleteBlinkIcon()
        {
            Destroy(m_IconStack.Pop());
        }
    }

}