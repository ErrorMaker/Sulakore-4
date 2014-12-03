using System;
using Sulakore.Protocol;
using System.Collections.Generic;
using Sulakore.Habbo;

namespace Sulakore.Communication
{
    public abstract class HTriggerBase : IDisposable
    {
        #region Subscribable Events
        public event EventHandler<HostSayEventArgs> HostSay;
        protected virtual void OnHostSay(HMessage packet)
        {
            if (HostSay != null)
                HostSay(this, new HostSayEventArgs(packet));
        }

        public event EventHandler<HostWalkEventArgs> HostWalk;
        protected virtual void OnHostWalk(HMessage packet)
        {
            if (HostWalk != null)
                HostWalk(this, new HostWalkEventArgs(packet));
        }

        public event EventHandler<HostDanceEventArgs> HostDance;
        protected virtual void OnHostDance(HMessage packet)
        {
            if (HostDance != null)
                HostDance(this, new HostDanceEventArgs(packet));
        }

        public event EventHandler<HostShoutEventArgs> HostShout;
        protected virtual void OnHostShout(HMessage packet)
        {
            if (HostShout != null)
                HostShout(this, new HostShoutEventArgs(packet));
        }

        public event EventHandler<HostGestureEventArgs> HostGesture;
        protected virtual void OnHostGesture(HMessage packet)
        {
            if (HostGesture != null)
                HostGesture(this, new HostGestureEventArgs(packet));
        }

        public event EventHandler<HostRoomExitEventArgs> HostRoomExit;
        protected virtual void OnHostRoomExit(HMessage packet)
        {
            if (HostRoomExit != null)
                HostRoomExit(this, new HostRoomExitEventArgs(packet));
        }

        public event EventHandler<HostRaiseSignEventArgs> HostRaiseSign;
        protected virtual void OnHostRaiseSign(HMessage packet)
        {
            if (HostRaiseSign != null)
                HostRaiseSign(this, new HostRaiseSignEventArgs(packet));
        }

        public event EventHandler<HostBanPlayerEventArgs> HostBanPlayer;
        protected virtual void OnHostBanPlayer(HMessage packet)
        {
            if (HostBanPlayer != null)
                HostBanPlayer(this, new HostBanPlayerEventArgs(packet));
        }

        public event EventHandler<HostMutePlayerEventArgs> HostMutePlayer;
        protected virtual void OnHostMutePlayer(HMessage packet)
        {
            if (HostMutePlayer != null)
                HostMutePlayer(this, new HostMutePlayerEventArgs(packet));
        }

        public event EventHandler<HostKickPlayerEventArgs> HostKickPlayer;
        protected virtual void OnHostKickPlayer(HMessage packet)
        {
            if (HostKickPlayer != null)
                HostKickPlayer(this, new HostKickPlayerEventArgs(packet));
        }

        public event EventHandler<HostClickPlayerEventArgs> HostClickPlayer;
        protected virtual void OnHostClickPlayer(HMessage packet)
        {
            if (HostClickPlayer != null)
                HostClickPlayer(this, new HostClickPlayerEventArgs(packet));
        }

        public event EventHandler<HostMottoChangedEventArgs> HostChangeMotto;
        protected virtual void OnHostChangeMotto(HMessage packet)
        {
            if (HostChangeMotto != null)
                HostChangeMotto(this, new HostMottoChangedEventArgs(packet));
        }

        public event EventHandler<HostTradePlayerEventArgs> HostTradePlayer;
        protected virtual void OnHostTradePlayer(HMessage packet)
        {
            if (HostTradePlayer != null)
                HostTradePlayer(this, new HostTradePlayerEventArgs(packet));
        }

        public event EventHandler<HostStanceChangedEventArgs> HostChangeStance;
        protected virtual void OnHostChangeStance(HMessage packet)
        {
            if (HostChangeStance != null)
                HostChangeStance(this, new HostStanceChangedEventArgs(packet));
        }

        public event EventHandler<HostRoomNavigateEventArgs> HostRoomNavigate;
        protected virtual void OnHostRoomNavigate(HMessage packet)
        {
            if (HostRoomNavigate != null)
                HostRoomNavigate(this, new HostRoomNavigateEventArgs(packet));
        }

        public event EventHandler<HostMoveFurnitureEventArgs> HostMoveFurniture;
        protected virtual void OnHostMoveFurniture(HMessage packet)
        {
            if (HostMoveFurniture != null)
                HostMoveFurniture(this, new HostMoveFurnitureEventArgs(packet));
        }

        public event EventHandler<HostClothesChangedEventArgs> HostChangeClothes;
        protected virtual void OnHostChangeClothes(HMessage packet)
        {
            if (HostChangeClothes != null)
                HostChangeClothes(this, new HostClothesChangedEventArgs(packet));
        }

        public event EventHandler<PlayerKickedHostEventArgs> PlayerKickedHost;
        protected virtual void OnPlayerKickedHost(HMessage packet)
        {
            if (PlayerKickedHost != null)
                PlayerKickedHost(this, new PlayerKickedHostEventArgs(packet));
        }

        public event EventHandler<PlayerDataLoadedEventArgs> PlayerDataLoaded;
        protected virtual void OnPlayerDataLoaded(HMessage packet)
        {
            if (PlayerDataLoaded != null)
                PlayerDataLoaded(this, new PlayerDataLoadedEventArgs(packet));
        }

        public event EventHandler<FurnitureDataLoadedEventArgs> FurnitureDataLoaded;
        protected virtual void OnFurnitureDataLoaded(HMessage packet)
        {
            if (FurnitureDataLoaded != null)
                FurnitureDataLoaded(this, new FurnitureDataLoadedEventArgs(packet));
        }
        #endregion

        #region Private Fields
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

        #region Virtual Methods
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
        #endregion

        #region Private Methods
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
        #endregion
    }
}