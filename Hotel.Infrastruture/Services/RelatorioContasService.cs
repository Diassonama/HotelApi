using System;
using System.Collections.Generic;
using System.Linq;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hotel.Infrastruture.Services
{
    public class RelatorioContasService : IRelatorioContasService
    {
        public RelatorioContasService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GerarRelatorioContasReceber(
            List<ContaReceberDto> contas,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string filtroEmpresa = null)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    // Cabeçalho
                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE CONTAS A RECEBER")
                                .FontSize(18)
                                .Bold()
                                .AlignCenter();

                            if (!string.IsNullOrEmpty(filtroEmpresa))
                            {
                                col.Item().PaddingTop(3).Text($"Empresa: {filtroEmpresa}")
                                    .FontSize(12)
                                    .SemiBold()
                                    .AlignCenter();
                            }

                            if (dataInicio.HasValue && dataFim.HasValue)
                            {
                                col.Item().PaddingTop(3).Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                                    .FontSize(10)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(3).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8)
                                .AlignCenter();

                            col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Darken2);
                        });
                    });

                    // Conteúdo
                    page.Content().Element(content =>
                    {
                        content.PaddingTop(10).Column(col =>
                        {
                            // Resumo
                            var totalGeral = contas.Sum(c => c.ValorTotal);
                            var totalPago = contas.Sum(c => c.ValorPago);
                            var totalSaldo = contas.Sum(c => c.Saldo);
                            var totalVencidas = contas.Where(c => c.Vencida).Sum(c => c.Saldo);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text($"Total de Contas: {contas.Count}").FontSize(10).Bold();
                                    resumo.Item().Text($"Valor Total: {totalGeral:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Green.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("Recebido").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalPago:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("A Receber").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalSaldo:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Red.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("Vencidas").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalVencidas:C2}").FontSize(10);
                                });
                            });

                            col.Item().PaddingTop(15);

                            // Tabela
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);   // ID
                                    columns.ConstantColumn(70);   // Data Emissão
                                    columns.ConstantColumn(70);   // Vencimento
                                    columns.ConstantColumn(80);   // Documento
                                    columns.RelativeColumn(3);    // Empresa
                                    columns.ConstantColumn(70);   // Valor
                                    columns.ConstantColumn(70);   // Pago
                                    columns.ConstantColumn(70);   // Saldo
                                    columns.ConstantColumn(50);   // Estado
                                    columns.RelativeColumn(2);    // Observação
                                });

                                // Cabeçalho
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("ID").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Emissão").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Vencimento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Documento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Empresa").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Valor").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Pago").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Saldo").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Estado").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Observação").FontColor(Colors.White).FontSize(8).Bold();
                                });

                                // Dados
                                foreach (var conta in contas.OrderBy(c => c.DataVencimento))
                                {
                                    var bgColor = conta.Vencida ? Colors.Red.Lighten5 :
                                                  conta.Estado == EstadoConta.Paga ? Colors.Green.Lighten5 :
                                                  Colors.White;

                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Id.ToString()).FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.DataEmissao.ToString("dd/MM/yyyy")).FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.DataVencimento?.ToString("dd/MM/yyyy") ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Documento ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.NomeEmpresa ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.ValorTotal.ToString("C2")).FontSize(8).AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.ValorPago.ToString("C2")).FontSize(8).AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Saldo.ToString("C2"))
                                        .FontSize(8).FontColor(conta.Saldo > 0 ? Colors.Red.Darken2 : Colors.Black).Bold().AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.EstadoDescricao).FontSize(7);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Observacao ?? "-").FontSize(7);
                                }
                            });

                            // Totais
                            col.Item().PaddingTop(10).BorderTop(2).BorderColor(Colors.Grey.Darken2)
                                .PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAIS:").FontSize(11).Bold();
                                    row.ConstantItem(70).Text(totalGeral.ToString("C2")).FontSize(11).Bold().AlignRight();
                                    row.ConstantItem(70).Text(totalPago.ToString("C2")).FontSize(11).Bold().AlignRight();
                                    row.ConstantItem(70).Text(totalSaldo.ToString("C2")).FontSize(11).Bold()
                                        .FontColor(Colors.Red.Darken2).AlignRight();
                                });
                        });
                    });

                    // Rodapé
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GerarRelatorioContasPagar(
            List<ContaPagarDto> contas,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string filtroFornecedor = null)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    // Cabeçalho
                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE CONTAS A PAGAR")
                                .FontSize(18)
                                .Bold()
                                .AlignCenter();

                            if (!string.IsNullOrEmpty(filtroFornecedor))
                            {
                                col.Item().PaddingTop(3).Text($"Fornecedor: {filtroFornecedor}")
                                    .FontSize(12)
                                    .SemiBold()
                                    .AlignCenter();
                            }

                            if (dataInicio.HasValue && dataFim.HasValue)
                            {
                                col.Item().PaddingTop(3).Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                                    .FontSize(10)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(3).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8)
                                .AlignCenter();

                            col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Darken2);
                        });
                    });

                    // Conteúdo
                    page.Content().Element(content =>
                    {
                        content.PaddingTop(10).Column(col =>
                        {
                            // Resumo
                            var totalGeral = contas.Sum(c => c.ValorTotal);
                            var totalPago = contas.Sum(c => c.ValorPago);
                            var totalSaldo = contas.Sum(c => c.Saldo);
                            var totalVencidas = contas.Where(c => c.Vencida).Sum(c => c.Saldo);

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text($"Total de Contas: {contas.Count}").FontSize(10).Bold();
                                    resumo.Item().Text($"Valor Total: {totalGeral:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Green.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("Pago").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalPago:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Orange.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("A Pagar").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalSaldo:C2}").FontSize(10);
                                });

                                row.RelativeItem().Background(Colors.Red.Lighten4).Padding(8).Column(resumo =>
                                {
                                    resumo.Item().Text("Vencidas").FontSize(10).Bold();
                                    resumo.Item().Text($"{totalVencidas:C2}").FontSize(10);
                                });
                            });

                            col.Item().PaddingTop(15);

                            // Tabela
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn(3);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(70);
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("ID").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Emissão").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Vencimento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Documento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Fornecedor").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Valor").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Pago").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Saldo").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Estado").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Grey.Darken2).Padding(4)
                                        .Text("Observação").FontColor(Colors.White).FontSize(8).Bold();
                                });

                                foreach (var conta in contas.OrderBy(c => c.DataVencimento))
                                {
                                    var bgColor = conta.Vencida ? Colors.Red.Lighten5 :
                                                  conta.Estado == EstadoConta.Paga ? Colors.Green.Lighten5 :
                                                  Colors.White;

                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Id.ToString()).FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.DataEmissao.ToString("dd/MM/yyyy")).FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.DataVencimento?.ToString("dd/MM/yyyy") ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Documento ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.FornecedorNome ?? conta.NomeEmpresa ?? "-").FontSize(8);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.ValorTotal.ToString("C2")).FontSize(8).AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.ValorPago.ToString("C2")).FontSize(8).AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Saldo.ToString("C2"))
                                        .FontSize(8).FontColor(conta.Saldo > 0 ? Colors.Red.Darken2 : Colors.Black).Bold().AlignRight();
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.EstadoDescricao).FontSize(7);
                                    table.Cell().Background(bgColor).Padding(4).Text(conta.Observacao ?? "-").FontSize(7);
                                }
                            });

                            col.Item().PaddingTop(10).BorderTop(2).BorderColor(Colors.Grey.Darken2)
                                .PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAIS:").FontSize(11).Bold();
                                    row.ConstantItem(70).Text(totalGeral.ToString("C2")).FontSize(11).Bold().AlignRight();
                                    row.ConstantItem(70).Text(totalPago.ToString("C2")).FontSize(11).Bold().AlignRight();
                                    row.ConstantItem(70).Text(totalSaldo.ToString("C2")).FontSize(11).Bold()
                                        .FontColor(Colors.Red.Darken2).AlignRight();
                                });
                        });
                    });

                    // Rodapé
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GerarRelatorioFluxoCaixa(
            List<ContaReceberDto> contasReceber,
            List<ContaPagarDto> contasPagar,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE FLUXO DE CAIXA")
                                .FontSize(18)
                                .Bold()
                                .AlignCenter();

                            if (dataInicio.HasValue && dataFim.HasValue)
                            {
                                col.Item().PaddingTop(3).Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                                    .FontSize(10)
                                    .AlignCenter();
                            }

                            col.Item().PaddingTop(3).Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(8)
                                .AlignCenter();

                            col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Darken2);
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.PaddingTop(10).Column(col =>
                        {
                            var totalReceber = contasReceber.Where(c => c.Estado != EstadoConta.Paga).Sum(c => c.Saldo);
                            var totalPagar = contasPagar.Where(c => c.Estado != EstadoConta.Paga).Sum(c => c.Saldo);
                            var saldoProjetado = totalReceber - totalPagar;

                            // Resumo Geral
                            col.Item().Background(Colors.Blue.Lighten5).Padding(15).Column(resumo =>
                            {
                                resumo.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Total a Receber:").FontSize(12).Bold();
                                    row.ConstantItem(150).Text(totalReceber.ToString("C2"))
                                        .FontSize(12).FontColor(Colors.Green.Darken2).Bold().AlignRight();
                                });

                                resumo.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Total a Pagar:").FontSize(12).Bold();
                                    row.ConstantItem(150).Text(totalPagar.ToString("C2"))
                                        .FontSize(12).FontColor(Colors.Red.Darken2).Bold().AlignRight();
                                });

                                resumo.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Grey.Darken2)
                                    .PaddingTop(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("SALDO PROJETADO:").FontSize(14).Bold();
                                        row.ConstantItem(150).Text(saldoProjetado.ToString("C2"))
                                            .FontSize(14)
                                            .FontColor(saldoProjetado >= 0 ? Colors.Green.Darken3 : Colors.Red.Darken3)
                                            .Bold().AlignRight();
                                    });
                            });

                            col.Item().PaddingTop(20).Text("CONTAS A RECEBER (Pendentes)")
                                .FontSize(12).Bold();

                            col.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                    columns.ConstantColumn(80);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                        .Text("Vencimento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                        .Text("Empresa").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                        .Text("Documento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                        .Text("Saldo").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                });

                                foreach (var conta in contasReceber.Where(c => c.Estado != EstadoConta.Paga).OrderBy(c => c.DataVencimento))
                                {
                                    table.Cell().Padding(4).Text(conta.DataVencimento?.ToString("dd/MM/yyyy") ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.NomeEmpresa ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.Documento ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.Saldo.ToString("C2"))
                                        .FontSize(8).FontColor(Colors.Green.Darken2).Bold().AlignRight();
                                }
                            });

                            col.Item().PaddingTop(20).Text("CONTAS A PAGAR (Pendentes)")
                                .FontSize(12).Bold();

                            col.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(100);
                                    columns.ConstantColumn(80);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Red.Darken2).Padding(4)
                                        .Text("Vencimento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Red.Darken2).Padding(4)
                                        .Text("Fornecedor").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Red.Darken2).Padding(4)
                                        .Text("Documento").FontColor(Colors.White).FontSize(8).Bold();
                                    header.Cell().Background(Colors.Red.Darken2).Padding(4)
                                        .Text("Saldo").FontColor(Colors.White).FontSize(8).Bold().AlignRight();
                                });

                                foreach (var conta in contasPagar.Where(c => c.Estado != EstadoConta.Paga).OrderBy(c => c.DataVencimento))
                                {
                                    table.Cell().Padding(4).Text(conta.DataVencimento?.ToString("dd/MM/yyyy") ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.FornecedorNome ?? conta.NomeEmpresa ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.Documento ?? "-").FontSize(8);
                                    table.Cell().Padding(4).Text(conta.Saldo.ToString("C2"))
                                        .FontSize(8).FontColor(Colors.Red.Darken2).Bold().AlignRight();
                                }
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(8);
                        x.CurrentPageNumber().FontSize(8);
                        x.Span(" de ").FontSize(8);
                        x.TotalPages().FontSize(8);
                    });
                });
            }).GeneratePdf();
        }
    }
}