using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;

namespace test_client_unity
{
    public struct t_Eve
    {
        public int eve;
        public int state_type;
        public Byte[] buf;
        public int buf_size;
    }

    public class NetMgr : Singleton<NetMgr>
    {
        // NullReferenceException이 발생하는 이유
        // - 사용시 꼭 동적할당을 한 다음 사용해야 한다.
        public Network m_netWork = new Network(); // 소켓용도

        public Thread m_RecvTrd;

        #region state init
        [HideInInspector]
        public State m_curState;
        [HideInInspector]
        public LoginState m_loginState;
        [HideInInspector]
        public LobbyState m_lobbyState;
        [HideInInspector]
        public StageState m_stageState;
        #endregion

        public Queue<t_Eve> m_recvQue = new Queue<t_Eve>();
        public Queue<t_Eve> m_sendQue = new Queue<t_Eve>();


        // 궁금점
        // - Awake는 코드가 실행이 안된다.
        void Start()
        {
            Init();

            m_netWork.Connect("127.0.0.1", 9000);

            m_curState = m_loginState; // state를 변경

            // Recv Thread 생성
            m_RecvTrd = new Thread(() => RecvProc(this));

            // Thread 시작
            m_RecvTrd.Start();
        }

        public void Init()
        {
            NetMgr netMgr = NetMgr.Instance;
            WindowMgr windowMgr = WindowMgr.Instance;
            LoginMgr loginMgr = LoginMgr.Instance;
            LobbyMgr lobbyMgr = LobbyMgr.Instance;
            ProtocolMgr protocolMgr = ProtocolMgr.Instance;

            m_loginState = new LoginState();
            m_lobbyState = new LobbyState();
            m_stageState = new StageState();
        }

        void Update()
        {
            SendQueProc(this);

            RecvQueProc(this);
        }

        public void SendQueProc(NetMgr _net)
        {
            if (_net.m_sendQue.Count > 0)
            {
                t_Eve eve = m_sendQue.Dequeue();
                _net.m_netWork.Send(eve.buf, eve.buf_size);
            }
        }

        public void RecvQueProc(NetMgr _net)
        {
            if (_net.m_recvQue.Count > 0)
            {
                t_Eve eve = _net.m_recvQue.Dequeue();
                _net.m_curState.RecvEvent(eve);

                //switch (eve.state_type)
                //{
                //    case (int)STATE_TYPE.LOGIN_STATE:
                //        {
                //            _net.m_curState = m_loginState;
                //            _net.m_curState.RecvEvent(eve);
                //        }
                //        break;
                //    case (int)STATE_TYPE.LOBBY_STATE:
                //        {
                //            _net.m_curState = _net.m_lobbyState;
                //            _net.m_curState.RecvEvent(eve);
                //        }
                //        break;
                //}
            }
        }

        void OnApplicationQuit() // 프로그램 종료시 소켓반환
        {
            this.m_netWork.m_socket.Close();
            Debug.Log("연결 종료");
        }

        public void RecvProc(NetMgr _net)
        {
            while (true)
            {
                Byte[] buf = new Byte[4096];
                int size = new int();

                size = _net.m_netWork.Recv(buf, 4);
                if (size == -1)
                    return;

                Byte[] data_size = new Byte[4];
                Array.Copy(buf, data_size, sizeof(int));

                size = _net.m_netWork.Recv(buf, BitConverter.ToUInt16(data_size));
                if (size == -1)
                    return;

                // recv는 버퍼를 언팩하는 일을 함.
                // main을 언팩 -> sub를 언팩 -> detail을 언팩
                // 이벤트에 따라서 하는 일을 정함.

                // protocol에 따라서 state를 나누고
                // 각 state의 recv에선 event를 켜주는 일을 한다.

                Byte[] protocol = _net.m_netWork.GetProtocol(buf);

                _net.m_curState.Recv(buf, protocol);
            }

        }
    }
}
