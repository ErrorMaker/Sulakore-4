using System;
using Sulakore.Habbo;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public abstract class HTriggerBase : IDisposable
    {
        private HMessage _lastToServer;

        #region Game Connection Events
        public event EventHandler<HostSayEventArgs> OnHostSay;
        public event EventHandler<HostExitEventArgs> OnHostExit;
        public event EventHandler<HostWalkEventArgs> OnHostWalk;
        public event EventHandler<HostSignEventArgs> OnHostSign;
        public event EventHandler<HostDanceEventArgs> OnHostDance;
        public event EventHandler<HostShoutEventArgs> OnHostShout;
        public event EventHandler<HostTradeEventArgs> OnHostTrade;
        public event EventHandler<HostKickedEventArgs> OnHostKicked;
        public event EventHandler<HostGestureEventArgs> OnHostGesture;
        public event EventHandler<HostNavigateEventArgs> OnHostNavigate;
        public event EventHandler<HostBanPlayerEventArgs> OnHostBanPlayer;
        public event EventHandler<HostMutePlayerEventArgs> OnHostMutePlayer;
        public event EventHandler<HostKickPlayerEventArgs> OnHostKickPlayer;
        public event EventHandler<HostClickPlayerEventArgs> OnHostClickPlayer;
        public event EventHandler<HostChangeMottoEventArgs> OnHostChangeMotto;
        public event EventHandler<HostChangeStanceEventArgs> OnHostChangeStance;
        public event EventHandler<HostMoveFurnitureEventArgs> OnHostMoveFurniture;
        public event EventHandler<HostChangeClothesEventArgs> OnHostChangeClothes;

        public event EventHandler<PlayerSayEventArgs> OnPlayerSay;
        public event EventHandler<PlayerWalkEventArgs> OnPlayerWalk;
        public event EventHandler<PlayerSignEventArgs> OnPlayerSign;
        public event EventHandler<PlayerShoutEventArgs> OnPlayerShout;
        public event EventHandler<PlayerEnterEventArgs> OnPlayerEnter;
        public event EventHandler<PlayerDanceEventArgs> OnPlayerDance;
        public event EventHandler<PlayerGestureEventArgs> OnPlayerGesture;
        public event EventHandler<PlayerChangeDataEventArgs> OnPlayerChangeData;
        public event EventHandler<PlayerChangeStanceEventArgs> OnPlayerChangeStance;
        public event EventHandler<PlayerMoveFurnitureEventArgs> OnPlayerMoveFurniture;
        public event EventHandler<PlayerDropFurnitureEventArgs> OnPlayerDropFurniture;
        #endregion

        protected void TriggerHostEvents(byte[] data)
        {
            var packet = new HMessage(data);
            try
            {
                if (OnHostSay != null && packet.IsHostSay)
                { _lastToServer = packet; OnHostSay(this, HostSayEventArgs.CreateArguments(packet)); return; }

                if (OnHostShout != null && packet.IsHostShout)
                { _lastToServer = packet; OnHostShout(this, HostShoutEventArgs.CreateArguments(packet)); return; }

                if (OnHostExit != null && _lastToServer != null && packet.IsHostExit && !_lastToServer.IsHostExit)
                { HMessage lastToServer = _lastToServer; _lastToServer = packet; OnHostExit(this, HostExitEventArgs.CreateArguments(lastToServer)); return; }

                if (OnHostTrade != null && _lastToServer != null && _lastToServer.IsHostTrade)
                { _lastToServer = packet; OnHostTrade(this, HostTradeEventArgs.CreateArguments(packet)); return; }

                if (OnHostSign != null && _lastToServer != null && !_lastToServer.IsHostSign && packet.IsHostSign)
                { HMessage lastToServer = _lastToServer; _lastToServer = packet; OnHostSign(this, HostSignEventArgs.CreateArguments(lastToServer)); return; }

                if (OnHostNavigate != null && _lastToServer != null && !_lastToServer.IsHostNavigate && packet.IsHostNavigate)
                { HMessage lastToServer = _lastToServer; _lastToServer = packet; OnHostNavigate(this, HostNavigateEventArgs.CreateArguments(lastToServer)); return; }

                if (OnHostGesture != null && _lastToServer != null && !packet.IsHostGesture && _lastToServer.IsHostGesture)
                { _lastToServer = packet; OnHostGesture(this, HostGestureEventArgs.CreateArguments(packet)); return; }

                if (OnHostChangeStance != null && _lastToServer != null && _lastToServer.IsHostChangeStance)
                { _lastToServer = packet; OnHostChangeStance(this, HostChangeStanceEventArgs.CreateArguments(packet)); return; }

                if (OnHostKickPlayer != null && _lastToServer != null && _lastToServer.IsHostKickPlayer)
                { _lastToServer = packet; OnHostKickPlayer(this, HostKickPlayerEventArgs.CreateArguments(packet)); return; }

                if (OnHostMutePlayer != null && _lastToServer != null && _lastToServer.IsHostMutePlayer)
                { _lastToServer = packet; OnHostMutePlayer(this, HostMutePlayerEventArgs.CreateArguments(packet)); return; }

                if (_lastToServer != null && _lastToServer.IsHostDance && OnHostDance != null)
                { _lastToServer = packet; OnHostDance(this, HostDanceEventArgs.CreateArguments(packet)); return; }

                if (_lastToServer != null && _lastToServer.IsHostBanPlayer && OnHostBanPlayer != null)
                { _lastToServer = packet; OnHostBanPlayer(this, HostBanPlayerEventArgs.CreateArguments(packet)); return; }

                if (_lastToServer != null && _lastToServer.IsCoordinate && packet.IsPossiblePlayerId && OnHostClickPlayer != null)
                { HMessage lastToServer = _lastToServer; _lastToServer = packet; OnHostClickPlayer(this, HostClickPlayerEventArgs.CreateArguments(lastToServer, packet.ReadInt(0))); return; }
                _lastToServer = packet;
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
        }
        protected void TriggerPlayerEvents(byte[] data)
        {
            var packet = new HMessage(data);
            try
            {
                if (_lastToServer != null && _lastToServer.Header == HHeaders.Dance)
                    HHeaders.PlayerDance = packet.Header;

                if (_lastToServer != null && packet.Length == 10 && _lastToServer.Header == HHeaders.Gesture)
                    HHeaders.PlayerGesture = packet.Header;

                if (_lastToServer != null && HHeaders.PlayerSay == 0 && packet.IsPlayerTalking && _lastToServer.Header == HHeaders.Say)
                    HHeaders.PlayerSay = packet.Header;

                if (_lastToServer != null && HHeaders.PlayerShout == 0 && packet.IsPlayerTalking && _lastToServer.Header == HHeaders.Shout)
                    HHeaders.PlayerShout = packet.Header;

                if (packet.IsPlayerSign && OnPlayerSign != null)
                {
                    OnPlayerSign(this, PlayerSignEventArgs.CreateArguments(packet)); return;
                }

                if (packet.IsHostKicked && OnHostKicked != null)
                { OnHostKicked(this, HostKickedEventArgs.CreateArguments(packet)); return; }

                if (packet.IsPlayerChangeData && OnPlayerChangeData != null)
                { OnPlayerChangeData(this, PlayerChangeDataEventArgs.CreateArguments(packet)); return; }

                if (packet.IsPlayerDropFurniture && OnPlayerDropFurniture != null)
                { OnPlayerDropFurniture(this, PlayerDropFurnitureEventArgs.CreateArguments(packet)); return; }

                if (packet.IsPlayerMoveFurniture)
                {
                    if (_lastToServer != null && OnHostMoveFurniture != null && _lastToServer.IsHostMoveFurniture && _lastToServer.ReadInt(0) == packet.ReadInt(0))
                    { HMessage lastToServer = _lastToServer; _lastToServer = null; OnHostMoveFurniture(this, HostMoveFurnitureEventArgs.CreateArguments(lastToServer, packet.ReadString(20))); }

                    if (OnPlayerMoveFurniture != null)
                        OnPlayerMoveFurniture(this, PlayerMoveFurnitureEventArgs.CreateArguments(packet));
                    return;
                }

                if (_lastToServer != null && packet.IsHostChangeData)
                {
                    if (_lastToServer.IsHostChangeMotto && OnHostChangeMotto != null)
                    { HMessage lastToServer = _lastToServer; _lastToServer = null; OnHostChangeMotto(this, HostChangeMottoEventArgs.CreateArguments(lastToServer)); return; }

                    if (_lastToServer.IsHostChangeClothes && OnHostChangeClothes != null)
                    { HMessage lastToServer = _lastToServer; _lastToServer = null; OnHostChangeClothes(this, HostChangeClothesEventArgs.CreateArguments(lastToServer)); return; }
                }

                if (packet.IsPlayerEntering && OnPlayerEnter != null)
                { OnPlayerEnter(this, PlayerEnterEventArgs.CreateArguments(packet)); return; }

                if (packet.Length == 10 && packet.Header == HHeaders.PlayerDance && OnPlayerDance != null)
                { OnPlayerDance(this, PlayerDanceEventArgs.CreateArguments(packet)); return; }

                if (packet.Header == HHeaders.PlayerGesture && OnPlayerGesture != null)
                { OnPlayerGesture(this, PlayerGestureEventArgs.CreateArguments(packet)); return; }

                if (packet.IsMultiplePlayerMovement)
                {
                    if (OnPlayerWalk != null)
                    {
                        if (PlayerWalkEventArgs.HasMultiplePlayers(packet))
                        {
                            PlayerWalkEventArgs[] playerWalkList = PlayerWalkEventArgs.GetPlayerWalkList(packet);
                            foreach (PlayerWalkEventArgs walkEventArgs in playerWalkList)
                                OnPlayerWalk(this, walkEventArgs);
                        }
                    }
                    if (OnPlayerChangeStance != null)
                    {
                        if (PlayerChangeStanceEventArgs.HasMultiplePlayers(packet))
                        {
                            PlayerChangeStanceEventArgs[] stanceList = PlayerChangeStanceEventArgs.GetPlayerChangeStanceList(packet);
                            foreach (PlayerChangeStanceEventArgs stanceEventArgs in stanceList)
                                OnPlayerChangeStance(this, stanceEventArgs);
                        }
                    }
                }

                if (packet.IsPlayerChangeStance && OnPlayerChangeStance != null)
                { OnPlayerChangeStance(this, PlayerChangeStanceEventArgs.CreateArguments(packet)); return; }

                if (packet.IsPlayerWalking && OnPlayerWalk != null)
                {
                    if (_lastToServer != null && _lastToServer.IsCoordinate && OnHostWalk != null)
                    { HMessage lastToServer = _lastToServer; _lastToServer = null; OnHostWalk(this, HostWalkEventArgs.CreateArguments(lastToServer)); }
                    OnPlayerWalk(this, PlayerWalkEventArgs.CreateArguments(packet));

                    if (packet.IsPlayerSign && OnPlayerSign != null)
                        OnPlayerSign(this, PlayerSignEventArgs.CreateArguments(packet));
                    return;
                }

                if (packet.Header == HHeaders.PlayerShout && OnPlayerShout != null)
                { OnPlayerShout(this, PlayerShoutEventArgs.CreateArguments(packet)); return; }

                if (packet.Header == HHeaders.PlayerSay && OnPlayerSay != null)
                { OnPlayerSay(this, PlayerSayEventArgs.CreateArguments(packet)); }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
        }

        public virtual void Dispose()
        {
            SKore.Unsubscribe(ref OnHostSay);
            SKore.Unsubscribe(ref OnHostExit);
            SKore.Unsubscribe(ref OnHostWalk);
            SKore.Unsubscribe(ref OnHostSign);
            SKore.Unsubscribe(ref OnHostDance);
            SKore.Unsubscribe(ref OnHostShout);
            SKore.Unsubscribe(ref OnHostTrade);
            SKore.Unsubscribe(ref OnHostKicked);
            SKore.Unsubscribe(ref OnHostGesture);
            SKore.Unsubscribe(ref OnHostNavigate);
            SKore.Unsubscribe(ref OnHostBanPlayer);
            SKore.Unsubscribe(ref OnHostMutePlayer);
            SKore.Unsubscribe(ref OnHostKickPlayer);
            SKore.Unsubscribe(ref OnHostClickPlayer);
            SKore.Unsubscribe(ref OnHostChangeMotto);
            SKore.Unsubscribe(ref OnHostChangeStance);
            SKore.Unsubscribe(ref OnHostMoveFurniture);
            SKore.Unsubscribe(ref OnHostChangeClothes);

            SKore.Unsubscribe(ref OnPlayerSay);
            SKore.Unsubscribe(ref OnPlayerWalk);
            SKore.Unsubscribe(ref OnPlayerSign);
            SKore.Unsubscribe(ref OnPlayerShout);
            SKore.Unsubscribe(ref OnPlayerEnter);
            SKore.Unsubscribe(ref OnPlayerDance);
            SKore.Unsubscribe(ref OnPlayerGesture);
            SKore.Unsubscribe(ref OnPlayerChangeData);
            SKore.Unsubscribe(ref OnPlayerChangeStance);
            SKore.Unsubscribe(ref OnPlayerMoveFurniture);
            SKore.Unsubscribe(ref OnPlayerDropFurniture);

            _lastToServer = null;
        }
    }
}