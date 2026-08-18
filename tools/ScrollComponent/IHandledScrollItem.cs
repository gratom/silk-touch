namespace Tools.ScrollComponent
{
    public interface IHandledScrollItem<T> : IScrollItem
    {
        void InitWith(IScrollItemHandler<T> handler);
    }
}