-- Scripts SQL para inserção de dados de exemplo na tabela Tenant

-- 1. Tenant para Hotel Cascais (Produção)
INSERT INTO [Tenant] (
    [Id], [Name], [DatabaseServerName], [UserID], [Password], [DatabaseName], 
    [ConnectionString], [Metadata_Region], [Metadata_MaxUsers], [Metadata_IsActive], 
    [Metadata_Prazo], [Metadata_KeySerial], [Metadata_CustomSettings]
) VALUES (
    'hotel-cascais',
    'Hotel Cascais Palace',
    'SQL1004.site4now.net',
    'db_abc375_ghotel_admin',
    'RENT2024',
    'db_abc375_ghotel_cascais',
    'Data Source=SQL1004.site4now.net;Initial Catalog=db_abc375_ghotel_cascais;User Id=db_abc375_ghotel_admin;Password=RENT2024;Encrypt=false',
    'Lisboa',
    50,
    1,
    365,
    'HSC-2025-001',
    '{"timezone":"Europe/Lisbon","currency":"EUR","language":"pt-PT","hotel_category":"5_stars","check_in_time":"15:00","check_out_time":"12:00","email_notifications":"true","backup_frequency":"daily"}'
);

-- 2. Tenant para Hotel Porto (Produção)
INSERT INTO [Tenant] (
    [Id], [Name], [DatabaseServerName], [UserID], [Password], [DatabaseName], 
    [ConnectionString], [Metadata_Region], [Metadata_MaxUsers], [Metadata_IsActive], 
    [Metadata_Prazo], [Metadata_KeySerial], [Metadata_CustomSettings]
) VALUES (
    'hotel-porto',
    'Hotel Porto Vintage',
    'SQL1004.site4now.net',
    'db_abc376_ghotel_admin',
    'RENT2024',
    'db_abc376_ghotel_porto',
    'Data Source=SQL1004.site4now.net;Initial Catalog=db_abc376_ghotel_porto;User Id=db_abc376_ghotel_admin;Password=RENT2024;Encrypt=false',
    'Porto',
    25,
    1,
    730,
    'HPV-2025-002',
    '{"timezone":"Europe/Lisbon","currency":"EUR","language":"pt-PT","hotel_category":"4_stars","check_in_time":"14:00","check_out_time":"11:00","wifi_included":"true","parking_available":"true"}'
);

-- 3. Tenant para Pousada Algarve (Produção)
INSERT INTO [Tenant] (
    [Id], [Name], [DatabaseServerName], [UserID], [Password], [DatabaseName], 
    [ConnectionString], [Metadata_Region], [Metadata_MaxUsers], [Metadata_IsActive], 
    [Metadata_Prazo], [Metadata_KeySerial], [Metadata_CustomSettings]
) VALUES (
    'pousada-algarve',
    'Pousada do Algarve',
    'SQL1004.site4now.net',
    'db_abc377_ghotel_admin',
    'RENT2024',
    'db_abc377_ghotel_algarve',
    'Data Source=SQL1004.site4now.net;Initial Catalog=db_abc377_ghotel_algarve;User Id=db_abc377_ghotel_admin;Password=RENT2024;Encrypt=false',
    'Faro',
    15,
    1,
    365,
    'PAL-2025-003',
    '{"timezone":"Europe/Lisbon","currency":"EUR","language":"pt-PT","hotel_category":"3_stars","seasonal_pricing":"true","pool_access":"true","beach_proximity":"500m"}'
);

-- 4. Tenant para Hotel Internacional (Luanda)
INSERT INTO [Tenant] (
    [Id], [Name], [DatabaseServerName], [UserID], [Password], [DatabaseName], 
    [ConnectionString], [Metadata_Region], [Metadata_MaxUsers], [Metadata_IsActive], 
    [Metadata_Prazo], [Metadata_KeySerial], [Metadata_CustomSettings]
) VALUES (
    'hotel-luanda',
    'Hotel Luanda Premium',
    'SQL1005.site4now.net',
    'db_abc378_ghotel_admin',
    'RENT2024',
    'db_abc378_ghotel_luanda',
    'Data Source=SQL1005.site4now.net;Initial Catalog=db_abc378_ghotel_luanda;User Id=db_abc378_ghotel_admin;Password=RENT2024;Encrypt=false',
    'Luanda',
    100,
    1,
    365,
    'HLP-2025-004',
    '{"timezone":"Africa/Luanda","currency":"AOA","language":"pt-AO","hotel_category":"5_stars","business_center":"true","conference_rooms":"5","gym_available":"true","spa_services":"true"}'
);

-- 5. Atualizar o tenant localhost existente (se necessário)
UPDATE [Tenant] SET 
    [Metadata_CustomSettings] = '{"timezone":"Europe/Lisbon","currency":"EUR","language":"pt-PT","hotel_category":"test","debug_mode":"true","test_environment":"true","backup_frequency":"never"}',
    [Metadata_KeySerial] = 'TEST-2025-001',
    [Metadata_Prazo] = 30
WHERE [Id] = 'localhost';

-- 6. Verificar os dados inseridos
SELECT 
    [Id],
    [Name],
    [DatabaseServerName],
    [Metadata_Region],
    [Metadata_MaxUsers],
    [Metadata_IsActive],
    [Metadata_Prazo],
    [Metadata_KeySerial],
    [Metadata_CustomSettings]
FROM [Tenant]
ORDER BY [Id];

-- 7. Exemplo de consulta para obter configurações customizadas
SELECT 
    [Id],
    [Name],
    JSON_VALUE([Metadata_CustomSettings], '$.timezone') as Timezone,
    JSON_VALUE([Metadata_CustomSettings], '$.currency') as Currency,
    JSON_VALUE([Metadata_CustomSettings], '$.hotel_category') as HotelCategory,
    JSON_VALUE([Metadata_CustomSettings], '$.check_in_time') as CheckInTime,
    JSON_VALUE([Metadata_CustomSettings], '$.check_out_time') as CheckOutTime
FROM [Tenant]
WHERE [Metadata_IsActive] = 1;
