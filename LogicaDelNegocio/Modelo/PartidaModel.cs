using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    public class PartidaModel
    {
        public int PuntuacionActual { get; set; }
        public int UvCoinsGanadas { get; set; }
        public Seguidor Seguidor { get; set; }
        public Corredor Corredor { get; set; }
    }
}
