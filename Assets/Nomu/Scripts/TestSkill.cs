using UnityEngine;
using System.Collections;

namespace MS.Games.Skills
{
	/// <summary>
	/// スキルのテスト用クラス
	/// (スキル作成時の参考例にしてもらっても可)
	/// </summary>
	public class TestSkill : PlayerBaseSkill
	{
		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				//ActivateSkill();
				StartCoroutine(SkillEffect());
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				if (m_CoolDownClass != null)
				{
					m_CoolDownClass.AddCount();
				}
			}
		}

		protected override void OnSkillStart()
		{
			Debug.Log("スキル使用");
		}

		protected override void OnSkillEnd()
		{
			Debug.Log("スキル終了");
		}

		IEnumerator SkillEffect()
		{
			if (ActivateSkill())
			{
				yield return new WaitForSeconds(5f);

				EndSkill();
			}
		}

	}

}
