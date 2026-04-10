# 🏨 RackHub Angular Integration - Guia Completo

Este diretório contém exemplos completos de como integrar o **RackHub SignalR** com Angular para receber notificações em tempo real do sistema de hotel.

## 📁 Estrutura dos Arquivos

```
angular-examples/
├── services/
│   └── rack-hub.service.ts           # Serviço principal do SignalR
├── components/
│   ├── rack-dashboard.component.ts   # Componente do dashboard
│   ├── rack-dashboard.component.html # Template do dashboard
│   └── rack-dashboard.component.scss # Estilos do dashboard
├── rack-hub.module.ts                # Módulo Angular
└── README.md                         # Este arquivo
```

## 🚀 Configuração Inicial

### 1. Dependências Necessárias

Instale as dependências no seu projeto Angular:

```bash
# Angular CLI (se ainda não tiver)
npm install -g @angular/cli

# SignalR Client
npm install @microsoft/signalr

# RxJS (geralmente já vem com Angular)
npm install rxjs

# Opcional: Para notificações toast
npm install ngx-toastr
# ou
npm install primeng
```

### 2. Configuração no `package.json`

```json
{
  "dependencies": {
    "@angular/core": "^17.0.0",
    "@angular/common": "^17.0.0",
    "@angular/forms": "^17.0.0",
    "@angular/router": "^17.0.0",
    "@microsoft/signalr": "^8.0.0",
    "rxjs": "^7.8.0"
  }
}
```

### 3. Configuração no `app.module.ts`

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { RackHubModule } from './rack-hub/rack-hub.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RackHubModule.forRoot()  // ← Importante: usar forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## 📋 Como Usar

### 1. Copiar os Arquivos

Copie todos os arquivos deste diretório para o seu projeto Angular:

```bash
# Criar a estrutura
mkdir -p src/app/rack-hub/services
mkdir -p src/app/rack-hub/components

# Copiar os arquivos
cp angular-examples/services/* src/app/rack-hub/services/
cp angular-examples/components/* src/app/rack-hub/components/
cp angular-examples/rack-hub.module.ts src/app/rack-hub/
```

### 2. Configurar a URL do SignalR

No arquivo `rack-hub.service.ts`, configure a URL do seu backend:

```typescript
// Linha ~47
private hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl('https://seu-backend.azurewebsites.net/rackhub', {  // ← Altere aqui
    accessTokenFactory: () => this.getAuthToken()
  })
  .build();
```

### 3. Configurar Autenticação

Se o seu RackHub exigir autenticação, configure o token:

```typescript
// No rack-hub.service.ts, método getAuthToken()
private getAuthToken(): string {
  // Obter o token do localStorage, sessionStorage, ou serviço de auth
  return localStorage.getItem('authToken') || '';
}
```

### 4. Usar o Componente

No template do seu componente principal (`app.component.html`):

```html
<app-rack-dashboard></app-rack-dashboard>
```

Ou criar uma rota específica no `app-routing.module.ts`:

```typescript
const routes: Routes = [
  {
    path: 'rack',
    loadChildren: () => import('./rack-hub/rack-hub.module').then(m => m.RackHubModule)
  }
];
```

## 🎯 Funcionalidades Implementadas

### ✅ Conexão SignalR
- Conexão automática ao inicializar
- Reconexão automática em caso de desconexão
- Status de conexão em tempo real

### ✅ Notificações em Tempo Real
- **Check-ins**: Novos check-ins com detalhes do hóspede
- **Check-outs**: Check-outs com valores pagos
- **Status dos Apartamentos**: Mudanças de status (Livre, Ocupado, Limpeza, etc.)
- **Pagamentos**: Notificações de pagamentos
- **Erros**: Notificações de erro do sistema

### ✅ Interface Completa
- Dashboard em tempo real com estatísticas
- Rack visual dos apartamentos
- Feed de notificações
- Controles de conexão e teste

### ✅ Gerenciamento de Grupos
- Entrar e sair de grupos específicos
- Notificações direcionadas (ex: "reception", "housekeeping")

## 🔧 Personalização

### Modificar Estilos

Edite o arquivo `rack-dashboard.component.scss` para personalizar a aparência:

```scss
// Cores do tema
.apartment-card.apartment-free {
  border-color: #28a745;  // Verde para livre
}

.apartment-card.apartment-occupied {
  border-color: #dc3545;  // Vermelho para ocupado
}
```

