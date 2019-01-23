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
using System.Web;

namespace PatreonSharp
{
    public class PatreonAPI
    {
        enum returnFormat
        {
            ApiReturnFormatJson = 0,
            ApiReturnFormatDictionary = 1
        }
        private String accessToken;
        private HttpClient patreonClient = new HttpClient() { BaseAddress = new Uri("https://api.patreon.com/oauth2/api/") };
        private returnFormat returnFormatType;
        
        public PatreonAPI(string accessToken)
        {
            this.accessToken = accessToken;
            patreonClient.DefaultRequestHeaders.Add("User-Agent", "PatreonSharp (" + System.Runtime.InteropServices.RuntimeInformation.OSDescription + "," + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture + ")");
            patreonClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public object fetchUser()
        {
            return getData("current_user");
        }
        
        public object fetchCampaignAndPatrons()
        {
            return getData("current_user/campaigns?include=rewards,creator,goals,pledges");
        }

        public object fetchCampaign()
        {
            return getData("current_user/campaigns?include=rewards,creator,goals");
        }

        public object fetchPageofPledges(long campaignId, int pageSize, string cursor = null)
        {
            String url = "campaigns/" + campaignId.ToString() + "/pledges?page%5Bcount%5D=" + pageSize.ToString();

            if (!string.IsNullOrEmpty(cursor))
            {
                string escapedCursor = HttpUtility.UrlEncode(cursor);
                url = url + "&page%5Bcursor%5D=" + escapedCursor;
            }

            return getData(url);
        }

        public object getData(String suffix)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(suffix, UriKind.Relative),
                Method = HttpMethod.Get
            };

            var response = patreonClient.SendAsync(request);

            if (response.Result.StatusCode.GetHashCode() >= 500)
            {
                return response.Result.Content.ReadAsStringAsync().Result;
            }

            switch (returnFormatType)
            {
                case returnFormat.ApiReturnFormatJson:
                default:
                    return response.Result.Content.ReadAsStringAsync().Result;
                case returnFormat.ApiReturnFormatDictionary:
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
