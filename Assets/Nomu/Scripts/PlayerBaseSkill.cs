using UnityEngine;
using MS.Systems.CoolDown;

namespace MS.Games.Skills
{
	public enum CDStartTiming
	{
		OnSkillStart,
		OnSkillEnd
	}

	public abstract class PlayerBaseSkill : MonoBehaviour
	{
		[SerializeField]
		public string m_SkillName = "Base";

		[Tooltip("クールダウンのタイプ")]
		public CooldownType m_CDType;

		[Tooltip("エディター変更感知用")]
		private CooldownType m_PrevCDType;

		[Tooltip("クールダウンの開始タイミング")]
		public CDStartTiming m_CDTiming = CDStartTiming.OnSkillStart;

		[SerializeReference]
		[Tooltip("クールダウンクラス")]
		public CooldownBase m_CoolDownClass;

		private bool m_IsFinished = true;


		/// <summary>
		/// スキルの有効化
		/// </summary>
		/// <returns>スキルが発動できたか</returns>
		public bool ActivateSkill()
		{
			if (!m_IsFinished)
			{
				Debug.Log("スキル発動中");
				return false;
			}

			if (m_CoolDownClass != null)
			{
				if (!m_CoolDownClass.IsComplete())
				{
					Debug.Log("クールダウン中");
					return false;
				}

				if (m_CDTiming == CDStartTiming.OnSkillStart)
				{
					m_CoolDownClass.StartCooldown();
				}
			}

			m_IsFinished = false;

			OnSkillStart();

			return true;
		}


		/// <summary>
		/// スキル終了時の処理
		/// </summary>
		public void EndSkill()
		{
			if (m_CoolDownClass != null)
			{
				if (m_CDTiming == CDStartTiming.OnSkillEnd)
				{
					m_CoolDownClass.StartCooldown();
				}
			}

			m_IsFinished = true;

			OnSkillEnd();
		}

		/// <summary>
		/// スキル発動時に実行
		/// </summary>
		protected abstract void OnSkillStart();

		/// <summary>
		/// スキル終了時に実行
		/// </summary>
		protected abstract void OnSkillEnd();


#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_PrevCDType == m_CDType)
			{
				return;
			}

			m_PrevCDType = m_CDType;
			switch (m_CDType)
			{
				case CooldownType.None:
					m_CoolDownClass = null;
					break;

				case CooldownType.Time:
					m_CoolDownClass = new TimeCooldown();
					m_CoolDownClass.m_Owner = this;
					break;

				case CooldownType.Count:
					m_CoolDownClass = new CountCooldown();
					m_CoolDownClass.m_Owner = this;
					break;
			}
		}
#endif

	}

}
