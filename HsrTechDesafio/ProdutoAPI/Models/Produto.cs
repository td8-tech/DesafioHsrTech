namespace ProdutoAPI.Models
{
    public abstract class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int CreatedByUserId { get; set; } // ID do usuário que criou o produto

        // Propriedade adicionada para o Discriminator
        public string Discriminator { get; set; } // Define o tipo (Livro/Electronicos)
    }
}
