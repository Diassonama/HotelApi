# Guia de Integração Angular + SignalR para Hotel

## 1. Instalação das Dependências

```bash
# Instalar SignalR Client para Angular
npm install @microsoft/signalr

# Instalar tipos TypeScript (opcional, mas recomendado)
npm install --save-dev @types/signalr
```

## 2. Configuração no app.module.ts (Para módulos tradicionais)

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { RackModule } from './rack/rack.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RackModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## 3. Configuração no main.ts (Para standalone components - Angular 14+)

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { RackSignalRService } from './app/rack/rack-signalr.service';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    RackSignalRService
  ]
});
```

## 4. Uso no Template Principal

### Opção A: Componente Simples (rack.component.ts)
```html
<app-rack></app-rack>
```

### Opção B: Componente Completo (hotel-rack.component.ts)
```html
<app-hotel-rack></app-hotel-rack>
```

## 5. Estrutura de Arquivos Recomendada

```
src/app/
├── rack/
│   ├── rack-signalr.service.ts      # Serviço base do SignalR
│   ├── rack-data.service.ts         # Serviço de dados integrado
│   ├── rack.component.ts            # Componente simples
│   ├── hotel-rack.component.ts      # Componente completo
│   └── rack.module.ts               # Módulo (opcional)
└── shared/
    └── interfaces/
        └── apartamento.interface.ts # Interfaces compartilhadas
```

## 6. Configuração do Backend (ASP.NET Core)

Certifique-se de que o backend está configurado corretamente:

### Program.cs
```csharp
builder.Services.AddSignalR();
builder.Services.AddScoped<IRackNotifier, RackNotifier>();

app.MapHub<RackHub>("/rackHub");
```

### Configuração CORS (se necessário)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200") // URL do Angular
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

app.UseCors("AllowAngular");
```

## 7. Uso Prático

### No seu app.component.html:
```html
<router-outlet></router-outlet>
<!-- OU -->
<app-hotel-rack></app-hotel-rack>
```

### No seu routing (se usar):
```typescript
const routes: Routes = [
  { path: 'rack', component: HotelRackComponent },
  { path: '', redirectTo: '/rack', pathMatch: 'full' }
];
```

## 8. Personalização

### Estilos CSS personalizados:
Você pode sobrescrever os estilos dos componentes criando um arquivo CSS global ou usando ViewEncapsulation.

### Eventos personalizados:
Adicione novos eventos no RackSignalRService conforme necessário:

```typescript
// No serviço
private eventoCustomizado = new BehaviorSubject<any>(null);
get eventoCustomizado$(): Observable<any> {
  return this.eventoCustomizado.asObservable();
}

// Na configuração de eventos
this.connection.on('EventoCustomizado', (data) => {
  this.eventoCustomizado.next(data);
});
```

## 9. Debugging

### Console do Browser:
- Abra F12 → Console
- Verifique mensagens de conexão do SignalR
- Monitore eventos em tempo real

### Network Tab:
- Verifique se a conexão WebSocket está estabelecida
- URL deve ser: `ws://localhost:porta/rackHub`

## 10. Deploy

### Desenvolvimento:
```bash
ng serve
# SignalR conectará em http://localhost:porta/rackHub
```

### Produção:
- Configure a URL base do SignalR para o ambiente de produção
- Use environment.ts para URLs diferentes por ambiente

```typescript
// environment.ts
export const environment = {
  production: false,
  signalRUrl: 'http://localhost:5000/rackHub'
};

// environment.prod.ts
export const environment = {
  production: true,
  signalRUrl: 'https://seu-servidor.com/rackHub'
};
```
