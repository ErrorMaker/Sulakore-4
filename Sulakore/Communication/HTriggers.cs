using System;
using System.Collections.Generic;

using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class HTriggers : IDisposable
    {
        #region Subscribable Events
        public event EventHandler<HostSayEventArgs> HostSay;
        protected virtual void OnHostSay(HMessage packet)
        {
            EventHandler<HostSayEventArgs> handler = HostSay;

            if (handler != null)
                handler(this, new HostSayEventArgs(packet));
        }

        public event EventHandler<HostWalkEventArgs> HostWalk;
        protected virtual void OnHostWalk(HMessage packet)
        {
            EventHandler<HostWalkEventArgs> handler = HostWalk;

            if (handler != null)
                handler(this, new HostWalkEventArgs(packet));
        }

        public event EventHandler<HostDanceEventArgs> HostDance;
        protected virtual void OnHostDance(HMessage packet)
        {
            EventHandler<HostDanceEventArgs> handler = HostDance;

            if (handler != null)
                handler(this, new HostDanceEventArgs(packet));
        }

        public event EventHandler<HostShoutEventArgs> HostShout;
        protected virtual void OnHostShout(HMessage packet)
        {
            EventHandler<HostShoutEventArgs> handler = HostShout;

            if (handler != null)
                handler(this, new HostShoutEventArgs(packet));
        }

        public event EventHandler<HostGestureEventArgs> HostGesture;
        protected virtual void OnHostGesture(HMessage packet)
        {
            EventHandler<HostGestureEventArgs> handler = HostGesture;

            if (handler != null)
                handler(this, new HostGestureEventArgs(packet));
        }

        public event EventHandler<HostRoomExitEventArgs> HostRoomExit;
        protected virtual void OnHostRoomExit(HMessage packet)
        {
            EventHandler<HostRoomExitEventArgs> handler = HostRoomExit;

            if (handler != null)
                handler(this, new HostRoomExitEventArgs(packet));
        }

        public event EventHandler<HostRaiseSignEventArgs> HostRaiseSign;
        protected virtual void OnHostRaiseSign(HMessage packet)
        {
            EventHandler<HostRaiseSignEventArgs> handler = HostRaiseSign;

            if (handler != null)
                handler(this, new HostRaiseSignEventArgs(packet));
        }

        public event EventHandler<HostBanPlayerEventArgs> HostBanPlayer;
        protected virtual void OnHostBanPlayer(HMessage packet)
        {
            EventHandler<HostBanPlayerEventArgs> handler = HostBanPlayer;

            if (handler != null)
                handler(this, new HostBanPlayerEventArgs(packet));
        }

        public event EventHandler<HostMutePlayerEventArgs> HostMutePlayer;
        protected virtual void OnHostMutePlayer(HMessage packet)
        {
            EventHandler<HostMutePlayerEventArgs> handler = HostMutePlayer;

            if (handler != null)
                handler(this, new HostMutePlayerEventArgs(packet));
        }

        public event EventHandler<HostKickPlayerEventArgs> HostKickPlayer;
        protected virtual void OnHostKickPlayer(HMessage packet)
        {
            EventHandler<HostKickPlayerEventArgs> handler = HostKickPlayer;

            if (handler != null)
                handler(this, new HostKickPlayerEventArgs(packet));
        }

        public event EventHandler<HostClickPlayerEventArgs> HostClickPlayer;
        protected virtual void OnHostClickPlayer(HMessage packet)
        {
            EventHandler<HostClickPlayerEventArgs> handler = HostClickPlayer;

            if (handler != null)
                handler(this, new HostClickPlayerEventArgs(packet));
        }

        public event EventHandler<HostMottoChangedEventArgs> HostChangeMotto;
        protected virtual void OnHostChangeMotto(HMessage packet)
        {
            EventHandler<HostMottoChangedEventArgs> handler = HostChangeMotto;

            if (handler != null)
                handler(this, new HostMottoChangedEventArgs(packet));
        }

        public event EventHandler<HostTradePlayerEventArgs> HostTradePlayer;
        protected virtual void OnHostTradePlayer(HMessage packet)
        {
            EventHandler<HostTradePlayerEventArgs> handler = HostTradePlayer;

            if (handler != null)
                handler(this, new HostTradePlayerEventArgs(packet));
        }

        public event EventHandler<HostStanceChangedEventArgs> HostChangeStance;
        protected virtual void OnHostChangeStance(HMessage packet)
        {
            EventHandler<HostStanceChangedEventArgs> handler = HostChangeStance;

            if (handler != null)
                handler(this, new HostStanceChangedEventArgs(packet));
        }

        public event EventHandler<HostRoomNavigateEventArgs> HostRoomNavigate;
        protected virtual void OnHostRoomNavigate(HMessage packet)
        {
            EventHandler<HostRoomNavigateEventArgs> handler = HostRoomNavigate;

            if (handler != null)
                handler(this, new HostRoomNavigateEventArgs(packet));
        }

        public event EventHandler<HostMoveFurnitureEventArgs> HostMoveFurniture;
        protected virtual void OnHostMoveFurniture(HMessage packet)
        {
            EventHandler<HostMoveFurnitureEventArgs> handler = HostMoveFurniture;

            if (handler != null)
                handler(this, new HostMoveFurnitureEventArgs(packet));
        }

        public event EventHandler<HostClothesChangedEventArgs> HostChangeClothes;
        protected virtual void OnHostChangeClothes(HMessage packet)
        {
            EventHandler<HostClothesChangedEventArgs> handler = HostChangeClothes;
            
            if (handler != null)
                handler(this, new HostClothesChangedEventArgs(packet));
        }

        public event EventHandler<PlayerKickedHostEventArgs> PlayerKickedHost;
        protected virtual void OnPlayerKickedHost(HMessage packet)
        {
            EventHandler<PlayerKickedHostEventArgs> handler = PlayerKickedHost;

            if (handler != null)
                handler(this, new PlayerKickedHostEventArgs(packet));
        }

        public event EventHandler<PlayerDataLoadedEventArgs> PlayerDataLoaded;
        protected virtual void OnPlayerDataLoaded(HMessage packet)
        {
            EventHandler<PlayerDataLoadedEventArgs> handler = PlayerDataLoaded;

            if (handler != null)
                handler(this, new PlayerDataLoadedEventArgs(packet));
        }

        public event EventHandler<FurnitureDataLoadedEventArgs> FurnitureDataLoaded;
        protected virtual void OnFurnitureDataLoaded(HMessage packet)
        {
            EventHandler<FurnitureDataLoadedEventArgs> handler = FurnitureDataLoaded;

            if (handler != null)
                handler(this, new FurnitureDataLoadedEventArgs(packet));
        }
        #endregion

        #region Private/Public Fields
        private bool _lockEvents, _captureEvents;
        public delegate void PacketCallback(HMessage packet);

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
            finally
            {
                if (packet != null)
                {
                    packet.Position = 0;
                    _previousOutgoing.Push(packet);
                }
            }
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
            finally
            {
                if (packet != null)
                {
                    packet.Position = 0;
                    _previousIncoming.Push(packet);
                }
            }
        }

        private void ProcessOutgoing(HMessage previous, ref HMessage current)
        {
            if (previous != null)
            {
                if (previous.Length >= 5 && previous.ReadShort(0) < previous.Length)
                {
                    switch (previous.ReadString())
                    {
                        case "OwnAvatarMenu":
                        {
                            if (previous.ReadString() != "click") break;
                            if (previous.ReadString() != "sign") ProcessAvatarMenuEvent(previous, current);
                            break;
                        }
                    }
                    switch (current.ReadString())
                    {
                        case "OwnAvatarMenu":
                        {
                            if (current.ReadString() != "click") break;
                            if (current.ReadString() == "sign") ProcessAvatarMenuEvent(current, previous);
                            break;
                        }
                    }
                    current = null;
                }
            }
            else
            {

            }
        }
        private void ProcessIncoming(HMessage previous, ref HMessage current)
        {
            if (previous != null) current = null;
            else
            {
                string currentPacketString = current.ToString();

                if (current.Length > 100 & currentPacketString.Contains("hd-"))
                    ProcessPlayerDataEvent(current);
                else current = null;
            }
        }

        private void ProcessPlayerDataEvent(HMessage packet)
        {
            int possiblePlayerCount = packet.ReadInt(0);
            int possiblePlayerId = packet.ReadInt(4);
            if (possiblePlayerCount > 0 && possiblePlayerCount < 250 && possiblePlayerId > 0)
            {
                if (LockEvents) _lockedOut[packet.Header] = OnPlayerDataLoaded;
                OnPlayerDataLoaded(packet);
            }
        }
        private void ProcessAvatarMenuEvent(HMessage logPacket, HMessage actionPacket)
        {
            int position = 0;
            logPacket.ReadString(ref position);
            logPacket.ReadString(ref position);
            switch (logPacket.ReadString(ref position))
            {
                case "sign":
                {
                    if (LockEvents) _lockedOut[actionPacket.Header] = OnHostRaiseSign;
                    OnHostRaiseSign(actionPacket); break;
                }
                case "stand":
                case "sit":
                {
                    if (LockEvents) _lockedOut[actionPacket.Header] = OnHostChangeStance;
                    OnHostChangeStance(actionPacket); break;
                }
                case "wave":
                case "idle":
                case "blow":
                case "laugh":
                {
                    if (LockEvents) _lockedOut[actionPacket.Header] = OnHostGesture;
                    OnHostGesture(actionPacket); break;
                }
                case "dance_stop":
                case "dance_start":
                {
                    if (LockEvents) _lockedOut[actionPacket.Header] = OnHostDance;
                    OnHostDance(actionPacket); break;
                }
            }
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
            }

            SKore.Unsubscribe(ref HostSay);
            SKore.Unsubscribe(ref HostWalk);
            SKore.Unsubscribe(ref HostDance);
            SKore.Unsubscribe(ref HostShout);
            SKore.Unsubscribe(ref HostGesture);
            SKore.Unsubscribe(ref HostRoomExit);
            SKore.Unsubscribe(ref HostRaiseSign);
            SKore.Unsubscribe(ref HostBanPlayer);
            SKore.Unsubscribe(ref HostMutePlayer);
            SKore.Unsubscribe(ref HostKickPlayer);
            SKore.Unsubscribe(ref HostClickPlayer);
            SKore.Unsubscribe(ref HostChangeMotto);
            SKore.Unsubscribe(ref HostTradePlayer);
            SKore.Unsubscribe(ref HostChangeStance);
            SKore.Unsubscribe(ref HostRoomNavigate);
            SKore.Unsubscribe(ref HostMoveFurniture);
            SKore.Unsubscribe(ref HostChangeClothes);
            SKore.Unsubscribe(ref PlayerKickedHost);
            SKore.Unsubscribe(ref PlayerDataLoaded);
            SKore.Unsubscribe(ref FurnitureDataLoaded);
        }
    }
}