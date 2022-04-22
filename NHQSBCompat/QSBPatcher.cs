using QSB.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QSB.WorldSync;

namespace NHQSBCompat
{
    public static class QSBPatcher
    {
        public static void Apply()
        {
            /*
            var joinLeaveSinularityCreate = typeof(JoinLeaveSingularity).GetMethod(nameof(JoinLeaveSingularity.Create));
            Main.Instance.ModHelper.HarmonyHelper.AddPrefix(joinLeaveSinularityCreate, typeof(QSBPatcher), nameof(QSBPatcher.OnJoinLeaveSingularityCreate));
            */
            var WorldSyncOnBuildWorldObjects = typeof(QSBWorldSync).GetMethod(nameof(QSBWorldSync.BuildWorldObjects));
            Main.Instance.ModHelper.HarmonyHelper.AddPrefix(WorldSyncOnBuildWorldObjects, typeof(QSBPatcher), nameof(OnQSBWorldSyncBuildWorldObjects));
        }

        public static bool OnJoinLeaveSingularityCreate()
        {
            return false;
        }
        
        private static readonly string[] ManagersToRemove =
        {
            "OrbManager",
            "SatelliteProjectorManager",
            "MeteorManager",
            "JellyfishManager",
            "AnglerManager",
            "OccasionalManager",
        };
        
        public static void OnQSBWorldSyncBuildWorldObjects()
        {
            List<WorldObjectManager> managers = QSBWorldSync.Managers.ToList();
            managers.RemoveAll(m => ManagersToRemove.Contains(m.ToString()));
            QSBWorldSync.Managers = managers.ToArray();
        }
    }
}
