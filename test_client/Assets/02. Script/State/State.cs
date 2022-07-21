using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    abstract public class State
    {
        public abstract void Recv(Byte[] _buf, uint _protocol);
        public abstract void RecvEvent(t_Eve _eve);

    }

    public class StateMgr : Singleton<StateMgr>
    {
        private LoginState loginState;
        private LobbyState lobbyState;

        public LoginState GetLogin
        {
            get
            {
                return loginState;
            }
        }
        public LobbyState GetLobby
        {
            get
            {
                return lobbyState;
            }
        }

        public StateMgr()
        {
            loginState = new LoginState();
            lobbyState = new LobbyState();
        }
    }
}


