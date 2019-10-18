using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Contiene la información de un corredor de la partida
    /// </summary>
    public class PersonajeCorredorModel
    {
        public double velocidad { get; set; }
        public String nombre { get; set; }
        public String poder { get; set; }
        public int precio { get; set; }

        public PersonajeCorredorModel()
        {
            velocidad = 0.2;
        }
    }
}
