using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Providers;
using Providers.Dto;
using Providers.Powerdns.Dto;
using RestSharp;

namespace Providers
{
    public class PowerDns : IDnsProvider
    {
        private readonly RestClient _restClient;

        public PowerDns(string apiUrl, string apiKey)
        {
            _restClient = (RestClient)new RestClient(apiUrl)
                .AddDefaultHeader("X-API-Key", apiKey);
                //.BaseUrl("api/v1/servers/localhost");

            //var checkConnectionResp = CheckConnection();
            //if (!checkConnectionResp.Success)
            //{
            //    throw new System.InvalidOperationException("Connection with DNS server is not possible");
            //}
        }

        public ProviderResponse CheckConnection()
        {
            var resp = CreateWebRequestAsync($"api/v1/servers/localhost");
            if (resp == "")
            {
                return new ProviderResponse
                {
                    Success = false
                };
            }
            return JsonReturnToproviderResponse(resp);
        }

        public ProviderResponse UnSign(string id, string pubKey)
        {
            id = TrailingDot(id);
            var zoneInfo = GetZoneInfo(id);
            var theRightCryptokey = zoneInfo.DnsZoneCryptokeys.FirstOrDefault(b => b.Key == pubKey);
            var providerResponse = new ProviderResponse();
            if (theRightCryptokey != null)
            {
                var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/cryptokeys/{theRightCryptokey.Id}", "DELETE");
                if (jsonString.StartsWith("Error: "))
                {
                    providerResponse.Success = false;
                    providerResponse.Error = jsonString;
                    providerResponse.RawMessage = jsonString;
                    return providerResponse;
                }
            }
            else
            {
                providerResponse.Success = false;
                providerResponse.Error = "That DNSSEC key is not found on the DNS Server";
                providerResponse.RawMessage = "That DNSSEC key is not found on the DNS Server";
                return providerResponse;
            }

            providerResponse.Success = true;
            return providerResponse;
        }      

        public ProviderResponse NotifyZone(string id)
        {
            id = TrailingDot(id);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/notify", "PUT");

            return JsonReturnToproviderResponse(jsonString);
        }

        public ProviderResponse RectifyZone(string id)
        {
            id = TrailingDot(id);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/rectify", "PUT");


            return JsonReturnToproviderResponse(jsonString);
        }

        public ProviderResponse RetrieveZone(string id)
        {
            id = TrailingDot(id);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/axfr-retrieve", "PUT");


            return JsonReturnToproviderResponse(jsonString);
        }

        public ProviderResponse VerifyZone(string id)
        {
            id = TrailingDot(id);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/check");


            return JsonReturnToproviderResponse(jsonString);
        }

        public ProviderResponse ChangeKind(string id, string kind)
        {
            id = TrailingDot(id);
            dynamic topost = new System.Dynamic.ExpandoObject();
            topost.kind = kind;

            var toPostJson = JsonConvert.SerializeObject(topost);

            var jsonStringResponse = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}", "PUT", toPostJson);

            return JsonReturnToproviderResponse(jsonStringResponse);

        }

        public ProviderResponse EditMasterIps(string id, List<string> ipList)
        {
            id = TrailingDot(id);
            dynamic topost = new System.Dynamic.ExpandoObject();
            topost.masters = ipList;

            var toPostJson = JsonConvert.SerializeObject(topost);

            var jsonStringResponse = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}", "PUT", toPostJson);

