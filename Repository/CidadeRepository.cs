using MySql.Data.MySqlClient;
using System.Text;
using WebAPI.Entidades;

namespace WebAPI.Repository
{
    public class CidadeRepository
    {
        private readonly MySqlDbContext _conexao;

        public CidadeRepository(MySqlDbContext conexao)
        {
            _conexao = conexao;
        }

        public void SaveAllCities(List<Entidades.Cidade> cidades)
        {
            MySql.Data.MySqlClient.MySqlTransaction? transacao = null;

            try
            {
                var conn = _conexao.GetConexao();

                using (var cmd = conn.CreateCommand())
                {
                    transacao = conn.BeginTransaction();
                    cmd.Transaction = transacao; // Vincula o comando à transação!

                    cmd.CommandText = @"INSERT INTO Cidades (Nome, Sigla, IBGEMunicipio, Latitude, Longitude)
                                 VALUES (@Nome, @Sigla, @IBGEMunicipio, @Latitude, @Longitude);";

                    cmd.Parameters.Add("@Nome", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@Sigla", MySqlDbType.VarChar);
                    cmd.Parameters.Add("@IBGEMunicipio", MySqlDbType.Int32);
                    cmd.Parameters.Add("@Latitude", MySqlDbType.Decimal);
                    cmd.Parameters.Add("@Longitude", MySqlDbType.Decimal);

                    foreach (var cidade in cidades)
                    {
                        // Apenas substitui os valores na memória a cada volta do loop
                        cmd.Parameters["@Nome"].Value = cidade.Nome.Trim('"');
                        cmd.Parameters["@Sigla"].Value = cidade.Sigla;
                        cmd.Parameters["@IBGEMunicipio"].Value = cidade.IBGEMunicipio;

                        // Trata o DBNull caso a coordenada seja nula
                        cmd.Parameters["@Latitude"].Value = cidade.Latitude.HasValue ? cidade.Latitude.Value : DBNull.Value;
                        cmd.Parameters["@Longitude"].Value = cidade.Longitude.HasValue ? cidade.Longitude.Value : DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }

                    transacao.Commit();
                }
            }
            catch (MySqlException)
            {
                transacao?.Rollback();
                throw;
            }
        }

        public void SaveAllCities2(List<Entidades.Cidade> cidades)
        {
            var conn = _conexao.GetConexao();
            MySql.Data.MySqlClient.MySqlTransaction? transacao = null;

            try
            {
                transacao = conn.BeginTransaction();

                // Vamos enviar de 1000 em 1000
                int tamanhoDoLote = 1000;

                for (int i = 0; i < cidades.Count; i += tamanhoDoLote)
                {
                    var lote = cidades.Skip(i).Take(tamanhoDoLote).ToList();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = transacao;

                        var sql = new StringBuilder("INSERT INTO Cidades (CidadeId, Nome, Sigla, IBGEMunicipio, Latitude, Longitude) VALUES ");

                        for (int j = 0; j < lote.Count; j++)
                        {
                            var cidade = lote[j];

                            sql.Append($"(@cid{j}, @n{j}, @s{j}, @ibge{j}, @lat{j}, @lon{j})");
                            sql.Append(j == lote.Count - 1 ? ";" : ", ");

                            cmd.Parameters.AddWithValue($"@cid{j}", cidade.CidadeId);
                            cmd.Parameters.AddWithValue($"@n{j}", cidade.Nome.Trim('"'));
                            cmd.Parameters.AddWithValue($"@s{j}", cidade.Sigla);
                            cmd.Parameters.AddWithValue($"@ibge{j}", cidade.IBGEMunicipio);
                            cmd.Parameters.AddWithValue($"@lat{j}", cidade.Latitude.HasValue ? cidade.Latitude.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue($"@lon{j}", cidade.Longitude.HasValue ? cidade.Longitude.Value : DBNull.Value);
                        }

                        cmd.CommandText = sql.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }

                transacao.Commit();
            }
            catch (MySqlException)
            {
                transacao?.Rollback();
                throw;
            }
        }

        public bool Create(Entidades.Cidade cidade)
        {
            bool flag = false;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cidades (Nome, Sigla, IBGEMunicipio, Latitude, Longitude)
                                         VALUES (@Nome, @Sigla, @IBGEMunicipio, @Latitude, @Longitude);";

                    cmd.Parameters.AddWithValue("@Nome", cidade.Nome);
                    cmd.Parameters.AddWithValue("@Sigla", cidade.Sigla);
                    cmd.Parameters.AddWithValue("@IBGEMunicipio", cidade.IBGEMunicipio);
                    cmd.Parameters.AddWithValue("@Latitude", cidade.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", cidade.Longitude);

                    cmd.ExecuteNonQuery();
                    cidade.CidadeId = (int)cmd.LastInsertedId;
                    
                    flag = true;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return flag;
        }

        public List<Entidades.Cidade> Read()
        {
            List<Entidades.Cidade> cid = new();
            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Cidades";
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cid.Add(Map(dr));
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }
            return cid;
        }

        public bool Update(Entidades.Cidade cidade)
        {
            bool flag = false;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cidades 
                                        SET Nome = @Nome, Sigla = @Sigla, IBGEMunicipio = @IBGEMunicipio, Latitude = @Latitude, Longitude = @Longitude
                                        WHERE CidadeId = @id;";

                    cmd.Parameters.AddWithValue("@Nome", cidade.Nome);
                    cmd.Parameters.AddWithValue("@Sigla", cidade.Sigla);
                    cmd.Parameters.AddWithValue("@IBGEMunicipio", cidade.IBGEMunicipio);
                    cmd.Parameters.AddWithValue("@Latitude", cidade.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", cidade.Longitude);
                    cmd.Parameters.AddWithValue("@id", cidade.CidadeId);
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    flag = linhasAfetadas > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return flag;
        }

        public bool Delete(int id)
        {
            bool flag = false;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Cidades WHERE CidadeId = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    flag = linhasAfetadas > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return flag;
        }

        public Entidades.Cidade ReadById(int id)
        {
            Cidade cid = null;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Cidades WHERE CidadeId = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        cid = Map(dr);
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cid;
        }

        public int Count()
        {
            int count = 0;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Cidades";
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return count;
        }

        public List<string> ReadEstados()
        {
            var siglas = new List<string>();

            using (var cmd = _conexao.GetConexao().CreateCommand())
            {
                cmd.CommandText = "SELECT DISTINCT Sigla FROM Cidades";

                // O DataReader também precisa do using para não travar o banco!
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        siglas.Add(dr.GetString("Sigla"));
                    }
                }
            }

            return siglas;
        }

        public List<Entidades.Cidade> ReadByEstado(string sigla)
        {
            var cidades = new List<Entidades.Cidade>();

            using (var cmd = _conexao.GetConexao().CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Cidades WHERE Sigla = @sigla";
                cmd.Parameters.AddWithValue("@sigla", sigla);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cidades.Add(Map(dr));
                    }
                }
            }

            return cidades;
        }

        public Entidades.Cidade Map(MySql.Data.MySqlClient.MySqlDataReader dr)
        {
            Cidade cid = new Cidade();
            cid.CidadeId = dr.GetInt32("CidadeId");
            cid.Nome = dr.GetString("Nome");
            cid.Sigla = dr.GetString("Sigla");
            cid.IBGEMunicipio = dr.GetInt32("IBGEMunicipio");
            cid.Latitude = dr.IsDBNull(dr.GetOrdinal("Latitude")) ? null : dr.GetDecimal("Latitude");
            cid.Longitude = dr.IsDBNull(dr.GetOrdinal("Longitude")) ? null : dr.GetDecimal("Longitude");
            return cid;
        }
    }
}
