using QSB.Messaging;

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
