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
    public class TokenRepository : BaseRepository
    {
        public TokenRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task AddAsync(Token token)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                await db.InsertAsync(token);
            }
        }

        public async Task RemoveAsync(Token token)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                await db.DeleteAsync(token);
            }
        }

        public async Task<List<Token>> GetAllAsync()
        {
            List<Token> result = new List<Token>();
            using (var db = new SqlConnection(_connectionString))
            {
                var enumerable = await db.GetAllAsync<Token>();
                result = enumerable.ToList();
            }
            return result;
        }
    }
}
