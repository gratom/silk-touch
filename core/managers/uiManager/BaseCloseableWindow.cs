#if UI_TMP
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace SilkTouch.UI
{
    using Managers;
    using Managers.Data;

    public abstract class BaseCloseableWindow : BaseWindow
    {
        protected IDataManager dataManager => Services.GetManager<IDataManager>();
        protected UIManager uiManager => Services.GetManager<UIManager>();

        #region buttons functions

        public void HideWindow()
        {
            Services.GetManager<UIManager>().HideWindow(WindowType);
        }

        #endregion
    }
}
#endif