using System;

namespace GameService.Dominio
{
    /// <summary>
    /// Almacena los datos de un evento muerte de jugador
    /// </summary>
    [Serializable]
    public class MuerteJugador
    {

        public String Usuario { get; }

        public int CantidadDeVidas { get; }

        public MuerteJugador(String usuario, int cantidadDeVidas)
        {
            Usuario = usuario;
            CantidadDeVidas = cantidadDeVidas;
        }
    }
}