### Adicionar Novos Tipos de Notificação

No `rack-hub.service.ts`, adicione novos observables:

```typescript
// Adicionar nova propriedade
private maintenanceSubject = new BehaviorSubject<MaintenanceData | null>(null);
public maintenance$ = this.maintenanceSubject.asObservable();

// Adicionar listener no setupEventListeners()
this.hubConnection.on('MaintenanceScheduled', (data: MaintenanceData) => {
  this.maintenanceSubject.next(data);
  this.addNotification('MAINTENANCE', 'Manutenção agendada', data);
});
```

### Integrar com Toasts

Para usar ngx-toastr, modifique o método `showToast` no componente:

```typescript
import { ToastrService } from 'ngx-toastr';

constructor(
  private rackHubService: RackHubService,
  private toastr: ToastrService  // ← Adicionar
) {}

private showToast(message: string, type: 'success' | 'info' | 'warning' | 'error'): void {
  this.toastr[type](message, 'Rack Hotel');
}
```

## 🐛 Resolução de Problemas

### Erro de Conexão

```typescript
// Verificar se o backend está executando
// Verificar se a URL está correta
// Verificar se o CORS está configurado

// No backend (Program.cs):
app.UseCors(policy => policy
  .WithOrigins("http://localhost:4200")  // URL do Angular
  .AllowAnyMethod()
  .AllowAnyHeader()
  .AllowCredentials());
```

### Erro de Autenticação

```typescript
// Verificar se o token está sendo enviado corretamente
// Verificar se o [Authorize] está configurado no Hub

// Para testar sem autenticação temporariamente:
// Remover [Authorize] do RackHub.cs
```

### Problemas de Performance

```typescript
// Limitar número de notificações exibidas
public notifications$ = this.rackHubService.notifications$.pipe(
  map(notifications => notifications.slice(0, 50))  // Máximo 50
);

// Implementar paginação
// Implementar filtros por tipo
```

## 📱 Responsividade

O componente é totalmente responsivo e funciona em:
- 📱 Mobile (320px+)
- 📱 Tablet (768px+)
- 💻 Desktop (1200px+)

## 🧪 Testes

### Testar a Conexão

```typescript
// No console do navegador:
window.rackHubService = this.rackHubService;

// Testar envio de mensagem:
window.rackHubService.sendTestMessage('Teste do console');
```

### Simular Notificações

Use os endpoints de teste do backend:

```http
POST /api/racknotification/test-checkin
POST /api/racknotification/test-checkout
POST /api/racknotification/test-apartment-status
```

## 🚀 Deployment

### Build para Produção

```bash
ng build --prod

# Os arquivos gerados estarão em dist/
# Configure o servidor web para servir os arquivos estáticos
```

### Configuração para Diferentes Ambientes

```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  rackHubUrl: 'https://hotel-api.azurewebsites.net/rackhub'
};

// No rack-hub.service.ts
import { environment } from '../../environments/environment';

private hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl(environment.rackHubUrl)  // ← Usar variável de ambiente
  .build();
```

## 📚 Documentação Adicional

- [SignalR para JavaScript](https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client)
- [Angular RxJS](https://angular.io/guide/rx-library)
- [RackHub Backend Documentation](../RACKHUB-DOCUMENTATION.md)

## 🆘 Suporte

Se tiver problemas:

1. Verifique o console do navegador para erros
2. Verifique o Network tab para conexões WebSocket
3. Teste os endpoints do backend diretamente
4. Verifique os logs do backend

## 🎉 Exemplo de Uso Completo

```typescript
// No seu componente
export class MyComponent implements OnInit {
  constructor(private rackHub: RackHubService) {}

  ngOnInit() {
    // Iniciar conexão
    this.rackHub.startConnection();

    // Escutar check-ins
    this.rackHub.checkins$.subscribe(checkin => {
      if (checkin) {
        console.log('Novo check-in:', checkin);
        this.updateDashboard();
      }
    });

    // Entrar no grupo da recepção
    this.rackHub.joinGroup('reception');
  }

  updateDashboard() {
    // Atualizar interface
  }
}
```

---

✨ **Pronto!** Agora você tem um sistema completo de notificações em tempo real para o seu hotel! 🏨
