using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

namespace test_client_unity
{
    public class Network : Packet
    {
        public TcpClient m_socket;

        public Network() { }

        public void Connect(string _addr, int _port)
        {
            try
            {
                m_socket = new TcpClient(_addr, _port);
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception" + e);
            }
        }

        public void Send(Byte[] _buffer, int _size)
        {
            if (m_socket == null)
            {
                return;
            }
            try
            {
                NetworkStream stream = m_socket.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(_buffer, 0, _size);
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }

        public int Recv(Byte[] _buffer, int _size)
        {
            if (m_socket == null) { }
            try
            {
                NetworkStream stream = m_socket.GetStream();
                if (stream.CanRead)
                {
                    int size = stream.Read(_buffer, 0, _size);
                    return size;
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
            catch (System.IO.IOException ex)
            {
                Debug.Log(ex);
            }
            return -1;
        }
    }
}


