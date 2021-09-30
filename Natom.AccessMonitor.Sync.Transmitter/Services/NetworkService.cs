using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class NetworkService
    {
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

        public async static Task<TransmitterResponseDto<TResult>> DoHttpPostAsync<TResult>(string url, object contentToSend)
        {
            TransmitterResponseDto<TResult> result = null;
            var content = await _DoHttpPostAsync(url, contentToSend);
            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto<TResult>>(content);
            return result;
        }

        private async static Task<string> _DoHttpPostAsync(string url, object contentToSend)
        {
            string contentResponse = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            string jsonContent = JsonConvert.SerializeObject(contentToSend);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

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
            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto>(content);
            return result;
        }

        public async static Task<TransmitterResponseDto<TResult>> DoHttpGetAsync<TResult>(string url)
        {
            TransmitterResponseDto<TResult> result = null;
            var content = await _DoHttpGetAsync(url);
            if (!string.IsNullOrEmpty(content))
                result = JsonConvert.DeserializeObject<TransmitterResponseDto<TResult>>(content);
            return result;
        }

        private async static Task<string> _DoHttpGetAsync(string url)
        {
            HttpClient client = new HttpClient();
            var content = await client.GetStringAsync(url);
            return content;
        }
    }
}
