using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProjetoMVC01.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMVC01.Reports
{

    public class ProdutoReportPDF
    {
        //método para gerar um relatorio em formato PDF
        //byte[] -> indica que o método retorna um arquivo..
        public byte[] GerarPdf(DateTime dataMin, DateTime dataMax, List<Produto> produtos)
        {
            //abrindo o documento PDF..
            var memoryStream = new MemoryStream();
            var pdf = new PdfDocument(new PdfWriter(memoryStream));

            //escrevendo o conteudo do PDF..
            using (var document = new Document(pdf))
            {
                document.Add(ObterLogotipo);

                document.Add(new Paragraph("Relatório de Produtos")
                    .AddStyle(FormatacaoTitulo).SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Produtos cadastrados entre: {dataMin.ToString("dd/MM/yyyy")} e { dataMax.ToString("dd/MM/yyyy")}")
                    .AddStyle(FormatacaoSubTitulo)
                    .SetTextAlignment(TextAlignment.CENTER));

                //escrever uma tabela contendo os produtos..
                var table = new Table(5); //5 -> numero de colunas da tabela
                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddHeaderCell("Nome do Produto");
                table.AddHeaderCell("Preço");
                table.AddHeaderCell("Quantidade");
                table.AddHeaderCell("Total");
                table.AddHeaderCell("Data de Cadastro");

                //exibir os produtos dentro da tabela..
                foreach (var item in produtos)
                {
                    table.AddCell(item.Nome);
                    table.AddCell(item.Preco.ToString("c")); //c -> currency (R$)
                    table.AddCell(item.Quantidade.ToString());
                    table.AddCell((item.Preco * item.Quantidade).ToString("c"));//c -> currency (R$)
                    table.AddCell(item.DataCadastro.ToString("dd/MM/yyyy"));
                }

                document.Add(table); //adicionando a tabela no documento PDF
            }

            //retornar o arquivo PDF:
            return memoryStream.ToArray();
        }

        //método para gerar o estilo do titulo do relatorio..
        private Style FormatacaoTitulo
        {
            get
            {

                var style = new Style();
                style.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                style.SetFontSize(26);
                style.SetFontColor(Color.ConvertRgbToCmyk(new DeviceRgb(0, 102, 284)));

                return style;
            }
        }

        //método para gerar o estilo do subtitulo do relatorio..
        private Style FormatacaoSubTitulo
        {
            get
            {
                var style = new Style();
                style.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                style.SetFontSize(13);
                style.SetFontColor(Color.ConvertRgbToCmyk(new DeviceRgb(0, 0, 0)));

                return style;
            }
        }

        private Image ObterLogotipo
        {
            get
            {
                ImageData imageData = ImageDataFactory.Create(new Uri("https://banner2.cleanpng.com/20191116/oyy/transparent-statistics-icon-growth-icon-economy-icon-5dd01fa9cbf0f0.6541296815739206818353.jpg"));
                var logotipo = new Image(imageData);
                logotipo.SetWidth(200);
                logotipo.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                return logotipo;
            }
        }
    }
}
