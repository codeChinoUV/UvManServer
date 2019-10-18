using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Util
{
    public class GeneradorCodigo
    {
        /// <summary>
        /// Genera un codigo aleatorio de 10 digitos
        /// </summary>
        /// <returns>Una cadena con 10 digitos aleatorios</returns>
        public static String GenerarCodigoActivacion()
        {
            int tiempoIniciado = Environment.TickCount;
            Random generador = new Random(tiempoIniciado);
            String codigo = "";
            for (int i = 0; i <10; i++)
            {
                codigo += Convert.ToString(generador.Next(0, 9));
            }
            return codigo;
        }
    }
}
