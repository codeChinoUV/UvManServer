using System;

namespace GameService.Dominio
{
    [Serializable]
    public class InicioPartida
    {
        public bool IniciarCuentaRegresivaInicioPartida { get; set; }

        public bool IniciarPartida { get; set; }

        public bool IniciarCuentaRegresivaInicioNivel { get; set; }

        public bool IniciarNivel { get; set; }
    }
}
