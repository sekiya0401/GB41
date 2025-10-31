using UnityEngine;

namespace MS.Systems.CoolDown
{
	public enum CooldownType
	{
		None,
		Time,
		Count
	}

	[System.Serializable]
	public abstract class CooldownBase
	{
		protected bool m_Complete = true;

		[Tooltip("コルーチン保存用")]
		protected Coroutine m_CDCoroutine;

		[Tooltip("クールダウンの所有者")]
		public MonoBehaviour m_Owner;

		/// <summary>
		/// クールダウン処理開始
		/// </summary>
		public abstract void StartCooldown();

		/// <summary>
		/// クールダウンのリスタート
		/// </summary>
		public abstract void RestartCooldown();

		/// <summary>
		/// クールダウンをリセット(クールダウン完了状態に)
		/// </summary>
		public abstract void ResetCooldown();

		/// <summary>
		/// カウント加算
		/// </summary>
		public virtual void AddCount(int value = 1) { }

		public bool IsComplete()
		{
			return m_Complete;
		}

	}

}