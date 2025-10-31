using UnityEngine;

namespace MS.Systems.CoolDown
{
	[System.Serializable]
	public class CountCooldown : CooldownBase
	{
		[Tooltip("クールダウンに必要なカウント")]
		public int m_RequiredCount = 5;

		public int m_CDCount = 0;

		public override void StartCooldown()
		{
			Debug.Log("クールダウン開始");
			m_CDCount = 0;
			m_Complete = false;
		}

		public override void RestartCooldown()
		{
			ResetCooldown();

			StartCooldown();
		}

		public override void ResetCooldown()
		{
			m_CDCount = m_RequiredCount;

			m_Complete = true;
		}

		public override void AddCount(int value = 1)
		{
			if (m_Complete)
			{
				return;
			}

			m_CDCount += value;

			if (m_CDCount >= m_RequiredCount)
			{
				m_Complete = true;
				Debug.Log("クールダウン完了");
			}
		}

	}

}