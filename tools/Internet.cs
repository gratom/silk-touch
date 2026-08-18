using UnityEngine;

namespace Tools
{
    public static class Internet
    {
        public static bool Any => Application.internetReachability != NetworkReachability.NotReachable;
        public static NetworkReachability ConnectionType => Application.internetReachability;
        public static bool IsWifi => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        public static bool IsMobile => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
    }
}