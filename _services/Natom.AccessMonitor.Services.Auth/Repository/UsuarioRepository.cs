using Dapper;
using Dapper.Contrib.Extensions;
using Natom.AccessMonitor.Services.Auth.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Auth.Repository
{
    public class UsuarioRepository : BaseRepository
    {
        public UsuarioRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task AddAsync(Usuario usuario)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                await db.InsertAsync(usuario);
            }
        }

        public async Task<Usuario> GetByEmailAndScopeAsync(string email, string scope)
        {
            Usuario usuario = null;
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_usuarios_select_by_email_and_scope] @Email, @Scope";
                using (var results = db.QueryMultiple(sql, new { Email = email, Scope = scope }))
                {
                    usuario = (await results.ReadAsync<Usuario>()).ToList().FirstOrDefault();
                    if (usuario != null)
                        usuario.Permisos = (await results.ReadAsync<UsuarioPermiso>()).ToList();
                }
            }
            return usuario;
        }
    }
}
