namespace DevBol.Domain.Events
{
    public class JogoFinalizadoEvent
    {
        public Guid JogoId { get; set; }

        // Você pode adicionar mais campos se o Worker precisar de algo imediato, 
        // mas o ID costuma ser suficiente para ele buscar o restante no banco.
    }
}