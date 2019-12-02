using System;

namespace GameService.Dominio
{
    [Serializable]
    public class InicioPartida
    {

        public bool CambiarPantallaMultijugador { get; set; }

        public bool IniciarPartida { get; set; }
    }
}
