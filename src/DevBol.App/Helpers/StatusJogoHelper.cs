using DevBol.Domain.Models.Jogos; // Adicione o namespace da sua enumeração

public static class StatusJogoHelper
{
    public static string ExibirStatusJogo(StatusJogo stat)
    {
        switch (stat)
        {
            case StatusJogo.NaoIniciado: // Assumindo que seja um valor da sua enumeração
                return "Não Iniciado";
            case StatusJogo.EmAndamento:
                return "Em Andamento";
            case StatusJogo.Finalizado:
                return "Finalizado";
            // Adicione outros casos conforme os valores da sua enumeração
            default:
                return "Desconhecido";
        }
    }
}