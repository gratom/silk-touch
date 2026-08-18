using UnityEngine;

namespace Tools.ScrollComponent
{
    public abstract class HandledScrollItem<T> : ScrollItem, IHandledScrollItem<T>
    {
        protected IScrollItemHandler<T> Handler;

        public void InitWith(IScrollItemHandler<T> handler)
        {
            Handler = handler;
        }
    }
}