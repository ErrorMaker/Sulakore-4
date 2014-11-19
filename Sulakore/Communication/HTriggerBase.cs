using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public abstract class HTriggerBase : IDisposable
    {
        #region Private Fields
        protected abstract IHConnection Connection { get; }

        private Action<HMessage, HMessage> _outEventDitch, _inEventDitch;

        private readonly Stack<HMessage> _previousOutgoing = new Stack<HMessage>();
        private readonly Stack<HMessage> _previousIncoming = new Stack<HMessage>();
        private readonly Dictionary<ushort, Action<HMessage>> _lockedOutgoing = new Dictionary<ushort, Action<HMessage>>();
        private readonly Dictionary<ushort, Action<HMessage>> _lockedIncoming = new Dictionary<ushort, Action<HMessage>>();
        #endregion

        #region Public Properties
        private bool _lockEvents;
        public bool LockEvents
        {
            get { return _lockEvents; }
            set
            {
                if (value == _lockEvents) return;

                if (!(_lockEvents = value))
                {
                    _lockedOutgoing.Clear();
                    _lockedIncoming.Clear();
                }
            }
        }
        #endregion

        #region Game Connection Events
        public event EventHandler<HostSayEventArgs> HostSay;
        public event EventHandler<HostWalkEventArgs> HostWalk;
        public event EventHandler<HostDanceEventArgs> HostDance; //
        public event EventHandler<HostShoutEventArgs> HostShout;
        public event EventHandler<HostGestureEventArgs> HostGesture; //
        public event EventHandler<HostRoomExitEventArgs> HostRoomExit;
        public event EventHandler<HostRaiseSignEventArgs> HostRaiseSign; //
        public event EventHandler<HostBanPlayerEventArgs> HostBanPlayer;
        public event EventHandler<HostMutePlayerEventArgs> HostMutePlayer;
        public event EventHandler<HostKickPlayerEventArgs> HostKickPlayer;
        public event EventHandler<HostClickPlayerEventArgs> HostClickPlayer;
        public event EventHandler<HostMottoChangedEventArgs> HostChangeMotto;
        public event EventHandler<HostTradePlayerEventArgs> HostTradePlayer;
        public event EventHandler<HostStanceChangedEventArgs> HostChangeStance; //
        public event EventHandler<HostRoomNavigateEventArgs> HostRoomNavigate;
        public event EventHandler<HostMoveFurnitureEventArgs> HostMoveFurniture;
        public event EventHandler<HostClothesChangedEventArgs> HostChangeClothes;

        public event EventHandler<PlayerKickedHostEventArgs> PlayerKickedHost;
        public event EventHandler<PlayerDataLoadedEventArgs> PlayerDataLoaded; //
        public event EventHandler<FurnitureDataLoadedEventArgs> FurnitureDataLoaded;
        #endregion

        #region Game Connection Event Overrides
        protected virtual void OnHostSay(HMessage packet)
        {
            if (HostSay != null)
                HostSay(Connection, new HostSayEventArgs(packet));
        }
        protected virtual void OnHostWalk(HMessage packet)
        {
            if (HostWalk != null)
                HostWalk(Connection, new HostWalkEventArgs(packet));
        }
        protected virtual void OnHostDance(HMessage packet)
        {
            if (HostDance != null)
                HostDance(Connection, new HostDanceEventArgs(packet));
        }
        protected virtual void OnHostShout(HMessage packet)
        {
            if (HostShout != null)
                HostShout(Connection, new HostShoutEventArgs(packet));
        }
        protected virtual void OnHostGesture(HMessage packet)
        {
            if (HostGesture != null)
                HostGesture(Connection, new HostGestureEventArgs(packet));
        }
        protected virtual void OnHostRoomExit(HMessage packet)
        {
            if (HostRoomExit != null)
                HostRoomExit(Connection, new HostRoomExitEventArgs(packet));
        }
        protected virtual void OnHostRaiseSign(HMessage packet)
        {
            if (HostRaiseSign != null)
                HostRaiseSign(Connection, new HostRaiseSignEventArgs(packet));
        }
        protected virtual void OnHostBanPlayer(HMessage packet)
        {
            if (HostBanPlayer != null)
                HostBanPlayer(Connection, new HostBanPlayerEventArgs(packet));
        }
        protected virtual void OnHostMutePlayer(HMessage packet)
        {
            if (HostMutePlayer != null)
                HostMutePlayer(Connection, new HostMutePlayerEventArgs(packet));
        }
        protected virtual void OnHostKickPlayer(HMessage packet)
        {
            if (HostKickPlayer != null)
                HostKickPlayer(Connection, new HostKickPlayerEventArgs(packet));
        }
        protected virtual void OnHostClickPlayer(HMessage packet)
        {
            if (HostClickPlayer != null)
                HostClickPlayer(Connection, new HostClickPlayerEventArgs(packet));
        }
        protected virtual void OnHostChangeMotto(HMessage packet)
        {
            if (HostChangeMotto != null)
                HostChangeMotto(Connection, new HostMottoChangedEventArgs(packet));
        }
        protected virtual void OnHostTradePlayer(HMessage packet)
        {
            if (HostTradePlayer != null)
                HostTradePlayer(Connection, new HostTradePlayerEventArgs(packet));
        }
        protected virtual void OnHostChangeStance(HMessage packet)
        {
            if (HostChangeStance != null)
                HostChangeStance(Connection, new HostStanceChangedEventArgs(packet));
        }
        protected virtual void OnHostRoomNavigate(HMessage packet)
        {
            if (HostRoomNavigate != null)
                HostRoomNavigate(Connection, new HostRoomNavigateEventArgs(packet));
        }
        protected virtual void OnHostMoveFurniture(HMessage packet)
        {
            if (HostMoveFurniture != null)
                HostMoveFurniture(Connection, new HostMoveFurnitureEventArgs(packet));
        }
        protected virtual void OnHostChangeClothes(HMessage packet)
        {
            if (HostChangeClothes != null)
                HostChangeClothes(Connection, new HostClothesChangedEventArgs(packet));
        }

        protected virtual void OnPlayerKickedHost(HMessage packet)
        {
            if (PlayerKickedHost != null)
                PlayerKickedHost(Connection, new PlayerKickedHostEventArgs(packet));
        }
        protected virtual void OnPlayerDataLoaded(HMessage packet)
        {
            if (PlayerDataLoaded != null)
                PlayerDataLoaded(Connection, new PlayerDataLoadedEventArgs(packet));
        }
        #endregion

        protected virtual void ProcessOutgoing(byte[] data)
        {
            var packet = new HMessage(data, HDestinations.Server);
            try
            {
                HMessage previousOutgoing = _previousOutgoing.Count > 0 ? _previousOutgoing.Pop() : null;
                if (LockEvents && _lockedOutgoing.ContainsKey(packet.Header))
                {
                    _lockedOutgoing[packet.Header](packet);
                    packet = null;
                }
                else ProcessOutgoing(previousOutgoing, ref packet);
            }
            catch (Exception ex)
            {
                SKore.Debugger(ex.ToString());
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
        private void ProcessOutgoing(HMessage previous, ref HMessage current)
        {
            if (_outEventDitch != null)
            {
                _outEventDitch(previous, current);
                _outEventDitch = null;
                current = null;
            }
            else if (previous != null)
            {
                if (previous.Length >= 5 && previous.ReadShort(0) < previous.Length)
                {
                    switch (previous.ReadString())
                    {
                        case "OwnAvatarMenu":
                        {
                            if (previous.ReadString() != "click") break;
                            if (previous.ReadString() != "sign")
                                ProcessAvatarMenuEvent(previous, current);
                            break;
                        }
                    }
                    switch (current.ReadString())
                    {
                        case "OwnAvatarMenu":
                        {
                            if (current.ReadString() != "click") break;
                            if (current.ReadString() == "sign")
                            {
                                ProcessAvatarMenuEvent(current, previous);
                            }
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

        protected virtual void ProcessIncoming(byte[] data)
        {
            var packet = new HMessage(data, HDestinations.Client);
            try
            {
                HMessage previousIncoming = _previousIncoming.Count > 0 ? _previousIncoming.Pop() : null;
                if (LockEvents && _lockedIncoming.ContainsKey(packet.Header))
                {
                    _lockedIncoming[packet.Header](packet);
                    packet = null;
                }
                else ProcessIncoming(previousIncoming, ref packet);
            }
            catch (Exception ex)
            {
                SKore.Debugger(ex.ToString());
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
        private void ProcessIncoming(HMessage previous, ref HMessage current)
        {
            if (_inEventDitch != null)
            {
                _inEventDitch(previous, current);
                _inEventDitch = null;
                current = null;
            }
            else if (previous != null) current = null;
            else
            {
                if (current.Length > 100 & current.ToString().Contains("hd-"))
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
                if (LockEvents) _lockedOutgoing[packet.Header] = OnPlayerDataLoaded;
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
                    if (LockEvents) _lockedOutgoing[actionPacket.Header] = OnHostRaiseSign;
                    OnHostRaiseSign(actionPacket); break;
                }
                case "stand":
                case "sit":
                {
                    if (LockEvents) _lockedOutgoing[actionPacket.Header] = OnHostChangeStance;
                    OnHostChangeStance(actionPacket); break;
                }
                case "wave":
                case "idle":
                case "blow":
                case "laugh":
                {
                    if (LockEvents) _lockedOutgoing[actionPacket.Header] = OnHostGesture;
                    OnHostGesture(actionPacket); break;
                }
                case "dance_stop":
                case "dance_start":
                {
                    if (LockEvents) _lockedOutgoing[actionPacket.Header] = OnHostDance;
                    OnHostDance(actionPacket); break;
                }
            }
        }

        public virtual void Dispose()
        {
            SKore.Unsubscribe(ref HostSay);
            SKore.Unsubscribe(ref HostRoomExit);
            SKore.Unsubscribe(ref HostWalk);
            SKore.Unsubscribe(ref HostRaiseSign);
            SKore.Unsubscribe(ref HostDance);
            SKore.Unsubscribe(ref HostShout);
            SKore.Unsubscribe(ref HostTradePlayer);
            SKore.Unsubscribe(ref HostGesture);
            SKore.Unsubscribe(ref HostRoomNavigate);
            SKore.Unsubscribe(ref HostBanPlayer);
            SKore.Unsubscribe(ref HostMutePlayer);
            SKore.Unsubscribe(ref HostKickPlayer);
            SKore.Unsubscribe(ref HostClickPlayer);
            SKore.Unsubscribe(ref HostChangeMotto);
            SKore.Unsubscribe(ref HostChangeStance);
            SKore.Unsubscribe(ref HostMoveFurniture);
            SKore.Unsubscribe(ref HostChangeClothes);

            SKore.Unsubscribe(ref PlayerKickedHost);
            SKore.Unsubscribe(ref PlayerDataLoaded);
        }
    }
}