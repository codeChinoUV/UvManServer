using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    public class Corredor
    {
        public int VidasDisponibles { get; set; }
        
        public int PoderesDisponibles { get; set; }

        public CorredorAdquiridoModel CorredorAUsar { get; set; }

    }
}
