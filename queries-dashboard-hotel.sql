-- ==============================================================================
-- QUERIES PARA DASHBOARD DO HOTEL
-- Data de criação: 11 de agosto de 2025
-- ==============================================================================

-- 1. CHECK-INS DE HOJE
-- ==============================================================================
SELECT 
    c.Id,
    c.DataEntrada,
    a.Codigo AS ApartamentoCodigo,
    ta.Descricao AS TipoApartamento,
    c.ValorTotalDiaria,
    c.ValorTotalFinal,
    c.CamaExtra,
    c.Observacao,
    u.Nome AS UtilizadorCheckin
FROM Checkins c
INNER JOIN Apartamentos a ON c.Id = a.CheckinsId
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
LEFT JOIN Utilizadores u ON c.IdUtilizadorCheckin = u.Id
WHERE CAST(c.DataEntrada AS DATE) = CAST(GETDATE() AS DATE)
    AND c.IsDeleted = 0
    AND a.IsDeleted = 0
ORDER BY c.DataEntrada DESC;

-- ==============================================================================
-- 2. CHECK-OUTS DE HOJE
-- ==============================================================================
SELECT 
    c.Id,
    c.DataSaida,
    c.DataEntrada,
    a.Codigo AS ApartamentoCodigo,
    ta.Descricao AS TipoApartamento,
    c.ValorTotalFinal,
    c.ValorDesconto,
    c.PercentualDesconto,
    c.CheckoutRealizado,
    u.Nome AS UtilizadorCheckout,
    DATEDIFF(day, c.DataEntrada, c.DataSaida) AS DiasHospedagem
FROM Checkins c
INNER JOIN Apartamentos a ON c.Id = a.CheckinsId
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
LEFT JOIN Utilizadores u ON c.IdUtilizadorCheckOut = u.Id
WHERE CAST(c.DataSaida AS DATE) = CAST(GETDATE() AS DATE)
    AND c.CheckoutRealizado = 1
    AND c.IsDeleted = 0
    AND a.IsDeleted = 0
ORDER BY c.DataSaida DESC;

-- ==============================================================================
-- 3. DADOS PARA GRÁFICO - CHECK-INS E CHECK-OUTS SEMANAL (ÚLTIMAS 4 SEMANAS)
-- ==============================================================================
WITH AllWeeks AS (
    SELECT 'Semana 1' AS SemanaLabel, 1 AS SemanaOrdem, DATEADD(week, -3, DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)) AS StartDate
    UNION ALL 
    SELECT 'Semana 2' AS SemanaLabel, 2 AS SemanaOrdem, DATEADD(week, -2, DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)) AS StartDate
    UNION ALL 
    SELECT 'Semana 3' AS SemanaLabel, 3 AS SemanaOrdem, DATEADD(week, -1, DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)) AS StartDate
    UNION ALL 
    SELECT 'Semana 4' AS SemanaLabel, 4 AS SemanaOrdem, DATEADD(week, 0, DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)) AS StartDate
),
WeeklyCheckins AS (
    SELECT 
        aw.SemanaLabel,
        aw.SemanaOrdem,
        COUNT(c.Id) AS TotalCheckins,
        SUM(c.ValorTotalFinal) AS ReceitaCheckins
    FROM AllWeeks aw
    LEFT JOIN Checkins c ON c.DataEntrada >= aw.StartDate 
        AND c.DataEntrada < DATEADD(week, 1, aw.StartDate)
        AND c.IsDeleted = 0
    GROUP BY aw.SemanaLabel, aw.SemanaOrdem
),
WeeklyCheckouts AS (
    SELECT 
        aw.SemanaLabel,
        aw.SemanaOrdem,
        COUNT(c.Id) AS TotalCheckouts,
        SUM(c.ValorTotalFinal) AS ReceitaCheckouts
    FROM AllWeeks aw
    LEFT JOIN Checkins c ON c.DataSaida >= aw.StartDate 
        AND c.DataSaida < DATEADD(week, 1, aw.StartDate)
        AND c.CheckoutRealizado = 1
        AND c.IsDeleted = 0
    GROUP BY aw.SemanaLabel, aw.SemanaOrdem
)
SELECT 
    aw.SemanaLabel,
    aw.SemanaOrdem,
    COALESCE(ci.TotalCheckins, 0) AS TotalCheckins,
    COALESCE(co.TotalCheckouts, 0) AS TotalCheckouts,
    COALESCE(ci.ReceitaCheckins, 0) AS ReceitaCheckins,
    COALESCE(co.ReceitaCheckouts, 0) AS ReceitaCheckouts
FROM AllWeeks aw
LEFT JOIN WeeklyCheckins ci ON aw.SemanaLabel = ci.SemanaLabel
LEFT JOIN WeeklyCheckouts co ON aw.SemanaLabel = co.SemanaLabel
ORDER BY aw.SemanaOrdem;

