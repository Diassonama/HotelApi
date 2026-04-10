-- ==============================================================================
-- QUERY ALTERNATIVA PARA DADOS SEMANAIS (MAIS SIMPLES)
-- ==============================================================================

-- Versão Simplificada - DADOS PARA GRÁFICO SEMANAL
WITH WeeklyData AS (
    -- Check-ins por semana (últimas 4 semanas)
    SELECT 
        'Semana ' + CAST((4 - DATEDIFF(week, c.DataEntrada, GETDATE())) AS VARCHAR(1)) AS SemanaLabel,
        (4 - DATEDIFF(week, c.DataEntrada, GETDATE())) AS SemanaOrdem,
        COUNT(*) AS TotalCheckins,
        SUM(c.ValorTotalFinal) AS ReceitaCheckins,
        0 AS TotalCheckouts,
        0 AS ReceitaCheckouts
    FROM Checkins c
    WHERE c.DataEntrada >= DATEADD(week, -4, GETDATE())
        AND c.DataEntrada < GETDATE()
        AND c.IsDeleted = 0
        AND DATEDIFF(week, c.DataEntrada, GETDATE()) BETWEEN 0 AND 3
    GROUP BY DATEDIFF(week, c.DataEntrada, GETDATE())
    
    UNION ALL
    
    -- Check-outs por semana (últimas 4 semanas)
    SELECT 
        'Semana ' + CAST((4 - DATEDIFF(week, c.DataSaida, GETDATE())) AS VARCHAR(1)) AS SemanaLabel,
        (4 - DATEDIFF(week, c.DataSaida, GETDATE())) AS SemanaOrdem,
        0 AS TotalCheckins,
        0 AS ReceitaCheckins,
        COUNT(*) AS TotalCheckouts,
        SUM(c.ValorTotalFinal) AS ReceitaCheckouts
    FROM Checkins c
    WHERE c.DataSaida >= DATEADD(week, -4, GETDATE())
        AND c.DataSaida < GETDATE()
        AND c.CheckoutRealizado = 1
        AND c.IsDeleted = 0
        AND DATEDIFF(week, c.DataSaida, GETDATE()) BETWEEN 0 AND 3
    GROUP BY DATEDIFF(week, c.DataSaida, GETDATE())
),
WeeklyAggregated AS (
    SELECT 
        SemanaLabel,
        SemanaOrdem,
        SUM(TotalCheckins) AS TotalCheckins,
        SUM(TotalCheckouts) AS TotalCheckouts,
        SUM(ReceitaCheckins) AS ReceitaCheckins,
        SUM(ReceitaCheckouts) AS ReceitaCheckouts
    FROM WeeklyData
    GROUP BY SemanaLabel, SemanaOrdem
),
AllWeeks AS (
    SELECT 'Semana 1' AS SemanaLabel, 1 AS SemanaOrdem
    UNION ALL SELECT 'Semana 2', 2
    UNION ALL SELECT 'Semana 3', 3
    UNION ALL SELECT 'Semana 4', 4
)
SELECT 
    aw.SemanaLabel,
    aw.SemanaOrdem,
    COALESCE(wa.TotalCheckins, 0) AS TotalCheckins,
    COALESCE(wa.TotalCheckouts, 0) AS TotalCheckouts,
    COALESCE(wa.ReceitaCheckins, 0) AS ReceitaCheckins,
    COALESCE(wa.ReceitaCheckouts, 0) AS ReceitaCheckouts
FROM AllWeeks aw
LEFT JOIN WeeklyAggregated wa ON aw.SemanaLabel = wa.SemanaLabel
ORDER BY aw.SemanaOrdem;
