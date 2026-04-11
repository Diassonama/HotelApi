using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Dtos;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace Hotel.Infrastruture.Services
{
    public class ReciboService : IReciboService
    {
           
        public ReciboService()
        {
            // ✅ CONFIGURAR LICENSE DO QUESTPDF (Usar Community License se disponível)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GerarReciboCheckout(ReciboCheckoutDto recibo)
        {
            var enUS = new System.Globalization.CultureInfo("en-US");

            DateTime angolaNow;
            try
            {
                angolaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Africa/Luanda"));
            }
            catch
            {
                try
                {
                    angolaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));
                }
                catch
                {
                    angolaNow = DateTime.UtcNow.AddHours(1);
                }
            }

            string Moeda(float v) => v.ToString("N2", enUS);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(90, 250, Unit.Millimetre));
                    page.Margin(8, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Content().Column(column =>
                    {
                        // ─── CABEÇALHO ───────────────────────────────
                        column.Item().Text(recibo.NomeHotel).Bold().FontSize(11).AlignCenter();
                        column.Item().Text(recibo.Endereco).FontSize(8).AlignCenter();
                        column.Item().Text(recibo.Cidade).FontSize(8).AlignCenter();
                        column.Item().Text(recibo.NumContribuinte).FontSize(8).AlignCenter();

                        column.Item().PaddingVertical(4).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── NÚMERO DO CHECKIN ────────────────────────
                        column.Item().PaddingVertical(3)
                            .Text($"Checkin nº {recibo.CheckinNumero.ToString("N0", enUS)}")
                            .Bold().FontSize(13).AlignCenter();

                        column.Item().Height(5);

                        // ─── HÓSPEDE ──────────────────────────────────
                        column.Item().PaddingBottom(3)
                            .Text($"Hóspedes: {recibo.NomeHospede}").Bold().FontSize(9).AlignCenter();

                        // ─── APARTAMENTO ──────────────────────────────
                        column.Item().Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);
                        column.Item().PaddingVertical(3)
                            .Text($"{recibo.ApartamentoCodigo} - {recibo.TipoApartamento}")
                            .Bold().FontSize(10).AlignCenter();
                        column.Item().Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── PERÍODO ──────────────────────────────────
                        column.Item().PaddingVertical(3)
                            .Text("Período").Bold().FontSize(9).AlignCenter();
                        column.Item()
                            .Text($"{recibo.DataEntrada:dd/MM/yyyy} a {recibo.DataSaida:dd/MM/yyyy} ({recibo.NumDias} diária{(recibo.NumDias != 1 ? "s" : "")})")
                            .FontSize(8).AlignCenter();
                        column.Item().PaddingBottom(4)
                            .Text($"Impresso: {angolaNow:d/M/yyyy h:mm:sstt}").FontSize(8).AlignCenter();

                        // ─── LINHAS DE VALORES ────────────────────────
                        column.Item().PaddingTop(3).Row(row =>
                        {
                            row.RelativeItem().Text("Valor Diária:").FontSize(9);
                            row.ConstantItem(65).Text(Moeda(recibo.ValorDiaria)).FontSize(9).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Estadia:").FontSize(9);
                            row.ConstantItem(65).Text($"{recibo.NumDias}  dia{(recibo.NumDias != 1 ? "s" : "")}").FontSize(9).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Diária").FontSize(9);
                            row.ConstantItem(65).Text(Moeda(recibo.ValorDiarias)).FontSize(9).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Consumo").FontSize(9);
                            row.ConstantItem(65).Text(Moeda(recibo.Consumo)).FontSize(9).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text(t =>
                            {
                                t.Span("Desconto ").FontSize(9);
                                t.Span(new string('.', 20)).FontSize(6);
                            });
                            row.ConstantItem(65).Text(Moeda(recibo.Desconto)).FontSize(9).AlignRight();
                        });

                        column.Item().PaddingVertical(3).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── TOTALIZADORES ────────────────────────────
                        column.Item().PaddingVertical(2).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL").Bold().FontSize(11);
                            row.ConstantItem(65).Text(Moeda(recibo.Total)).Bold().FontSize(11).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("PAGOU(-)").Bold().FontSize(9);
                            row.ConstantItem(65).Text(Moeda(recibo.Pago)).FontSize(9).AlignRight();
                        });
                        column.Item().PaddingBottom(3).Row(row =>
                        {
                            row.RelativeItem().Text("Forma Pagamento").FontSize(9);
                            row.ConstantItem(65).Text(recibo.FormaPagamento ?? "").FontSize(9).AlignRight();
                        });

                        column.Item().PaddingVertical(2).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── A PAGAR ──────────────────────────────────
                        column.Item().PaddingVertical(3).Row(row =>
                        {
                            row.RelativeItem().Text("A PAGAR (=)").Bold().FontSize(10);
                            row.ConstantItem(65).Text(Moeda(recibo.APagar)).Bold().FontSize(10).AlignRight();
                        });

                        column.Item().PaddingVertical(3).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── RODAPÉ ───────────────────────────────────
                        column.Item().PaddingTop(10)
                            .Text($"Operador: {recibo.Operador}").FontSize(8).AlignCenter();
                        column.Item().PaddingTop(10)
                            .Text("SELO PAGO POR GUIA").FontSize(7).AlignCenter();
                        column.Item()
                            .Text("DECRETO Nº 18/92 D.R. I - SERIE Nº19 DE 15 DE MAIO DE 1992").FontSize(7).AlignCenter();
                        column.Item().PaddingTop(3)
                            .Text("Processador por computador").FontSize(7).AlignCenter();
                    });
                });
            }).GeneratePdf();
        }

        public void SalvarReciboCheckout(ReciboCheckoutDto recibo, string caminhoSaida)
        {
            var pdf = GerarReciboCheckout(recibo);
            File.WriteAllBytes(caminhoSaida, pdf);
        }

        public byte[] GerarNotaHospedagem(NotaHospedagemDto nota)
        {
            var enUS = new System.Globalization.CultureInfo("en-US");

            string Moeda(float valor) => valor.ToString("N2", enUS);
            string NomeArquivoValido(string texto) => string.IsNullOrWhiteSpace(texto) ? "-" : texto;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(18);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Content().Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(65).Height(60).Element(container =>
                            {
                                if (!string.IsNullOrWhiteSpace(nota.LogoCaminho) && File.Exists(nota.LogoCaminho))
                                    container.Image(File.ReadAllBytes(nota.LogoCaminho)).FitArea();
                                else
                                    container.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle()
                                        .Text("HOTEL").Bold().FontSize(12);
                            });

                            row.RelativeItem().PaddingLeft(10).Column(left =>
                            {
                                left.Item().Text(NomeArquivoValido(nota.NomeHotel)).Bold().FontSize(11);
                                left.Item().Text(NomeArquivoValido(nota.Endereco)).Bold().FontSize(9);
                                left.Item().Text(NomeArquivoValido(nota.Cidade)).Bold().FontSize(9);
                                left.Item().Text(NomeArquivoValido(nota.NumContribuinte)).Bold().FontSize(9);
                            });
                        });

                        column.Item().AlignRight().Text($"Impresso: {nota.DataImpressao:d/M/yyyy h:mm:sstt}")
                            .FontSize(8);

                        column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Black);

                        column.Item().PaddingTop(14).Column(headerInfo =>
                        {
                            headerInfo.Item().Row(row =>
                            {
                                row.RelativeItem().Row(r =>
                                {
                                    r.ConstantItem(72).Text("Hóspede:").Bold();
                                    r.RelativeItem().Text(NomeArquivoValido(nota.NomeHospede));
                                });

                                row.ConstantItem(180).Row(r =>
                                {
                                    r.RelativeItem().Text("RECIBO:").Bold().AlignRight();
                                    r.ConstantItem(60).Text(nota.NumeroDocumento.ToString("N0", enUS)).Bold().AlignRight();
                                });
                            });

                            headerInfo.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Row(r =>
                                {
                                    r.ConstantItem(72).Text("Empresa:").Bold();
                                    r.RelativeItem().Text(NomeArquivoValido(nota.Empresa));
                                });

                                row.ConstantItem(180).Text(string.Empty);
                            });
                        });

                        column.Item().PaddingTop(18).AlignRight().Text($"Utilizador Checkin: {NomeArquivoValido(nota.UtilizadorCheckin)}").Bold().FontSize(8.5f);
                        column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Black);

                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(55);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(82);
                                columns.ConstantColumn(82);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Quarto").Bold();
                                header.Cell().Element(HeaderCell).Text("Tipo Quarto").Bold();
                                header.Cell().Element(HeaderCell).Text("Período").Bold();
                                header.Cell().Element(HeaderCell).Text("Valor Diaria").Bold().AlignRight();
                                header.Cell().Element(HeaderCell).Text("Total").Bold().AlignRight();
                            });

                            table.Cell().Element(BodyCell).Text(NomeArquivoValido(nota.Quarto));
                            table.Cell().Element(BodyCell).Text(NomeArquivoValido(nota.TipoQuarto));
                            table.Cell().Element(BodyCell).Text($"{nota.DataEntrada:dd/MM/yyyy} a {nota.DataSaida:dd/MM/yyyy} ({nota.NumDias} diária{(nota.NumDias != 1 ? "s" : "")})");
                            table.Cell().Element(BodyCell).AlignRight().Text(Moeda(nota.ValorDiaria));
                            table.Cell().Element(BodyCell).AlignRight().Text(Moeda(nota.ValorDiarias));
                        });

                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(48);
                                columns.ConstantColumn(72);
                                columns.ConstantColumn(82);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(72);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Pag.").Bold();
                                header.Cell().Element(HeaderCell).Text("Tipo").Bold();
                                header.Cell().Element(HeaderCell).Text("Data").Bold();
                                header.Cell().Element(HeaderCell).Text("Hospede").Bold();
                                header.Cell().Element(HeaderCell).Text("Operador").Bold();
                                header.Cell().Element(HeaderCell).Text("Valor").Bold().AlignRight();
                            });

                            if (nota.Pagamentos.Any())
                            {
                                foreach (var pagamento in nota.Pagamentos)
                                {
                                    table.Cell().Element(BodyCell).Text(pagamento.Numero.ToString("N0", enUS));
                                    table.Cell().Element(BodyCell).Text(NomeArquivoValido(pagamento.Tipo));
                                    table.Cell().Element(BodyCell).Text($"{pagamento.Data:dd/MM/yyyy HH:mm}");
                                    table.Cell().Element(BodyCell).Text(NomeArquivoValido(pagamento.Hospede));
                                    table.Cell().Element(BodyCell).Text(NomeArquivoValido(pagamento.Operador));
                                    table.Cell().Element(BodyCell).AlignRight().Text(Moeda(pagamento.Valor));
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(6).Element(BodyCell).Text("Sem pagamentos registados.").Italic();
                            }
                        });

                        column.Item().PaddingTop(18).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(85);
                                columns.RelativeColumn();
                                columns.ConstantColumn(110);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Historico").Bold();
                                header.Cell().Element(HeaderCell).Text("Data").Bold();
                                header.Cell().Element(HeaderCell).Text("Observacao").Bold();
                                header.Cell().Element(HeaderCell).Text("Operador").Bold();
                            });

                            if (nota.Historicos.Any())
                            {
                                foreach (var historico in nota.Historicos)
                                {
                                    table.Cell().Element(BodyCell).Text(historico.Numero.ToString("N0", enUS));
                                    table.Cell().Element(BodyCell).Text(historico.Data.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(BodyCell).Text(NomeArquivoValido(historico.Observacao));
                                    table.Cell().Element(BodyCell).Text(NomeArquivoValido(historico.Operador));
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(4).Element(BodyCell).Text("Sem histórico registado.").Italic();
                            }
                        });

                        // ─── PEDIDOS (consumos) ───────────────────────────────────
                        if (nota.Pedidos != null && nota.Pedidos.Count > 0)
                        {
                            column.Item().PaddingTop(18).Text("Consumos / Pedidos").Bold().FontSize(9);

                            column.Item().PaddingTop(4).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);  // pedido / ponto venda
                                    columns.RelativeColumn();    // descrição
                                    columns.ConstantColumn(30);  // qtd
                                    columns.ConstantColumn(72);  // preço
                                    columns.ConstantColumn(72);  // total
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCell).Text("Pedido").Bold();
                                    header.Cell().Element(HeaderCell).Text("Descrição").Bold();
                                    header.Cell().Element(HeaderCell).Text("Qtd").Bold().AlignCenter();
                                    header.Cell().Element(HeaderCell).Text("P. Unit.").Bold().AlignRight();
                                    header.Cell().Element(HeaderCell).Text("Total").Bold().AlignRight();
                                });

                                foreach (var pedido in nota.Pedidos)
                                {
                                    var nomeRef = $"{NomeArquivoValido(pedido.NumePedido)}\n{NomeArquivoValido(pedido.PontoVendaNome)}\n{pedido.DataPedido:dd/MM/yyyy HH:mm}";

                                    if (pedido.Itens != null && pedido.Itens.Count > 0)
                                    {
                                        var firstItem = pedido.Itens[0];
                                        table.Cell().RowSpan((uint)pedido.Itens.Count).Element(BodyCell).Text(nomeRef).FontSize(7.5f);

                                        table.Cell().Element(BodyCell).Text(NomeArquivoValido(firstItem.Descricao));
                                        table.Cell().Element(BodyCell).AlignCenter().Text(firstItem.Quantidade.ToString());
                                        table.Cell().Element(BodyCell).AlignRight().Text(Moeda(firstItem.PrecoUnitario));
                                        table.Cell().Element(BodyCell).AlignRight().Text(Moeda(firstItem.Total));

                                        for (int idx = 1; idx < pedido.Itens.Count; idx++)
                                        {
                                            var item = pedido.Itens[idx];
                                            table.Cell().Element(BodyCell).Text(NomeArquivoValido(item.Descricao));
                                            table.Cell().Element(BodyCell).AlignCenter().Text(item.Quantidade.ToString());
                                            table.Cell().Element(BodyCell).AlignRight().Text(Moeda(item.PrecoUnitario));
                                            table.Cell().Element(BodyCell).AlignRight().Text(Moeda(item.Total));
                                        }
                                    }
                                    else
                                    {
                                        table.Cell().Element(BodyCell).Text(nomeRef).FontSize(7.5f);
                                        table.Cell().ColumnSpan(4).Element(BodyCell).Text("Sem itens.").Italic();
                                    }
                                }
                            });
                        }

                        column.Item().PaddingTop(22).LineHorizontal(1).LineColor(Colors.Black);
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Utilizador CheckOut: {NomeArquivoValido(nota.UtilizadorCheckout)}").FontSize(9);
                                if (!string.IsNullOrWhiteSpace(nota.Operador))
                                    left.Item().PaddingTop(4).Text($"Operador: {NomeArquivoValido(nota.Operador)}").FontSize(9);
                                left.Item().PaddingTop(12).Text("Documento Processado por Computador").FontSize(8);
                            });

                            row.ConstantItem(170).Border(1).Column(summary =>
                            {
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("Diária");
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.ValorDiarias));
                                });
                                summary.Item().LineHorizontal(1);
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("Consumo");
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.Consumo));
                                });
                                summary.Item().LineHorizontal(1);
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("Desconto");
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.Desconto));
                                });
                                summary.Item().LineHorizontal(1);
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("TOTAL").Bold();
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.Total)).Bold();
                                });
                                summary.Item().LineHorizontal(1);
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("PAGOU(-)");
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.Pago));
                                });
                                summary.Item().LineHorizontal(1);
                                summary.Item().Row(r =>
                                {
                                    r.RelativeItem().Padding(5).Text("A PAGAR (=)").Bold();
                                    r.ConstantItem(82).Padding(5).BorderLeft(1).AlignRight().Text(Moeda(nota.APagar)).Bold();
                                });
                            });
                        });
                    });

                    static IContainer HeaderCell(IContainer container)
                    {
                        return container.BorderBottom(1).PaddingVertical(4).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8.5f));
                    }

                    static IContainer BodyCell(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8));
                    }
                });
            }).GeneratePdf();
        }

        public byte[] GerarMovimentoDiario(MovimentoDiarioDto movimento)
        {
            var enUS = new System.Globalization.CultureInfo("en-US");
            string Moeda(float v) => v.ToString("N2", enUS);
            string S(string t) => string.IsNullOrWhiteSpace(t) ? "-" : t;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(18);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    // ─── CABEÇALHO ───────────────────────────────────────
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.ConstantItem(65).Height(55).Element(c =>
                            {
                                if (!string.IsNullOrWhiteSpace(movimento.LogoCaminho) && File.Exists(movimento.LogoCaminho))
                                    c.Image(File.ReadAllBytes(movimento.LogoCaminho)).FitArea();
                                else
                                    c.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle()
                                        .Text("HOTEL").Bold().FontSize(12);
                            });

                            row.RelativeItem().PaddingLeft(10).Column(left =>
                            {
                                left.Item().Text(S(movimento.NomeHotel)).Bold().FontSize(11);
                                left.Item().Text(S(movimento.Endereco)).FontSize(9);
                                left.Item().Text(S(movimento.Cidade)).FontSize(9);
                                left.Item().Text($"NIF: {S(movimento.NumContribuinte)}").FontSize(9);
                                left.Item().Text($"Tel: {S(movimento.Telefone)}").FontSize(9);
                            });

                            row.ConstantItem(140).Column(right =>
                            {
                                right.Item().AlignRight().Text($"Impresso: {movimento.DataImpressao:d/M/yyyy HH:mm}").FontSize(8);
                                right.Item().PaddingTop(4).AlignRight().Text($"Período: {movimento.DataInicio:d/M/yyyy} a {movimento.DataFim:d/M/yyyy}").Bold().FontSize(9);
                            });
                        });

                        header.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(Colors.Black);

                        header.Item().PaddingVertical(4).AlignCenter()
                            .Text("MOVIMENTO DIÁRIO").Bold().FontSize(13).LetterSpacing(2);

                        header.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(10).Column(column =>
                    {
                        // ─── PAGAMENTOS EFECTUADOS ────────────────────────
                        column.Item().Text("Pagamentos efectuados.").Bold().FontSize(10);
                        column.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(70);   // Forma Pag.
                                cols.ConstantColumn(72);   // Data
                                cols.RelativeColumn();     // Observacao
                                cols.ConstantColumn(100);  // Operador
                                cols.ConstantColumn(72);   // Entradas
                                cols.ConstantColumn(55);   // Saidas
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(HCell).Text("Forma Pag.").Bold();
                                h.Cell().Element(HCell).Text("Data").Bold();
                                h.Cell().Element(HCell).Text("Observacao").Bold();
                                h.Cell().Element(HCell).Text("Operador").Bold();
                                h.Cell().Element(HCell).AlignRight().Text("Entradas").Bold();
                                h.Cell().Element(HCell).AlignRight().Text("Saidas").Bold();
                            });

                            if (movimento.Pagamentos.Any())
                            {
                                foreach (var p in movimento.Pagamentos)
                                {
                                    table.Cell().Element(BCell).Text(S(p.FormaPagamento));
                                    table.Cell().Element(BCell).Text(p.Data.ToString("dd-MM-yyyy"));
                                    table.Cell().Element(BCell).Text(S(p.Observacao));
                                    table.Cell().Element(BCell).Text(S(p.Operador));
                                    table.Cell().Element(BCell).AlignRight().Text(Moeda(p.Entradas));
                                    table.Cell().Element(BCell).AlignRight().Text(Moeda(p.Saidas));
                                }

                                // Linha de totais
                                var totalEntradas = movimento.Pagamentos.Sum(x => x.Entradas);
                                var totalSaidas = movimento.Pagamentos.Sum(x => x.Saidas);
                                table.Cell().ColumnSpan(4).Element(BCell).Text(string.Empty);
                                table.Cell().Element(c => c.PaddingVertical(5).PaddingHorizontal(2)
                                    .DefaultTextStyle(x => x.FontSize(8.5f).Bold()))
                                    .AlignRight().Text(Moeda(totalEntradas));
                                table.Cell().Element(c => c.PaddingVertical(5).PaddingHorizontal(2)
                                    .DefaultTextStyle(x => x.FontSize(8.5f).Bold()))
                                    .AlignRight().Text(Moeda(totalSaidas));
                            }
                            else
                            {
                                table.Cell().ColumnSpan(6).Element(BCell).Text("Sem pagamentos registados.").Italic();
                            }
                        });

                        column.Item().PaddingTop(16).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        // ─── CHECKINS EFECTUADOS ─────────────────────────
                        column.Item().PaddingTop(10).Text("Checkins efectuados.").Bold().FontSize(10);
                        column.Item().PaddingTop(4).Table(table =>
                        {
                            BuildCheckinTable(table, movimento.Checkins);
                        });

                        column.Item().PaddingTop(16).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        // ─── CHECKOUTS EFECTUADOS ────────────────────────
                        column.Item().PaddingTop(10).Text("Checkout efectuados.").Bold().FontSize(10);
                        column.Item().PaddingTop(4).Table(table =>
                        {
                            BuildCheckinTable(table, movimento.Checkouts);
                        });

                        column.Item().PaddingTop(16).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        // ─── HISTÓRICO ───────────────────────────────────
                        column.Item().PaddingTop(10).Text("Histórico.").Bold().FontSize(10);
                        column.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(45);   // #
                                cols.ConstantColumn(100);  // DataHora
                                cols.RelativeColumn();     // Observacao
                                cols.ConstantColumn(110);  // Utilizador
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(HCell).Text("#").Bold();
                                h.Cell().Element(HCell).Text("DataHora").Bold();
                                h.Cell().Element(HCell).Text("Observacao").Bold();
                                h.Cell().Element(HCell).Text("Utilizador").Bold();
                            });

                            if (movimento.Historicos.Any())
                            {
                                foreach (var h in movimento.Historicos)
                                {
                                    table.Cell().Element(BCell).Text(h.Numero.ToString());
                                    table.Cell().Element(BCell).Text(h.DataHora.ToString("d/M/yyyy H:mm:ss"));
                                    table.Cell().Element(BCell).Text(S(h.Observacao));
                                    table.Cell().Element(BCell).Text(S(h.Utilizador));
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(4).Element(BCell).Text("Sem histórico registado.").Italic();
                            }
                        });
                    });

                    // ─── RODAPÉ ──────────────────────────────────────────
                    page.Footer().PaddingTop(8).Column(footer =>
                    {
                        footer.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                        footer.Item().PaddingTop(6).Row(row =>
                        {
                            row.RelativeItem().Text($"Período: {movimento.DataInicio:d/M/yyyy} a {movimento.DataFim:d/M/yyyy}").FontSize(9);
                            row.ConstantItem(200).AlignRight()
                                .Text($"Assinatura: ___________________________").FontSize(9);
                        });
                    });

                    static IContainer HCell(IContainer c) =>
                        c.BorderBottom(1).PaddingVertical(4).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8.5f));

                    static IContainer BCell(IContainer c) =>
                        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8));
                });
            }).GeneratePdf();

            static void BuildCheckinTable(TableDescriptor table, List<MovimentoCheckinDto> rows)
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(48);   // Checkin
                    cols.ConstantColumn(48);   // Quarto
                    cols.RelativeColumn();     // Periodo
                    cols.ConstantColumn(80);   // Hospede
                    cols.ConstantColumn(110);  // Empresa
                    cols.ConstantColumn(80);   // Utilizador
                });

                table.Header(h =>
                {
                    h.Cell().Element(HH).Text("Checkin").Bold();
                    h.Cell().Element(HH).Text("Quarto").Bold();
                    h.Cell().Element(HH).Text("Periodo").Bold();
                    h.Cell().Element(HH).Text("Hospede").Bold();
                    h.Cell().Element(HH).Text("Empresa").Bold();
                    h.Cell().Element(HH).Text("Utilizador").Bold();
                });

                if (rows != null && rows.Any())
                {
                    foreach (var r in rows)
                    {
                        table.Cell().Element(BB).Text(r.CheckinId.ToString());
                        table.Cell().Element(BB).Text(r.Quarto ?? "-");
                        table.Cell().Element(BB).Text(r.Periodo ?? "-");
                        table.Cell().Element(BB).Text(r.Hospede ?? "-");
                        table.Cell().Element(BB).Text(r.Empresa ?? "-");
                        table.Cell().Element(BB).Text(r.Utilizador ?? "-");
                    }
                }
                else
                {
                    table.Cell().ColumnSpan(6).Element(BB).Text("Sem registos.").Italic();
                }

                static IContainer HH(IContainer c) =>
                    c.BorderBottom(1).PaddingVertical(4).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8.5f));

                static IContainer BB(IContainer c) =>
                    c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8));
            }
        }

        public byte[] GerarMovimentoCaixa(MovimentoCaixaDto movimento)
        {
            var enUS = new System.Globalization.CultureInfo("en-US");
            string Moeda(float v) => v.ToString("N2", enUS);
            string S(string t) => string.IsNullOrWhiteSpace(t) ? "-" : t;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    // ─── CABEÇALHO ───────────────────────────────────────
                    page.Header().Column(header =>
                    {
                        header.Item().AlignCenter().Text("LANÇAMENTO DO CAIXA DO DIA").Bold().FontSize(14).LetterSpacing(1);

                        header.Item().PaddingVertical(6).Row(row =>
                        {
                            row.RelativeItem().Text($"Período da lagem {movimento.Periodo}")
                                .FontSize(9);
                            row.ConstantItem(80).AlignRight()
                                .Text(movimento.DataRelatorio.ToString("d/M/yyyy")).FontSize(9);
                        });

                        header.Item().LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingTop(8).Column(column =>
                    {
                        // ─── TABELA PRINCIPAL ─────────────────────────────
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(75);   // Data
                                cols.ConstantColumn(90);   // Forma Pag.
                                cols.RelativeColumn();     // Observacao
                                cols.ConstantColumn(110);  // Operador
                                cols.ConstantColumn(85);   // Entradas
                                cols.ConstantColumn(85);   // Saidas
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(HCell).Text("Data").Bold();
                                h.Cell().Element(HCell).Text("Forma Pag.").Bold();
                                h.Cell().Element(HCell).Text("Observacao").Bold();
                                h.Cell().Element(HCell).Text("Operador").Bold();
                                h.Cell().Element(HCell).AlignRight().Text("Entradas").Bold();
                                h.Cell().Element(HCell).AlignRight().Text("Saidas").Bold();
                            });

                            if (movimento.Linhas.Any())
                            {
                                foreach (var linha in movimento.Linhas)
                                {
                                    table.Cell().Element(BCell).Text(linha.Data);
                                    table.Cell().Element(BCell).Text(S(linha.FormaPagamento));
                                    table.Cell().Element(BCell).Text(S(linha.Observacao));
                                    table.Cell().Element(BCell).Text(S(linha.Operador));
                                    table.Cell().Element(BCell).AlignRight().Text(Moeda(linha.Entradas));
                                    table.Cell().Element(BCell).AlignRight().Text(Moeda(linha.Saidas));
                                }

                                // Linha de TOTAIS
                                table.Cell().ColumnSpan(4).Element(c => 
                                    c.BorderTop(1.5f).BorderColor(Colors.Black).PaddingVertical(6).PaddingHorizontal(2)
                                        .AlignRight().Text("TOTAIS").Bold().FontSize(9.5f));
                                table.Cell().Element(c =>
                                    c.BorderTop(1.5f).BorderColor(Colors.Black).PaddingVertical(6).PaddingHorizontal(2)
                                        .AlignRight().Text(Moeda(movimento.TotalEntradas)).Bold().FontSize(9.5f));
                                table.Cell().Element(c =>
                                    c.BorderTop(1.5f).BorderColor(Colors.Black).PaddingVertical(6).PaddingHorizontal(2)
                                        .AlignRight().Text(Moeda(movimento.TotalSaidas)).Bold().FontSize(9.5f));
                            }
                            else
                            {
                                table.Cell().ColumnSpan(6).Element(BCell).Text("Sem movimentação de caixa registada neste período.").Italic();
                            }
                        });
                    });

                    // ─── RODAPÉ ──────────────────────────────────────────
                    page.Footer().PaddingTop(8).Column(footer =>
                    {
                        footer.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                        footer.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem().Text($"Impresso: {movimento.DataImpressao:d/M/yyyy HH:mm:ss}").FontSize(8);
                            row.ConstantItem(150).AlignRight().Text("Documento Processado por Computador").FontSize(8);
                        });
                    });

                    static IContainer HCell(IContainer c) =>
                        c.BorderBottom(1.5f).BorderColor(Colors.Black).PaddingVertical(4).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8.5f));

                    static IContainer BCell(IContainer c) =>
                        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(2).DefaultTextStyle(x => x.FontSize(8.5f));
                });
            }).GeneratePdf();
        }

        public byte[] GerarHistoricoOcupacao(IEnumerable<HistoricoOcupacaoDto> linhas, string titulo, int totalQuartos, int quartosOcupados)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var itens = linhas?.ToList() ?? new List<HistoricoOcupacaoDto>();
            var quartosLivres = Math.Max(0, totalQuartos - quartosOcupados);
            var ocupacaoPct = totalQuartos > 0 ? Math.Round(100.0 * quartosOcupados / totalQuartos, 2) : 0;
            var livrePct = totalQuartos > 0 ? Math.Round(100.0 * quartosLivres / totalQuartos, 2) : 0;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // A4 paisagem para melhor largura da tabela
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().PaddingTop(10).Column(col =>
                    {
                        col.Item().AlignCenter().Text(titulo ?? "Histórico de Ocupação").FontSize(14).Bold();
                        col.Item().PaddingTop(18).LineHorizontal(1.2f).LineColor(Colors.Black);
                    });

                    page.Content().PaddingVertical(5).Column(column =>
                    {
                        // Tabela com colunas definidas
                        column.Item().Table(table =>
                        {
                            // Ajuste as larguras conforme necessidade
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);   // Checkin
                                columns.ConstantColumn(70);   // Quarto
                                columns.RelativeColumn();     // Hospede
                                columns.ConstantColumn(200);  // Empresa
                                columns.ConstantColumn(90);   // DataAbertura
                                columns.ConstantColumn(90);   // Checkout
                            });

                            // Cabeçalho
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Checkin").FontSize(9).Bold();
                                header.Cell().Element(CellStyle).Text("Quarto").FontSize(9).Bold();
                                header.Cell().Element(CellStyle).Text("Hóspede").FontSize(9).Bold();
                                header.Cell().Element(CellStyle).Text("Empresa").FontSize(9).Bold();
                                header.Cell().Element(CellStyle).Text("DataAbertura").FontSize(9).Bold();
                                header.Cell().Element(CellStyle).Text("Checkout").FontSize(9).Bold();
                            });

                            // Linhas
                            foreach (var item in itens)
                            {
                                table.Cell().Element(CellStyle).Text(item.Checkin.ToString()).FontSize(9);
                                table.Cell().Element(CellStyle).Text(item.Quarto).FontSize(9);
                                table.Cell().Element(CellStyle).Text(item.Hospede).FontSize(9);
                                table.Cell().Element(CellStyle).Text(item.Empresa).FontSize(9);
                                table.Cell().Element(CellStyle).Text(item.DataAbertura.ToString("dd-MM-yyyy")).FontSize(9);
                                table.Cell().Element(CellStyle).Text(item.Checkout).FontSize(9);
                            }

                            // estilo da célula reutilizável
                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(4)
                                    .PaddingHorizontal(6)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .AlignLeft();
                            }
                        });

                        // Espaço antes do rodapé
                        column.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        column.Item().Height(14);

                        // Rodapé com totais
                        column.Item().Row(row =>
                        {
                            // Tabela de totais à esquerda
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Quartos Ocupados:                {quartosOcupados}").FontSize(10);
                                left.Item().Text($"Quartos Livres:                      {quartosLivres}").FontSize(10);
                                left.Item().Text($"Total de Quartos :                 {totalQuartos}").FontSize(10);
                            });

                            // Percentagens à direita
                            row.ConstantItem(220).Column(right =>
                            {
                                right.Item().Text($"% Ocupação:                   {ocupacaoPct:0.##}").FontSize(10).AlignRight();
                                right.Item().Text($"% Livre:                           {livrePct:0.##}").FontSize(10).AlignRight();
                            });
                        });

                    });

                    page.Footer().AlignCenter().Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9);
                });
            }).GeneratePdf();
}

        public byte[] GerarRelatorioGovernancaArrumacao(IEnumerable<GovernancaArrumacaoDto> linhas, string titulo)
        {
            var itens = linhas?.ToList() ?? new List<GovernancaArrumacaoDto>();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().PaddingTop(8).AlignCenter()
                        .Text(titulo ?? "GOVERNANÇA RELATÓRIO DE ARRUMAÇÃO")
                        .FontSize(13)
                        .Bold();

                    page.Content().PaddingTop(14).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // Id
                                columns.ConstantColumn(58); // Codigo
                                columns.RelativeColumn(2);  // Hospede
                                columns.ConstantColumn(95); // Tipo
                                columns.ConstantColumn(36); // Pax
                                columns.ConstantColumn(58); // Checkin
                                columns.ConstantColumn(58); // Checkout
                                columns.ConstantColumn(42); // PaxGov
                                columns.ConstantColumn(42); // Limpo
                                columns.ConstantColumn(36); // PV
                                columns.ConstantColumn(36); // NQA
                                columns.ConstantColumn(36); // DF
                                columns.ConstantColumn(36); // SB
                                columns.ConstantColumn(36); // MB
                                columns.ConstantColumn(36); // PB
                                columns.RelativeColumn();   // OBS
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Id").Bold();
                                header.Cell().Element(CellStyle).Text("Codigo").Bold();
                                header.Cell().Element(CellStyle).Text("Hospede").Bold();
                                header.Cell().Element(CellStyle).Text("Tipo").Bold();
                                header.Cell().Element(CellStyle).Text("PAX").Bold();
                                header.Cell().Element(CellStyle).Text("Checkin").Bold();
                                header.Cell().Element(CellStyle).Text("Checkout").Bold();
                                header.Cell().Element(CellStyle).Text("PaxGov").Bold();
                                header.Cell().Element(CellStyle).Text("Limpo").Bold();
                                header.Cell().Element(CellStyle).Text("P.V").Bold();
                                header.Cell().Element(CellStyle).Text("NQA").Bold();
                                header.Cell().Element(CellStyle).Text("D.F").Bold();
                                header.Cell().Element(CellStyle).Text("S.B").Bold();
                                header.Cell().Element(CellStyle).Text("M.B").Bold();
                                header.Cell().Element(CellStyle).Text("P.B").Bold();
                                header.Cell().Element(CellStyle).Text("OBS").Bold();
                            });

                            foreach (var item in itens)
                            {
                                DataCell(table, item.CheckinId.ToString("N0", new System.Globalization.CultureInfo("en-US")));
                                DataCell(table, item.Codigo);
                                DataCell(table, item.Hospede);
                                DataCell(table, item.Tipo);
                                DataCell(table, item.Pax.ToString());
                                DataCell(table, item.Checkin.ToString("dd-MM-yyyy"));
                                DataCell(table, item.Checkout.ToString("dd-MM-yyyy"));
                                DataCell(table, item.PaxGov);
                                DataCell(table, item.Limpo);
                                DataCell(table, item.PV);
                                DataCell(table, item.NQA);
                                DataCell(table, item.DF);
                                DataCell(table, item.SB);
                                DataCell(table, item.MB);
                                DataCell(table, item.PB);
                                DataCell(table, item.Observacao);
                            }

                            if (!itens.Any())
                            {
                                table.Cell().ColumnSpan(16).Element(CellStyle).Text("Sem registos para apresentar.").Italic();
                            }
                        });

                        column.Item().PaddingTop(18).Text("Legenda").FontSize(8).Bold();
                        column.Item().PaddingTop(4).Column(legend =>
                        {
                            legend.Item().Text("- D.F = o hospede dormiu fora na noite anterior").FontSize(7);
                            legend.Item().Text("- NQA = nao quis arrumacao;").FontSize(7);
                            legend.Item().Text("- P.V = significa placa vermelha, ou seja, favor nao perturbe, pois o apartamento ainda esta ocupado pelo hospede").FontSize(7);
                            legend.Item().Text("- PAX = numero de hospedes cadastrados no apartamento").FontSize(7);
                            legend.Item().Text("- MB = muita bagagem").FontSize(7);
                            legend.Item().Text("- PB = pouca bagagem").FontSize(7);
                            legend.Item().Text("- SB = sem bagagem").FontSize(7);
                            legend.Item().Text("- PaxGov = indica se o numero de hospedes informados no check-in e o mesmo conferido pela governanca ao checar o quarto").FontSize(7);
                        });
                    });

                    static void DataCell(TableDescriptor table, string text)
                    {
                        table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(text) ? string.Empty : text);
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Border(1)
                            .BorderColor(Colors.Grey.Darken2)
                            .PaddingVertical(4)
                            .PaddingHorizontal(3)
                            .DefaultTextStyle(x => x.FontSize(7));
                    }
                });
            }).GeneratePdf();
        }

        public byte[] GerarRelatorioVistoriaQuartos(IEnumerable<VistoriaQuartoDto> linhas, string titulo, string funcionario, DateTime dataReferencia)
        {
            var itens = linhas?.ToList() ?? new List<VistoriaQuartoDto>();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().PaddingTop(4).AlignCenter()
                            .Text(titulo ?? "Vistória de Quartos")
                            .FontSize(13)
                            .Bold();

                        column.Item().PaddingTop(18).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Text($"O Funcionário {funcionario}").FontSize(8).Bold();
                            row.ConstantItem(180).AlignRight().Text($"Data ____/____/______").FontSize(8);
                        });
                    });

                    page.Content().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(65);
                            columns.RelativeColumn(1.3f);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(1.8f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Quarto").Bold();
                            header.Cell().Element(CellStyle).Text("Tipo de quarto").Bold();
                            header.Cell().Element(CellStyle).Text("situacao").Bold();
                            header.Cell().Element(CellStyle).Text("PAX").Bold();
                            header.Cell().Element(CellStyle).Text("Observação").Bold();
                        });

                        foreach (var item in itens)
                        {
                            table.Cell().Element(CellStyle).Text(item.Quarto ?? string.Empty);
                            table.Cell().Element(CellStyle).Text(item.TipoQuarto ?? string.Empty);
                            table.Cell().Element(CellStyle).Text(item.Situacao ?? string.Empty);
                            table.Cell().Element(CellStyle).Text(item.Pax > 0 ? item.Pax.ToString() : string.Empty);
                            table.Cell().Element(CellStyle).Text(item.Observacao ?? string.Empty);
                        }

                        if (!itens.Any())
                        {
                            table.Cell().ColumnSpan(5).Element(CellStyle).Text("Sem registos para apresentar.").Italic();
                        }
                    });

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Border(1)
                            .BorderColor(Colors.Grey.Darken2)
                            .PaddingVertical(6)
                            .PaddingHorizontal(5)
                            .DefaultTextStyle(x => x.FontSize(7));
                    }
                });
            }).GeneratePdf();
        }

        public byte[] GerarReciboPedido(ReciboPedidoDto recibo)
        {
            var enUS = new System.Globalization.CultureInfo("en-US");

            DateTime angolaNow;
            try
            {
                angolaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Africa/Luanda"));
            }
            catch
            {
                try
                {
                    angolaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));
                }
                catch
                {
                    angolaNow = DateTime.UtcNow.AddHours(1);
                }
            }

            string Moeda(decimal v) => v.ToString("N2", enUS);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(new PageSize(90, 250, Unit.Millimetre));
                    page.Margin(8, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Content().Column(column =>
                    {
                        // ─── CABEÇALHO ────────────────────────────────────────────
                        column.Item().Text(recibo.NomeHotel ?? "-").Bold().FontSize(11).AlignCenter();
                        if (!string.IsNullOrWhiteSpace(recibo.Endereco))
                            column.Item().Text(recibo.Endereco).FontSize(8).AlignCenter();
                        if (!string.IsNullOrWhiteSpace(recibo.Cidade))
                            column.Item().Text(recibo.Cidade).FontSize(8).AlignCenter();
                        if (!string.IsNullOrWhiteSpace(recibo.NumContribuinte))
                            column.Item().Text(recibo.NumContribuinte).FontSize(8).AlignCenter();

                        column.Item().PaddingVertical(4).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── TÍTULO DO PEDIDO ─────────────────────────────────────
                        column.Item().PaddingVertical(3)
                            .Text(recibo.NumePedido ?? "PEDIDO").Bold().FontSize(13).AlignCenter();

                        if (!string.IsNullOrWhiteSpace(recibo.PontoVendaNome))
                            column.Item().Text(recibo.PontoVendaNome).FontSize(9).AlignCenter();

                        // ─── CLIENTE / QUARTO ─────────────────────────────────────
                        column.Item().Height(4);
                        column.Item().Text(recibo.NomeCliente ?? "Cliente Diverso").Bold().FontSize(9).AlignCenter();

                        if (!string.IsNullOrWhiteSpace(recibo.ApartamentoCodigo))
                            column.Item().Text($"Quarto: {recibo.ApartamentoCodigo}").FontSize(8).AlignCenter();

                        column.Item().PaddingBottom(2)
                            .Text($"Data: {recibo.DataPedido:dd/MM/yyyy HH:mm}").FontSize(8).AlignCenter();
                        column.Item().PaddingBottom(4)
                            .Text($"Impresso: {angolaNow:d/M/yyyy h:mm:sstt}").FontSize(8).AlignCenter();

                        column.Item().Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── TABELA DE ITENS ──────────────────────────────────────
                        column.Item().PaddingVertical(4).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);   // descrição
                                columns.ConstantColumn(22);  // qtd
                                columns.ConstantColumn(30);  // preço unit
                                columns.ConstantColumn(30);  // total
                            });

                            // cabeçalho da tabela
                            table.Header(header =>
                            {
                                header.Cell().PaddingBottom(3).Text("Descrição").Bold().FontSize(7.5f);
                                header.Cell().PaddingBottom(3).AlignCenter().Text("Qtd").Bold().FontSize(7.5f);
                                header.Cell().PaddingBottom(3).AlignRight().Text("PUnit").Bold().FontSize(7.5f);
                                header.Cell().PaddingBottom(3).AlignRight().Text("Total").Bold().FontSize(7.5f);
                            });

                            if (recibo.Itens != null && recibo.Itens.Count > 0)
                            {
                                foreach (var item in recibo.Itens)
                                {
                                    table.Cell().PaddingVertical(2).Text(item.Descricao ?? "-").FontSize(8);
                                    table.Cell().PaddingVertical(2).AlignCenter().Text(item.Quantidade.ToString()).FontSize(8);
                                    table.Cell().PaddingVertical(2).AlignRight().Text(Moeda(item.PrecoUnitario)).FontSize(8);
                                    table.Cell().PaddingVertical(2).AlignRight().Text(Moeda(item.ValorTotal)).FontSize(8);
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(4).PaddingVertical(4).Text("Sem itens registados.").Italic().FontSize(8);
                            }
                        });

                        column.Item().Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── TOTALIZADORES ────────────────────────────────────────
                        column.Item().PaddingVertical(3).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL").Bold().FontSize(11);
                            row.ConstantItem(55).Text(Moeda(recibo.ValorTotal)).Bold().FontSize(11).AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("PAGOU(-)").Bold().FontSize(9);
                            row.ConstantItem(55).Text(Moeda(recibo.ValorPago)).FontSize(9).AlignRight();
                        });

                        if (!string.IsNullOrWhiteSpace(recibo.FormaPagamento))
                        {
                            column.Item().PaddingBottom(3).Row(row =>
                            {
                                row.RelativeItem().Text("Forma Pag.").FontSize(9);
                                row.ConstantItem(55).Text(recibo.FormaPagamento).FontSize(9).AlignRight();
                            });
                        }

                        column.Item().PaddingVertical(2).Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);

                        // ─── OBSERVAÇÃO ───────────────────────────────────────────
                        if (!string.IsNullOrWhiteSpace(recibo.Observacao))
                        {
                            column.Item().PaddingVertical(4).Text($"Obs: {recibo.Observacao}").FontSize(8);
                            column.Item().Height(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black);
                        }

                        // ─── RODAPÉ ───────────────────────────────────────────────
                        column.Item().PaddingTop(8)
                            .Text($"Operador: {recibo.Operador ?? "-"}").FontSize(8).AlignCenter();
                        column.Item().PaddingTop(6)
                            .Text("Processado por computador").FontSize(7).AlignCenter();
                    });
                });
            }).GeneratePdf();
        }
    }
}