            return JsonReturnToproviderResponse(jsonStringResponse);
        }

        public DnsZoneCryptokey Sign(string domainname, string algorithm, int bits)
        {
            domainname = TrailingDot(domainname);
            var toPostJson = JsonConvert.SerializeObject(new SignDnssec
            {
                Dnssec = true,
                Keytype = "csk",
                Active = true,
                Bits = bits,
                Algorithm = algorithm
            });

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{domainname}/cryptokeys", "POST", toPostJson);
            if (jsonString.StartsWith("Error: "))
            {
                return new DnsZoneCryptokey();
            }
            var cryptokeysJson = JsonConvert.DeserializeObject<CryptokeysJson>(jsonString);

            var dnskey = cryptokeysJson.dnskey.Split(' ');
            var keyTag = cryptokeysJson.ds.First().Split(' ')[0];

            var dnsZoneCryptokey = new DnsZoneCryptokey
            {
                Flag = dnskey[0],
                Algo = dnskey[2],
                Key = dnskey[3],
                Id = cryptokeysJson.id,
                KeyTag = keyTag
            };

            return dnsZoneCryptokey;
        }

        public List<string> GetZones()
        {
            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones");
            if (jsonString.StartsWith("Error: "))
            {
                return  new List<string>();
            }

            var items = JsonConvert.DeserializeObject<List<Zone>>(jsonString);
            var returnList = new List<string>();

            foreach (var powerDnsZone in items)
            {
                returnList.Add(powerDnsZone.Name.TrimEnd('.'));
            }
            return returnList;
        }

        public DnsZoneInfo GetZoneInfo(string id)
        {
            id = TrailingDot(id);
            var zoneInfo = GetApiZoneInfo(id);
            var type = GetDnsZoneTypeFromString(zoneInfo.Kind);

            var masters = zoneInfo.Masters.Cast<string>().ToList();

            var dnsZoneInfo = new DnsZoneInfo
            {
                Name = zoneInfo.Name,
                Type = type,
                DnsZoneCryptokeys = GetApiCryptokeys(id),
                MasterIps = masters
            };


            return dnsZoneInfo;
        }

        public List<DnsRrset> GetRrsets(string id)
        {
            id = TrailingDot(id);
            var zoneInfo = GetApiZoneInfo(id);
            if (zoneInfo.Rrsets != null)
            {
                return zoneInfo.Rrsets.ToList();
            }
            return new List<DnsRrset>();
        }

        public ProviderResponse DeleteRrset(string id, string name, string type, string content)
        {
            id = TrailingDot(id);
            var rrsets = GetRrsets(id);

            rrsets = rrsets.Where(b => b.Name == name && b.Type == type).ToList();
            foreach (var rrset in rrsets)
            {
                var records = rrset.Records.ToList();
                foreach (var rrsetRecord in records.ToList())
                {
                    if (rrsetRecord.Content == content)
                    {
                        records.Remove(rrsetRecord);
                    }
                }

                rrset.Records = records.ToArray();
                rrset.Changetype = "REPLACE";
            }
            dynamic rrsets2 = new System.Dynamic.ExpandoObject();
            rrsets2.rrsets = rrsets;

            var json = JsonConvert.SerializeObject(rrsets2);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}", "PATCH", json);

            return JsonReturnToproviderResponse(jsonString);
        }

        public ProviderResponse CreateRrset(string id, string name, int ttl, string type, string content, bool replace = false)
        {
            id = TrailingDot(id);

            var rrsets = GetRrsets(id);
            rrsets = rrsets.Where(b => b.Name == name && b.Type == type).ToList();
            var found = false;
            var toBeReplaced = false;

            foreach (var dnsRrset in rrsets)
            {
                if (dnsRrset.Ttl == ttl)
                {
                    found = true;
                    var records = dnsRrset.Records.ToList();
                    records.Add(new DnsRecord
                    {
                        Disabled = false,
                        Content = content
                    });
                    dnsRrset.Changetype = "REPLACE";
                    dnsRrset.Records = records.ToArray();
                }
            }

            if (!found && rrsets.Count > 0)
            {
                if (replace)
                {
                    var newRecord = new DnsRecord
                    {
                        Content = content,
                        Disabled = false
                    };

                    rrsets[0].Ttl = ttl;
                    rrsets[0].Changetype = "REPLACE";
                    var currentRecords = rrsets[0].Records.ToList();
                    currentRecords.Add(newRecord);
                    rrsets[0].Records = currentRecords.ToArray();
                }
                
                toBeReplaced = true;
            }
            else if (!found && rrsets.Count == 0)
            {
                var dnsRecords = new List<DnsRecord>
                {
                    new DnsRecord
                    {
                        Content = content,
                        Disabled = false
                    }
                };
                rrsets.Add(new DnsRrset
                {
                    Changetype = "REPLACE",
                    Name = name,
                    Records = dnsRecords.ToArray(),
                    Ttl = ttl,
                    Type = type
                });
            }

            ProviderResponse providerResponse = new ProviderResponse();
            if(!replace && toBeReplaced)
            {
                providerResponse.Error = "ttl replaced";
            }
            else
            {
                dynamic rrsets2 = new System.Dynamic.ExpandoObject();
                rrsets2.rrsets = rrsets;

                var json = JsonConvert.SerializeObject(rrsets2);

                var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}", "PATCH", json);

                providerResponse = JsonReturnToproviderResponse(jsonString);
            }
            return providerResponse;
        }

        public ProviderResponse EditRrset(string id, string oldName, int oldTtl, string oldType, string oldContent, string newName,
            int newTtl, string newContent, bool replace = false)
        {
            id = TrailingDot(id);
            if (replace)
            {
                var deleteResponse = DeleteRrset(id, oldName, oldType, oldContent);
                if (!deleteResponse.Success)
                {
                    return deleteResponse;
                }
            }

            // creating the new record, if the ttl is different and all the other records will change it won't do that yet and it will return "ttl replaced", GUI has to deal with confirmation
            var resp = CreateRrset(id, newName, newTtl, oldType, newContent, replace);
            if ((!replace && resp.Error == "ttl replaced") || !resp.Success)
            {
                return resp;
            }

            // Deleting the old record
            if (!replace)
            {
                resp = DeleteRrset(id, oldName, oldType, oldContent);
            }

            return resp;
        }

        public int GetTtl(string id)
        {
            id = TrailingDot(id);
            var rrsets = GetRrsets(id);
            var soaRrset = rrsets.FirstOrDefault(b => b.Type == "SOA");

            if (soaRrset == null)
            {
                return 86400;
            }
            return soaRrset.Ttl;

        }

        public ProviderResponse DeleteZone(string id)
        {
            id = TrailingDot(id);
            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}", "DELETE");
            if (jsonString.StartsWith("Error: "))
            {
                var returnproviderResponse = new ProviderResponse
                {
                    Error = jsonString,
                    Success = false,
                    RawMessage = jsonString
                };
                return returnproviderResponse;
            }
            var providerResponse = new ProviderResponse
            {
                Success = true,
            };
            return providerResponse;
        }

        public ProviderResponse CreateZone(string id)
        {
            id = TrailingDot(id);
            var newZone = new NewZone
            {
                Name = id,
                Kind = "Native",
                Nameservers = new List<string>()
            };

            var json = JsonConvert.SerializeObject(newZone);

            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones", "POST", json);

            return JsonReturnToproviderResponse(jsonString);
        }

        private string CreateWebRequestAsync(string url, string method = "GET", string toPost = "")
        {
            var request = new RestRequest("url");
            switch (method)
            {
                case "PATCH":
                    request.AddJsonBody(toPost);
                    var patchResponse = _restClient.Execute(request, Method.PATCH);

                    return patchResponse.Content;
                case "POST":
                    request.AddJsonBody(toPost);
                    var postResponse = _restClient.Execute(request, Method.POST);

                    return postResponse.Content;
                case "PUT":
                    request.AddJsonBody(toPost);
                    var putResponse = _restClient.Execute(request, Method.PUT);

                    return putResponse.Content;
                case "DELETE":
                    request.AddJsonBody(toPost);
                    var deleteResponse = _restClient.Execute(request, Method.DELETE);

                    return deleteResponse.Content;
                default:
                    var getResponse = _restClient.Execute(request, Method.GET);
                    return getResponse.Content;
            }
        }

        private ZoneInfo GetApiZoneInfo(string id)
        {
            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}");
            if (jsonString.StartsWith("Error: "))
            {
                return new ZoneInfo(); 
            }
            var zoneInfo = JsonConvert.DeserializeObject<ZoneInfo>(jsonString);

            return zoneInfo;
        }

        private List<DnsZoneCryptokey> GetApiCryptokeys(string id)
        {
            var jsonString = CreateWebRequestAsync($"api/v1/servers/localhost/zones/{id}/cryptokeys");
            if (jsonString.StartsWith("Error: "))
            {
                return new List<DnsZoneCryptokey>();
            }

            var dnsZoneCryptokeys = new List<DnsZoneCryptokey>();

            var cryptoKeys = JsonConvert.DeserializeObject<List<CryptokeysJson>>(jsonString);

            foreach (var cryptoKey in cryptoKeys)
            {
                var strings = cryptoKey.dnskey.Split(' ');
                var dnsZoneCrtptokey = new DnsZoneCryptokey
                {
                    Flag = strings[0],
                    Algo = strings[2],
                    Key = strings[3],
                    Id = cryptoKey.id
                };
                dnsZoneCryptokeys.Add(dnsZoneCrtptokey);
            }

            return dnsZoneCryptokeys;
        }

        private static DnsZoneTypes GetDnsZoneTypeFromString(string type)
        {
            DnsZoneTypes dnsZoneTypes;
            switch (type)
            {
                case "Slave":
                    dnsZoneTypes = DnsZoneTypes.Slave;
                    break;
                case "Master":
                    dnsZoneTypes = DnsZoneTypes.Master;
                    break;
                case "Native":
                    dnsZoneTypes = DnsZoneTypes.Native;
                    break;
                default:
                    dnsZoneTypes = DnsZoneTypes.Slave;
                    break;
            }

            return dnsZoneTypes;
        }

        private static string TrailingDot(string id)
        {
            if (!id.EndsWith("."))
            {
                return id + ".";
            }
            return id;
        }

        private static ProviderResponse JsonReturnToproviderResponse(string json)
        {
            ProviderResponse providerResponse;
            dynamic jsonObj = new System.Dynamic.ExpandoObject();
            jsonObj = JsonConvert.DeserializeObject(json);
            if (IsPropertyExist(() => jsonObj.error))
            {
                providerResponse = new ProviderResponse
                {
                    Success = false,
                    Error = jsonObj.error,
                    RawMessage = json
                };
                return providerResponse;
            }

            providerResponse = new ProviderResponse
            {
                Success = true
            };

            return providerResponse;
        }

        static bool IsPropertyExist(GetValueDelegate getValueMethod)
        {
            try
            {
                //we're not interesting in the return value. What we need to know is whether an exception occurred or not
                var ok = getValueMethod();
                if (ok == null)
                {
                    return false;
                }
                return true;
            }
            catch (RuntimeBinderException)
            {
                // RuntimeBinderException occurred during accessing the property
                // and it means there is no such property         
                return false;
            }
            catch
            {
                //property exists, but an exception occurred during getting of a value
                return true;
            }
        }

        delegate string GetValueDelegate();
    }
}
