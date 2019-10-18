using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Avance
    /// Contiene todo el progreso del jugador
    /// </summary>
    public class AvanceModel
    {
        public int uvCoins { get; set; }
        public int mejorPuntuacion { get; set; }
        public List<PersonajeCorredorModel> corredoresAdquiridos { get; set; }

    }
}
