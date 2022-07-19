using ProjetoMVC01.Entities;
using ProjetoMVC01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMVC01.Interfaces
{
    public interface IProdutoRepository
    {
        //métodos abstratos
        void Inserir(Produto produto);
        void Alterar(Produto produto);
        void Excluir(Produto produto);

        List<Produto> Consultar();
        Produto ObterPorId(Guid idProduto);

        List<Produto> ConsultarPorDatas (DateTime dataMin, DateTime dataMax);

        List<ProdutoGraficoModel> ConsultarTotal();

    }

}
