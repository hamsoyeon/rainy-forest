using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class StageState : State
    {
        public enum EVENT { }
        public override void Recv(Byte[] _buf, Byte[] _protocol)
        {

        }
        public override void RecvEvent(t_Eve _teve)
        {

        }

        public override void Send(Byte[] _buf, int _protocol)
        {
            switch (_protocol)
            {

            }
        }

        public override void SendEvent(t_Eve _eve)
        {
            // 이벤트 발생시 send
            switch (_eve.eve)
            {

            }
        }
    }
}