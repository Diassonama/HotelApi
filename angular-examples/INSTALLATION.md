# 🚀 RackHub Angular - Instalação e Deployment

## 📦 Instalação Rápida (Copy & Paste)

### 1. Instalar Dependências

```bash
# Criar projeto Angular (se não existir)
ng new hotel-rack-frontend
cd hotel-rack-frontend

# Instalar dependências essenciais
npm install @microsoft/signalr rxjs

# Opcional: Para notificações bonitas
npm install ngx-toastr
npm install @angular/animations
```

### 2. Copiar Arquivos

```bash
# Criar estrutura
mkdir -p src/app/rack-hub/{services,components}

# Copiar os arquivos dos exemplos para o seu projeto:
# - rack-hub.service.ts → src/app/rack-hub/services/
# - rack-dashboard.component.* → src/app/rack-hub/components/
# - rack-hub.module.ts → src/app/rack-hub/
```

### 3. Configurar `app.module.ts`

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { RackHubModule } from './rack-hub/rack-hub.module';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    HttpClientModule,
    RackHubModule.forRoot()  // ← Importante!
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### 4. Configurar `app.component.html`

```html
<div class="app-container">
  <header>
    <h1>🏨 Hotel Management System</h1>
  </header>
  
  <main>
    <!-- Dashboard completo -->
    <app-rack-dashboard></app-rack-dashboard>
    
    <!-- OU exemplo simples -->
    <!-- <app-simple-rack-example></app-simple-rack-example> -->
  </main>
</div>
```

### 5. Configurar URL do Backend

No arquivo `src/app/rack-hub/services/rack-hub.service.ts`:

```typescript
// Linha ~47 - Alterar a URL para o seu backend
private hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl('https://SEU-BACKEND.azurewebsites.net/rackhub', {
    accessTokenFactory: () => this.getAuthToken()
  })
  .build();
```

## 🔧 Configuração para Desenvolvimento

### `angular.json` - Proxy para Desenvolvimento

```json
{
  "serve": {
    "builder": "@angular-devkit/build-angular:dev-server",
    "options": {
      "proxyConfig": "proxy.conf.json"
    }
  }
}
```

### `proxy.conf.json`

```json
{
  "/rackhub": {
    "target": "https://localhost:7000",
    "secure": true,
    "changeOrigin": true,
    "logLevel": "debug"
  },
  "/api/*": {
    "target": "https://localhost:7000",
    "secure": true,
    "changeOrigin": true,
    "logLevel": "debug"
  }
}
```

### Executar em Desenvolvimento

```bash
# Terminal 1: Backend (.NET)
cd /path/to/Hotel.Api
dotnet run

# Terminal 2: Frontend (Angular)
cd /path/to/hotel-rack-frontend
ng serve

# Abrir: http://localhost:4200
```

## 🌍 Configuração para Ambientes

### `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  rackHubUrl: 'https://localhost:7000/rackhub',
  apiUrl: 'https://localhost:7000/api'
};
```

### `src/environments/environment.prod.ts`

```typescript
export const environment = {
  production: true,
  rackHubUrl: 'https://hotel-api.azurewebsites.net/rackhub',
  apiUrl: 'https://hotel-api.azurewebsites.net/api'
};
```

### Usar no Serviço

```typescript
import { environment } from '../../environments/environment';

private hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl(environment.rackHubUrl)
  .build();
```

## 🔐 Configuração de Autenticação

### Método 1: Token Bearer

```typescript
// No rack-hub.service.ts
private getAuthToken(): string {
  // Obter do localStorage/sessionStorage
  const token = localStorage.getItem('access_token');
  return token || '';
}

// Ou obter de um serviço de autenticação
constructor(private authService: AuthService) {}

private getAuthToken(): string {
  return this.authService.getToken();
}
```

### Método 2: Cookies de Autenticação

```typescript
// Se usar cookies, configure no backend para permitir
private hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl(environment.rackHubUrl, {
    withCredentials: true  // Enviar cookies
  })
  .build();
```

## 🚀 Build e Deployment

### Build para Produção

```bash
# Build otimizado
ng build --configuration=production

# Verificar tamanho dos bundles
npx webpack-bundle-analyzer dist/hotel-rack-frontend/main.*.js
```

### Deploy no Azure Static Web Apps

```bash
# Instalar Azure CLI
npm install -g @azure/static-web-apps-cli

