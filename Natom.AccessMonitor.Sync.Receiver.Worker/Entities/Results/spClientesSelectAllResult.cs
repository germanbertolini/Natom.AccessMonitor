using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker.Entities.Results
{
    public class spClientesSelectAllResult
    {
        public int ClienteId { get; set; }
        public bool EsEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
    }
}
