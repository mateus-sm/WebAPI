# Guia de Configuração — MySQL com ADO.NET

## 1. Pacote Necessário

Instalar via NuGet:

- `MySql.Data`

---

## 2. String de Conexão

Em `appsettings.json`, string de conexão:

```json
"StringConexao": "Server=;Database=;Uid=;Pwd=;"
```

---

## 3. Configuração no `Program.cs`

Lê a string do `appsettings.json` e registra como variável de ambiente:

```csharp
var stringConexao = builder.Configuration["StringConexao"];
Environment.SetEnvironmentVariable("STRING_CONEXAO", stringConexao);
```

```csharp
// IOC
builder.Services.AddScoped<Repository.MySqlDbContext>();
builder.Services.AddScoped<Repository.AlunoRepository>(); // Exemplo de repository generico
```

Registrar contexto e os repositories no IOC antes de `builder.Build()`:

> `AddScoped` garante que uma instância é criada por requisição HTTP e descartada ao final dela.

---

## 4. Criando o `MySqlDbContext`

Pasta `Repository/`:

```csharp
namespace WebAPI.Repository
{
    public class MySqlDbContext : IDisposable
    {
        private readonly MySql.Data.MySqlClient.MySqlConnection _conexao;

        public MySqlDbContext()
        {
            if (Environment.GetEnvironmentVariable("StringConexao") == null)
                throw new Exception("Variável de ambiente STRING_CONEXAO não encontrada");

            string stringConexao = Environment.GetEnvironmentVariable("StringConexao");
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
```

---

## 5. Criando um Repository

Injetar o `MySqlDbContext` via construtor:

```csharp
namespace WebAPI.Repository
{
    public class CidadeRepository
    {
        private readonly MySqlDbContext _context;

        public CidadeRepository(MySqlDbContext context)
        {
            _context = context;
        }
    }
}
```

---

## 6. Uso — Exemplos

Abrir conexão sem se preocupar com fechar
```csharp
    using (var cmd = _conexao.GetConexao().CreateCommand()) { }
```

Criar comando SQL
```csharp
    cmd.CommandText = "SELECT * FROM aluno20.Cidades
```

Executar leitura 'SELECT'
```csharp
    var dr = cmd.ExecuteReader();
```

Exemplo de método get com `SELECT`:

```csharp
    public IEnumerable<Entidades.Cidade> Get()
    {
        List<Entidades.Cidade> cid = new();

        try
        {
            using (var cmd = _conexao.GetConexao().CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM aluno20.Cidades";
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    cid.Add(Map(dr));
                }
            }
        } catch (MySqlException ex)
        {
            throw;
        }

        return cid;
    }

    public Entidades.Cidade Map(MySql.Data.MySqlClient.MySqlDataReader dr)
    {
        Cidade cid = new Cidade();
        cid.CidadeId = dr.GetInt32("CidadeId");
        cid.Nome = dr.GetString("Nome");
        cid.Sigla = dr.GetString("Sigla");
        cid.IBGEMunicipio = dr.GetInt32("IBGEMunicipio");
        cid.Latitude = dr.GetDecimal("Latitude");
        cid.Longitude = dr.GetDecimal("Longitude");

        return cid;
    }
```

### Extra

| Ponto | Motivo |
|---|---|
| `dr.Close()` após `ExecuteReader()` | O DataReader mantém a conexão ocupada — qualquer comando seguinte na mesma conexão lança exceção se o reader ainda estiver aberto |
| `cmd.Parameters.Clear()` | Limpa os parâmetros antes de reutilizar o mesmo `cmd` para outro comando |
| `using` no `CreateCommand()` | Garante o dispose do comando ao sair do bloco |
| `transacao?.Rollback()` | O `?.` é seguro caso a transação nunca tenha sido iniciada |
| `throw` sem argumento | Preserva o stack trace; `throw ex` o perderia |
