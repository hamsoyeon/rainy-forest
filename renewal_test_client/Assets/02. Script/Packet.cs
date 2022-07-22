using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

namespace test_client_unity
{
    public class Packet
    {
        public int PackPacket(ref Byte[] _buf, int _protocol, Byte[] _data_buf, int _data_size)
        {
            _buf = new Byte[4096];
            int len = 0;
            int total_size = sizeof(int) + sizeof(int) + sizeof(int) + _data_size; // 패킷넘버 / 프로토콜 / 데이터 사이즈 / 데이터

            if (_data_size == 0) // 프로토콜만 보내는 경우 데이터 사이즈가 들어갈 필요없다.
                total_size -= sizeof(int);

            Byte[] tmp_byte = BitConverter.GetBytes(total_size);
            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 총 데이터 사이즈 ( 이건 전체 데이터 사이즈를 받고 버림 )
            len = len + sizeof(int);

            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 패킷넘버
            len = len + sizeof(int);

            tmp_byte = BitConverter.GetBytes(_protocol);
            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 프로토콜
            len = len + sizeof(int);

            if (_data_size == 0)
            {
                return len;
            }

            tmp_byte = BitConverter.GetBytes(_data_size);
            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 데이터 사이즈
            len = len + sizeof(int);

            Array.Copy(_data_buf, 0, _buf, len, _data_size); // 데이터
            len = len + _data_size;

            return len;
        }

        public Byte[] GetProtocol(Byte[] _buf)
        {
            Byte[] protocol_byte = new Byte[4];
            int len = sizeof(int); // 패킷넘버 건너뛰고
            Array.Copy(_buf, len, protocol_byte, 0, sizeof(int));
            return protocol_byte;
        }

        public int GetMainProtocol(Byte[] _buf)
        {
            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, 0, bytes_protocol, 0, sizeof(int));
            int protocol = ProtocolMgr.Instance.GetMainProtocol(bytes_protocol);
            return protocol;
        }

        public int GetSubProtocol(Byte[] _buf)
        {
            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, 0, bytes_protocol, 0, sizeof(int));
            int protocol = ProtocolMgr.Instance.GetSubProtocol(bytes_protocol);
            return protocol;
        }

        public int GetDetailProtocol(Byte[] _buf)
        {
            int len = sizeof(int);

            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, 0, bytes_protocol, 0, sizeof(int));
            int protocol = ProtocolMgr.Instance.GetDetailProtocol(bytes_protocol);
            return protocol;
        }

    }
}