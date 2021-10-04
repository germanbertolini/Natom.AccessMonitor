using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class NetworkService
    {
        private static string _currentVersion = GetAssemblyVersion();
        private static string _currentLanguage = GetLanguage();

        private static string GetAssemblyVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return version;
        }

        private static string GetLanguage()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            return currentCulture.ToString().Split('-').FirstOrDefault();
        }

        public static PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress();
                }
            }
            return null;
        }

        public async static Task<TransmitterResponseDto> DoHttpPostAsync(string url, object contentToSend)
        {
            TransmitterResponseDto result = null;
            var content = await _DoHttpPostAsync(url, contentToSend);
            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto>(content);
            return result;
        }

        private static AsyncRetryPolicy BuildRetryPolicy()
                            => Policy
                                .Handle<Exception>()
                                .WaitAndRetryAsync(retryCount: 3,
                                                    sleepDurationProvider: (retryCounter) => TimeSpan.FromMilliseconds(500));


        public async static Task<TransmitterResponseDto<TResult>> DoHttpPostAsync<TResult>(string url, object contentToSend = null)
        {
            TransmitterResponseDto<TResult> result = null;

            var policy = BuildRetryPolicy();
            var content = await policy.ExecuteAsync(async () => await _DoHttpPostAsync(url, contentToSend));

            if (content == null)    //POTENCIAL 403 FORBIDDEN
                ActivationService.Inactivate();

            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto<TResult>>(content);

            return result;
        }


        private async static Task<string> _DoHttpPostAsync(string url, object contentToSend)
        {
            string contentResponse = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = System.Environment.OSVersion.ToString();
            request.Method = "POST";

            string jsonContent = contentToSend == null ? "{}" : JsonConvert.SerializeObject(contentToSend);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            if (!string.IsNullOrEmpty(ConfigService.Config?.AccessToken))
                request.Headers.Add("Authorization", $"Bearer {ConfigService.Config.AccessToken}");
            request.Headers.Add("INSTANCE_ID", ConfigService.Config?.InstanceId);
            request.Headers.Add("APP_VERSION", _currentVersion);
            request.Headers.Add("LANG", _currentLanguage);

            using (Stream dataStream = await request.GetRequestStreamAsync())
            {
                await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    contentResponse = await reader.ReadToEndAsync();
                }
            }

            return contentResponse;
        }

        public async static Task<TransmitterResponseDto> DoHttpGetAsync(string url)
        {
            TransmitterResponseDto result = null;
            var content = await _DoHttpGetAsync(url);

            if (content == null)    //POTENCIAL 403 FORBIDDEN
                ActivationService.Inactivate();

            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto>(content);
            return result;
        }

        public async static Task<TransmitterResponseDto<TResult>> DoHttpGetAsync<TResult>(string url)
        {
            TransmitterResponseDto<TResult> result = null;

            var policy = BuildRetryPolicy();
            var content = await policy.ExecuteAsync(async () => await _DoHttpGetAsync(url));

            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto<TResult>>(content);
            return result;
        }

        private async static Task<string> _DoHttpGetAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", System.Environment.OSVersion.ToString());

            string content = null;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (!string.IsNullOrEmpty(ConfigService.Config?.AccessToken))
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ConfigService.Config.AccessToken);
                requestMessage.Headers.Add("INSTANCE_ID", ConfigService.Config?.InstanceId);
                requestMessage.Headers.Add("APP_VERSION", _currentVersion);
                requestMessage.Headers.Add("LANG", _currentLanguage);

                var response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                    content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }
    }
}
