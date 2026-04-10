import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { RackSignalRService, ApartamentoStatus, RackEventData } from './rack-signalr.service';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-rack',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="rack-container">
      <h1>Rack Hotel - Tempo Real</h1>
      
      <!-- Status da conexão -->
      <div class="connection-status" [ngClass]="getConnectionStatusClass()">
        <span>{{ getConnectionStatusText() }}</span>
        <button *ngIf="!isConnected" (click)="reconectar()" class="btn-reconnect">
          Reconectar
        </button>
      </div>

      <!-- Notificações -->
      <div class="notifications">
        <div *ngFor="let notification of notifications" 
             class="notification" 
             [class.fade-out]="notification.fadeOut">
          {{ notification.message }}
        </div>
      </div>

      <!-- Grid de apartamentos -->
      <div class="apartamentos-grid">
        <div *ngFor="let apartamento of apartamentos" 
             class="apartamento" 
             [ngClass]="apartamento.status.toLowerCase()">
          <h3>{{ apartamento.numero }}</h3>
          <div class="status">{{ apartamento.status }}</div>
          <div *ngIf="apartamento.hospedeNome" class="hospede">
            {{ apartamento.hospedeNome }}
          </div>
          <div *ngIf="apartamento.dataCheckIn" class="check-in">
            Check-in: {{ apartamento.dataCheckIn | date:'dd/MM/yyyy HH:mm' }}
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .rack-container {
      padding: 20px;
    }

    .connection-status {
      margin-bottom: 20px;
      padding: 10px;
      border-radius: 5px;
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .connection-status.connected {
      background-color: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    .connection-status.disconnected {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .connection-status.reconnecting {
      background-color: #fff3cd;
      color: #856404;
      border: 1px solid #ffeaa7;
    }

    .btn-reconnect {
      background: #007bff;
      color: white;
      border: none;
      padding: 5px 10px;
      border-radius: 3px;
      cursor: pointer;
    }

    .notifications {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 1000;
      max-width: 300px;
    }

    .notification {
      background: #007bff;
      color: white;
      padding: 10px 15px;
      margin: 5px 0;
      border-radius: 5px;
      animation: slideIn 0.3s ease;
      transition: opacity 0.3s ease;
    }

    .notification.fade-out {
      opacity: 0;
    }

    @keyframes slideIn {
      from { transform: translateX(100%); }
      to { transform: translateX(0); }
    }

    .apartamentos-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 15px;
      margin-top: 20px;
    }

    .apartamento {
      border: 2px solid #ccc;
      border-radius: 8px;
      padding: 15px;
      text-align: center;
      transition: all 0.3s ease;
      position: relative;
    }

    .apartamento.livre {
      background-color: #d4edda;
      border-color: #28a745;
    }

    .apartamento.ocupado {
      background-color: #f8d7da;
      border-color: #dc3545;
    }

    .apartamento.reservado {
      background-color: #fff3cd;
      border-color: #ffc107;
    }

    .apartamento.manutencao {
      background-color: #e2e3e5;
      border-color: #6c757d;
    }

    .status {
      font-weight: bold;
      font-size: 14px;
      margin: 10px 0;
    }

    .hospede {
      font-size: 12px;
      color: #666;
      margin-top: 5px;
    }

    .check-in {
      font-size: 11px;
      color: #888;
      margin-top: 5px;
    }

    h3 {
      margin: 0 0 10px 0;
      font-size: 18px;
    }
  `]
})
export class RackComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  apartamentos: ApartamentoStatus[] = [
    { apartamentoId: '101', numero: 'Apto 101', status: 'Livre' },
    { apartamentoId: '102', numero: 'Apto 102', status: 'Ocupado', hospedeNome: 'João Silva' },
    { apartamentoId: '103', numero: 'Apto 103', status: 'Reservado' },
    { apartamentoId: '104', numero: 'Apto 104', status: 'Livre' },
    { apartamentoId: '105', numero: 'Apto 105', status: 'Manutencao' }
  ];

  notifications: Array<{message: string, fadeOut: boolean}> = [];
  connectionState: signalR.HubConnectionState = signalR.HubConnectionState.Disconnected;

  constructor(
    private rackService: RackSignalRService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.subscribeToSignalREvents();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private subscribeToSignalREvents(): void {
    // Monitorar estado da conexão
    this.rackService.connectionState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => {
        this.connectionState = state;
        this.cdr.detectChanges();
      });

    // Rack atualizado
    this.rackService.rackAtualizado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(updated => {
        if (updated) {
          this.showNotification('Rack atualizado!');
          // Aqui você poderia recarregar os dados via API
        }
      });

    // Status de apartamento alterado
    this.rackService.statusApartamentoAlterado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.updateApartamento(data.apartamentoId, data.novoStatus!);
          this.showNotification(`Apartamento ${data.apartamentoId}: ${data.novoStatus}`);
        }
      });

    // Nova reserva
    this.rackService.novaReserva$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.updateApartamento(data.apartamentoId, 'Reservado');
          this.showNotification(`Nova reserva: Apartamento ${data.apartamentoId}`);
        }
      });

    // Check-in realizado
    this.rackService.checkInRealizado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.updateApartamentoCheckIn(data);
          this.showNotification(`Check-in: Apartamento ${data.apartamentoId}`);
        }
      });

    // Check-out realizado
    this.rackService.checkOutRealizado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.updateApartamentoCheckOut(data);
          this.showNotification(`Check-out: Apartamento ${data.apartamentoId}`);
        }
      });
  }

  private updateApartamento(apartamentoId: string, novoStatus: string): void {
    const apartamento = this.apartamentos.find(apt => apt.apartamentoId === apartamentoId);
    if (apartamento) {
      apartamento.status = novoStatus as any;
      this.cdr.detectChanges();
    }
  }

  private updateApartamentoCheckIn(data: RackEventData): void {
    const apartamento = this.apartamentos.find(apt => apt.apartamentoId === data.apartamentoId);
    if (apartamento) {
      apartamento.status = 'Ocupado';
      apartamento.hospedeNome = data.hospedeNome;
      apartamento.dataCheckIn = data.dataCheckIn;
      this.cdr.detectChanges();
    }
  }

  private updateApartamentoCheckOut(data: RackEventData): void {
    const apartamento = this.apartamentos.find(apt => apt.apartamentoId === data.apartamentoId);
    if (apartamento) {
      apartamento.status = 'Livre';
      apartamento.hospedeNome = undefined;
      apartamento.dataCheckIn = undefined;
      apartamento.dataCheckOut = data.dataCheckOut;
      this.cdr.detectChanges();
    }
  }

  private showNotification(message: string): void {
    const notification = { message, fadeOut: false };
    this.notifications.push(notification);

    // Remover após 5 segundos
    setTimeout(() => {
      notification.fadeOut = true;
      setTimeout(() => {
        const index = this.notifications.indexOf(notification);
        if (index > -1) {
          this.notifications.splice(index, 1);
          this.cdr.detectChanges();
        }
      }, 300);
    }, 5000);

    this.cdr.detectChanges();
  }

  get isConnected(): boolean {
    return this.connectionState === signalR.HubConnectionState.Connected;
  }

  getConnectionStatusClass(): string {
    switch (this.connectionState) {
      case signalR.HubConnectionState.Connected:
        return 'connected';
      case signalR.HubConnectionState.Reconnecting:
        return 'reconnecting';
      default:
        return 'disconnected';
    }
  }

  getConnectionStatusText(): string {
    switch (this.connectionState) {
      case signalR.HubConnectionState.Connected:
        return 'Conectado ao servidor';
      case signalR.HubConnectionState.Connecting:
        return 'Conectando...';
      case signalR.HubConnectionState.Reconnecting:
        return 'Reconectando...';
      case signalR.HubConnectionState.Disconnected:
        return 'Desconectado';
      default:
        return 'Status desconhecido';
    }
  }

  async reconectar(): Promise<void> {
    await this.rackService.reconectar();
  }
}
