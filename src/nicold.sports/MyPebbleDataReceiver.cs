using Android.Content;
using Pebble = Com.Getpebble.Android.Kit;
using PebbleUtil = Com.Getpebble.Android.Kit.Util;

namespace nicold.sports
{
    class MyPebbleDataReceiver : Pebble.PebbleKit.PebbleDataReceiver
    {
        private MainActivity _activity;
        public MyPebbleDataReceiver(MainActivity activity) : base(Pebble.Constants.SportsUuid)
        {
            _activity = activity;
        }

        public override void ReceiveData(Context p0, int id, PebbleUtil.PebbleDictionary data)
        {
            Pebble.PebbleKit.SendAckToPebble(_activity.ApplicationContext, id);

            int state = data.GetUnsignedIntegerAsLong(Pebble.Constants.SportsStateKey).IntValue();
            _activity.SetStatusText(state == Pebble.Constants.SportsStatePaused);
        }
    }
}