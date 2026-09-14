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
        protected DataManager dataManager => Services.GetManager<DataManager>();
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