# Deploy
swa deploy --app-location "dist/hotel-rack-frontend" --api-location "" --deployment-token "YOUR_TOKEN"
```

### Deploy no Vercel

```bash
# Instalar Vercel CLI
npm install -g vercel

# Deploy
vercel --prod
```

### Deploy no Netlify

```bash
# Build
ng build --prod

# Upload da pasta dist/ para Netlify
# Ou usar Netlify CLI:
npx netlify-cli deploy --prod --dir=dist/hotel-rack-frontend
```

### Deploy no GitHub Pages

```bash
# Instalar angular-cli-ghpages
npm install -g angular-cli-ghpages

# Deploy
ng build --prod --base-href "/NOME_DO_REPO/"
npx angular-cli-ghpages --dir=dist/hotel-rack-frontend
```

## 🔧 Configuração de CORS no Backend

No seu `Program.cs` (.NET):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",                    // Desenvolvimento
                "https://hotel-frontend.azurewebsites.net", // Produção
                "https://meusite.com"                       // Domínio personalizado
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Para SignalR
    });
});

var app = builder.Build();

// Usar CORS
app.UseCors();

// SignalR Hub
app.MapHub<RackHub>("/rackhub");
```

## 📊 Monitoramento e Performance

### Application Insights (Azure)

```typescript
// npm install @microsoft/applicationinsights-web

import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const appInsights = new ApplicationInsights({
  config: {
    instrumentationKey: 'YOUR_INSTRUMENTATION_KEY'
  }
});

appInsights.loadAppInsights();

// No rack-hub.service.ts
private logEvent(name: string, properties?: any): void {
  appInsights.trackEvent({ name, properties });
}
```

### Performance Monitoring

```typescript
// Monitorar tempo de conexão
const startTime = performance.now();
await this.rackHubService.startConnection();
const connectionTime = performance.now() - startTime;
console.log(`Conexão estabelecida em ${connectionTime}ms`);

// Monitorar número de notificações
this.rackHubService.notifications$.subscribe(notifications => {
  console.log(`Total de notificações: ${notifications.length}`);
});
```

## 🧪 Testes

### Teste Unitário - Serviço

```typescript
// rack-hub.service.spec.ts
import { TestBed } from '@angular/core/testing';
import { RackHubService } from './rack-hub.service';

describe('RackHubService', () => {
  let service: RackHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RackHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize connection status as disconnected', () => {
    service.connectionStatus$.subscribe(status => {
      expect(status.isConnected).toBeFalse();
    });
  });
});
```

### Teste E2E - Dashboard

```typescript
// rack-dashboard.component.e2e.spec.ts
import { browser, by, element } from 'protractor';

describe('Rack Dashboard', () => {
  beforeEach(() => {
    browser.get('/rack');
  });

  it('should display connection status', () => {
    const status = element(by.css('.connection-status'));
    expect(status.isPresent()).toBeTruthy();
  });

  it('should show apartment grid', () => {
    const apartments = element.all(by.css('.apartment-card'));
    expect(apartments.count()).toBeGreaterThan(0);
  });
});
```

## 🐛 Troubleshooting

### Problema: Conexão WebSocket Falha

```typescript
// Verificar se WebSocket está disponível
if (!WebSocket) {
  console.error('WebSocket não suportado neste navegador');
}

// Verificar no Network tab se a conexão é estabelecida
// URL deve mostrar: ws://localhost:4200/rackhub
```

### Problema: CORS Error

```bash
# Verificar headers CORS
curl -H "Origin: http://localhost:4200" \
     -H "Access-Control-Request-Method: GET" \
     -H "Access-Control-Request-Headers: X-Requested-With" \
     -X OPTIONS \
     https://sua-api.com/rackhub
```

### Problema: Notificações Não Chegam

```typescript
// Verificar se está no grupo correto
await this.rackHubService.joinGroup('reception');

// Verificar logs do backend
// Verificar se o Hub está enviando para o grupo correto
```

## 📱 PWA (Progressive Web App)

```bash
# Adicionar PWA
ng add @angular/pwa

# Configurar notificações push (opcional)
npm install web-push
```

## 🎯 Checklist de Deploy

- [ ] ✅ Backend funcionando
- [ ] ✅ CORS configurado
- [ ] ✅ URLs de produção configuradas
- [ ] ✅ Autenticação funcional
- [ ] ✅ Build sem erros
- [ ] ✅ Testes passando
- [ ] ✅ Performance otimizada
- [ ] ✅ Monitoramento configurado

---

🎉 **Pronto para produção!** 🏨
