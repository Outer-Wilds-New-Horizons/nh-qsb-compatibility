using QSB.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public static bool OnJoinLeaveSingularityCreate()
        {
            return false;
        }
    }
}
