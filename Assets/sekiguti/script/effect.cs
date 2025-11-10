using Prototype.Games;
using Prototype.ScriptableObjects;
using UnityEngine;

public class effect : MonoBehaviour 
{
    public ParticleSystem moveEffect; // ダウンロードしたエフェクトを指定
    private Player player;
    private bool isPlaying = false;
    
    void Start()
    {
        player = GetComponent<Player>();
    }
    void Update()
    {

        //プレイヤーが動いているなら再生
        if (player.m_IsMoveing)
        {
            if (!isPlaying)
            {
                moveEffect.Play();
                isPlaying = true;
            }
        }
        // 止まったら停止
        else
        {
            if (isPlaying)
            {
                moveEffect.Stop();
                isPlaying = false;
            }
        }
    }
}
