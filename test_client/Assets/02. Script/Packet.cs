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
        public int PackPacket(ref Byte[] _buf, Byte[] _data_buf, int _data_size)
        {
            _buf = new Byte[4096];
            int len = 0;
            int total_size = sizeof(int) + sizeof(int) + sizeof(int) + _data_size; // 패킷넘버 / 프로토콜 / 데이터 사이즈 / 데이터

            Byte[] tmp_byte = BitConverter.GetBytes(total_size);
            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 총 데이터 사이즈 ( 이건 전체 데이터 사이즈를 받고 버림 )
            len = len + sizeof(int);

            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 패킷넘버
            len = len + sizeof(int);

            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 프로토콜
            len = len + sizeof(int);

            tmp_byte = BitConverter.GetBytes(_data_size);
            Array.Copy(tmp_byte, 0, _buf, len, sizeof(int)); // 데이터 사이즈
            len = len + sizeof(int);

            Array.Copy(_data_buf, 0, _buf, len, _data_size); // 데이터
            len = len + _data_size;

            return len;
        }

        //public int Packpacket(ref Byte[] _buf, int _protocol)
        //{
        //    // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈
        //    int size = sizeof(int) + sizeof(int);
        //    int len = 0;

        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    return len;
        //}

        //public int Packpacket(ref Byte[] _buf, int _protocol, string _id, string _pw)
        //{
        //    int id_size = _id.Length * 2;
        //    int pw_size = _pw.Length * 2;

        //    // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈 / ID 사이즈 / ID / PW 사이즈 / PW
        //    int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int); // 총사이즈
        //    int len = 0;

        //    // 전체 데이터 사이즈
        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    // 전체 데이터 사이즈 +
        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(id_size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    Encoding.Unicode.GetBytes(_id).CopyTo(_buf, len);
        //    len = len + id_size;

        //    BitConverter.GetBytes(pw_size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    Encoding.Unicode.GetBytes(_pw).CopyTo(_buf, len);
        //    len = len + pw_size;

        //    return len;
        //}

        //public int Packpacket(ref Byte[] _buf, int _protocol, string _id, string _pw, string _nick)
        //{
        //    int id_size = _id.Length * 2;
        //    int pw_size = _pw.Length * 2;
        //    int nick_size = _nick.Length * 2;

        //    // 전체 데이터 사이즈 / 프로토콜 / 전체 데이터 사이즈 / ID 사이즈 / ID / PW 사이즈 / PW / NICK 사이즈 / NICK
        //    int size = sizeof(int) + sizeof(int) + id_size + sizeof(int) + pw_size + sizeof(int) + nick_size + sizeof(int); // 총사이즈
        //    int len = 0;

        //    // 전체 데이터 사이즈
        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(_protocol).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    // 전체 데이터 사이즈 +
        //    BitConverter.GetBytes(size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    BitConverter.GetBytes(id_size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    Encoding.Unicode.GetBytes(_id).CopyTo(_buf, len);
        //    len = len + id_size;

        //    BitConverter.GetBytes(pw_size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    Encoding.Unicode.GetBytes(_pw).CopyTo(_buf, len);
        //    len = len + pw_size;

        //    BitConverter.GetBytes(nick_size).CopyTo(_buf, len);
        //    len = len + sizeof(int);

        //    Encoding.Unicode.GetBytes(_nick).CopyTo(_buf, len);
        //    len = len + nick_size;

        //    return len;
        //}

        //public void Unpackpacket(Byte[] _buf, ref int _result, ref string _msg)
        //{
        //    int len = sizeof(int) + sizeof(int) + sizeof(int);
        //    Byte[] msg_size = new Byte[4];
        //    Byte[] result = new Byte[4];

        //    Array.Copy(_buf, len, result, 0, sizeof(int));
        //    _result = BitConverter.ToInt32(result);
        //    len = len + sizeof(int);

        //    Array.Copy(_buf, len, msg_size, 0, sizeof(int));
        //    len = len + sizeof(int);

        //    _msg = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(msg_size));
        //}

        //public void Unpackpacket(Byte[] _buf, ref string _msg)
        //{
        //    // 패킷넘버 / 프로토콜 / 데이터 사이즈
        //    int len = sizeof(int) + sizeof(int) + sizeof(int);
        //    Byte[] msg_size = new Byte[4];

        //    Array.Copy(_buf, len, msg_size, 0, sizeof(int));
        //    len = len + sizeof(int);

        //    _msg = Encoding.Unicode.GetString(_buf, len, BitConverter.ToInt32(msg_size));
        //}

        public uint GetMainProtocol(Byte[] _buf)
        {
            int len = sizeof(int);

            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, len, bytes_protocol, 0, sizeof(int));
            uint protocol = ProtocolMgr.Instance.GetMainProtocol(bytes_protocol);
            return protocol;
        }

        public uint GetSubProtocol(Byte[] _buf)
        {
            int len = sizeof(int);

            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, len, bytes_protocol, 0, sizeof(int));
            uint protocol = ProtocolMgr.Instance.GetSubProtocol(bytes_protocol);
            return protocol;
        }

        public uint GetDetailProtocol(Byte[] _buf)
        {
            int len = sizeof(int);

            Byte[] bytes_protocol = new Byte[4];
            Array.Copy(_buf, len, bytes_protocol, 0, sizeof(int));
            uint protocol = ProtocolMgr.Instance.GetDetailProtocol(bytes_protocol);
            return protocol;
        }

    }
}