-- ==============================================================================
-- 4. TAXA DE OCUPAÇÃO ATUAL (FORMATO PARA GRÁFICO DOUGHNUT)
-- ==============================================================================
WITH OcupacaoAtual AS (
    SELECT 
        COUNT(*) AS TotalApartamentos,
        SUM(CASE WHEN a.Situacao = 1 THEN 1 ELSE 0 END) AS ApartamentosOcupados, -- 1 = Ocupado
        SUM(CASE WHEN a.Situacao = 0 THEN 1 ELSE 0 END) AS ApartamentosLivres -- 0 = Livre
    FROM Apartamentos a
    WHERE a.IsDeleted = 0
)
SELECT 
    'Ocupados' AS Label,
    ApartamentosOcupados AS Valor,
    CASE 
        WHEN TotalApartamentos > 0 
        THEN CAST((ApartamentosOcupados * 100.0 / TotalApartamentos) AS DECIMAL(5,2))
        ELSE 0 
    END AS Percentual,
    '#28a745' AS Cor
FROM OcupacaoAtual
WHERE ApartamentosOcupados > 0

UNION ALL

SELECT 
    'Disponíveis' AS Label,
    ApartamentosLivres AS Valor,
    CASE 
        WHEN TotalApartamentos > 0 
        THEN CAST((ApartamentosLivres * 100.0 / TotalApartamentos) AS DECIMAL(5,2))
        ELSE 0 
    END AS Percentual,
    '#e9ecef' AS Cor
FROM OcupacaoAtual
WHERE ApartamentosLivres > 0

ORDER BY Valor DESC;

-- ==============================================================================
-- 5. RECEITA DO MÊS ATUAL
-- ==============================================================================
SELECT 
    MONTH(GETDATE()) AS Mes,
    YEAR(GETDATE()) AS Ano,
    COUNT(*) AS TotalCheckouts,
    SUM(c.ValorTotalFinal) AS ReceitaTotal,
    SUM(c.ValorTotalDiaria) AS ReceitaDiarias,
    SUM(c.ValorTotalConsumo) AS ReceitaConsumo,
    SUM(c.ValorTotalLigacao) AS ReceitaLigacoes,
    SUM(c.ValorDesconto) AS TotalDescontos,
    AVG(c.ValorTotalFinal) AS TicketMedio,
    AVG(DATEDIFF(day, c.DataEntrada, c.DataSaida)) AS MediaDiasHospedagem
FROM Checkins c
WHERE MONTH(c.DataSaida) = MONTH(GETDATE())
    AND YEAR(c.DataSaida) = YEAR(GETDATE())
    AND c.CheckoutRealizado = 1
    AND c.IsDeleted = 0;

-- ==============================================================================
-- 6. EVOLUÇÃO DA RECEITA AO LONGO DO ANO (FORMATO PARA GRÁFICO DE LINHA)
-- ==============================================================================
WITH MesesAno AS (
    SELECT 1 AS MesNumero, 'Jan' AS MesNome
    UNION ALL SELECT 2, 'Fev'
    UNION ALL SELECT 3, 'Mar'
    UNION ALL SELECT 4, 'Abr'
    UNION ALL SELECT 5, 'Mai'
    UNION ALL SELECT 6, 'Jun'
    UNION ALL SELECT 7, 'Jul'
    UNION ALL SELECT 8, 'Ago'
    UNION ALL SELECT 9, 'Set'
    UNION ALL SELECT 10, 'Out'
    UNION ALL SELECT 11, 'Nov'
    UNION ALL SELECT 12, 'Dez'
),
ReceitaMensal AS (
    SELECT 
        MONTH(c.DataSaida) AS MesNumero,
        SUM(c.ValorTotalFinal) AS ReceitaTotal,
        COUNT(*) AS TotalCheckouts
    FROM Checkins c
    WHERE YEAR(c.DataSaida) = YEAR(GETDATE())
        AND c.CheckoutRealizado = 1
        AND c.IsDeleted = 0
    GROUP BY MONTH(c.DataSaida)
)
SELECT 
    ma.MesNome AS Label,
    ma.MesNumero,
    COALESCE(rm.ReceitaTotal, 0) AS Receita,
    COALESCE(rm.TotalCheckouts, 0) AS TotalCheckouts,
    '#007bff' AS CorLinha,
    'rgba(0, 123, 255, 0.1)' AS CorFundo
FROM MesesAno ma
LEFT JOIN ReceitaMensal rm ON ma.MesNumero = rm.MesNumero
WHERE ma.MesNumero <= MONTH(GETDATE()) -- Apenas meses até o atual
ORDER BY ma.MesNumero;

