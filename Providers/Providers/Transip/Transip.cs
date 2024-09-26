using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Nager.PublicSuffix;
using Providers.Dto;
using Providers.Providers.Transip.Dto;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Providers;
using Providers.Powerdns.Dto;
using RestSharp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace Providers
{
    public class Transip : IRegistryProvider
    {
        private readonly RestClient _client;
        private readonly string _apiVersion = "v6";
        private readonly string _username;
        private readonly string _privatekey;
        //private string _token;

        public Transip(string url, string username, string privatekey)
        {
            _username = username;
            _privatekey = privatekey;
            var tokenResponse = CreateToken();
            //_token = tokenResponse.Token;
            var bearertoken = "Bearer " + tokenResponse.Token;
            _client = (RestClient)new RestClient(url)
                .AddDefaultHeader("Authorization", bearertoken);
        }

        public ProviderResponse CheckConnection()
        {
            //if (string.IsNullOrWhiteSpace(_token))
            //{
            //    var tokenResponse = CreateToken();
            //    if (string.IsNullOrWhiteSpace(tokenResponse.Token) || !string.IsNullOrWhiteSpace(tokenResponse.Error))
            //    {
            //        return new ProviderResponse
            //        {
            //            Success = false,
            //            Error = tokenResponse.Error
            //        };
            //    }
            //    _token = tokenResponse.Token;
            //}

            var response = CreateWebRequest("api-test");           
            if (!response.IsSuccessful)
            {
                return new ProviderResponse
                {
                    Success = false,
                    Error = response.ErrorMessage
                };
            }

            return new ProviderResponse
            {
                Success = true,
            };
        }

        public bool DomainExists(string domainname)
        {
            var domainRestResponse = CreateWebRequest("domains/" + domainname);
            return domainRestResponse.IsSuccessful;
        }

        public RegistryDomainInfo GetDomainInfo(string domainname)
        {
            var returnRegistryDomainInfo = new RegistryDomainInfo
            {
                Name = domainname,
                Error = "",
                NameServers = new List<string>(),
                RegistryDnsSecs = new List<RegistryDnsSec>()
            };

            var domainRestResponse = CreateWebRequest("domains/" + domainname);

            if (!domainRestResponse.IsSuccessful)
            {
                return new RegistryDomainInfo
                {
                    Error = domainRestResponse.ErrorMessage + domainRestResponse.StatusCode + domainRestResponse.Content
                };
            }

            var domainResponse = JsonConvert.DeserializeObject<Domain>(domainRestResponse.Content);
            if (domainResponse.IsDnsOnly)
            {
                return new RegistryDomainInfo
                {
                    Error = "This domain isDnsOnly"
                };
            }

            var nameserversRestResponse = CreateWebRequest("domains/" + domainname + "/nameservers");
            var nameserversResponse = JsonConvert.DeserializeObject<Nameservers>(nameserversRestResponse.Content);
            foreach (var nameserver in nameserversResponse.NameserverList)
            {
                returnRegistryDomainInfo.NameServers.Add(nameserver.Hostname);
            }

            var dnssecRestResponse = CreateWebRequest("domains/" + domainname + "/dnssec");
            var dnssecResponse = JsonConvert.DeserializeObject<DnssecEntries>(dnssecRestResponse.Content);
            foreach (var dnssecEntry in dnssecResponse.DnssecEntryList)
            {
                var registryDnsSec = new RegistryDnsSec
                {
                    Flag = dnssecEntry.Flags,
                    Algo = dnssecEntry.Algorithm,
                    Key = dnssecEntry.PublicKey
                };
                returnRegistryDomainInfo.RegistryDnsSecs.Add(registryDnsSec);
            }

            return returnRegistryDomainInfo;

        }

        public ProviderResponse Sign(string domainname, string flags, string algo, string pubKey, string keyTag)
        {
            // Get current DNSSEC entries
            var dnssecRestResponse = CreateWebRequest("domains/" + domainname + "/dnssec");
            var dnssecEntries = JsonConvert.DeserializeObject<DnssecEntries>(dnssecRestResponse.Content);

            // Add new entry
            var newDnssecEntry = new DnssecEntry { Flags = flags, Algorithm = algo, PublicKey = pubKey, KeyTag = keyTag };
            dnssecEntries.DnssecEntryList.Add(newDnssecEntry);
            var payload = JsonConvert.SerializeObject(dnssecEntries);
            var response = CreateWebRequest("domains/" + domainname + "/dnssec", "PUT", payload);  

            return new ProviderResponse { Success = response.IsSuccessful, Error = response.StatusCode.ToString() + response.Content };
        }

        public ProviderResponse DeleteKey(string domainname, string flags, string algo, string pubKey)
        {
            // Get current DNSSEC entries
            var dnssecRestResponse = CreateWebRequest("domains/" + domainname + "/dnssec");
            var dnssecEntries = JsonConvert.DeserializeObject<DnssecEntries>(dnssecRestResponse.Content);

            // Find the entry to delete
            var entryfound = false;
            foreach (var dnssecEntry in dnssecEntries.DnssecEntryList.ToList())
            {
                if (flags == dnssecEntry.Flags &&
                    algo == dnssecEntry.Algorithm &&
                    pubKey == dnssecEntry.PublicKey)
                {
                    dnssecEntries.DnssecEntryList.Remove(dnssecEntry);
                    entryfound = true;
                }
            }

            // Not found
            if (!entryfound)
            { 
                return new ProviderResponse { Success = false, Error = "This DNSSEC entry could not be found" };
            }

            var payload = JsonConvert.SerializeObject(dnssecEntries);
            var response = CreateWebRequest("domains/" + domainname + "/dnssec", "PUT", payload);

            return new ProviderResponse { Success = response.IsSuccessful, Error = response.StatusCode.ToString() + response.Content };
        }

        public void Close()
        {
        }

        private TokenResponse CreateToken()
        {
            var nonce = DateTime.Now.Ticks.ToString();
            var tokenRequest = new TokenRequest
            {
                Login = _username,
                Nonce = nonce, // Perhaps use a better source of randomness (must be between 6 and 32 characters long)
                IsReadOnlyEnabled = false,
                ExpirationTime = "30 minutes",
                Label = "DNSSEC app " + DateTime.Now, // Must be unique. Is this the way to go?
                IsGlobalKey = false // Setting to false will generate a token that can only be used by whitelisted IP’s
            };

            var tokenResponse = new TokenRequestService(_privatekey).Request(tokenRequest);

            return tokenResponse;
        }

        private RestResponse CreateWebRequest(string url, string method = "GET", string payload = "")
        {
            var request = new RestRequest(_apiVersion + "/" + url);
            

            switch (method)
            {
                case "PATCH":
                    request.AddJsonBody(payload);
                    return (RestResponse)_client.Patch(request);

                case "POST":
                    request.AddJsonBody(payload);
                    return (RestResponse)_client.Post(request);

                case "PUT":
                    request.AddJsonBody(payload);
                    return (RestResponse)_client.Put(request);

                case "DELETE":
                    request.AddJsonBody(payload);
                    return (RestResponse)_client.Delete(request);

                default:
                    return (RestResponse)_client.Get(request);
            }
        }
    }
}
