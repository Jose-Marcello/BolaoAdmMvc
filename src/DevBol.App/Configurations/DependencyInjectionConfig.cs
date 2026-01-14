using DevBol.Application.Base;
using DevBol.Application.Services.Campeonatos;
using DevBol.Application.Services.Rodadas;
using DevBol.Application.Services.Equipes;
using DevBol.Infrastructure.Data.Context;
using DevBol.Presentation.Services;
using DevBol.Application.Services.Ufs;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Application.Services.Jogos;
using DevBol.Application.Services.Apostadors;
using DevBol.Application.Services.Apostas;
using DevBol.Infrastructure.Data.Repository.Apostadores;
using DevBol.Infrastructure.Data.Repository.Apostas;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Equipes;
using DevBol.Infrastructure.Data.Repository.Estadios;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Infrastructure.Data.Interfaces.Apostadores;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Equipes;
using DevBol.Infrastructure.Data.Interfaces.Estadios;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Infrastructure.Data.Interfaces.RankingRodadas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Domain.Notificacoes;
using DevBol.Application.Interfaces.Equipes;
using DevBol.Application.Interfaces.Rodadas;
using DevBol.Application.Interfaces.RankingRodadas;
using DevBol.Application.Interfaces.Ufs;
using DevBol.Application.Interfaces.Estadios;
using DevBol.Application.Interfaces.Jogos;
using DevBol.Application.Interfaces.Apostadores;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Application;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;
using DevBol.Application.Interfaces.Usuarios;
using DevBol.Application.Services.Usuarios;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Domain.Interfaces.Ufs;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Infrastructure.Data.Repository.Apostas;
using DevBol.Application.Services.Palpites;
using DevBol.Application.Services;
using DevBol.Presentation.BackgroundServices;
using DevBol.Application.BackGroundsServices;

namespace DevBol.Presentation.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<MeuDbContext>();  //já está injetado pelo Identity          
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>)); // Manter para outros cenários, se necessário
            services.AddSingleton<ILoggerFactory, LoggerFactory>(); // Garante que LoggerFactory esteja registrado
            //services.AddScoped<IUrlHelper, UrlHelper>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            //services.AddScoped<IEmailService, EmailService>();  //fechmanento provisório            
            services.AddScoped<ICampeonatoRepository, CampeonatoRepository>();
            services.AddScoped<IRodadaRepository, RodadaRepository>();
            services.AddScoped<IRankingRodadaRepository, RankingRodadaRepository>();
            services.AddScoped<IEquipeRepository, EquipeRepository>();
            services.AddScoped<IUfRepository, UfRepository>();
            services.AddScoped<IEstadioRepository, EstadioRepository>();
            services.AddScoped<IJogoRepository, JogoRepository>();
            services.AddScoped<IApostadorRepository, ApostadorRepository>();
            services.AddScoped<IEquipeCampeonatoRepository, EquipeCampeonatoRepository>();
            services.AddScoped<IApostadorCampeonatoRepository, ApostadorCampeonatoRepository>();
            services.AddScoped<IApostaRodadaRepository, ApostaRodadaRepository>();
            services.AddScoped<IPalpiteRepository, PalpiteRepository>();
            services.AddScoped<IImageUploadService, ImageUploadService>();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<ICampeonatoService, CampeonatoService>();
            services.AddScoped<IEquipeCampeonatoService, EquipeCampeonatoService>();
            services.AddScoped<IApostadorCampeonatoService, ApostadorCampeonatoService>();
            services.AddScoped<IEquipeService, EquipeService>();
            services.AddScoped<IRodadaService, RodadaService>();
            services.AddScoped<IRankingRodadaService, RankingRodadaService>();
            services.AddScoped<IEstadioService, EstadioService>();
            services.AddScoped<IJogoService, JogoService>();
            services.AddScoped<IApostadorService, ApostadorService>();
            services.AddScoped<IApostaRodadaService, ApostaRodadaService>();
            services.AddScoped<IPalpiteService, PalpiteService>();
            services.AddScoped<IUfService, UfService>();

            // Registro da Fila de Mensageria Interna como Singleton
            services.AddSingleton<LancamentoPlacarQueue>();
            //services.AddSingleton<RankingUpdateQueue>();

            // Se você já quiser deixar o Worker engatilhado para amanhã:
            services.AddHostedService<RankingWorker>(); 
            //services.AddHostedService<LancamentoPlacarWorker>();

            return services;
        }
    }
}