-- ==============================================================================
-- 7. DISTRIBUIÇÃO DOS APARTAMENTOS (OCUPADO vs DISPONÍVEL)
-- ==============================================================================
SELECT 
    a.Id,
    a.Codigo,
    ta.Descricao AS TipoApartamento,
    ta.Capacidade,
    ta.ValorDiaria,
    CASE a.Situacao
        WHEN 0 THEN 'Livre'
        WHEN 1 THEN 'Ocupado'
        WHEN 2 THEN 'Manutenção'
        WHEN 3 THEN 'Atrasado'
        WHEN 4 THEN 'Hoje'
        WHEN 5 THEN 'Amanhã'
        WHEN 6 THEN 'Limpeza'
        WHEN 7 THEN 'Bloqueado'
        ELSE 'Indefinido'
    END AS SituacaoDescricao,
    a.Situacao,
    a.CodigoRamal,
    a.CafeDaManha,
    a.NaoPertube,
    tg.Descricao AS TipoGovernanca,
    CASE 
        WHEN c.Id IS NOT NULL THEN c.DataEntrada
        ELSE NULL 
    END AS DataCheckin,
    CASE 
        WHEN c.Id IS NOT NULL THEN c.DataSaida
        ELSE NULL 
    END AS DataCheckout
FROM Apartamentos a
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
LEFT JOIN TipoGovernanca tg ON a.TipoGovernancasId = tg.Id
LEFT JOIN Checkins c ON a.CheckinsId = c.Id AND c.IsDeleted = 0
WHERE a.IsDeleted = 0
ORDER BY a.Codigo;

