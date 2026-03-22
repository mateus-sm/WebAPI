# Guia de Configuração — OpenAPI (Swagger) com Scalar

## 1. Pacotes Necessários

Instale via NuGet:

- `Microsoft.AspNetCore.OpenApi`
- `Scalar.AspNetCore`

---

## 2. Configuração no `Program.cs`

### Antes de `var app = builder.Build();`

Registra o documento OpenAPI com suporte a autenticação JWT via Bearer:

```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Informe o token JWT: Bearer {token}"
        };
        return Task.CompletedTask;
    });
});
```

### Após `var app = builder.Build();`

Mapeia o endpoint JSON do OpenAPI e a interface visual do Scalar:

```csharp
// endpoint do JSON OpenAPI
app.MapOpenApi();

// interface visual do Scalar
app.MapScalarApiReference("/doc", options => 
{
    // Design e Layout
    options.WithTheme(ScalarTheme.Laserwave).WithClassicLayout();
});
```

---

## 3. Geração do Arquivo XML de Documentação

Ative no arquivo `.csproj`:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>1591</NoWarn>
</PropertyGroup>
```

> `NoWarn>1591` suprime avisos de membros públicos sem comentários XML.

---

## 4. Documentando Controllers

Use os atributos e comentários abaixo em cada endpoint:

### `[ProducesResponseType]`

Indica o tipo de resposta e o status HTTP que um endpoint pode retornar.
Permite ao Swagger gerar a documentação mostrando quais códigos de status e tipos de dados são esperados na resposta.

```csharp
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

### `[Produces]`

Indica o tipo de conteúdo que o endpoint retorna.
Ajuda o Swagger a documentar o tipo de mídia retornado pela API.

```csharp
[Produces("application/json")]
```

### `[Consumes]`

Indica o tipo de conteúdo que o endpoint aceita na requisição.
Especifica para o Swagger qual formato o cliente deve enviar no corpo da requisição.

```csharp
[Consumes("application/json")]
// ou para upload de arquivos:
[Consumes("multipart/form-data")]
```

### Comentários XML (`///`)

Comentários colocados acima de classes, métodos ou parâmetros usando `///`.
Permitem gerar documentação detalhada no Swagger (resumo, parâmetros, respostas).

```csharp
/// <summary>
/// Retorna um produto pelo ID.
/// </summary>
/// <param name="id">ID do produto</param>
/// <returns>Produto encontrado</returns>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
public IActionResult GetById(int id) { ... }
```

---

## 5. Documentando DTOs com Data Annotations

Use atributos de validação para documentar e validar os dados de entrada:

```csharp
public class CreateProductDto
{
    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; }

    [Range(0.01, 10000, ErrorMessage = "O preço deve ser entre 0,01 e 10.000")]
    public decimal Price { get; set; }

    [Range(0, 1000, ErrorMessage = "Estoque deve ser entre 0 e 1000 unidades")]
    public int Stock { get; set; }
}
```

| Atributo | Descrição |
|---|---|
| `[Required]` | Campo obrigatório |
| `[StringLength]` | Limite de caracteres |
| `[Range]` | Intervalo numérico permitido |
