# RackHub - Sistema de Notificações em Tempo Real

## Visão Geral

O **RackHub** é um sistema de notificações em tempo real para o sistema de gestão hoteleira, utilizando **SignalR** para comunicação bidirecional entre servidor e clientes.

## Características

- ✅ Notificações de check-in em tempo real
- ✅ Notificações de check-out em tempo real  
- ✅ Atualizações de status de apartamentos
- ✅ Notificações de pagamentos
- ✅ Atualizações de hospedagem
- ✅ Métricas do dashboard em tempo real
- ✅ Sistema de grupos para segmentação
- ✅ Notificações de erro e informação
- ✅ Logs detalhados de todas as operações

## Endpoints SignalR

### Hub Principal
```
/rackHub
```

### Eventos do Cliente (Recebidos)

| Evento | Descrição | Dados |
|--------|-----------|-------|
| `CheckinUpdate` | Novo check-in realizado | `{ Type, CheckinId, ApartamentoId, ApartamentoCodigo, DataEntrada, Hospedes, Timestamp }` |
| `CheckoutUpdate` | Check-out realizado | `{ Type, CheckinId, ApartamentoId, ApartamentoCodigo, DataSaida, Hospedes, Timestamp }` |
| `ApartmentStatusUpdate` | Mudança de status do apartamento | `{ Type, ApartamentoId, Codigo, Nome, Situacao, CheckinsId, Timestamp }` |
| `HospedagemUpdate` | Alteração na hospedagem | `{ Type, HospedagemId, CheckinsId, DataAbertura, PrevisaoFechamento, ... }` |
| `PaymentUpdate` | Novo pagamento | `{ Type, PagamentoId, CheckinsId, Valor, FormaPagamento, DataPagamento, Timestamp }` |
| `ApartamentosOcupadosUpdate` | Lista de apartamentos ocupados | `{ Type, Apartamentos[], Count, Timestamp }` |
| `DashboardMetricsUpdate` | Métricas do dashboard | `{ Type, Metrics, Timestamp }` |
| `GeneralRackUpdate` | Atualização geral do rack | `{ Type, Message, Timestamp }` |
| `ErrorNotification` | Notificação de erro | `{ Type, Message, Details, Timestamp }` |
| `InfoNotification` | Notificação informativa | `{ Type, Message, Data, Timestamp }` |
| `RackUpdate` | Evento geral de atualização | Varia conforme o tipo |

### Métodos do Cliente (Enviados)

| Método | Descrição | Parâmetros |
|--------|-----------|------------|
| `JoinGroup(groupName)` | Entrar em um grupo específico | `string groupName` |
| `LeaveGroup(groupName)` | Sair de um grupo específico | `string groupName` |
| `SendTestMessage(message)` | Enviar mensagem de teste | `string message` |

## API REST para Testes

### Base URL
```
/api/RackNotification
```

### Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/test-notification` | Teste de notificação geral |
| POST | `/test-info` | Teste de notificação informativa |
| POST | `/test-error` | Teste de notificação de erro |
| POST | `/update-dashboard-metrics` | Força atualização das métricas |

## Grupos do SignalR

### Grupo Principal
- **RackGroup**: Todos os clientes conectados ao rack

### Grupos Personalizados (Exemplos)
- **Floor1**: Apartamentos do 1º andar
- **Floor2**: Apartamentos do 2º andar
- **VIP**: Apartamentos VIP
- **Standard**: Apartamentos padrão

## Implementação no Cliente

### Conexão JavaScript/TypeScript
```typescript
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
    .withUrl("/rackHub")
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();

// Conectar
await connection.start();

// Escutar eventos
connection.on("CheckinUpdate", (data) => {
    console.log("Novo check-in:", data);
    // Atualizar UI
});

connection.on("CheckoutUpdate", (data) => {
    console.log("Check-out realizado:", data);
    // Atualizar UI
});

connection.on("ApartmentStatusUpdate", (data) => {
    console.log("Status do apartamento alterado:", data);
    // Atualizar rack visual
});

connection.on("RackUpdate", (data) => {
    console.log("Atualização geral do rack:", data);
    // Refresh geral ou específico baseado no Type
});

// Entrar em grupo
await connection.invoke("JoinGroup", "Floor1");
```

