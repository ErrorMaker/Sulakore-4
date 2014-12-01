using System;
using Sulakore.Protocol;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sulakore.Communication.Bridge
{
    public abstract class HExtension : HTriggerBase, IHExtension
    {
        private readonly Dictionary<ushort, Action<HMessage>> _inCallbacks = new Dictionary<ushort, Action<HMessage>>();
        private readonly Dictionary<ushort, Action<HMessage>> _outCallbacks = new Dictionary<ushort, Action<HMessage>>();

        private const TaskCreationOptions _eventCallFlags = (TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);

        public bool CaptureEvents { get; set; }
        public IHContractor Contractor { get; set; }

        void IHExtension.DisposeExtension()
        {
            base.Dispose();

            CaptureEvents = LockEvents = false;
            _inCallbacks.Clear();
            _outCallbacks.Clear();

            Disposed();
        }
        protected abstract void Disposed();

        void IHExtension.InitializeExtension()
        {
            Initialized();
        }
        protected abstract void Initialized();

        public abstract string Authors { get; }
        public abstract string Version { get; }
        public abstract string Identifier { get; }

        public virtual void OnDataToClient(byte[] data)
        {
            if (_inCallbacks.Count > 0)
            {
                int headerOffset = (Contractor.Protocol == HProtocols.Modern ? 4 : 0);
                ushort header = (headerOffset == 4 ? Modern.DecypherShort(data, 4) : Ancient.DecypherShort(data));
                if (_inCallbacks.ContainsKey(header))
                {
                    var packet = new HMessage(data, HDestinations.Client);
                    Task.Factory.StartNew(() => _inCallbacks[header](packet), _eventCallFlags);
                }
            }

            DataToClient(data);

            if (CaptureEvents)
                Task.Factory.StartNew(() => ProcessIncoming(data),
                    TaskCreationOptions.PreferFairness);
        }
        protected abstract void DataToClient(byte[] data);

        public virtual void OnDataToServer(byte[] data)
        {
            try
            {
                if (_outCallbacks.Count > 0)
                {
                    int headerOffset = (Contractor.Protocol == HProtocols.Modern ? 4 : 0);
                    ushort header = (headerOffset == 4 ? Modern.DecypherShort(data, 4) : Ancient.DecypherShort(data));
                    if (_outCallbacks.ContainsKey(header))
                    {
                        var packet = new HMessage(data, HDestinations.Server);
                        Task.Factory.StartNew(() => _outCallbacks[header](packet), _eventCallFlags);
                    }
                }

                DataToServer(data);

                if (CaptureEvents)
                    Task.Factory.StartNew(() => ProcessOutgoing(data),
                        TaskCreationOptions.PreferFairness);
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
        }
        protected abstract void DataToServer(byte[] data);

        public void AttachIncoming(ushort header, Action<HMessage> callback)
        {
            _inCallbacks[header] = callback;
        }
        public void DetachIncoming(ushort header)
        {
            if (_inCallbacks.ContainsKey(header))
                _inCallbacks.Remove(header);
        }

        public void AttachOutgoing(ushort header, Action<HMessage> callback)
        {
            _outCallbacks[header] = callback;
        }
        public void DetachOutgoing(ushort header)
        {
            if (_outCallbacks.ContainsKey(header))
                _outCallbacks.Remove(header);
        }

        public void Unload()
        {
            Contractor.Unload(this);
        }
    }
}