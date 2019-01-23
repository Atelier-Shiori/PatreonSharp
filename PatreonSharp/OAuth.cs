/*    Copyright 2019 Moy IT Solutions

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0


    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace PatreonSharp
{
    class OAuth
    {
        private string clientId;
        private string clientSecret;
        private HttpClient oAuthClient = new HttpClient() { BaseAddress = new Uri("https://api.patreon.com/") };

        public OAuth (string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            oAuthClient.DefaultRequestHeaders.Add("User-Agent", "PatreonSharp (" + System.Runtime.InteropServices.RuntimeInformation.OSDescription + "," + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + ")");
        }

        public Dictionary<string, object> getTokens(String code, String redirectUri)
        {
            return this.updateToken(new Dictionary<string, string> {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret},
                {"redirect_uri", redirectUri }
            });
        }

        public Dictionary<string, object> refreshToken(String refreshToken, String redirectUri)
        {
            return this.updateToken(new Dictionary<string, string> {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", clientId },
                { "client_secret", clientSecret},
            });
        }

        private Dictionary<string, object> updateToken(Dictionary<string,string>param)
        {
            FormUrlEncodedContent formParameters = new FormUrlEncodedContent(param);
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri("oauth2/token", UriKind.Relative),
                Method = HttpMethod.Post,
                Content = formParameters
            };

            var response = oAuthClient.SendAsync(request);

            using (HttpContent content = response.Result.Content)
            {
                string data = content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Dictionary<string,object>>(data);
            }
        }
    }
}
