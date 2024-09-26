using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Nager.PublicSuffix;
using Providers.Dto;
using Providers.Providers.Openprovider.Dto;

namespace Providers
{
    public class Openprovider : IRegistryProvider
    {
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;

        public Openprovider(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        public ProviderResponse CheckConnection()
        {
            var openXml = new OpenXml
            {
                Credentials = new Credentials
                {
                    Username = _username,
                    Password = _password
                },
                RetrieveDomainRequest = new RetrieveDomainRequest()
            };
            var resp = CreateWebRequest(openXml);
            var code = resp.reply.code;

            if (code == null) 
            {
                return new ProviderResponse
                {
                    Success = false,
                    Error = "Could not connect to the server"
                };
            }

            if (code == "196")
            {
                return new ProviderResponse
                {
                    Success = false,
                    Error = resp.reply.desc
                };
            }

            return new ProviderResponse
            {
                Success = true
            };
        }

        public bool DomainExists(string domain)
        {
            var resp = GetDomainInfo(domain);

            return resp.Error == null;
        }

        public RegistryDomainInfo GetDomainInfo(string domain)
        {
            var returnRegistryDomainInfo = new RegistryDomainInfo
            {
                NameServers = new List<string>(),
                RegistryDnsSecs = new List<RegistryDnsSec>()
            };

            var domainParser = new DomainParser(new WebTldRuleProvider());
            var fullDomain = domainParser.Get(domain);
            var domainWithoutTld = domain.Remove(domain.Length - ("." + fullDomain.TLD).Length);
            var openXml = new OpenXml
            {
                Credentials = new Credentials
                {
                    Username = _username,
                    Password = _password
                },
                RetrieveDomainRequest = new RetrieveDomainRequest
                {
                    Domain = new Domain
                    {
                        Name = domainWithoutTld,
                        Extension = fullDomain.TLD
                    },
                    WithadditionalData = 1
                }
            };
            var response = CreateWebRequest(openXml);

            var reply = response.reply;
            var code = reply.code;

            if (code != "0")
            {
                return new RegistryDomainInfo
                {
                    Error = reply.desc
                };
            }

            var data = reply.data;

            // Check if domain is active https://doc.openprovider.eu/API_Format_Domain_Status
            if (data.status != "ACT")
            {
                return new RegistryDomainInfo
                {
                    Error = "Domain is found but is not active (Domain Status = " + data.status + ")"
                };
            }

            var nameservers = data.nameServers.array.item;
            var nsType = nameservers.GetType();
            if (nsType.IsSerializable)
            {
                foreach (var nameserver in nameservers)
                {
                    returnRegistryDomainInfo.NameServers.Add(nameserver.name);
                }
            }

            returnRegistryDomainInfo.RegistryDnsSecs = GetDnsSecs(data);
            

            return returnRegistryDomainInfo;

        }

        public ProviderResponse Sign(string domain, string flag, string algo, string pubKey, string keyTag)
        {
            var domainInfo = GetDomainInfo(domain);
            var modifyDomainKeys = new List<ModifyDomainKey>();
            var openXml = GenerateModifyDomainRequest(domain);

            foreach (var domainInfoRegistryDnsSec in domainInfo.RegistryDnsSecs)
            {
                modifyDomainKeys.Add(new ModifyDomainKey
                {
                    Alg = domainInfoRegistryDnsSec.Algo,
                    Flags = domainInfoRegistryDnsSec.Flag,
                    Protocol = "3",
                    PubKey = domainInfoRegistryDnsSec.Key
                });
            }

            modifyDomainKeys.Add(new ModifyDomainKey
            {
                Alg = algo,
                Flags = flag,
                Protocol = "3",
                PubKey = pubKey
            });

            openXml.ModifyDomainRequest.Dnsseckeys.ModifyDomainSubArray.ModifyDomainKeys = modifyDomainKeys;

            var response = CreateWebRequest(openXml);

            return ModifyRequestError(response);
        }

        public ProviderResponse DeleteKey(string domain, string flag, string algo, string pubKey)
        {
            var domainInfo = GetDomainInfo(domain);
            var modifyDomainKeys = new List<ModifyDomainKey>();
            var openXml = GenerateModifyDomainRequest(domain);

            foreach (var domainInfoRegistryDnsSec in domainInfo.RegistryDnsSecs)
            {
                if (domainInfoRegistryDnsSec.Key != pubKey)
                {
                    modifyDomainKeys.Add(new ModifyDomainKey
                    {
                        Alg = domainInfoRegistryDnsSec.Algo,
                        Flags = domainInfoRegistryDnsSec.Flag,
                        Protocol = "3",
                        PubKey = domainInfoRegistryDnsSec.Key
                    });
                }
            }
            openXml.ModifyDomainRequest.Dnsseckeys.ModifyDomainSubArray.ModifyDomainKeys = modifyDomainKeys;

            var response = CreateWebRequest(openXml);

            return ModifyRequestError(response);
        }

        public void Close()
        {
        }

        private ProviderResponse ModifyRequestError(dynamic response)
        {
            var reply = response.reply;
            var code = reply.code;

            if (code != "0")
            {
                return new ProviderResponse
                {
                    Success = false,
                    Error = reply.desc,
                    RawMessage = reply.ToString()
                };
            }

            return new ProviderResponse
            {
                Success = true
            };
        }

        private OpenXml GenerateModifyDomainRequest(string domain)
        {
            var domainParser = new DomainParser(new WebTldRuleProvider());
            var fullDomain = domainParser.Get(domain);
            var openXml = new OpenXml
            {
                Credentials = new Credentials
                {
                    Username = _username,
                    Password = _password
                },
                ModifyDomainRequest = new ModifyDomainRequest
                {
                    Domain = new Domain
                    {
                        Name = fullDomain.Domain,
                        Extension = fullDomain.TLD
                    },
                    IsDnssecEnabled = 1,
                    Dnsseckeys = new ModifyDomainDnsseckeys
                    {
                        ModifyDomainSubArray = new ModifyDomainSubArray()
                    }
                }
            };

            return openXml;
        }

        private List<RegistryDnsSec> GetDnsSecs(dynamic responseData)
        {
            var dnsSecs = new List<RegistryDnsSec>();

            var dnssecKeys = responseData.dnssecKeys;

            if (dnssecKeys != null)
            {
                var items = dnssecKeys.array.item;
                var getType = items.GetType();
                if (getType.IsSerializable)
                {
                    foreach (var dnssecKey in items)
                    {
                        dnsSecs.Add(new RegistryDnsSec
                        {
                            Algo = dnssecKey.alg,
                            Flag = dnssecKey.flags,
                            Key = dnssecKey.pubKey
                        });
                    }
                }
                else
                {
                    dnsSecs.Add(new RegistryDnsSec
                    {
                        Algo = items.alg,
                        Flag = items.flags,
                        Key = items.pubKey
                    });
                }
            }

            return dnsSecs;
        }

        private dynamic CreateWebRequest(OpenXml openXml)
        {
            var serializedString = SerializeToXml(openXml);

            string returnString;

            var webReq = (HttpWebRequest)WebRequest.Create(_url);
            webReq.ContentType = "text/xml";
            webReq.Method = "POST";

            using (var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.UTF8))
            {
                streamWriter.Write(serializedString);
            }

            var webResp = (HttpWebResponse)webReq.GetResponse();

            using (var streamReader = new StreamReader(webResp.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                returnString = streamReader.ReadToEnd();
            }

            dynamic returnObj = DynamicXml.Parse(returnString);

            return returnObj;
        }

        private string SerializeToXml(OpenXml openXml)
        {
            var serializer = new XmlSerializer(typeof(OpenXml));

            string serializedString;

            using (var textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, openXml);
                serializedString = textWriter.ToString();
            }

            return serializedString;
        }

        public static T DeserializeXmlFileToObject<T>(string xmlFilename)
        {
            if (string.IsNullOrEmpty(xmlFilename))
            {
                return default(T);
            }

            var xmlStream = new StreamReader(xmlFilename);
            var serializer = new XmlSerializer(typeof(T));
            var returnObject = (T)serializer.Deserialize(xmlStream);
            return returnObject;
        }
    }
}
