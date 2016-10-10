using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;

using Pebble = Com.Getpebble.Android.Kit;
using PebbleUtil = Com.Getpebble.Android.Kit.Util;

namespace nicold.sports
{
    [Activity(Label = "nicold.sports", MainLauncher = true, Icon = "@drawable/icon")] 
    public class MainActivity : AppCompatActivity
    {
        private Switch switchUnits;
        private Switch switchPace;
        private TextView textStatus;
        private Pebble.PebbleKit.PebbleDataReceiver receiver;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.Title = "nicold PebbleKit Sports API Demo";

            Button buttonLaunchSport = FindViewById<Button>(Resource.Id.button_launch_sports);
            buttonLaunchSport.Click += ButtonLaunchSport_Click;

            Button buttonCloseSport = FindViewById<Button>(Resource.Id.button_close_sports);
            buttonCloseSport.Click += ButtonCloseSport_Click;

            Button buttonSendDummy = FindViewById<Button>(Resource.Id.button_send_dummy_data);
            buttonSendDummy.Click += ButtonSendDummy_Click;

            switchUnits = FindViewById<Switch>(Resource.Id.switch_sports_units);
            switchUnits.CheckedChange += SwitchUnits_CheckedChange;

            switchPace = FindViewById<Switch>(Resource.Id.switch_sports_pace);
            switchPace.CheckedChange += SwitchPace_CheckedChange;

            textStatus = FindViewById<TextView>(Resource.Id.textStatus);
            textStatus.Text = "unknown";
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (receiver == null)
            {
                receiver = new MyPebbleDataReceiver(this);
                Pebble.PebbleKit.RegisterReceivedDataHandler(ApplicationContext, receiver);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            try
            {
                UnregisterReceiver(receiver);
            }
            catch
            { }
            receiver = null;
        }
        private void SwitchPace_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            sbyte value = (sbyte)(switchPace.Checked ? Pebble.Constants.SportsDataSpeed : Pebble.Constants.SportsDataPace);
            PebbleUtil.PebbleDictionary pd = new PebbleUtil.PebbleDictionary();
            pd.AddUint8(Pebble.Constants.SportsLabelKey, value);
            Pebble.PebbleKit.SendDataToPebble(ApplicationContext, Pebble.Constants.SportsUuid, pd);

            switchPace.Text = $"Pace or Speed: { (switchPace.Checked? @"Pace": @"Speed") }";
        }

        private void SwitchUnits_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            sbyte value = (sbyte)(switchUnits.Checked ? Pebble.Constants.SportsUnitsImperial : Pebble.Constants.SportsUnitsMetric);
            PebbleUtil.PebbleDictionary pd = new PebbleUtil.PebbleDictionary();
            pd.AddUint8(Pebble.Constants.SportsUnitsKey, value);
            Pebble.PebbleKit.SendDataToPebble(ApplicationContext, Pebble.Constants.SportsUuid, pd);

            switchUnits.Text = $"Units: { (switchPace.Checked ? @"Imperial" : @"Metric") }";
        }

        private void ButtonSendDummy_Click(object sender, EventArgs e)
        {
            // Send some dummy data
            Random rnd = new Random();
            PebbleUtil.PebbleDictionary o = new PebbleUtil.PebbleDictionary();
            o.AddString(Pebble.Constants.SportsTimeKey, rnd.Next(99) + ":" + rnd.Next(99));
            o.AddString(Pebble.Constants.SportsDistanceKey, rnd.Next(99) + "." + rnd.Next(99));
            o.AddString(Pebble.Constants.SportsDataKey, rnd.Next(99) + "." + rnd.Next(99));
            Pebble.PebbleKit.SendDataToPebble(ApplicationContext, Pebble.Constants.SportsUuid, o);
        }

        private void ButtonCloseSport_Click(object sender, EventArgs e)
        {
            Pebble.PebbleKit.CloseAppOnPebble(ApplicationContext, Pebble.Constants.SportsUuid);
        }

        private void ButtonLaunchSport_Click(object sender, EventArgs e)
        {
            Pebble.PebbleKit.StartAppOnPebble(ApplicationContext, Pebble.Constants.SportsUuid);
        }

        public void SetStatusText(bool pause)
        {
            textStatus.Text = pause ? "paused" : "running";
        }
    }
}

