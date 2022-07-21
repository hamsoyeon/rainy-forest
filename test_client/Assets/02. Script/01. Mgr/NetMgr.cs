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
        public int     eve;
        public int     state_type;
        public Byte[]  buf;
        public int     buf_size;
    }

    public class NetMgr : Singleton<NetMgr>
    {
        // NullReferenceException�� �߻��ϴ� ����
        // - ���� �� �����Ҵ��� �� ���� ����ؾ� �Ѵ�.
        public Network  m_netWork = new Network(); // ���Ͽ뵵
        public State    m_curState;

        public Thread   m_RecvTrd;
        public Thread   m_RecvQueTrd;
        public Thread   m_SendQueTrd;

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

            m_curState = StateMgr.Instance.GetLogin; // state�� ����
            
            // Recv Thread ����
            m_RecvTrd = new Thread(()=>RecvProc(this));
            m_RecvQueTrd = new Thread(() => RecvQueProc(this));
            m_SendQueTrd = new Thread(()=>SendQueProc(this));

            // Thread ����
            m_RecvTrd.Start();
            m_RecvQueTrd.Start();
            m_SendQueTrd.Start();
        }

        public Queue<t_Eve> m_recvQue = new Queue<t_Eve>();

        private void Update()
        {
            // ť�� �� �̺�Ʈ�� ������ ��� �Լ�ȣ��
            // switch������ ������ ���
            if (m_recvQue.Count > 0) // 
            {
                t_Eve eve = m_recvQue.Dequeue();
                m_curState.RecvEvent(eve);
            }
        }

        static public void SendQueProc(NetMgr _net)
        {

        }

        static public void RecvQueProc(NetMgr _net)
        {

        }

        void OnApplicationQuit() // ���α׷� ����� ���Ϲ�ȯ
        {
            this.m_netWork.m_socket.Close();
            Debug.Log("���� ����");
        }

        static public void RecvProc(NetMgr _net)
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
                _net.m_curState.Recv(buf, protocol);
            }

        }
    }
}
