using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDelNegocio.Modelo
{
    /// <summary>
    /// Clase Usuario
    /// Contiene toda la información del usuario que se almacenara en la base de datos
    /// </summary>
    public class UsuarioModel
    {
        public String correo { get; set; }
        public String edad { get; set; }
        public AvanceModel avanceDeUsuario { get; set; }

    }
}
