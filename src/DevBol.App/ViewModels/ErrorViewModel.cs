namespace DevBol.Presentation.ViewModels
{
    public class ErrorViewModel
    {
        public int ErroCode { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }


        //testes DATATABLE
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);



    }
}