using Natom.AccessMonitor.Core.Biz.Entities.Models;
using Natom.AccessMonitor.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.DTO.Clientes
{
	public class ClienteMontoDTO
	{
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("monto")]
		public decimal Monto { get; set; }

		[JsonProperty("desde")]
		public DateTime Desde { get; set; }

		public ClienteMontoDTO From(ClienteMonto entity)
		{
			EncryptedId = EncryptionService.Encrypt<ClienteMonto>(entity.ClienteMontoId);
			Monto = entity.Monto;
			Desde = entity.Desde;

			return this;
		}

		public ClienteMonto ToModel()
		{
			return new ClienteMonto
			{
				ClienteMontoId = EncryptionService.Decrypt<int, ClienteMonto>(EncryptedId),
				Desde = Desde,
				Monto = Monto
			};
		}
	}
}
