using System.Collections.Generic;

namespace CirrasTec.API.Data
{
    public interface IProdutoRepository
    {

        void Inserir(Produto produto);
        void Editar(Produto produto);
        void Excluir(Produto produto);
        Produto Obter(int id);
        Produto ObterPorCodigo(string codigo);
        IEnumerable<Produto> Obter();


    }
}