using System;

namespace Tools.ScrollComponent
{
    public abstract class ScrollItem : RectComponent, IScrollItem
    {
        public abstract void Refresh(int index);
        public abstract void OnClick(int index);
        public abstract void OnDrag(int index);
        public abstract void OnGrab(int index);
    }
}