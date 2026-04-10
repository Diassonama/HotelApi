using System;
using System.Collections.Generic;
using System.Linq;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hotel.Infrastruture.Services
{
    public class RelatorioSaldoService : IRelatorioSaldoService
    {
        public RelatorioSaldoService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Gera relatório de movimentações de crédito/débito de uma empresa
        /// </summary>
        public byte[] GerarRelatorioMovimentacoes(
            string nomeEmpresa,
            decimal saldoAtual,
            List<EmpresaSaldoMovimentoDto> movimentacoes,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            return Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Cabeçalho
                        page.Header().Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE MOVIMENTAÇÕES DE SALDO")
                                .FontSize(16)
                                .Bold()
                                .AlignCenter();

                            col.Item().PaddingTop(5).Text(nomeEmpresa)
                                .FontSize(14)
                                .SemiBold()
                                .AlignCenter();

                            if (dataInicio.HasValue && dataFim.HasValue)
                            {
                                col.Item().PaddingTop(3).Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                                    .FontSize(10)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(3).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9)
                                .AlignCenter();

                            col.Item().PaddingTop(10)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Darken2);
                        });

                        // Conteúdo
                        page.Content().PaddingTop(20).Column(col =>
                        {
                            // Saldo Atual
                            col.Item().Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Row(row =>
                                {
                                    row.RelativeItem().Text("SALDO ATUAL")
                                        .FontSize(12)
                                        .Bold();
                                    row.ConstantItem(150).Text(saldoAtual.ToString("C2"))
                                        .FontSize(12)
                                        .Bold()
                                        .AlignRight();
                                });

                            col.Item().PaddingTop(20);

                            // Tabela de Movimentações
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);  // Data
                                    columns.ConstantColumn(60);  // Tipo
                                    columns.RelativeColumn(2);   // Documento
                                    columns.RelativeColumn(3);   // Observação
                                    columns.ConstantColumn(80);  // Valor
                                    columns.ConstantColumn(100); // Usuário
                                });

                                // Cabeçalho da tabela
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Data")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold();

                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Tipo")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold();

                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Documento")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold();

                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Observação")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold();

                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Valor")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold()
                                        .AlignRight();

                                    header.Cell().Background(Colors.Grey.Darken2)
                                        .Padding(5)
                                        .Text("Usuário")
                                        .FontColor(Colors.White)
                                        .FontSize(9)
                                        .Bold();
                                });

                                // Linhas de dados
                                foreach (var mov in movimentacoes.OrderByDescending(m => m.Data))
                                {
                                    var isCredito = mov.TipoLancamento == TipoLancamento.E;
                                    var bgColor = isCredito ? Colors.Green.Lighten4 : Colors.Red.Lighten4;

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(mov.Data.ToString("dd/MM/yyyy"))
                                        .FontSize(9);

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(isCredito ? "Crédito" : "Débito")
                                        .FontSize(9)
                                        .FontColor(isCredito ? Colors.Green.Darken2 : Colors.Red.Darken2)
                                        .Bold();

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(mov.Documento ?? "-")
                                        .FontSize(9);

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(mov.Observacao ?? "-")
                                        .FontSize(9);

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(mov.Valor.ToString("C2"))
                                        .FontSize(9)
                                        .FontColor(isCredito ? Colors.Green.Darken2 : Colors.Red.Darken2)
                                        .Bold()
                                        .AlignRight();

                                    table.Cell().Background(bgColor)
                                        .Padding(5)
                                        .Text(mov.NomeUtilizador ?? "-")
                                        .FontSize(8);
                                }
                            });

                            // Resumo Financeiro
                            col.Item().PaddingTop(20);

                            var totalCreditos = movimentacoes
                                .Where(m => m.TipoLancamento == TipoLancamento.E)
                                .Sum(m => m.Valor);

                            var totalDebitos = movimentacoes
                                .Where(m => m.TipoLancamento == TipoLancamento.S)
                                .Sum(m => m.Valor);

                            col.Item().BorderTop(1)
                                .BorderColor(Colors.Grey.Darken1)
                                .PaddingTop(10)
                                .Column(resumo =>
                                {
                                    resumo.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Total de Créditos:")
                                            .FontSize(11)
                                            .FontColor(Colors.Green.Darken2);
                                        row.ConstantItem(150).Text(totalCreditos.ToString("C2"))
                                            .FontSize(11)
                                            .FontColor(Colors.Green.Darken2)
                                            .Bold()
                                            .AlignRight();
                                    });

                                    resumo.Item().PaddingTop(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("Total de Débitos:")
                                            .FontSize(11)
                                            .FontColor(Colors.Red.Darken2);
                                        row.ConstantItem(150).Text(totalDebitos.ToString("C2"))
                                            .FontSize(11)
                                            .FontColor(Colors.Red.Darken2)
                                            .Bold()
                                            .AlignRight();
                                    });

                                    resumo.Item().PaddingTop(10)
                                        .BorderTop(2)
                                        .BorderColor(Colors.Grey.Darken2)
                                        .PaddingTop(10)
                                        .Row(row =>
                                        {
                                            row.RelativeItem().Text("SALDO FINAL:")
                                                .FontSize(13)
                                                .Bold();
                                            row.ConstantItem(150).Text(saldoAtual.ToString("C2"))
                                                .FontSize(13)
                                                .Bold()
                                                .AlignRight();
                                        });
                                });
                        });

                        // Rodapé
                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                                x.Span(" de ");
                                x.TotalPages();
                            });
                    });
                })
                .GeneratePdf();
        }

        /// <summary>
        /// Gera relatório resumido apenas de créditos
        /// </summary>
        public byte[] GerarRelatorioCreditos(
            string nomeEmpresa,
            List<EmpresaSaldoMovimentoDto> creditos,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            var creditosFiltrados = creditos
                .Where(m => m.TipoLancamento == TipoLancamento.E)
                .ToList();

            var totalCreditos = creditosFiltrados.Sum(m => m.Valor);

            return GerarRelatorioSimples(
                "RELATÓRIO DE CRÉDITOS",
                nomeEmpresa,
                creditosFiltrados,
                totalCreditos,
                dataInicio,
                dataFim,
                Colors.Green.Darken2);
        }

        /// <summary>
        /// Gera relatório resumido apenas de débitos
        /// </summary>
        public byte[] GerarRelatorioDebitos(
            string nomeEmpresa,
            List<EmpresaSaldoMovimentoDto> debitos,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            var debitosFiltrados = debitos
                .Where(m => m.TipoLancamento == TipoLancamento.S)
                .ToList();

            var totalDebitos = debitosFiltrados.Sum(m => m.Valor);

            return GerarRelatorioSimples(
                "RELATÓRIO DE DÉBITOS",
                nomeEmpresa,
                debitosFiltrados,
                totalDebitos,
                dataInicio,
                dataFim,
                Colors.Red.Darken2);
        }

        /// <summary>
        /// Método auxiliar para relatórios simples (só crédito ou só débito)
        /// </summary>
        private byte[] GerarRelatorioSimples(
            string titulo,
            string nomeEmpresa,
            List<EmpresaSaldoMovimentoDto> movimentacoes,
            decimal total,
            DateTime? dataInicio,
            DateTime? dataFim,
            string corDestaque)
        {
            return Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(col =>
                        {
                            col.Item().Text(titulo)
                                .FontSize(16)
                                .Bold()
                                .AlignCenter();

                            col.Item().PaddingTop(5).Text(nomeEmpresa)
                                .FontSize(14)
                                .SemiBold()
                                .AlignCenter();

                            if (dataInicio.HasValue && dataFim.HasValue)
                            {
                                col.Item().PaddingTop(3).Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                                    .FontSize(10)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(3).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9)
                                .AlignCenter();

                            col.Item().PaddingTop(10)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Darken2);
                        });

                        page.Content().PaddingTop(20).Column(col =>
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.ConstantColumn(100);
                                    columns.ConstantColumn(100);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(5)
                                        .Text("Data").FontColor(Colors.White).FontSize(9).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(5)
                                        .Text("Documento").FontColor(Colors.White).FontSize(9).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(5)
                                        .Text("Observação").FontColor(Colors.White).FontSize(9).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(5)
                                        .Text("Valor").FontColor(Colors.White).FontSize(9).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(5)
                                        .Text("Usuário").FontColor(Colors.White).FontSize(9).Bold();
                                });

                                foreach (var mov in movimentacoes.OrderByDescending(m => m.Data))
                                {
                                    table.Cell().Padding(5).Text(mov.Data.ToString("dd/MM/yyyy")).FontSize(9);
                                    table.Cell().Padding(5).Text(mov.Documento ?? "-").FontSize(9);
                                    table.Cell().Padding(5).Text(mov.Observacao ?? "-").FontSize(9);
                                    table.Cell().Padding(5).Text(mov.Valor.ToString("C2"))
                                        .FontSize(9).FontColor(corDestaque).Bold().AlignRight();
                                    table.Cell().Padding(5).Text(mov.NomeUtilizador ?? "-").FontSize(8);
                                }
                            });

                            col.Item().PaddingTop(20)
                                .BorderTop(2)
                                .BorderColor(Colors.Grey.Darken2)
                                .PaddingTop(10)
                                .Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAL:")
                                        .FontSize(13)
                                        .Bold();
                                    row.ConstantItem(150).Text(total.ToString("C2"))
                                        .FontSize(13)
                                        .FontColor(corDestaque)
                                        .Bold()
                                        .AlignRight();
                                });
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Página ");
                                x.CurrentPageNumber();
                                x.Span(" de ");
                                x.TotalPages();
                            });
                    });
                })
                .GeneratePdf();
        }

        public byte[] GerarRelatorioAdiantamentosHistorico(
            string nomeEmpresaFiltro,
            List<EmpresaSaldoDto> saldos,
            List<EmpresaSaldoMovimentoDto> movimentacoes,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            saldos ??= new List<EmpresaSaldoDto>();
            movimentacoes ??= new List<EmpresaSaldoMovimentoDto>();

            var totalAdiantamentos = saldos.Sum(s => s.Saldo);
            var totalCreditos = movimentacoes.Where(m => m.TipoLancamento == TipoLancamento.E).Sum(m => m.Valor);
            var totalDebitos = movimentacoes.Where(m => m.TipoLancamento == TipoLancamento.S).Sum(m => m.Valor);

            return Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(25);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE ADIANTAMENTOS E HISTÓRICO")
                                .FontSize(15)
                                .Bold()
                                .AlignCenter();

                            col.Item().PaddingTop(3).Text($"Empresa: {nomeEmpresaFiltro}")
                                .FontSize(11)
                                .AlignCenter();

                            if (dataInicio.HasValue || dataFim.HasValue)
                            {
                                var inicioTxt = dataInicio?.ToString("dd/MM/yyyy") ?? "Início";
                                var fimTxt = dataFim?.ToString("dd/MM/yyyy") ?? "Hoje";
                                col.Item().PaddingTop(2).Text($"Período: {inicioTxt} a {fimTxt}")
                                    .FontSize(9)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(2).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8)
                                .AlignCenter();

                            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Darken2);
                        });

                        page.Content().PaddingTop(10).Column(col =>
                        {
                            col.Item().Text("Resumo de adiantamentos").Bold().FontSize(10);
                            col.Item().PaddingTop(4).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);
                                    columns.ConstantColumn(130);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Empresa").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Saldo Adiantamento").FontColor(Colors.White).Bold().AlignRight();
                                });

                                foreach (var saldo in saldos.OrderBy(s => s.NomeEmpresa))
                                {
                                    table.Cell().Padding(4).Text(saldo.NomeEmpresa ?? "-");
                                    table.Cell().Padding(4).Text(saldo.Saldo.ToString("C2")).AlignRight().Bold();
                                }

                                if (!saldos.Any())
                                {
                                    table.Cell().ColumnSpan(2).Padding(4).Text("Sem dados de adiantamento para o filtro informado.").Italic();
                                }
                            });

                            col.Item().PaddingTop(8).AlignRight()
                                .Text($"Total de adiantamentos: {totalAdiantamentos:C2}")
                                .Bold().FontSize(10);

                            col.Item().PaddingTop(14).Text("Histórico de movimentos").Bold().FontSize(10);
                            col.Item().PaddingTop(4).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(75);   // Data
                                    columns.RelativeColumn(3);    // Empresa
                                    columns.ConstantColumn(60);   // Tipo
                                    columns.ConstantColumn(110);  // Documento
                                    columns.RelativeColumn(4);    // Observação
                                    columns.ConstantColumn(95);   // Valor
                                    columns.ConstantColumn(120);  // Utilizador
                                });

                                table.Header(header =>
                                {
                                    void Head(string text, bool alignRight = false)
                                    {
                                        var cell = header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                            .Text(text).FontColor(Colors.White).FontSize(8).Bold();
                                        if (alignRight) cell.AlignRight();
                                    }

                                    Head("Data");
                                    Head("Empresa");
                                    Head("Tipo");
                                    Head("Documento");
                                    Head("Observação");
                                    Head("Valor", true);
                                    Head("Utilizador");
                                });

                                foreach (var mov in movimentacoes.OrderByDescending(m => m.Data))
                                {
                                    var credito = mov.TipoLancamento == TipoLancamento.E;
                                    var cor = credito ? Colors.Green.Darken2 : Colors.Red.Darken2;

                                    table.Cell().Padding(4).Text(mov.Data.ToString("dd/MM/yyyy"));
                                    table.Cell().Padding(4).Text(mov.NomeEmpresa ?? "-");
                                    table.Cell().Padding(4).Text(credito ? "Crédito" : "Débito").FontColor(cor).Bold();
                                    table.Cell().Padding(4).Text(mov.Documento ?? "-");
                                    table.Cell().Padding(4).Text(mov.Observacao ?? "-");
                                    table.Cell().Padding(4).Text(mov.Valor.ToString("C2")).FontColor(cor).Bold().AlignRight();
                                    table.Cell().Padding(4).Text(mov.NomeUtilizador ?? "-");
                                }

                                if (!movimentacoes.Any())
                                {
                                    table.Cell().ColumnSpan(7).Padding(4).Text("Sem histórico de movimentos para o filtro informado.").Italic();
                                }
                            });

                            col.Item().PaddingTop(12).BorderTop(1).BorderColor(Colors.Grey.Darken1).PaddingTop(8)
                                .Row(row =>
                                {
                                    row.RelativeItem().Text($"Total créditos: {totalCreditos:C2}").FontColor(Colors.Green.Darken2).Bold();
                                    row.RelativeItem().AlignCenter().Text($"Total débitos: {totalDebitos:C2}").FontColor(Colors.Red.Darken2).Bold();
                                    row.RelativeItem().AlignRight().Text($"Saldo consolidado: {totalAdiantamentos:C2}").Bold();
                                });
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                })
                .GeneratePdf();
        }
    }
}