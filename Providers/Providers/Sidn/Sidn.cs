using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EppLib;
using EppLib.Entities;
using EppLib.Extensions.SecDNS;
using Providers.Dto;

namespace Providers
{
    public class Sidn : IRegistryProvider
    {

        private readonly string _username;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly X509Certificate _certificate;

        private TcpTransport _tcpTransport;
        private Service _service;
        private Login _loginCmd;
        private Services _services;
        
        public Sidn(string username, string password, string host)
        {
            _username = username;
            _password = password;
            _host = host;
            _port = 700;
            _certificate = null;

            Connect();
            Login();
        }

        public Sidn(string username, string password, string host, int port) : this(username, password, host)
        {
            _port = port;
        }

        public Sidn(string username, string password, string host, int port, X509Certificate certificate) : this(username, password, host, port)
        {
            _certificate = certificate;
        }

        public ProviderResponse DomainCreate(string domain, string id)
        {
            var domainCreateCmd = new DomainCreate(domain, id);
            domainCreateCmd.DomainContacts.Add(new DomainContact(id, "admin"));
            domainCreateCmd.DomainContacts.Add(new DomainContact(id, "tech"));
            domainCreateCmd.Password = "s0mer4ndom";
            var providerResponse = new ProviderResponse{Success = true};
            try
            {
                var resp = _service.Execute(domainCreateCmd);
                if (!resp.Code.Equals("1000"))
                {
                    providerResponse.Success = false;
                    providerResponse.Error = resp.Message;
                    providerResponse.RawMessage = resp.Xml;
                }
            }
            catch (Exception e)
            {
                providerResponse.Success = false;
                providerResponse.Error = e.Message;
            }

            return providerResponse;
        }

        public void Hello()
        {
            var helloCmd = new Hello();
            try
            {
                var resp = _service.Execute(helloCmd);
                if (!resp.Code.Equals("1000"))
                {

                }
            }
            catch (Exception)
            {
            }

        }

        public ProviderResponse CheckConnection()
        {
            var providerResponse = new ProviderResponse
            {
                Success = true
            };

            Close();
            Connect();
            try
            {
                var loginResponse = Login();
                if (!loginResponse.Code.Equals("1000"))
                {
                    providerResponse.Success = false;
                    providerResponse.Error = loginResponse.Message;
                }

            }
            catch (Exception e)
            {
                providerResponse.Success = false;
                providerResponse.Error = e.Message;
            }

            return providerResponse;
        }

        public bool DomainExists(string domain)
        {
            var resp = GetDomainInfo(domain);

            return resp.Error == null && resp.Name != null;
        }

        public RegistryDomainInfo GetDomainInfo(string domain)
        {
            var registryDomainInfo = new RegistryDomainInfo
            {
                RegistryDnsSecs = new List<RegistryDnsSec>()
            };

            var domainInfoCmd = new DomainInfo(domain);
            DomainInfoResponse resp;

            try
            {
                resp = _service.Execute(domainInfoCmd);
            }
            catch (Exception e)
            {
                registryDomainInfo.Error = e.Message;
                return registryDomainInfo;
            }
            
           

            if (resp.Code == "2303")
            {
                registryDomainInfo.Error = "404";
            }
            else
            {
                var domainResponse = resp.Domain;
                registryDomainInfo.Name = domainResponse.Name;
                registryDomainInfo.NameServers = domainResponse.NameServers.ToList();
                if (domainResponse.SecDnses == null)
                {
                    return registryDomainInfo;
                }

                foreach (var secDns in domainResponse.SecDnses)
                {
                    var newSecDns = new RegistryDnsSec
                    {
                        Algo = secDns.Alg,
                        Flag = secDns.Flags,
                        Key = secDns.PubKey
                    };

                    registryDomainInfo.RegistryDnsSecs.Add(newSecDns);
                }


            }

            return registryDomainInfo;
        }
        
        public ProviderResponse Sign(string domain, string flag, string algo, string pubKey, string keyTag)
        {
            return ChangeDnsKey(domain, flag, algo, pubKey, "secDNS:add");
        }

        public ProviderResponse DeleteKey(string domain, string flag, string algo, string pubKey)
        {
            return ChangeDnsKey(domain, flag, algo, pubKey, "secDNS:rem");
        }

        public void Close()
        {
            var logout = new Logout();
            var logoutResponse = _service.Execute(logout);
            _service.Disconnect();
        }

        private void Connect()
        {
            _tcpTransport = new TcpTransport(_host, _port, _certificate);
            _service = new Service(_tcpTransport);
            _service.Connect();
        }

        private LoginResponse Login()
        {
            _loginCmd = new Login(_username, _password)
            {
                Options = new Options
                {
                    MLang = "en",
                    MVersion = "1.0"
                }
            };

            _services = new Services();

            _services.ObjURIs.Add("urn:ietf:params:xml:ns:epp-1.0");
            _services.ObjURIs.Add("urn:ietf:params:xml:ns:domain-1.0");
            _services.ObjURIs.Add("urn:ietf:params:xml:ns:host-1.0");
            _services.ObjURIs.Add("urn:ietf:params:xml:ns:contact-1.0");
            _services.Extensions.Add("urn:ietf:params:xml:ns:cira-1.0");
            _services.Extensions.Add("urn:ietf:params:xml:ns:poll-1.0");

            _loginCmd.Services = _services;

            var loginResponse = _service.Execute(_loginCmd);

            return loginResponse;
        }

        private ProviderResponse ChangeDnsKey(string domain, string flag, string algo, string pubKey, string func)
        {
            var command = new DomainUpdate(domain);
            var extension = new SecDNSCreate();

            extension.DsData.Add(new SecDNSData
            {
                fun = func,
                flag = flag,
                alg = algo,
                protocol = "3",
                pubkey = pubKey
            });

            command.Extensions.Add(extension);
            var response = _service.Execute(command);

            var providerResponse = new ProviderResponse
            {
                Success = true,
                Error = ""
            };

            if (response.Code == "1000")
            {
                return providerResponse;
            }

            providerResponse.Success = false;
            providerResponse.Error = response.Message;
            providerResponse.RawMessage = response.Xml;

            return providerResponse;
        }

    }
}
