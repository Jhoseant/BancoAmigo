//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banco_Amigo.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BancoAmigoEntities : DbContext
    {
        public BancoAmigoEntities()
            : base("name=BancoAmigoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ba_persona> ba_persona { get; set; }
        public virtual DbSet<ba_roles> ba_roles { get; set; }
        public virtual DbSet<ba_usuarios> ba_usuarios { get; set; }
    }
}
