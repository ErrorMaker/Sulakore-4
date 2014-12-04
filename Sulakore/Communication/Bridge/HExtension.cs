using System;
using Sulakore.Protocol;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sulakore.Communication.Bridge
{
    public abstract class HExtension : HTriggerBase, IHExtension
    {
        #region Private Fields
        private readonly Dictionary<ushort, Action<HMessage>> _inCallbacks = new Dictionary<ushort, Action<HMessage>>();
        private readonly Dictionary<ushort, Action<HMessage>> _outCallbacks = new Dictionary<ushort, Action<HMessage>>();
        #endregion

        #region Public Properties
        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }
        public abstract string Location { get; set; }

        string IHExtension.Identifier { get; set; }
        public IHContractor Contractor { get; set; }
        public override bool CaptureEvents { get; set; }
        #endregion

        #region Protected/Explicit Methods
        void IHExtension.DisposeExtension()
        {
            base.Dispose();

            _inCallbacks.Clear();
            _outCallbacks.Clear();

            OnDisposed();
        }
        protected abstract void OnDisposed();

        void IHExtension.InitializeExtension()
        {
            OnInitialized();
        }
        protected abstract void OnInitialized();

        void IHExtension.SetExtensionState(HExtensionState state)
        {
            OnSetExtensionState(state);
        }
        protected abstract void OnSetExtensionState(HExtensionState state);

        void IHExtension.DataToClient(byte[] data)
        {
            if (_inCallbacks.Count > 0)
            {
                int headerOffset = (Contractor.Protocol == HProtocol.Modern ? 4 : 0);
                ushort header = (headerOffset == 4 ? Modern.DecypherShort(data, 4) : Ancient.DecypherShort(data));
                if (_inCallbacks.ContainsKey(header))
                {
                    var packet = new HMessage(data, HDestination.Client);
                    Task.Factory.StartNew(() => _inCallbacks[header](packet), TaskCreationOptions.LongRunning);
                }
            }

            OnDataToClient(data);
            if (CaptureEvents) Task.Factory.StartNew(() => ProcessIncoming(data), TaskCreationOptions.PreferFairness);
        }
        protected abstract void OnDataToClient(byte[] data);

        void IHExtension.DataToServer(byte[] data)
        {
            if (_outCallbacks.Count > 0)
            {
                int headerOffset = (Contractor.Protocol == HProtocol.Modern ? 4 : 0);
                ushort header = (headerOffset == 4 ? Modern.DecypherShort(data, 4) : Ancient.DecypherShort(data));
                if (_outCallbacks.ContainsKey(header))
                {
                    var packet = new HMessage(data, HDestination.Server);
                    Task.Factory.StartNew(() => _outCallbacks[header](packet), TaskCreationOptions.LongRunning);
                }
            }

            OnDataToServer(data);
            if (CaptureEvents) Task.Factory.StartNew(() => ProcessOutgoing(data), TaskCreationOptions.PreferFairness);
        }
        protected abstract void OnDataToServer(byte[] data);
        #endregion

        #region Public Methods
        public void DetachIn(ushort header)
        {
            if (_inCallbacks.ContainsKey(header))
                _inCallbacks.Remove(header);
        }
        public void AttachIn(ushort header, Action<HMessage> callback)
        {
            _inCallbacks[header] = callback;
        }

        public void DetachOut(ushort header)
        {
            if (_outCallbacks.ContainsKey(header))
                _outCallbacks.Remove(header);
        }
        public void AttachOut(ushort header, Action<HMessage> callback)
        {
            _outCallbacks[header] = callback;
        }

        public void Unload()
        {
            Contractor.InitiateUnload(this);
        }
        #endregion
    }
}