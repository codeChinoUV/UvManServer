using System;

namespace LogicaDelNegocio.Util
{
    public class GeneradorCodigo
    {
        private const int TAMANO_DE_CADENA_DE_TEXTO = 10;
        private const int TAMANO_DE_CODIGO_VERIFICACION = 10;

        /// <summary>
        /// Genera un codigo aleatorio de 10 digitos
        /// </summary>
        /// <returns>Una cadena con 10 digitos aleatorios</returns>
        public static String GenerarCodigoActivacion()
        {
            int tiempoIniciado = Environment.TickCount;
            Random generador = new Random(tiempoIniciado);
            String codigo = "";
            for (int i = 0; i <TAMANO_DE_CODIGO_VERIFICACION; i++)
            {
                codigo += Convert.ToString(generador.Next(0, 9));
            }
            return codigo;
        }

        /// <summary>
        /// Genera una cadena de texto aleatorio de 5 caracteres
        /// </summary>
        /// <returns>String</returns>
        public static string GenerarCadena()
        {
            Random GeneradorRandom = new Random();
            string CadenaDeCaracteresPosibles = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int Largo = CadenaDeCaracteresPosibles.Length;
            char Caracter;
            string CadenaGenerada = string.Empty;
            for (int i = 0; i < TAMANO_DE_CADENA_DE_TEXTO; i++)
            {
                Caracter = CadenaDeCaracteresPosibles[GeneradorRandom.Next(Largo)];
                CadenaGenerada += Caracter.ToString();
            }
            return CadenaGenerada;
        }
    }

   
}
