using UnityEngine;
using System.Collections.Generic;

public class Core : MonoBehaviour,IDamagable
{
    [SerializeField]
    private int m_hp = 10;

    void Update()
    {
        if(m_hp <= 0)
        {
            Debug.Log("GameOver");
        }
    }

    public void Damage(int damage)
    {
        m_hp -= damage;
        Debug.Log("Core Hp:" + m_hp);
    }
}
