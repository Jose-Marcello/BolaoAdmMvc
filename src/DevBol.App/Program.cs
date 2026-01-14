using DevBol.Presentation.Configurations;
using DevBol.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DevBol.Domain.Models.Usuarios;
using Npgsql.EntityFrameworkCore.PostgreSQL; // NOVO: Driver do PostgreSQL
using System.Text.Json; // Manter para JsonNamingPolicy
using System.Text.Json.Serialization; // Manter para ReferenceHandler
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // Necessário para LogLevel

namespace DevBol.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.WriteLine("Iniciando a configuração dos serviços...");

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // --- INÍCIO DA CONFIGURAÇÃO DO POSTGRESQL ---
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DI
            builder.Services.ResolveDependencies();

            // Log de configurações (mantido do original)
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            // 🛑 CONFIGURAÇÃO DO DBContext: TROCA DE UseSqlServer PARA UseNpgsql
            builder.Services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseNpgsql(connectionString) // 🛑 AQUI ESTÁ A MUDANÇA
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
            });


            // Identity Configuration (Usa MeuDbContext, que agora é PostgreSQL)
            builder.Services.AddIdentity<Usuario, IdentityRole>()
            .AddEntityFrameworkStores<MeuDbContext>()
            .AddDefaultTokenProviders();

            // Adicione outras chamadas de Identity que estavam comentadas (se necessário)
            // Exemplo: builder.Services.AddScoped<IDbContextFactory, MeuDbContextFactory>();

            // --- FIM DA CONFIGURAÇÃO DO POSTGRESQL ---


            builder.Services.AddControllersWithViews();


            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Configuração de Identity (opcional, mantida do original)
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            });


            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            // --- PIPELINE DE MIDDLEWARE (MANTIDO) ---

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseExceptionHandler("/erro/500");
                // app.UseStatusCodePagesWithRedirects("/erro/{0}");
            }

            Console.WriteLine("Serviços configurados. Iniciando o pipeline de middleware...");

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            // Seus registros de log de ciclo de vida mantidos
            lifetime.ApplicationStarted.Register(() => { Console.WriteLine("Application started. Press Ctrl+C to shut down."); });
            lifetime.ApplicationStopping.Register(() => { Console.WriteLine("Application is shutting down..."); });
            lifetime.ApplicationStopped.Register(() => { Console.WriteLine("Application shutdown complete."); });


            // Middlewares
            app.UseHttpsRedirection(); //fechados p/ teste
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //fechados p/ teste
            app.UseAuthorization();

            // Endpoints (Suas rotas de administração)
            app.UseEndpoints(endpoints =>
            {
                // Rota de consulta de apostas, rotas de jogo, etc.
                endpoints.MapControllerRoute(name: "ConsultarApostas", pattern: "ApostaRodada/ConsultarApostasRodada", defaults: new { controller = "ApostaRodada", action = "ConsultarApostasRodada" });
                endpoints.MapControllerRoute(name: "DeletarApostas", pattern: "ApostaRodada/DeletarApostasRodadas", defaults: new { controller = "ApostaRodada", action = "DeletarApostasRodadas" });
                endpoints.MapControllerRoute(name: "GerenciarRodadas", pattern: "Rodada/GerenciarRodadas", defaults: new { controller = "Rodada", action = "GerenciarRodadas" });
                endpoints.MapControllerRoute(name: "LarcarRodadas", pattern: "Rodada/ListaRodadas", defaults: new { controller = "Rodada", action = "ListaRodadas" });
                endpoints.MapControllerRoute(name: "ManterJogos", pattern: "Jogo/ManterJogos/{id}", defaults: new { controller = "Jogo", action = "ManterJogos" });
                endpoints.MapControllerRoute(name: "EditarJogo", pattern: "Jogo/editar-jogo/{id}", defaults: new { controller = "Jogo", action = "Edit" });
                endpoints.MapControllerRoute(name: "SalvarJogo", pattern: "LancarPlacar/SalvarJogo", defaults: new { controller = "LancarPlacar", action = "SalvarJogo" });
                endpoints.MapControllerRoute(name: "AtualizarRankingRodada", pattern: "LancarPlacar/atualizar-ranking-rodada/{rodadaid}", defaults: new { controller = "LancarPlacar", action = "AtualizarRankingRodada" });
                endpoints.MapControllerRoute(name: "SalvarApostas", pattern: "ApostadorCampeonato/SalvarApostas", defaults: new { controller = "ApostadorCampeonato", action = "SalvarApostas" });

                // Rota Padrão (default)
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}
