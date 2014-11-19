using System;
using System.Globalization;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sulakore.Protocol.Components
{
    public class HMScheduler : ListView
    {
        private readonly List<HMSchedule> _schedules;
        public delegate void HMScheduleCallback(HMSchedule schedule);

        public bool LockColumns { get; set; }

        public HMScheduler()
        {
            _schedules = new List<HMSchedule>();

            var packetCol = new ColumnHeader { Name = "PacketCol", Text = "Packet", Width = 138 };
            var directionCol = new ColumnHeader { Name = "DirectionCol", Text = "Direction", Width = 63 };
            var burstCol = new ColumnHeader { Name = "BurstCol", Text = "Burst", Width = 44 };
            var intervalCol = new ColumnHeader { Name = "IntervalCol", Text = "Interval", Width = 58 };
            var statusCol = new ColumnHeader { Name = "StatusCol", Text = "Status", Width = 62 };
            Columns.AddRange(new[] { packetCol, directionCol, burstCol, intervalCol, statusCol });
            FullRowSelect = true;
            GridLines = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            MultiSelect = false;
            ShowItemToolTips = true;
            Size = new Size(386, 141);
            UseCompatibleStateImageBehavior = false;
            View = View.Details;
            LockColumns = true;
        }

        public void AddSchedule(HMessage packet, int interval, int burst, bool autoStart, string description, HMScheduleCallback callback)
        {
            var item = new ListViewItem(new[] { packet.ToString(), packet.Destination.ToString(), burst.ToString(), interval.ToString(), "Running" })
            {
                ToolTipText = description
            };

            Focus();
            Items.Add(item);
            item.Selected = true;
            EnsureVisible(Items.Count - 1);

            var schedule = new HMSchedule(packet, interval, burst, callback);
            _schedules.Add(schedule);

            if (autoStart) schedule.Start();
        }

        public void StopAll()
        {
            foreach (HMSchedule schedule in _schedules)
                schedule.Stop();
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            if (LockColumns)
            {
                e.Cancel = true;
                e.NewWidth = Columns[e.ColumnIndex].Width;
            }
            base.OnColumnWidthChanging(e);
        }

        public class HMSchedule : IDisposable
        {
            private readonly HMScheduleCallback _callback;
            private readonly System.Timers.Timer _ticker;

            public int Burst { get; set; }
            public int Interval { get; set; }
            public HMessage Packet { get; set; }

            public HMSchedule(HMessage packet, int interval, int burst, HMScheduleCallback callback)
            {
                if (burst < 1) throw new Exception("The burst value must be higher than one.");

                Packet = packet;
                Interval = interval;
                Burst = burst;
                _callback = callback;

                _ticker = new System.Timers.Timer(interval);
                _ticker.Elapsed += Ticker_Elapsed;
            }
            private void Ticker_Elapsed(object sender, ElapsedEventArgs e)
            {
                _ticker.Stop();
                for (int i = 0; i < Burst; i++)
                {
                    _callback(this);
                }

                _ticker.Start();
            }
            public void Start()
            {
                if (!_ticker.Enabled)
                    _ticker.Start();
            }
            public void Stop()
            {
                if (_ticker.Enabled)
                    _ticker.Stop();
            }

            public void Dispose()
            {
                ((IDisposable)_ticker).Dispose();
            }
        }
    }
}