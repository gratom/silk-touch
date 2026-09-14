using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilkTouch.Boot
{
    using Managers;
    using Tools;

    [NamedBehavior]
    public class Boot : MonoBehaviour
    {
#pragma warning disable
        [SerializeField] private BootSettings bootSetting;
#pragma warning restore

        #region Unity functions

        private void Start()
        {
            PreBootProjectSettings();
            ManagersCreating();
        }

        private void PreBootProjectSettings()
        {
            Application.targetFrameRate = 60;
            Time.timeScale = 1;
        }

        #endregion Unity functions

        #region private functions

        private void ManagersCreating()
        {
#pragma warning disable
            Services.InitAppWith(bootSetting.ManagerContainers);
#pragma warning restore
            StartCoroutine(Loading());
        }

        private IEnumerator Loading()
        {
            yield return new WaitForSeconds(bootSetting.BootTime);
            if (bootSetting.NextSceneIndex == 0)
            {
                Debug.Log("Next scene after boot is null, please, check the boot settings.");
                yield break;
            }

            SceneLoader.LoadScene(bootSetting.NextSceneIndex, () => Services.GetManager<MainManager>().EntryPoint());
        }

        #endregion private functions
    }
}