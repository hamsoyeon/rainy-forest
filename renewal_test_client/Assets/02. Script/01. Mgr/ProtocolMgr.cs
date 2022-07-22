using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace test_client_unity
{
    public class ProtocolMgr :Singleton<ProtocolMgr>
    {
        public enum MAIN_PROTOCOL
        {
            NONE,
            LOGIN,
            LOBBY,
            MAX,
        }

        // main_protocol
        public int GetMainProtocol(Byte[] _bytes)
        {
            int protocol = BitConverter.ToInt32(_bytes, 0);
            return (0xff00000 & protocol) >> 24;
        }

        // sub_protocol
        public int GetSubProtocol(Byte[] _bytes)
        {
            int protocol = BitConverter.ToInt32(_bytes, 0);
            return (0x00ff0000 & protocol) >> 16;
        }

        // detail_protocol
        public int GetDetailProtocol(Byte[] _bytes)
        {
            int protocol = BitConverter.ToInt32(_bytes, 0);
            return (0x0000ffff & protocol) >> 8;
        }

        public void AddMainProtocol(ref uint _protocolmem, uint _protocoltype)
        {
            uint main_protocol = _protocoltype << 24;
            _protocolmem |= main_protocol;
        }

        public void AddSubProtocol(ref uint _protocolmem, uint _protocoltype)
        {
            uint sub_protocol = _protocoltype << 16;
            _protocolmem |= sub_protocol;
        }

        public void AddDetailProtocol(ref uint _protocolmem, uint _protocoltype)
        {
            uint detail_protocol = _protocoltype << 8;
            _protocolmem |= detail_protocol;
        }
    }
}


