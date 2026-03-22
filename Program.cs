using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // gera o documento OpenAPI
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

            var app = builder.Build();

            // endpoint do json OpenAPI
            app.MapOpenApi();

            // interface visual do Scalar
            app.MapScalarApiReference("/doc", options =>
            {
                // Design e Layout
                options.WithTheme(ScalarTheme.Laserwave).WithClassicLayout();
            });


            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
