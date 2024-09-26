using Newtonsoft.Json;
using OpenSSL.PrivateKeyDecoder;
using Providers.Providers.Transip.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Providers
{
    class TokenRequestService
    {
        private const string AUTH_ENDPOINT = "https://api.transip.nl/v6/auth";
        private string _privateKeyText { get; set; }

        public TokenRequestService(string privateKeyText)
        {
            _privateKeyText = privateKeyText;
        }

        public TokenResponse Request(TokenRequest request)
        {
            var jsonPayload = JsonConvert.SerializeObject(request);

            using (var httpClient = new HttpClient())
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, AUTH_ENDPOINT))
            {
                httpRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                httpRequest.Content.Headers.Add("Signature", SignPayload(jsonPayload));
                var response = httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<TokenResponse>(response.Content.ReadAsStringAsync().Result);
            }
        }

        private string SignPayload(string jsonPayload)
        {
            using (var sha512 = new SHA512Managed())
            using (var rsa = new OpenSSLPrivateKeyDecoder().Decode(_privateKeyText))
            {
                var signer = new RSAPKCS1SignatureFormatter(rsa);
                signer.SetHashAlgorithm("SHA512");

                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(jsonPayload));

                return Convert.ToBase64String(signer.CreateSignature(hash));
            }
        }
    }
}