### Conexão Angular Service
```typescript
@Injectable({
  providedIn: 'root'
})
export class RackSignalRService {
  private hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/rackHub')
      .withAutomaticReconnect()
      .build();
  }

  async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
      console.log('Conectado ao RackHub');
    } catch (err) {
      console.error('Erro ao conectar:', err);
    }
  }

  // Eventos
  onCheckinUpdate(callback: (data: any) => void): void {
    this.hubConnection.on('CheckinUpdate', callback);
  }

  onCheckoutUpdate(callback: (data: any) => void): void {
    this.hubConnection.on('CheckoutUpdate', callback);
  }

  onApartmentStatusUpdate(callback: (data: any) => void): void {
    this.hubConnection.on('ApartmentStatusUpdate', callback);
  }

  onRackUpdate(callback: (data: any) => void): void {
    this.hubConnection.on('RackUpdate', callback);
  }

  // Métodos
  async joinGroup(groupName: string): Promise<void> {
    await this.hubConnection.invoke('JoinGroup', groupName);
  }

  async leaveGroup(groupName: string): Promise<void> {
    await this.hubConnection.invoke('LeaveGroup', groupName);
  }
}
```

## Integração com Serviços

### DashboardController
```csharp
// No controller, inject IRackNotificationService
public DashboardController(
    IDashboardService dashboardService,
    IRackNotificationService rackNotificationService)

// Após operações importantes
await _rackNotificationService.NotifyApartamentosOcupadosAsync(data);
```

### CheckinService (Exemplo)
```csharp
public class CheckinService
{
    private readonly IRackNotificationService _notificationService;

    public async Task ProcessCheckin(Checkins checkin)
    {
        // Salvar check-in
        await _repository.AddAsync(checkin);
        
        // Notificar
        await _notificationService.NotifyCheckinAsync(checkin);
        await _notificationService.NotifyRackUpdateAsync();
    }
}
```

## Logs e Monitoring

### Logs Automáticos
- Conexões e desconexões de clientes
- Envio de notificações
- Erros de comunicação
- Operações de grupos

### Níveis de Log
- **Information**: Operações normais
- **Warning**: Notificações de erro enviadas
- **Error**: Falhas na comunicação SignalR

## Configuração

### Program.cs
```csharp
// Adicionar SignalR
builder.Services.AddSignalR();

// Registrar serviços
services.AddScoped<IRackNotificationService, RackNotificationService>();

// Mapear Hub
app.MapHub<RackHub>("/rackHub");
```

### Autorização
- O Hub requer autorização (`[Authorize]`)
- Todos os métodos verificam autenticação
- Logs incluem identificação do usuário

## Casos de Uso

### 1. Dashboard em Tempo Real
- Métricas atualizadas automaticamente
- Apartamentos ocupados em tempo real
- Notificações de novos check-ins/check-outs

### 2. Rack Visual
- Status dos apartamentos atualizado instantaneamente
- Cores e estados sincronizados entre clientes
- Notificações de limpeza e manutenção

### 3. Gestão de Receção
- Alertas de check-in esperado
- Notificações de check-out em atraso
- Atualizações de pagamentos

### 4. Relatórios Dinâmicos
- Dados atualizados em tempo real
- Notificações de alterações importantes
- Sincronização entre múltiplos usuários

## Performance

### Otimizações
- Grupos para reduzir tráfego desnecessário
- Dados estruturados e compactos
- Reconnexão automática
- Logs otimizados

### Escalabilidade
- Suporte a múltiplos clientes
- Grupos dinâmicos
- Balanceamento de carga (com Redis backplane se necessário)

## Troubleshooting

### Problemas Comuns
1. **Conexão falhando**: Verificar autenticação e URL
2. **Eventos não recebidos**: Verificar se está no grupo correto
3. **Performance lenta**: Revisar quantidade de clientes e frequência de envios

### Debug
```typescript
// Ativar logs detalhados
const connection = new HubConnectionBuilder()
    .withUrl("/rackHub")
    .configureLogging(LogLevel.Debug)
    .build();
```

## Roadmap

### Funcionalidades Futuras
- [ ] Notificações push para dispositivos móveis
- [ ] Integração com webhooks externos
- [ ] Filtros avançados de notificações
- [ ] Histórico de notificações
- [ ] Dashboard de monitoramento do SignalR
- [ ] Suporte a Redis para clusters

## Conclusão

O RackHub fornece uma base sólida para notificações em tempo real no sistema hoteleiro, permitindo uma experiência mais responsiva e integrada para todos os usuários do sistema.
