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
        public Network      m_netWork = new Network(); // 소켓용도

        public Thread       m_RecvTrd;
        public Thread       m_RecvQueTrd;
        public Thread       m_SendQueTrd;

        #region state init
        [HideInInspector]
        public State       m_curState;
        [HideInInspector]
        public LoginState  m_loginState = new LoginState();
        [HideInInspector]
        public LobbyState  m_lobbyState = new LobbyState();
        #endregion

        public Queue<t_Eve> m_recvQue = new Queue<t_Eve>();
        public Queue<t_Eve> m_sendQue = new Queue<t_Eve>();

        // 궁금점
        // - Awake는 코드가 실행이 안된다.
        void Start()
        {
            m_netWork.Connect("127.0.0.1", 9000);

            // 매니저들을 따로 먼저 실행시켜주는 mgr을 따로 만들어준다.
            //LoginMgr mgr = LoginMgr.Instance;
            //StageMgr psmgr = StageMgr.Instance;
            //NetMgr netmgr = NetMgr.Instance;
            //ProtocolMgr pmgr = ProtocolMgr.Instance;
            //StateMgr smgr = StateMgr.Instance;
            //RoomMgr rmgr = RoomMgr.Instance;

            m_curState = m_loginState; // state를 변경

            // Recv Thread 생성
            m_RecvTrd = new Thread(() => RecvProc(this));
            m_SendQueTrd = new Thread(() => SendQueProc(this));
            m_RecvQueTrd = new Thread(() => RecvQueProc(this));

            // Thread 시작
            m_RecvTrd.Start();
            m_SendQueTrd.Start();
            m_RecvQueTrd.Start();
        }


        private void Update()
        {
            //// 큐에 들어간 이벤트를 꺼내서 사용 함수호출
            //// switch문으로 나누어 사용
            //if (m_recvQue.Count > 0) // 
            //{
            //    t_Eve eve = m_recvQue.Dequeue();
            //    m_curState.RecvEvent(eve);
            //}
        }

        public void SendQueProc(NetMgr _net)
        {
            while (true)
            {
                if (m_sendQue.Count > 0)
                {
                    t_Eve eve = m_sendQue.Dequeue();

                    NetMgr.Instance.m_netWork.Send(eve.buf, eve.buf_size);
                }
            }
        }

        public void RecvQueProc(NetMgr _net)
        {
            while (true)
            {
                if (m_recvQue.Count > 0)
                {
                    t_Eve eve = m_recvQue.Dequeue();
                    switch (eve.state_type)
                    {
                        case (int)STATE_TYPE.LOGIN_STATE:
                            {
                                m_curState = m_loginState;
                                m_curState.RecvEvent(eve);
                            }
                            break;
                        case (int)STATE_TYPE.LOBBY_STATE:
                            {
                                m_curState = m_lobbyState;
                                m_curState.RecvEvent(eve);
                            }
                            break;
                    }
                }
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

                uint protocol = _net.m_netWork.GetSubProtocol(buf);
                uint main_protocol = _net.m_netWork.GetMainProtocol(buf);

                switch(main_protocol)
                {
                    case (int)ProtocolMgr.MAIN_PROTOCOL.LOGIN:
                        _net.m_curState = m_loginState;
                        break;
                    case (int)ProtocolMgr.MAIN_PROTOCOL.LOBBY:
                        break;
                }

                _net.m_curState.Recv(buf, protocol);
            }

        }
    }
}
