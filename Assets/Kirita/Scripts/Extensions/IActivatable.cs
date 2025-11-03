using UnityEngine.UIElements;

namespace MS.Systems
{
    /// <summary>
    /// オブジェクトが有効化できる機能を持つことを定義するインターフェース。
    /// </summary>
    public interface IEnable
    {
        /// <summary>
        /// オブジェクトを有効化する。
        /// </summary>
        public void Enable();

        /// <summary>
        /// オブジェクトが現在有効化されているかどうかを取得する。
        /// </summary>
        /// <returns>有効化されている場合はtrue、そうでない場合はfalse。</returns>
        public bool IsEnabled();
    }
    /// <summary>
    /// オブジェクトが無効化できる機能を持つことを定義するインターフェース。
    /// </summary>
    public interface IDisable
    {
        /// <summary>
        /// オブジェクトを無効化する。
        /// </summary>
        public void Disable();
        /// <summary>
        /// オブジェクトが現在無効化されているかどうかを取得する。
        /// </summary>
        /// <returns>無効化されている場合はtrue、そうでない場合はfalse。</returns>
        public bool IsDisabled();
    }
    /// <summary>
    /// オブジェクトが有効化と無効化の両方の機能を持つことを定義する複合インターフェース。
    /// IEnableとIDisableの両方のメンバーを継承する。
    /// </summary>
    public interface IActivatable : IEnable, IDisable { }
}