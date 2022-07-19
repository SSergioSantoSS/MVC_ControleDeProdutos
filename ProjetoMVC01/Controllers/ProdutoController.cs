using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoMVC01.Entities;
using ProjetoMVC01.Models;
using ProjetoMVC01.Reports;
using ProjetoMVC01.Repositories;
using System;
using static ProjetoMVC01.Models.ProdutoConsultaModel;

namespace ProjetoMVC01.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost] //faz com que o método receba o evento SUBMIT da página
        public IActionResult Cadastro(ProdutoCadastroModel model,
            [FromServices] ProdutoRepository produtoRepository)
        {
            //verificando se todos os campos da model
            //passaram nas regras de validação..
            if (ModelState.IsValid)
            {
                try
                {
                    //cadastrar no banco de dados..
                    Produto produto = new Produto();
                    produto.Nome = model.Nome;
                    produto.Preco = Convert.ToDecimal(model.Preco);
                    produto.Quantidade = Convert.ToInt32(model.Quantidade);

                    //inserir o produto no banco de dados..
                    produtoRepository.Inserir(produto);

                    TempData["Mensagem"] = $"Produto {produto.Nome}, cadastrado com sucesso.";
                    ModelState.Clear(); //limpar os campos do formulário


                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = "Erro ao cadastrar o produto: " + e.Message;
                }
            }

            return View();
        }

        //método que abre a página de consulta
        public IActionResult Consulta([FromServices] ProdutoRepository produtoRepository)
        {
            //classe de modelo de dados criada para a página de consulta
            var model = new ProdutoConsultaModel();

            try
            {
                //executar a consulta no banco de dados 
                //e armazenar o resultado
                //no atributo 'Produtos' da classe ProdutoConsultaModel
                model.Produtos = produtoRepository.Consultar();
            }
            catch (Exception e)
            {
                //exibir mensagem de erro na página..
                TempData["Mensagem"] = "Erro ao consultar o produto: " + e.Message;
            }

            //enviando o objeto 'model' para a página..
            return View(model);
        }

        public IActionResult Exclusao(Guid id,
            [FromServices] ProdutoRepository produtoRepository)
        {
            try
            {
                //buscar no banco de dados o produto atraves do id..
                var produto = produtoRepository.ObterPorId(id);
                //excluindo o produto..
                produtoRepository.Excluir(produto);

                TempData["Mensagem"] = "Produto excluído com sucesso.";
            }
            catch (Exception e)
            {
                //exibir mensagem de erro na página..
                TempData["Mensagem"] = "Erro ao excluir o produto: " + e.Message;
            }

            //redirecionamento do usuário de volta para a página de consulta..
            return RedirectToAction("Consulta");
        }

        public IActionResult Edicao(Guid id, [FromServices] ProdutoRepository produtoRepository)
        {
            //classe de modelo de dados..
            var model = new ProdutoEdicaoModel();
            try
            {
                //buscar o produto no banco de dados atraves do id..
                var produto = produtoRepository.ObterPorId(id);

                //transferir os dados do produto para a classe model..
                model.IdProduto = produto.IdProduto;
                model.Nome = produto.Nome;
                model.Preco = produto.Preco;
                model.Quantidade = produto.Quantidade;
            }
            catch (Exception e)
            {
                //exibir mensagem de erro na página..
                TempData["Mensagem"] = "Erro ao exibir o produto: " + e.Message;
            }
            //enviando o objeto model para a página..
            return View(model);
        }

        [HttpPost] //recebe o evento SUBMIT do formulário
        public IActionResult Edicao(ProdutoEdicaoModel model, [FromServices] ProdutoRepository produtoRepository)
        {
            //verifica se todos os campos da model passaram nas regras
            //de validação do formulário (se foram validados com sucesso)
            if (ModelState.IsValid)
            {
                try
                {
                    //buscar o produto no banco de dados atraves do ID..
                    var produto = produtoRepository.ObterPorId(model.IdProduto);

                    //alterando os dados do produto..
                    produto.Nome = model.Nome;

                    produto.Preco = Convert.ToDecimal(model.Preco);
                    produto.Quantidade = Convert.ToInt32(model.Quantidade);

                    //atualizando no banco de dados..
                    produtoRepository.Alterar(produto);
                    TempData["Mensagem"] = "Produto atualizado com sucesso.";

                    //redirecionamento de volta para a página de consulta..
                    return RedirectToAction("Consulta");
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = "Erro ao atualizar o produto: " + e.Message;
                }
            }
            return View();
        }

        public IActionResult Relatorio()
        {
            return View();
        }

        [HttpPost] //recebe os dados enviados pelo formulário
        public IActionResult Relatorio(ProdutoRelatorioModel model,[FromServices] ProdutoRepository produtoRepository)
        {
            //verifica se todos os campos da model foram validados com sucesso!
            if (ModelState.IsValid)
            {

                try
                {
                    //capturando as datas informadas na página (formulario)
                    var filtroDataMin = Convert.ToDateTime(model.DataMin);
                    var filtroDataMax = Convert.ToDateTime(model.DataMax);

                    //executando a consulta de produtos no banco de dados..
                    var produtos = produtoRepository.ConsultarPorDatas(filtroDataMin, filtroDataMax);                    

                    //verificando se o tipo de relatorio selecionado é PDF..
                    if (model.TipoRelatorio.Equals("pdf"))
                    {
                        var produtoReport = new ProdutoReportPDF();
                        var pdf = produtoReport.GerarPdf(filtroDataMin, filtroDataMax, produtos);

                        //fazer o download do arquivo..
                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.Headers.Add("content-disposition","attachment; filename=produtos.pdf");
                        Response.Body.WriteAsync(pdf, 0, pdf.Length);
                        Response.Body.Flush();
                        Response.StatusCode = StatusCodes.Status200OK;
                    }

                    //verificando se o tipo de relatorio selecionado é EXCEL..
                    else if (model.TipoRelatorio.Equals("excel"))
                    {
                        var produtoReport = new ProdutoReportExcel();
                        var excel = produtoReport.GerarExcel(filtroDataMin, filtroDataMax, produtos);

                        //fazer o download do arquivo..
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.Headers.Add("content-disposition","attachment; filename=produtos.xlsx");
                        Response.Body.WriteAsync(excel, 0, excel.Length);
                        Response.Body.Flush();
                        Response.StatusCode = StatusCodes.Status200OK;
                    }

                }
                catch (Exception e)

                {
                    TempData["Mensagem"] = "Erro ao gerar relatório: " + e.Message;
                }
            }

            return View();
        }

        //método que será chamado (executado) por um código JavaScript
        //localizado em alguma página no sistema..
        public JsonResult ObterDadosGrafico([FromServices] ProdutoRepository produtoRepository)
        {
            try
            {
                //retornar para o javascript, o conteudo 
                //da consulta feita no banco de dados..
                return Json(produtoRepository.ConsultarTotal());
            }
            catch (Exception e)
            {
                //retornando mensagem de erro..
                return Json(e.Message);
            }
        }



    }

}


