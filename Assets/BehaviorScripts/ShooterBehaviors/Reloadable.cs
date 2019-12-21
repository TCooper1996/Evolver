namespace BehaviorScripts
{
    public interface IReloadable
    {
        void Reload();
        float reloadTime { get; }
        int magazineSize { get; }

    }
}