using System;

namespace GameService.Dominio
{
    [Serializable]
    public class MovimientoJugador
    {

        public String Usuario { get; }

        public float PosicionX { get; }

        public float PosicionY { get; }

        public float MovimientoX { get; }
        
        public float MovimentoY { get; }

        public MovimientoJugador(String usuario, float posicionX, float posicionY, float movimientoX, float movimentoY)
        {
            Usuario = usuario;
            PosicionX = posicionX;
            PosicionY = posicionY;
            MovimientoX = movimientoX;
            MovimentoY = movimentoY;
        }

    }
}
