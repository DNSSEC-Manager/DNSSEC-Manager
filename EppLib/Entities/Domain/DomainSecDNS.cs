using System;
using System.Collections.Generic;
using System.Text;

namespace EppLib.Entities
{
    public class DomainSecDns
    {
        public readonly string Flags;

        public readonly string Protocol;

        public readonly string Alg;

        public readonly string PubKey;

        public DomainSecDns(string flags, string protocol, string alg, string pubKey)
        {
            Flags = flags;
            Protocol = protocol;
            Alg = alg;
            PubKey = pubKey;
        }
    }
}
