using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public enum STATE_TYPE
    {
        LOGIN_STATE,
        LOBBY_STATE,
    }

    abstract public class State : MonoBehaviour
    {
        public abstract void Recv(Byte[] _buf, uint _protocol);
        public abstract void RecvEvent(t_Eve _eve);

        public abstract void Send(Byte[] _buf, uint _protocol);
        public abstract void SendEvent(t_Eve _eve);
    }
}


