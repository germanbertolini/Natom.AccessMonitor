using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Clientes.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.DTO.Dockets
{
    public class DocketDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("docket_number")]
        public string DocketNumber { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("dni")]
        public string DNI { get; set; }

        [JsonProperty("title_encrypted_id")]
        public string TitleEncryptedId { get; set; }

        [JsonProperty("apply_ranges")]
        public bool ApplyRanges { get; set; }

        [JsonProperty("ranges")]
        public List<DocketRangeDTO> Ranges { get; set; }

        [JsonProperty("hour_value")]
        public decimal? HourValue { get; set; }

        [JsonProperty("extra_hour_value")]
        public decimal? ExtraHourValue { get; set; }

        [JsonProperty("encrypted_place_id")]
        public string EncryptedPlaceId { get; set; }


        public DocketDTO From(Docket entity)
        {
            EncryptedId = EncryptionService.Encrypt<Docket>(entity.DocketId);
            DocketNumber = entity.DocketNumber;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            DNI = entity.DNI;
            TitleEncryptedId = EncryptionService.Encrypt<Title>(entity.TitleId);
            ApplyRanges = entity.Ranges != null && entity.Ranges.Count > 0 && entity.ApplyInOutControl;
            Ranges = entity.Ranges == null || entity.Ranges.Count == 0
                        ? new List<DocketRangeDTO>()
                        : entity.Ranges.Select(r => new DocketRangeDTO().MapFrom(r)).ToList();
            HourValue = entity.HourValue;
            ExtraHourValue = entity.ExtraHourValue;
            EncryptedPlaceId = EncryptionService.Encrypt<Place>(entity.PlaceId) ?? "";

            return this;
        }

        public Docket ToModel(int clientId)
        {
            int docketId = EncryptionService.Decrypt<int, Docket>(EncryptedId);
            return new Docket
            {
                DocketId = docketId,
                DocketNumber = DocketNumber,
                FirstName = FirstName,
                LastName = LastName,
                DNI = DNI,
                ApplyInOutControl = ApplyRanges && Ranges.Count > 0,
                TitleId = EncryptionService.Decrypt<int, Title>(TitleEncryptedId),
                Ranges = !ApplyRanges || Ranges.Count == 0
                            ? new List<DocketRange>()
                            : Ranges.Select(r => new DocketRangeDTO().ToModel(r, docketId)).ToList(),
                HourValue = HourValue,
                ExtraHourValue = ExtraHourValue,
                ClientId = clientId,
                PlaceId = ApplyRanges ? EncryptionService.Decrypt<int?, Place>(EncryptedPlaceId) : null
            };
        }
    }
}
