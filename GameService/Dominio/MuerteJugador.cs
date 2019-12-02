using System;

namespace GameService.Dominio
{
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