-- ==============================================================================
-- 8. RESUMO ESTATÍSTICO PARA DASHBOARD
-- ==============================================================================
WITH EstatisticasHoje AS (
    SELECT 
        COUNT(CASE WHEN CAST(c.DataEntrada AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) AS CheckinsHoje,
        COUNT(CASE WHEN CAST(c.DataSaida AS DATE) = CAST(GETDATE() AS DATE) AND c.CheckoutRealizado = 1 THEN 1 END) AS CheckoutsHoje,
        COUNT(CASE WHEN c.CheckoutRealizado = 0 AND c.IsDeleted = 0 THEN 1 END) AS HospedesAtivos
    FROM Checkins c
    WHERE c.IsDeleted = 0
),
EstatisticasApartamentos AS (
    SELECT 
        COUNT(*) AS TotalApartamentos,
        COUNT(CASE WHEN a.Situacao = 0 THEN 1 END) AS ApartamentosLivres,
        COUNT(CASE WHEN a.Situacao = 1 THEN 1 END) AS ApartamentosOcupados,
        COUNT(CASE WHEN a.Situacao = 2 THEN 1 END) AS ApartamentosManutencao,
        COUNT(CASE WHEN a.Situacao = 6 THEN 1 END) AS ApartamentosLimpeza
    FROM Apartamentos a
    WHERE a.IsDeleted = 0
)
SELECT 
    eh.CheckinsHoje,
    eh.CheckoutsHoje,
    eh.HospedesAtivos,
    ea.TotalApartamentos,
    ea.ApartamentosLivres,
    ea.ApartamentosOcupados,
    ea.ApartamentosManutencao,
    ea.ApartamentosLimpeza,
    CASE 
        WHEN ea.TotalApartamentos > 0 
        THEN CAST((ea.ApartamentosOcupados * 100.0 / ea.TotalApartamentos) AS DECIMAL(5,2))
        ELSE 0 
    END AS TaxaOcupacao
FROM EstatisticasHoje eh
CROSS JOIN EstatisticasApartamentos ea;

-- ==============================================================================
-- 9. TOP 10 APARTAMENTOS MAIS UTILIZADOS (ÚLTIMOS 6 MESES)
-- ==============================================================================
SELECT TOP 10
    a.Codigo AS ApartamentoCodigo,
    ta.Descricao AS TipoApartamento,
    COUNT(c.Id) AS TotalReservas,
    SUM(c.ValorTotalFinal) AS ReceitaTotal,
    AVG(c.ValorTotalFinal) AS TicketMedio,
    AVG(DATEDIFF(day, c.DataEntrada, c.DataSaida)) AS MediaDiasOcupacao
FROM Checkins c
INNER JOIN Apartamentos a ON c.Id = a.CheckinsId
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
WHERE c.DataEntrada >= DATEADD(month, -6, GETDATE())
    AND c.IsDeleted = 0
    AND a.IsDeleted = 0
GROUP BY a.Codigo, ta.Descricao
ORDER BY TotalReservas DESC, ReceitaTotal DESC;

-- ==============================================================================
-- 10. PREVISÃO DE CHECK-OUTS PRÓXIMOS (PRÓXIMOS 3 DIAS)
-- ==============================================================================
SELECT 
    c.Id,
    c.DataSaida,
    a.Codigo AS ApartamentoCodigo,
    ta.Descricao AS TipoApartamento,
    c.ValorTotalFinal,
    c.CheckoutRealizado,
    DATEDIFF(day, GETDATE(), c.DataSaida) AS DiasRestantes,
    CASE 
        WHEN DATEDIFF(day, GETDATE(), c.DataSaida) = 0 THEN 'Hoje'
        WHEN DATEDIFF(day, GETDATE(), c.DataSaida) = 1 THEN 'Amanhã'
        ELSE CONCAT(DATEDIFF(day, GETDATE(), c.DataSaida), ' dias')
    END AS TempoRestante
FROM Checkins c
INNER JOIN Apartamentos a ON c.Id = a.CheckinsId
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
WHERE c.DataSaida BETWEEN GETDATE() AND DATEADD(day, 3, GETDATE())
    AND c.CheckoutRealizado = 0
    AND c.IsDeleted = 0
    AND a.IsDeleted = 0
ORDER BY c.DataSaida ASC;

-- ==============================================================================
-- 11. QUERY SIMPLIFICADA PARA CARDS DO DASHBOARD
-- ==============================================================================
SELECT 
    'checkins_hoje' AS Metric,
    COUNT(*) AS Valor,
    'check-ins hoje' AS Label,
    '#28a745' AS Cor
FROM Checkins c
WHERE CAST(c.DataEntrada AS DATE) = CAST(GETDATE() AS DATE)
    AND c.IsDeleted = 0

UNION ALL

SELECT 
    'checkouts_hoje' AS Metric,
    COUNT(*) AS Valor,
    'check-outs hoje' AS Label,
    '#dc3545' AS Cor
FROM Checkins c
WHERE CAST(c.DataSaida AS DATE) = CAST(GETDATE() AS DATE)
    AND c.CheckoutRealizado = 1
    AND c.IsDeleted = 0

UNION ALL

SELECT 
    'ocupacao_atual' AS Metric,
    CAST((COUNT(CASE WHEN a.Situacao = 1 THEN 1 END) * 100.0 / COUNT(*)) AS INT) AS Valor,
    '% ocupação' AS Label,
    '#007bff' AS Cor
FROM Apartamentos a
WHERE a.IsDeleted = 0

UNION ALL

SELECT 
    'receita_mes' AS Metric,
    CAST(COALESCE(SUM(c.ValorTotalFinal), 0) AS INT) AS Valor,
    'receita mês (€)' AS Label,
    '#ffc107' AS Cor
FROM Checkins c
WHERE MONTH(c.DataSaida) = MONTH(GETDATE())
    AND YEAR(c.DataSaida) = YEAR(GETDATE())
    AND c.CheckoutRealizado = 1
    AND c.IsDeleted = 0;

-- ==============================================================================
-- 12. RESERVAS PRÓXIMAS PARA LISTA DO DASHBOARD
-- ==============================================================================
SELECT TOP 5
    c.Id,
    a.Codigo AS Apartamento,
    ta.Descricao AS TipoApartamento,
    FORMAT(c.DataEntrada, 'dd/MM/yyyy HH:mm') AS DataEntrada,
    FORMAT(c.DataSaida, 'dd/MM/yyyy') AS DataSaida,
    c.ValorTotalFinal,
    CASE 
        WHEN c.CheckoutRealizado = 1 THEN 'Check-out'
        WHEN CAST(c.DataEntrada AS DATE) = CAST(GETDATE() AS DATE) THEN 'Check-in'
        WHEN CAST(c.DataEntrada AS DATE) > CAST(GETDATE() AS DATE) THEN 'Confirmado'
        ELSE 'Em andamento'
    END AS Status,
    CASE 
        WHEN c.CheckoutRealizado = 1 THEN 'badge-secondary'
        WHEN CAST(c.DataEntrada AS DATE) = CAST(GETDATE() AS DATE) THEN 'badge-success'
        WHEN CAST(c.DataEntrada AS DATE) > CAST(GETDATE() AS DATE) THEN 'badge-warning'
        ELSE 'badge-primary'
    END AS StatusClass
FROM Checkins c
INNER JOIN Apartamentos a ON c.Id = a.CheckinsId
INNER JOIN TipoApartamento ta ON a.TipoApartamentosId = ta.Id
WHERE c.IsDeleted = 0
    AND a.IsDeleted = 0
    AND (
        CAST(c.DataEntrada AS DATE) >= CAST(GETDATE() AS DATE)
        OR (c.CheckoutRealizado = 0 AND CAST(c.DataSaida AS DATE) >= CAST(GETDATE() AS DATE))
    )
ORDER BY 
    CASE WHEN c.CheckoutRealizado = 0 THEN 0 ELSE 1 END,
    c.DataEntrada ASC;
