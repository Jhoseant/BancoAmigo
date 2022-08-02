using System.ComponentModel;

namespace Banco_Amigo.Models.ViewModels
{
    using System;
    using System.Collections.Generic;

    public partial class PersonaData
    {
        public IEnumerable<ba_persona> Personas { get; set; }
        public IEnumerable<ba_usuarios> Usuarios { get; set; }
    }
}
