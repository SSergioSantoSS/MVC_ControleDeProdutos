using ProjetoMVC01.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMVC01.Models
{
    public class ProdutoConsultaModel
    {

        /*
         * A página de consulta de produtos do sistema irá exibir
         * uma lista contendo todos os produtos obtidos do banco de dados
         */

        public List<Produto> Produtos { get; set; }



    }
}
