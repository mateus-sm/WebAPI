using System.Data;

namespace WebAPI.Repository
{
    public class MySqlDbContext : IDisposable
    {
        private readonly MySql.Data.MySqlClient.MySqlConnection _conexao;

        public MySqlDbContext()
        {
            if (Environment.GetEnvironmentVariable("StringConexao") == null)
                throw new Exception("Variável de ambiente STRING_CONEXAO não encontrada");

            string? stringConexao = Environment.GetEnvironmentVariable("StringConexao");
            _conexao = new MySql.Data.MySqlClient.MySqlConnection(stringConexao);
        }

        public MySql.Data.MySqlClient.MySqlConnection GetConexao()
        {
            if (_conexao.State == ConnectionState.Closed)
                _conexao.Open();
            return _conexao;
        }

        public void Dispose()
        {
            if (_conexao.State == ConnectionState.Open)
                _conexao.Close();
            _conexao.Dispose();
        }
    }
}
