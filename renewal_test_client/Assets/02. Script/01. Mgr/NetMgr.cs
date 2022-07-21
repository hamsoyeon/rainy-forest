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
        // NullReferenceException�� �߻��ϴ� ����
        // - ���� �� �����Ҵ��� �� ���� ����ؾ� �Ѵ�.
        public Network      m_netWork = new Network(); // ���Ͽ뵵

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

        // �ñ���
        // - Awake�� �ڵ尡 ������ �ȵȴ�.
        void Start()
        {
            m_netWork.Connect("127.0.0.1", 9000);

            // �Ŵ������� ���� ���� ��������ִ� mgr�� ���� ������ش�.
            //LoginMgr mgr = LoginMgr.Instance;
            //StageMgr psmgr = StageMgr.Instance;
            //NetMgr netmgr = NetMgr.Instance;
            //ProtocolMgr pmgr = ProtocolMgr.Instance;
            //StateMgr smgr = StateMgr.Instance;
            //RoomMgr rmgr = RoomMgr.Instance;

            m_curState = m_loginState; // state�� ����

            // Recv Thread ����
            m_RecvTrd = new Thread(() => RecvProc(this));
            m_SendQueTrd = new Thread(() => SendQueProc(this));
            m_RecvQueTrd = new Thread(() => RecvQueProc(this));

            // Thread ����
            m_RecvTrd.Start();
            m_SendQueTrd.Start();
            m_RecvQueTrd.Start();
        }


        private void Update()
        {
            //// ť�� �� �̺�Ʈ�� ������ ��� �Լ�ȣ��
            //// switch������ ������ ���
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

        void OnApplicationQuit() // ���α׷� ����� ���Ϲ�ȯ
        {
            this.m_netWork.m_socket.Close();
            Debug.Log("���� ����");
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

                // recv�� ���۸� �����ϴ� ���� ��.
                // main�� ���� -> sub�� ���� -> detail�� ����
                // �̺�Ʈ�� ���� �ϴ� ���� ����.

                // protocol�� ���� state�� ������
                // �� state�� recv���� event�� ���ִ� ���� �Ѵ�.

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
