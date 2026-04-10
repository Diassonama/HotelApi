import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { RackHubService, CheckinData, CheckoutData } from '../services/rack-hub.service';

/**
 * Exemplo simples de como usar o RackHub Service
 * 
 * Este componente mostra uma implementação minimalista para:
 * - Conectar ao RackHub
 * - Escutar notificações básicas
 * - Exibir informações em tempo real
 */
@Component({
  selector: 'app-simple-rack-example',
  template: `
    <div class="simple-rack-container">
      <h2>🏨 Hotel Notifications</h2>
      
      <!-- Status da Conexão -->
      <div class="status" [class.connected]="isConnected">
        <p>Status: {{ isConnected ? '🟢 Conectado' : '🔴 Desconectado' }}</p>
        <button (click)="reconnect()" [disabled]="!canReconnect">
          Reconectar
        </button>
      </div>
      
      <!-- Últimas Notificações -->
      <div class="notifications">
        <h3>📢 Últimas Notificações</h3>
        
        <div *ngIf="lastCheckin" class="notification checkin">
          <strong>✅ Check-in:</strong>
          {{ lastCheckin.nomeHospede }} - 
          Apartamento{{ lastCheckin.apartamentosCodigos.length > 1 ? 's' : '' }}: 
          {{ lastCheckin.apartamentosCodigos.join(', ') }}
          <small>({{ formatTime(lastCheckin.dataCheckin) }})</small>
        </div>
        
        <div *ngIf="lastCheckout" class="notification checkout">
          <strong>🚪 Check-out:</strong>
          {{ lastCheckout.nomeHospede }} - 
          Apartamento{{ lastCheckout.apartamentosCodigos.length > 1 ? 's' : '' }}: 
          {{ lastCheckout.apartamentosCodigos.join(', ') }}
          <small>({{ formatTime(lastCheckout.dataCheckout) }})</small>
        </div>
      </div>
      
      <!-- Contador de Eventos -->
      <div class="counters">
        <div class="counter">
          <span class="number">{{ checkinCount }}</span>
          <span class="label">Check-ins Hoje</span>
        </div>
        <div class="counter">
          <span class="number">{{ checkoutCount }}</span>
          <span class="label">Check-outs Hoje</span>
        </div>
      </div>
      
      <!-- Teste -->
      <div class="test-section">
        <button (click)="sendTest()" [disabled]="!isConnected">
          🧪 Enviar Teste
        </button>
      </div>
    </div>
  `,
  styles: [`
    .simple-rack-container {
      padding: 20px;
      max-width: 600px;
      margin: 0 auto;
      font-family: Arial, sans-serif;
    }

    .status {
      background: #f8d7da;
      padding: 15px;
      border-radius: 8px;
      margin-bottom: 20px;
      border: 1px solid #f5c6cb;
    }

    .status.connected {
      background: #d4edda;
      border-color: #c3e6cb;
    }

    .status p {
      margin: 0 0 10px 0;
      font-weight: bold;
    }

    .status button {
      background: #007bff;
      color: white;
      border: none;
      padding: 8px 16px;
      border-radius: 4px;
      cursor: pointer;
    }

    .status button:disabled {
      background: #6c757d;
      cursor: not-allowed;
    }

    .notifications {
      background: #f8f9fa;
      padding: 15px;
      border-radius: 8px;
      margin-bottom: 20px;
      border: 1px solid #dee2e6;
    }

    .notifications h3 {
      margin: 0 0 15px 0;
      color: #333;
    }

    .notification {
      background: white;
      padding: 12px;
      border-radius: 6px;
      margin-bottom: 10px;
      border-left: 4px solid #007bff;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }

    .notification.checkin {
      border-left-color: #28a745;
    }

    .notification.checkout {
      border-left-color: #dc3545;
    }

    .notification small {
      color: #666;
      font-size: 0.85em;
      display: block;
      margin-top: 5px;
    }

    .counters {
      display: flex;
      gap: 20px;
      margin-bottom: 20px;
    }

    .counter {
      background: white;
      padding: 20px;
      border-radius: 8px;
      text-align: center;
      flex: 1;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      border-left: 4px solid #007bff;
    }

    .counter .number {
      display: block;
      font-size: 2em;
      font-weight: bold;
      color: #333;
    }

    .counter .label {
      display: block;
      color: #666;
      font-size: 0.9em;
      margin-top: 5px;
    }

    .test-section {
      text-align: center;
    }

    .test-section button {
      background: #28a745;
      color: white;
      border: none;
      padding: 12px 24px;
      border-radius: 6px;
      cursor: pointer;
      font-size: 16px;
    }

    .test-section button:disabled {
      background: #6c757d;
      cursor: not-allowed;
    }
  `]
})
export class SimpleRackExampleComponent implements OnInit, OnDestroy {
  
  // Estado da conexão
  isConnected = false;
  canReconnect = true;
  
  // Últimos eventos
  lastCheckin: CheckinData | null = null;
  lastCheckout: CheckoutData | null = null;
  
  // Contadores
  checkinCount = 0;
  checkoutCount = 0;
  
  // Subscriptions
  private subscriptions: Subscription[] = [];

  constructor(private rackHubService: RackHubService) {}

  async ngOnInit(): Promise<void> {
    // Iniciar conexão
    await this.startConnection();
    
    // Configurar listeners
    this.setupListeners();
  }

