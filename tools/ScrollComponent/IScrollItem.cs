namespace Tools.ScrollComponent
{
    public interface IScrollItem
    {
        void Refresh(int index);
        void OnClick(int index);
        void OnDrag(int index);
        void OnGrab(int index);
    }
}