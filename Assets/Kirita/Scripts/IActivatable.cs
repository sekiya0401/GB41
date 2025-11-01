namespace MS.Systems
{
    public interface IEnable
    {
        public void Enable();

        public void IsEnabled();
    }

    public interface IDisable
    {
        public void Disable();
        public bool IsDisabled();
    }
    public interface IActivatable : IEnable, IDisable { }
}