  ngOnDestroy(): void {
    // Limpar subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
    
    // Parar conexão
    this.rackHubService.stopConnection();
  }

  /**
   * Inicia a conexão com o RackHub
   */
  private async startConnection(): Promise<void> {
    try {
      await this.rackHubService.startConnection();
      console.log('✅ Conectado ao RackHub');
    } catch (error) {
      console.error('❌ Erro ao conectar:', error);
    }
  }

  /**
   * Configura os listeners dos eventos
   */
  private setupListeners(): void {
    
    // Status da conexão
    const connectionSub = this.rackHubService.connectionStatus$.subscribe(status => {
      this.isConnected = status.isConnected;
      this.canReconnect = !status.isConnected;
    });
    this.subscriptions.push(connectionSub);

    // Check-ins
    const checkinSub = this.rackHubService.checkins$.subscribe(checkin => {
      if (checkin) {
        this.lastCheckin = checkin;
        this.checkinCount++;
        console.log('🏨 Novo check-in:', checkin);
      }
    });
    this.subscriptions.push(checkinSub);

    // Check-outs
    const checkoutSub = this.rackHubService.checkouts$.subscribe(checkout => {
      if (checkout) {
        this.lastCheckout = checkout;
        this.checkoutCount++;
        console.log('🚪 Novo check-out:', checkout);
      }
    });
    this.subscriptions.push(checkoutSub);
  }

  /**
   * Reconecta ao RackHub
   */
  async reconnect(): Promise<void> {
    this.canReconnect = false;
    
    try {
      await this.rackHubService.stopConnection();
      await this.rackHubService.startConnection();
      console.log('🔄 Reconectado com sucesso');
    } catch (error) {
      console.error('❌ Erro ao reconectar:', error);
    } finally {
      this.canReconnect = true;
    }
  }

  /**
   * Envia uma mensagem de teste
   */
  async sendTest(): Promise<void> {
    try {
      const message = `Teste do componente simples - ${new Date().toLocaleTimeString()}`;
      await this.rackHubService.sendTestMessage(message);
      console.log('🧪 Teste enviado:', message);
    } catch (error) {
      console.error('❌ Erro ao enviar teste:', error);
    }
  }

  /**
   * Formata hora para exibição
   */
  formatTime(date: Date): string {
    return new Date(date).toLocaleTimeString('pt-PT', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}

// ==========================================
// EXEMPLO DE USO AINDA MAIS SIMPLES
// ==========================================

/**
 * Exemplo super simples - apenas log no console
 */
@Component({
  selector: 'app-minimal-rack-example',
  template: `
    <div>
      <h3>🏨 Minimal Rack Example</h3>
      <p>Status: {{ status }}</p>
      <p>Check console for notifications...</p>
      <button (click)="toggle()">{{ isListening ? 'Stop' : 'Start' }} Listening</button>
    </div>
  `
})
export class MinimalRackExampleComponent implements OnInit, OnDestroy {
  
  status = 'Disconnected';
  isListening = false;
  private subscriptions: Subscription[] = [];

  constructor(private rackHub: RackHubService) {}

  async ngOnInit(): Promise<void> {
    await this.toggle();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  async toggle(): Promise<void> {
    if (this.isListening) {
      // Parar
      this.subscriptions.forEach(sub => sub.unsubscribe());
      this.subscriptions = [];
      await this.rackHub.stopConnection();
      this.status = 'Disconnected';
      this.isListening = false;
    } else {
      // Iniciar
      await this.rackHub.startConnection();
      
      // Listeners simples
      this.subscriptions.push(
        this.rackHub.connectionStatus$.subscribe(s => this.status = s.status),
        this.rackHub.checkins$.subscribe(c => c && console.log('✅ Check-in:', c)),
        this.rackHub.checkouts$.subscribe(c => c && console.log('🚪 Check-out:', c)),
        this.rackHub.apartmentStatus$.subscribe(a => a && console.log('🏨 Status:', a))
      );
      
      this.isListening = true;
    }
  }
}

// ==========================================
// EXEMPLO USANDO ASYNC PIPE
// ==========================================

/**
 * Exemplo usando Angular async pipe
 */
@Component({
  selector: 'app-async-rack-example',
  template: `
    <div *ngIf="connectionStatus$ | async as status">
      <h3>🏨 Async Example</h3>
      <p>Status: {{ status.status }}</p>
      
      <div *ngIf="checkins$ | async as checkin">
        <h4>Last Check-in:</h4>
        <p>{{ checkin.nomeHospede }} - {{ checkin.apartamentosCodigos.join(', ') }}</p>
      </div>
      
      <div *ngIf="checkouts$ | async as checkout">
        <h4>Last Check-out:</h4>
        <p>{{ checkout.nomeHospede }} - {{ checkout.apartamentosCodigos.join(', ') }}</p>
      </div>
    </div>
  `
})
export class AsyncRackExampleComponent implements OnInit {
  
  connectionStatus$ = this.rackHub.connectionStatus$;
  checkins$ = this.rackHub.checkins$;
  checkouts$ = this.rackHub.checkouts$;

  constructor(private rackHub: RackHubService) {}

  async ngOnInit(): Promise<void> {
    await this.rackHub.startConnection();
  }
}
