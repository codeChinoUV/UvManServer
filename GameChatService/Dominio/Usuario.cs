﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameChatService.Dominio
{
    [DataContract]
    public class Usuario
    {
        [DataMember]
        public String correo { get; set; }

        [DataMember]
        public String edad { get; set; }
    }
}
