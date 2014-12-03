using System;
using System.Timers;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sulakore.Protocol.Controls
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class HMScheduler : ListView
    {
        #region Subscribable Events
        public event EventHandler<ScheduleTriggeredEventArgs> ScheduleTriggered;
        private void OnScheduleTriggered(object sender, ScheduleTriggeredEventArgs e)
        {
            if (ScheduleTriggered != null)
            {
                ScheduleTriggered(sender, e);
                if (e.Cancel)
                {
                    SchedulesRunning--;
                    _bySchedule[(HSchedule)sender].SubItems[4].Text = StatusSTOPPED;
                }
            }
        }
        #endregion

        #region Private Fields
        private bool _suppressSelectionChanged;

        private readonly Dictionary<HSchedule, ListViewItem> _bySchedule;
        private readonly Dictionary<ListViewItem, HSchedule> _schedules;

        private const string StatusRUNNING = "Running", StatusSTOPPED = "Stopped";
        #endregion

        #region Public Properties
        public bool LockColumns { get; set; }
        public int SchedulesRunning { get; private set; }
        #endregion

        #region Constructor(s)
        public HMScheduler()
        {
            _schedules = new Dictionary<ListViewItem, HSchedule>();
            _bySchedule = new Dictionary<HSchedule, ListViewItem>();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.EnableNotifyMessage, true);

            var packetCol = new ColumnHeader { Name = "PacketCol", Text = "Packet", Width = 131 };
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
        #endregion

        #region Public Methods
        public void StopAll()
        {
            BeginUpdate();
            IEnumerable<ListViewItem> items = _schedules.Keys;
            foreach (ListViewItem item in items)
            {
                _schedules[item].Stop();
                item.SubItems[4].Text = StatusSTOPPED;
            }
            EndUpdate();

            SchedulesRunning = 0;
        }
        public void ToggleSelected()
        {
            ListViewItem selectedItem = SelectedItems[0];
            if (_schedules.ContainsKey(selectedItem))
            {
                HSchedule schedule = _schedules[selectedItem];
                bool shouldStop = (schedule.IsRunning);

                if (shouldStop)
                {
                    schedule.Stop();
                    SchedulesRunning--;
                }
                else
                {
                    schedule.Start();
                    SchedulesRunning++;
                }
                selectedItem.SubItems[4].Text = (shouldStop ? StatusSTOPPED : StatusRUNNING);
            }
        }
        public void RemoveSelected()
        {
            ListViewItem selectedItem = SelectedItems[0];
            if (_schedules.ContainsKey(selectedItem))
            {
                HSchedule schedule = _schedules[selectedItem];

                if (schedule.IsRunning) SchedulesRunning--;
                schedule.Dispose();

                _schedules.Remove(selectedItem);
                _bySchedule.Remove(schedule);

                int index = SelectedIndices[0];
                _suppressSelectionChanged = (Items.Count > 1);
                Items.RemoveAt(index);

                if (Items.Count > 0)
                {
                    _suppressSelectionChanged = true;
                    Items[index - (index < Items.Count - 1 && index != 0 || index == Items.Count ? 1 : 0)].Selected = true;
                }
            }
        }

        public void AddSchedule(HSchedule schedule, bool autoStart, string description)
        {
            if (_schedules.ContainsValue(schedule)) return;

            var item = new ListViewItem(new string[]
            {
                schedule.Packet.ToString(),
                schedule.Packet.Destination.ToString(),
                schedule.Burst.ToString(),
                schedule.Interval.ToString(),
                autoStart ? StatusRUNNING : StatusSTOPPED
            });

            if (!string.IsNullOrEmpty(description))
                item.ToolTipText = string.Format("Description: {0}\n{1}",
                    description, schedule.Packet);

            Focus();
            Items.Add(item);
            _suppressSelectionChanged = Items.Count > 1;
            item.Selected = true;
            EnsureVisible(Items.Count - 1);

            _schedules.Add(item, schedule);
            _bySchedule.Add(schedule, item);

            schedule.ScheduleTriggered += OnScheduleTriggered;
            if (autoStart)
            {
                SchedulesRunning++;
                schedule.Start();
            }
        }
        #endregion

        #region Overrided Methods
        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _suppressSelectionChanged = (GetItemAt(e.X, e.Y) != null);
            base.OnMouseDown(e);
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
        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (_suppressSelectionChanged && !e.IsSelected) _suppressSelectionChanged = false;
            else
            {
                base.OnItemSelectionChanged(e);
                if (e.IsSelected) _suppressSelectionChanged = false;
            }
        }
        #endregion
    }

    public class HSchedule : IDisposable
    {
        #region Subscribable Events
        public event EventHandler<ScheduleTriggeredEventArgs> ScheduleTriggered;
        protected virtual void OnScheduleTriggered(int burstCount, int burstLeft, bool isFinalBurst)
        {
            if (ScheduleTriggered != null)
            {
                var arguments = new ScheduleTriggeredEventArgs(Packet, burstCount, burstLeft, isFinalBurst);
                ScheduleTriggered(this, arguments);
                if (arguments.Cancel) IsRunning = false;
            }
        }
        #endregion

        #region Private Fields
        private readonly System.Timers.Timer _ticker;
        #endregion

        #region Public Properties
        public int Burst { get; set; }
        public int Interval { get; set; }
        public HMessage Packet { get; set; }
        public bool IsRunning { get; private set; }
        #endregion

        #region Constructor(s)
        public HSchedule(HMessage packet, int interval, int burst)
        {
            Packet = packet;
            Interval = interval;
            Burst = burst;

            _ticker = new System.Timers.Timer(interval);
            _ticker.Elapsed += Ticker_Elapsed;
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (IsRunning) return;

            _ticker.Start();
            IsRunning = true;
        }
        public void Stop()
        {
            if (!IsRunning) return;

            _ticker.Stop();
            IsRunning = false;
        }

        public void Dispose()
        {
            SKore.Unsubscribe(ref ScheduleTriggered);

            Stop();
            _ticker.Dispose();
        }
        #endregion

        #region Private Methods
        private void Ticker_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ticker.Stop();
            int tmpBurst = Burst, burstCount;
            for (int i = 0; i < tmpBurst && IsRunning; i++)
            {
                burstCount = i + 1;
                OnScheduleTriggered(burstCount,
                     tmpBurst - burstCount,
                    burstCount >= tmpBurst);
            }

            if (IsRunning) _ticker.Start();
        }
        #endregion
    }
    public class ScheduleTriggeredEventArgs : CancelEventArgs
    {
        public int BurstLeft { get; private set; }
        public int BurstCount { get; private set; }
        public HMessage Packet { get; private set; }
        public bool IsFinalBurst { get; private set; }

        public ScheduleTriggeredEventArgs(HMessage packet, int burstCount, int burstLeft, bool isFinalBurst)
        {
            Packet = packet;
            BurstCount = burstCount;
            BurstLeft = burstLeft;
            IsFinalBurst = isFinalBurst;
        }

        public override string ToString()
        {
            return string.Format("Packet: {0}, BurstCount: {1}, BurstLeft: {2}, IsFinalBurst: {3}",
                Packet, BurstCount, BurstLeft, IsFinalBurst);
        }
    }
}