namespace Tools.ScrollComponent
{
    public interface IScrollItemHandler<T>
    {
        void ClickOnScrollItem(int index);
        bool TryGetElementForRefresh(int index, out T t);
    }
}