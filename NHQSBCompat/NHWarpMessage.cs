using QSB.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHQSBCompat
{
    public class NHWarpMessage : QSBMessage<string>
    {
        public NHWarpMessage(string data) : base(data) { }

        public override void OnReceiveRemote()
        {
            Main.Instance.ModHelper.Console.WriteLine($"Another player warped to {Data}");
            Main.Instance.RemoteChangeStarSystem(Data);
        }
    }
}
