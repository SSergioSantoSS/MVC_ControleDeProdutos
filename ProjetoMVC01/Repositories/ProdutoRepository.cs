using Dapper;
using ProjetoMVC01.Entities;
using ProjetoMVC01.Interfaces;
using ProjetoMVC01.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMVC01.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        //atributo (campo) privado para armazenar a connectionstring
        //readonly -> somente leitura (valor não poderá ser modificado)
        private readonly string _connectionstring;

        //método construtor da classe, faz com que seja obrigatorio passarmos
        //para a classe o valor da connectionstring

        public ProdutoRepository(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public void Inserir(Produto produto)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute("SP_INSERIRPRODUTO",
                    new
                    {
                        @NOME = produto.Nome,
                        @PRECO = produto.Preco,
                        @QUANTIDADE = produto.Quantidade
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Alterar(Produto produto)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute("SP_ALTERARPRODUTO",
                    new
                    {
                        @IDPRODUTO = produto.IdProduto,
                        @NOME = produto.Nome,
                        @PRECO = produto.Preco,
                        @QUANTIDADE = produto.Quantidade
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void Excluir(Produto produto)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute("SP_EXCLUIRPRODUTO",
                    new
                    {
                        @IDPRODUTO = produto.IdProduto
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }


        public List<Produto> Consultar()
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                        .Query<Produto>("SP_CONSULTARPRODUTOS",
                        commandType: CommandType.StoredProcedure)
                        .ToList(); //retornar muitos registros
            }
        }

        public Produto ObterPorId(Guid idProduto)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                        .Query<Produto>("SP_OBTERPRODUTOPORID",
                        new
                        {
                            @IDPRODUTO = idProduto
                        },
                        commandType: CommandType.StoredProcedure)
                        .FirstOrDefault(); //retornar 1 único registro
            }
        }

        public List<Produto> ConsultarPorDatas (DateTime dataMin, DateTime dataMax)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                        .Query<Produto>("SP_CONSULTARPRODUTOSPORDATAS",
                        new
                        {
                            @DataMin = dataMin,
                            @DataMax = dataMax
                        },
                        commandType: CommandType.StoredProcedure)
                        .ToList();
            }
        }

        public List<ProdutoGraficoModel> ConsultarTotal()
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                        .Query<ProdutoGraficoModel>("SP_CONSULTARTOTALPRODUTOS",
                        commandType: CommandType.StoredProcedure)
                        .ToList();
            }
        }

    }

}
