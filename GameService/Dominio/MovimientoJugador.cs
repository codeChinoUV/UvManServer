using System;

namespace GameService.Dominio
{
    [Serializable]
    public class MovimientoJugador
    {

        public String Usuario { get; }

        public float PosicionX { get; }

        public float PosicionY { get; }

        public String Movimiento { get; }

        public MovimientoJugador(String usuario, float posicionX, float posicionY, String movimiento)
        {
            Usuario = usuario;
            PosicionX = posicionX;
            PosicionY = posicionY;
            Movimiento = movimiento;
        }

    }
}
