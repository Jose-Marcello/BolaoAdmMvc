using AutoMapper;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Ufs;

namespace DevBol.Presentation.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {            
            
            CreateMap<Campeonato, CampeonatoViewModel>().ReverseMap();
            CreateMap<Rodada, RodadaViewModel>().ReverseMap();
            CreateMap<Equipe, EquipeViewModel>().ReverseMap();
            CreateMap<EquipeCampeonato, EquipeCampeonatoViewModel>().ReverseMap();
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoViewModel>().ReverseMap();
            CreateMap<Uf, UfViewModel>().ReverseMap();
            CreateMap<Uf, UfDataTableViewModel>().ReverseMap();
            CreateMap<Estadio, EstadioViewModel>().ReverseMap();
            CreateMap<Jogo, JogoViewModel>().ReverseMap();
            CreateMap<Jogo, JogoAtualizacaoViewModel>().ReverseMap();            
            CreateMap<JogoFinalizadoEvent, ApostadorViewModel>().ReverseMap();
            CreateMap<ApostaRodada, ApostaRodadaViewModel>().ReverseMap();
            //CreateMap<ApostaRodada, SalvarApostaViewModel>().ReverseMap();
            CreateMap<Palpite, PalpiteViewModel>().ReverseMap();
            CreateMap<RankingRodada, RankingRodadaViewModel>().ReverseMap();

        }
    }
}