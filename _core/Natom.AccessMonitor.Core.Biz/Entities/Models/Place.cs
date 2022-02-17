using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
    [Table("Place")]
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Cliente Cliente { get; set; }

        public DateTime? RemovedAt { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}