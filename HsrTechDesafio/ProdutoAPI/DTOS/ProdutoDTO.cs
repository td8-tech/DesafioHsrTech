namespace ProdutoAPI.DTOS
{
    public class ProdutoDTO
    {
        public string Nome { get; set; }
        public string Tipo { get; set; } // "Livro" ou "Electronicos"
        public string Autor { get; set; }
        public int PeriodoGarantia { get; set; }
    }
}
