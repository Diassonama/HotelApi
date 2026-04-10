# 📱 WhatsApp Business Integration - Documentação Completa

## 🚀 Funcionalidades Implementadas

### ✅ Endpoints Criados

1. **`POST /api/racknotification/whatsapp/send-notification`**
   - Envia notificações WhatsApp para hóspedes
   - Suporta templates predefinidos
   - Validação de números de telefone
   - Integração com RackHub para notificações em tempo real

2. **`POST /api/racknotification/whatsapp/webhook`**
   - Recebe status de entrega do WhatsApp Business
   - Processa confirmações de entrega, leitura e falhas
   - Endpoint público (sem autenticação) para webhooks

3. **`GET /api/racknotification/whatsapp/templates`**
   - Lista templates de mensagens disponíveis
   - Inclui templates para check-in, check-out, pagamentos, etc.

## 📋 Tipos de Mensagens Suportadas

### 1. Confirmação de Check-in
```json
{
  "messageType": "checkin_confirmation",
  "content": "Informações adicionais do hotel"
}
```
**Resultado**: "🏨 Olá! Bem-vindo ao nosso hotel. Seu check-in no apartamento 101 foi confirmado. [conteúdo]"

### 2. Lembrete de Check-out
```json
{
  "messageType": "checkout_reminder",
  "content": "Instruções de check-out"
}
```
**Resultado**: "📅 Lembrete: Seu check-out está previsto para hoje. Apartamento: 101. [conteúdo]"

### 3. Confirmação de Pagamento
```json
{
  "messageType": "payment_confirmation",
  "content": "Detalhes do pagamento"
}
```
**Resultado**: "💰 Pagamento confirmado! Obrigado. [conteúdo]"

### 4. Mensagem de Boas-vindas
```json
{
  "messageType": "welcome_message",
  "content": "Informações do hotel"
}
```
**Resultado**: "🎉 Bem-vindo! Esperamos que tenha uma estadia incrível. [conteúdo]"

### 5. Mensagem Personalizada
```json
{
  "messageType": "custom_message",
  "content": "Qualquer texto personalizado"
}
```
**Resultado**: Texto exato do campo `content`

## 🔧 Configuração da API WhatsApp Business

### Pré-requisitos

