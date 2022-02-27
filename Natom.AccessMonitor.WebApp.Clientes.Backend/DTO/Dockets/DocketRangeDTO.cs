using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Dockets
{
    public class DocketRangeDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("dayOfWeek")]
        public int DayOfWeek { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }


        public DocketRangeDTO MapFrom(DocketRange entity)
        {
            EncryptedId = EncryptionService.Encrypt<DocketRange>(entity.DocketRangeId);
            DayOfWeek = entity.DayOfWeek;
            From = entity.From?.ToString(@"hh\:mm");
            To = entity.To?.ToString(@"hh\:mm");

            return this;
        }

        public DocketRange ToModel(DocketRangeDTO dto, int docketId)
        {
            return new DocketRange
            {
                DocketRangeId = EncryptionService.Decrypt<int, DocketRange>(dto.EncryptedId),
                DayOfWeek = dto.DayOfWeek,
                From = string.IsNullOrEmpty(dto.From) ? (TimeSpan?)null : TimeSpan.Parse($"{dto.From}:00"),
                To = string.IsNullOrEmpty(dto.To) ? (TimeSpan?)null : TimeSpan.Parse($"{dto.To}:00"),
                DocketId = docketId
            };
        }
    }
}