using System;
using System.Collections.Generic;

using Sulakore.Protocol;
using System.Diagnostics;

namespace Sulakore.Communication
{
    public class HTriggers : IDisposable
    {
        #region Outgoing Event Handlers
        public event EventHandler<HostBanPlayerEventArgs> HostBanPlayer;
        protected virtual void OnHostBanPlayer(HostBanPlayerEventArgs e)
        {
            EventHandler<HostBanPlayerEventArgs> handler = HostBanPlayer;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostChangeClothesEventArgs> HostChangeClothes;
        protected virtual void OnHostChangeClothes(HostChangeClothesEventArgs e)
        {
            EventHandler<HostChangeClothesEventArgs> handler = HostChangeClothes;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostChangeMottoEventArgs> HostChangeMotto;
        protected virtual void OnHostChangeMotto(HostChangeMottoEventArgs e)
        {
            EventHandler<HostChangeMottoEventArgs> handler = HostChangeMotto;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostChangeStanceEventArgs> HostChangeStance;
        protected virtual void OnHostChangeStance(HostChangeStanceEventArgs e)
        {
            EventHandler<HostChangeStanceEventArgs> handler = HostChangeStance;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostClickPlayerEventArgs> HostClickPlayer;
        protected virtual void OnHostClickPlayer(HostClickPlayerEventArgs e)
        {
            EventHandler<HostClickPlayerEventArgs> handler = HostClickPlayer;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostDanceEventArgs> HostDance;
        protected virtual void OnHostDance(HostDanceEventArgs e)
        {
            EventHandler<HostDanceEventArgs> handler = HostDance;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostGestureEventArgs> HostGesture;
        protected virtual void OnHostGesture(HostGestureEventArgs e)
        {
            EventHandler<HostGestureEventArgs> handler = HostGesture;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostKickPlayerEventArgs> HostKickPlayer;
        protected virtual void OnHostKickPlayer(HostKickPlayerEventArgs e)
        {
            EventHandler<HostKickPlayerEventArgs> handler = HostKickPlayer;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostMoveFurnitureEventArgs> HostMoveFurniture;
        protected virtual void OnHostMoveFurniture(HostMoveFurnitureEventArgs e)
        {
            EventHandler<HostMoveFurnitureEventArgs> handler = HostMoveFurniture;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostMutePlayerEventArgs> HostMutePlayer;
        protected virtual void OnHostMutePlayer(HostMutePlayerEventArgs e)
        {
            EventHandler<HostMutePlayerEventArgs> handler = HostMutePlayer;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostRaiseSignEventArgs> HostRaiseSign;
        protected virtual void OnHostRaiseSign(HostRaiseSignEventArgs e)
        {
            EventHandler<HostRaiseSignEventArgs> handler = HostRaiseSign;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostRoomExitEventArgs> HostRoomExit;
        protected virtual void OnHostRoomExit(HostRoomExitEventArgs e)
        {
            EventHandler<HostRoomExitEventArgs> handler = HostRoomExit;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostRoomNavigateEventArgs> HostRoomNavigate;
        protected virtual void OnHostRoomNavigate(HostRoomNavigateEventArgs e)
        {
            EventHandler<HostRoomNavigateEventArgs> handler = HostRoomNavigate;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostSayEventArgs> HostSay;
        protected virtual void OnHostSay(HostSayEventArgs e)
        {
            EventHandler<HostSayEventArgs> handler = HostSay;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostShoutEventArgs> HostShout;
        protected virtual void OnHostShout(HostShoutEventArgs e)
        {
            EventHandler<HostShoutEventArgs> handler = HostShout;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostTradePlayerEventArgs> HostTradePlayer;
        protected virtual void OnHostTradePlayer(HostTradePlayerEventArgs e)
        {
            EventHandler<HostTradePlayerEventArgs> handler = HostTradePlayer;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<HostWalkEventArgs> HostWalk;
        protected virtual void OnHostWalk(HostWalkEventArgs e)
        {
            EventHandler<HostWalkEventArgs> handler = HostWalk;
            if (handler != null) handler(this, e);
        }
        #endregion
        #region Incoming Event Handlers
        public event EventHandler<FurnitureDataLoadedEventArgs> FurnitureDataLoaded;
        protected virtual void OnFurnitureDataLoaded(FurnitureDataLoadedEventArgs e)
        {
            EventHandler<FurnitureDataLoadedEventArgs> handler = FurnitureDataLoaded;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerChangeDataEventArgs> PlayerChangeData;
        protected virtual void OnPlayerChangeData(PlayerChangeDataEventArgs e)
        {
            EventHandler<PlayerChangeDataEventArgs> handler = PlayerChangeData;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerChangeStanceEventArgs> PlayerChangeStance;
        protected virtual void OnPlayerChangeStance(PlayerChangeStanceEventArgs e)
        {
            EventHandler<PlayerChangeStanceEventArgs> handler = PlayerChangeStance;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerDanceEventArgs> PlayerDance;
        protected virtual void OnPlayerDance(PlayerDanceEventArgs e)
        {
            EventHandler<PlayerDanceEventArgs> handler = PlayerDance;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerDataLoadedEventArgs> PlayerDataLoaded;
        protected virtual void OnPlayerDataLoaded(PlayerDataLoadedEventArgs e)
        {
            EventHandler<PlayerDataLoadedEventArgs> handler = PlayerDataLoaded;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerDropFurnitureEventArgs> PlayerDropFurniture;
        protected virtual void OnPlayerDropFurniture(PlayerDropFurnitureEventArgs e)
        {
            EventHandler<PlayerDropFurnitureEventArgs> handler = PlayerDropFurniture;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerGestureEventArgs> PlayerGesture;
        protected virtual void OnPlayerGesture(PlayerGestureEventArgs e)
        {
            EventHandler<PlayerGestureEventArgs> handler = PlayerGesture;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerKickHostEventArgs> PlayerKickHost;
        protected virtual void OnPlayerKickHost(PlayerKickHostEventArgs e)
        {
            EventHandler<PlayerKickHostEventArgs> handler = PlayerKickHost;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerMoveFurnitureEventArgs> PlayerMoveFurniture;
        protected virtual void OnPlayerMoveFurniture(PlayerMoveFurnitureEventArgs e)
        {
            EventHandler<PlayerMoveFurnitureEventArgs> handler = PlayerMoveFurniture;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<PlayerSpeakEventArgs> PlayerSpeak;
        protected virtual void OnPlayerSpeak(PlayerSpeakEventArgs e)
        {
            EventHandler<PlayerSpeakEventArgs> handler = PlayerSpeak;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Private/Public Fields
        public delegate void PacketCallback(HMessage packet);

        private bool _lockEvents, _captureEvents;

        private readonly Stack<HMessage> _previousOutgoing, _previousIncoming;
        private readonly IDictionary<ushort, PacketCallback> _lockedOut, _lockedIn, _inCallbacks, _outCallbacks;
        #endregion

        #region Private/Public Properties
        public bool LockEvents
        {
            get { return _lockEvents; }
            set
            {
                if (value == _lockEvents) return;

                if (!(_lockEvents = value))
                {
                    _lockedOut.Clear();
                    _lockedIn.Clear();
                }
            }
        }
        public bool CaptureEvents
        {
            get { return _captureEvents; }
            set
            {
                if (value == _captureEvents) return;

                if (!(_captureEvents = value))
                {
                    _previousOutgoing.Clear();
                    _previousIncoming.Clear();
                }
            }
        }
        #endregion

        #region Constructor(s)
        public HTriggers()
        {
            _previousOutgoing = new Stack<HMessage>();
            _previousIncoming = new Stack<HMessage>();

            _lockedIn = new Dictionary<ushort, PacketCallback>();
            _lockedOut = new Dictionary<ushort, PacketCallback>();

            _inCallbacks = new Dictionary<ushort, PacketCallback>();
            _outCallbacks = new Dictionary<ushort, PacketCallback>();
        }
        #endregion

        public void DetachIn(ushort header)
        {
            if (_inCallbacks.ContainsKey(header))
                _inCallbacks.Remove(header);
        }
        public void AttachIn(ushort header, PacketCallback callback)
        {
            _inCallbacks[header] = callback;
        }

        public void DetachOut(ushort header)
        {
            if (_outCallbacks.ContainsKey(header))
                _outCallbacks.Remove(header);
        }
        public void AttachOut(ushort header, PacketCallback callback)
        {
            _outCallbacks[header] = callback;
        }

        public virtual void ProcessOutgoing(byte[] data)
        {
            var packet = new HMessage(data, HDestination.Server);
            if (_outCallbacks.Count > 0 && _outCallbacks.ContainsKey(packet.Header))
            {
                _outCallbacks[packet.Header](packet);
                packet.Position = 0;
            }
            if (!CaptureEvents) return;
            try
            {
                HMessage previousOutgoing = (_previousOutgoing.Count > 0 ? _previousOutgoing.Pop() : null);
                if (LockEvents && _lockedOut.ContainsKey(packet.Header))
                {
                    _lockedOut[packet.Header](packet);
                    packet = null;
                }
                else ProcessOutgoing(previousOutgoing, ref packet);
            }
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            finally
            {
                if (packet != null)
                {
                    packet.Position = 0;
                    _previousOutgoing.Push(packet);
                }
            }
        }
        private void ProcessOutgoing(HMessage previous, ref HMessage current)
        {

        }

        public virtual void ProcessIncoming(byte[] data)
        {
            var packet = new HMessage(data, HDestination.Client);
            if (_inCallbacks.Count > 0 && _inCallbacks.ContainsKey(packet.Header))
            {
                _inCallbacks[packet.Header](packet);
                packet.Position = 0;
            }
            if (!CaptureEvents) return;
            try
            {
                HMessage previousIncoming = (_previousIncoming.Count > 0 ? _previousIncoming.Pop() : null);
                if (LockEvents && _lockedIn.ContainsKey(packet.Header))
                {
                    _lockedIn[packet.Header](packet);
                    packet = null;
                }
                else ProcessIncoming(previousIncoming, ref packet);
            }
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            finally
            {
                if (packet != null)
                {
                    packet.Position = 0;
                    _previousIncoming.Push(packet);
                }
            }
        }
        private void ProcessIncoming(HMessage previous, ref HMessage current)
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CaptureEvents = LockEvents = false;

                if (_previousOutgoing != null)
                    _previousOutgoing.Clear();

                if (_previousIncoming != null)
                    _previousIncoming.Clear();

                if (_lockedOut != null)
                    _lockedOut.Clear();

                if (_lockedIn != null)
                    _lockedIn.Clear();


                SKore.Unsubscribe(ref HostBanPlayer);
                SKore.Unsubscribe(ref HostChangeClothes);
                SKore.Unsubscribe(ref HostChangeMotto);
                SKore.Unsubscribe(ref HostChangeStance);
                SKore.Unsubscribe(ref HostClickPlayer);
                SKore.Unsubscribe(ref HostDance);
                SKore.Unsubscribe(ref HostGesture);
                SKore.Unsubscribe(ref HostKickPlayer);
                SKore.Unsubscribe(ref HostMoveFurniture);
                SKore.Unsubscribe(ref HostMutePlayer);
                SKore.Unsubscribe(ref HostRaiseSign);
                SKore.Unsubscribe(ref HostRoomExit);
                SKore.Unsubscribe(ref HostRoomNavigate);
                SKore.Unsubscribe(ref HostSay);
                SKore.Unsubscribe(ref HostShout);
                SKore.Unsubscribe(ref HostTradePlayer);
                SKore.Unsubscribe(ref HostWalk);

                SKore.Unsubscribe(ref FurnitureDataLoaded);
                SKore.Unsubscribe(ref PlayerChangeData);
                SKore.Unsubscribe(ref PlayerChangeStance);
                SKore.Unsubscribe(ref PlayerDance);
                SKore.Unsubscribe(ref PlayerDataLoaded);
                SKore.Unsubscribe(ref PlayerDropFurniture);
                SKore.Unsubscribe(ref PlayerGesture);
                SKore.Unsubscribe(ref PlayerKickHost);
                SKore.Unsubscribe(ref PlayerMoveFurniture);
                SKore.Unsubscribe(ref PlayerSpeak);
            }
        }
    }
}