1. **Conta WhatsApp Business**
   - Criar conta no [Meta for Developers](https://developers.facebook.com/)
   - Configurar WhatsApp Business API
   - Obter Phone Number ID e Access Token

2. **Webhook Configuration**
   - URL do webhook: `https://seu-hotel-api.com/api/racknotification/whatsapp/webhook`
   - Verificar token: configurar no Meta Developer Console

### Configuração no `appsettings.json`

```json
{
  "WhatsAppBusiness": {
    "AccessToken": "SEU_ACCESS_TOKEN",
    "PhoneNumberId": "SEU_PHONE_NUMBER_ID",
    "WebhookVerifyToken": "SEU_WEBHOOK_VERIFY_TOKEN",
    "BaseUrl": "https://graph.facebook.com/v17.0"
  }
}
```

### Implementação Real da API (Substitua a simulação)

```csharp
public class WhatsAppBusinessService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WhatsAppBusinessService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<bool> SendMessageAsync(string phoneNumber, string message)
    {
        var accessToken = _configuration["WhatsAppBusiness:AccessToken"];
        var phoneNumberId = _configuration["WhatsAppBusiness:PhoneNumberId"];
        var baseUrl = _configuration["WhatsAppBusiness:BaseUrl"];

        var payload = new
        {
            messaging_product = "whatsapp",
            to = phoneNumber,
            type = "text",
            text = new { body = message }
        };

        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/{phoneNumberId}/messages", payload);

        return response.IsSuccessStatusCode;
    }
}
```

## 🔐 Segurança e Validação

### Validação de Números de Telefone
- Formato internacional obrigatório: `+[código país][número]`
- Validação de comprimento (10-15 dígitos)
- Verificação de caracteres numéricos

### Limitações de Conteúdo
- Máximo 1000 caracteres por mensagem
- Validação de campos obrigatórios
- Sanitização de entrada

### Autenticação
- Endpoints protegidos com `[Authorize]`
- Webhook público (como requerido pelo WhatsApp)
- Logs detalhados para auditoria

## 📊 Integração com RackHub

### Notificações em Tempo Real
Todas as ações WhatsApp geram notificações no RackHub:

```csharp
// Envio de mensagem
await _rackNotificationService.NotifyInfoAsync(
    $"WhatsApp {request.MessageType} enviado para {request.PhoneNumber}", 
    notificationData);

// Status de entrega
await _rackNotificationService.NotifyInfoAsync(
    "✅ WhatsApp entregue para +351912345678", 
    webhookData);
```

### Dados Transmitidos
- Número de telefone
- Tipo de mensagem
- Conteúdo enviado
- Status de entrega
- Timestamps
- IDs de hóspede/check-in

## 🧪 Testes

### 1. Teste Local (Simulação)
```bash
# Executar API
dotnet run

# Testar endpoint
curl -X POST "https://localhost:7000/api/racknotification/whatsapp/send-notification" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "phoneNumber": "+351912345678",
    "messageType": "welcome_message",
    "content": "Bem-vindo ao nosso hotel!"
  }'
```

### 2. Teste com Postman/Insomnia
- Importar arquivo `whatsapp-business.http`
- Configurar token de autenticação
- Executar cenários de teste

### 3. Teste de Webhook
```bash
# Simular webhook do WhatsApp
curl -X POST "https://localhost:7000/api/racknotification/whatsapp/webhook" \
  -H "Content-Type: application/json" \
  -d '{
    "messageId": "test123",
    "status": "delivered",
    "phoneNumber": "+351912345678",
    "timestamp": "2025-08-13T14:30:00Z"
  }'
```

## 🚀 Deploy em Produção

### 1. Configurar Domínio HTTPS
- WhatsApp Business requer HTTPS
- Certificado SSL válido
- Webhook acessível publicamente

### 2. Configurar Webhook no Meta
1. Acesse o [Meta for Developers](https://developers.facebook.com/)
2. Selecione sua aplicação WhatsApp Business
3. Configure webhook URL: `https://seu-dominio.com/api/racknotification/whatsapp/webhook`
4. Adicione verify token
5. Subscrever aos eventos: `messages`, `message_deliveries`, `message_reads`

### 3. Variáveis de Ambiente
```bash
# .env ou Azure App Settings
WHATSAPP_ACCESS_TOKEN=your_access_token
WHATSAPP_PHONE_NUMBER_ID=your_phone_number_id
WHATSAPP_WEBHOOK_VERIFY_TOKEN=your_verify_token
```

## 📈 Monitoramento e Logs

### Logs Implementados
```csharp
// Tentativa de envio
_logger.LogInformation("Tentativa de envio de WhatsApp para {PhoneNumber}, Tipo: {MessageType}");

// Webhook recebido
_logger.LogInformation("Webhook WhatsApp recebido: {MessageId}, Status: {Status}");

// Erro
_logger.LogError(ex, "Erro ao enviar notificação WhatsApp para {PhoneNumber}");
```

### Métricas Recomendadas
- Taxa de entrega
- Taxa de leitura
- Tempo de resposta
- Falhas de envio
- Volume de mensagens por tipo

## ⚡ Casos de Uso Práticos

### 1. Automatização de Check-in
```csharp
// Quando check-in é realizado
await whatsAppService.SendNotificationAsync(new WhatsAppNotificationRequest
{
    PhoneNumber = hospede.Telefone,
    MessageType = "checkin_confirmation",
    Content = $"WiFi: {hotel.WifiPassword}. Café da manhã: 7h-10h.",
    ApartamentoCodigo = apartamento.Codigo,
    HospedeId = hospede.Id,
    CheckinId = checkin.Id
});
```

### 2. Lembretes Automáticos
```csharp
// Agendar lembrete de check-out (usar Hangfire ou similar)
BackgroundJob.Schedule(() => 
    whatsAppService.SendCheckoutReminderAsync(checkin.Id), 
    checkin.DataCheckout.AddHours(-2)); // 2h antes
```

### 3. Notificações de Emergência
```csharp
// Broadcast para todos os hóspedes
var hospedes = await context.Hospedes
    .Where(h => h.CheckinAtivo)
    .ToListAsync();

foreach (var hospede in hospedes)
{
    await whatsAppService.SendEmergencyNotificationAsync(hospede.Telefone, 
        "🚨 Evacuação do edifício. Dirija-se ao ponto de encontro.");
}
```

## 🔄 Integração com Sistema Existente

### 1. Modificar Serviços Existentes
```csharp
// No CheckinService
public async Task<Response<CheckinDto>> CreateCheckinAsync(CreateCheckinDto dto)
{
    var checkin = await CreateCheckin(dto);
    
    // Enviar WhatsApp de confirmação
    await _whatsAppService.SendCheckinConfirmationAsync(checkin);
    
    return Response<CheckinDto>.Success(checkin);
}
```

### 2. Adicionar Configurações de Hóspede
```csharp
public class Hospede
{
    // ... propriedades existentes
    public bool AceitaWhatsApp { get; set; } = true;
    public string TelefoneWhatsApp { get; set; } // Pode ser diferente do telefone principal
}
```

## 📱 Exemplo Frontend (Angular)

```typescript
// Serviço Angular para WhatsApp
export class WhatsAppService {
  constructor(private http: HttpClient) {}

  sendNotification(request: WhatsAppNotificationRequest): Observable<any> {
    return this.http.post('/api/racknotification/whatsapp/send-notification', request);
  }

  getTemplates(): Observable<any> {
    return this.http.get('/api/racknotification/whatsapp/templates');
  }
}

// Componente para enviar mensagem
sendWelcomeMessage(hospede: Hospede) {
  const request: WhatsAppNotificationRequest = {
    phoneNumber: hospede.telefoneWhatsApp,
    messageType: 'welcome_message',
    content: 'Esperamos que tenha uma estadia incrível!',
    hospedeId: hospede.id
  };

  this.whatsAppService.sendNotification(request).subscribe({
    next: (response) => this.toastr.success('WhatsApp enviado!'),
    error: (error) => this.toastr.error('Erro ao enviar WhatsApp')
  });
}
```

## 🎯 Roadmap de Melhorias

### Fase 1 (Atual) ✅
- [x] Endpoints básicos
- [x] Templates de mensagem
- [x] Webhook de status
- [x] Integração com RackHub

### Fase 2 (Próxima)
- [ ] Templates dinâmicos com variáveis
- [ ] Agendamento de mensagens
- [ ] Mensagens com mídia (imagens, documentos)
- [ ] Chatbot básico

### Fase 3 (Futuro)
- [ ] Inteligência artificial para respostas
- [ ] Integração com sistema de CRM
- [ ] Analytics avançado
- [ ] Campanhas de marketing

---

## 🔧 Troubleshooting

### Problema: Webhook não recebe dados
**Solução**: Verificar se a URL está acessível publicamente e se o verify token está correto.

### Problema: Número de telefone inválido
**Solução**: Usar formato internacional (+351XXXXXXXXX) e verificar se o número está registrado no WhatsApp.

### Problema: Mensagem não entregue
**Solução**: Verificar se o número está ativo no WhatsApp e se não está bloqueado.

---

✨ **Sistema WhatsApp Business totalmente integrado com o RackHub!** 📱🏨
