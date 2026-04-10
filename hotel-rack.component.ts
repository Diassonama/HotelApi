import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { RackDataService } from './rack-data.service';
import { RackSignalRService, ApartamentoStatus } from './rack-signalr.service';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-hotel-rack',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="hotel-rack">
      <div class="header">
        <h1>Rack Hotel - Sistema Integrado</h1>
        
        <!-- Status da conexão SignalR -->
        <div class="connection-indicator" [ngClass]="getConnectionClass()">
          <span class="status-dot"></span>
          {{ getConnectionText() }}
        </div>
        
        <!-- Controles -->
        <div class="controls">
          <button (click)="recarregarRack()" [disabled]="loading">
            {{ loading ? 'Carregando...' : 'Recarregar Rack' }}
          </button>
        </div>
      </div>

      <!-- Filtros -->
      <div class="filters">
        <label>
          Filtrar por status:
          <select [(ngModel)]="filtroStatus" (change)="aplicarFiltro()">
            <option value="">Todos</option>
            <option value="Livre">Livre</option>
            <option value="Ocupado">Ocupado</option>
            <option value="Reservado">Reservado</option>
            <option value="Manutencao">Manutenção</option>
          </select>
        </label>
        
        <span class="apartamento-count">
          {{ apartamentosFiltrados.length }} de {{ apartamentos.length }} apartamentos
        </span>
      </div>

      <!-- Grid de apartamentos -->
      <div class="apartamentos-grid" *ngIf="!loading">
        <div *ngFor="let apartamento of apartamentosFiltrados; trackBy: trackByApartamento" 
             class="apartamento-card" 
             [ngClass]="apartamento.status.toLowerCase()">
          
          <div class="apartamento-header">
            <h3>{{ apartamento.numero }}</h3>
            <span class="status-badge">{{ apartamento.status }}</span>
          </div>
          
          <div class="apartamento-details">
            <div *ngIf="apartamento.hospedeNome" class="hospede-info">
              <strong>Hóspede:</strong> {{ apartamento.hospedeNome }}
            </div>
            
            <div *ngIf="apartamento.dataCheckIn" class="check-dates">
              <div><strong>Check-in:</strong> {{ apartamento.dataCheckIn | date:'dd/MM/yyyy HH:mm' }}</div>
            </div>
            
            <div *ngIf="apartamento.dataCheckOut" class="check-dates">
              <div><strong>Check-out:</strong> {{ apartamento.dataCheckOut | date:'dd/MM/yyyy HH:mm' }}</div>
            </div>
          </div>
          
          <!-- Ações rápidas -->
          <div class="apartamento-actions">
            <button *ngIf="apartamento.status === 'Reservado'" 
                    (click)="realizarCheckIn(apartamento)" 
                    class="btn-action btn-checkin">
              Check-in
            </button>
            
            <button *ngIf="apartamento.status === 'Ocupado'" 
                    (click)="realizarCheckOut(apartamento)" 
                    class="btn-action btn-checkout">
              Check-out
            </button>
            
            <button *ngIf="apartamento.status === 'Livre'" 
                    (click)="marcarManutencao(apartamento)" 
                    class="btn-action btn-manutencao">
              Manutenção
            </button>
            
            <button *ngIf="apartamento.status === 'Manutencao'" 
                    (click)="liberarApartamento(apartamento)" 
                    class="btn-action btn-liberar">
              Liberar
            </button>
          </div>
        </div>
      </div>

      <!-- Loading -->
      <div *ngIf="loading" class="loading">
        <div class="spinner"></div>
        <p>Carregando apartamentos...</p>
      </div>

      <!-- Notificações -->
      <div class="notifications">
        <div *ngFor="let notification of notifications; trackBy: trackByNotification" 
             class="notification" 
             [ngClass]="notification.type"
             [class.fade-out]="notification.fadeOut">
          {{ notification.message }}
        </div>
      </div>
    </div>
  `,
  styles: [`
    .hotel-rack {
      padding: 20px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      flex-wrap: wrap;
      gap: 15px;
    }

    .connection-indicator {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 8px 12px;
      border-radius: 20px;
      font-size: 14px;
      font-weight: 500;
    }

    .connection-indicator.connected {
      background: #d4edda;
      color: #155724;
    }

    .connection-indicator.disconnected {
      background: #f8d7da;
      color: #721c24;
    }

    .connection-indicator.reconnecting {
      background: #fff3cd;
      color: #856404;
    }

    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: currentColor;
    }

    .controls button {
      background: #007bff;
      color: white;
      border: none;
      padding: 10px 20px;
      border-radius: 5px;
      cursor: pointer;
      font-size: 14px;
    }

    .controls button:disabled {
      background: #6c757d;
      cursor: not-allowed;
    }

    .filters {
      display: flex;
      align-items: center;
      gap: 20px;
      margin-bottom: 20px;
      padding: 15px;
      background: #f8f9fa;
      border-radius: 8px;
    }

    .filters select {
      padding: 5px 10px;
      border: 1px solid #ddd;
      border-radius: 4px;
      margin-left: 8px;
    }

    .apartamento-count {
      color: #666;
      font-size: 14px;
    }

    .apartamentos-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 20px;
    }

    .apartamento-card {
      border: 2px solid #ddd;
      border-radius: 12px;
      padding: 20px;
      background: white;
      transition: all 0.3s ease;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .apartamento-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }

    .apartamento-card.livre {
      border-color: #28a745;
      background: #f8fff9;
    }

    .apartamento-card.ocupado {
      border-color: #dc3545;
      background: #fff8f8;
    }

    .apartamento-card.reservado {
      border-color: #ffc107;
      background: #fffef8;
    }

    .apartamento-card.manutencao {
      border-color: #6c757d;
      background: #f8f9fa;
    }

    .apartamento-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 15px;
    }

    .apartamento-header h3 {
      margin: 0;
      font-size: 18px;
      color: #333;
    }

    .status-badge {
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: bold;
      text-transform: uppercase;
    }

    .livre .status-badge {
      background: #28a745;
      color: white;
    }

    .ocupado .status-badge {
      background: #dc3545;
      color: white;
    }

    .reservado .status-badge {
      background: #ffc107;
      color: #333;
    }

    .manutencao .status-badge {
      background: #6c757d;
      color: white;
    }

    .apartamento-details {
      margin-bottom: 15px;
      font-size: 14px;
      line-height: 1.5;
    }

    .hospede-info {
      color: #333;
      margin-bottom: 8px;
    }

    .check-dates {
      color: #666;
      font-size: 13px;
    }

    .apartamento-actions {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }

    .btn-action {
      padding: 6px 12px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 12px;
      font-weight: 500;
      text-transform: uppercase;
      transition: background 0.2s;
    }

    .btn-checkin {
      background: #28a745;
      color: white;
    }

    .btn-checkout {
      background: #dc3545;
      color: white;
    }

    .btn-manutencao {
      background: #6c757d;
      color: white;
    }

    .btn-liberar {
      background: #17a2b8;
      color: white;
    }

    .btn-action:hover {
      opacity: 0.8;
    }

    .loading {
      text-align: center;
      padding: 60px 20px;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #007bff;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 20px;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .notifications {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 1000;
      max-width: 350px;
    }

    .notification {
      padding: 12px 16px;
      margin: 8px 0;
      border-radius: 6px;
      color: white;
      font-weight: 500;
      animation: slideIn 0.3s ease;
      transition: opacity 0.3s ease;
    }

    .notification.success {
      background: #28a745;
    }

    .notification.info {
      background: #007bff;
    }

    .notification.warning {
      background: #ffc107;
      color: #333;
    }

    .notification.error {
      background: #dc3545;
    }

    .notification.fade-out {
      opacity: 0;
    }

    @keyframes slideIn {
      from { transform: translateX(100%); }
      to { transform: translateX(0); }
    }

    @media (max-width: 768px) {
      .apartamentos-grid {
        grid-template-columns: 1fr;
      }
      
      .header {
        flex-direction: column;
        align-items: stretch;
      }
      
      .filters {
        flex-direction: column;
        align-items: stretch;
        gap: 10px;
      }
    }
  `]
})
export class HotelRackComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  apartamentos: ApartamentoStatus[] = [];
  apartamentosFiltrados: ApartamentoStatus[] = [];
  loading = false;
  filtroStatus = '';
  
  notifications: Array<{
    id: number;
    message: string;
    type: 'success' | 'info' | 'warning' | 'error';
    fadeOut: boolean;
  }> = [];
  
  private notificationId = 0;
  connectionState: signalR.HubConnectionState = signalR.HubConnectionState.Disconnected;

  constructor(
    private rackDataService: RackDataService,
    private signalRService: RackSignalRService
  ) {}

  ngOnInit(): void {
    this.carregarDadosIniciais();
    this.subscribeToUpdates();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private carregarDadosIniciais(): void {
    this.rackDataService.carregarRack()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => this.showNotification('Dados carregados com sucesso', 'success'),
        error: (error) => {
          console.error('Erro ao carregar dados:', error);
          this.showNotification('Erro ao carregar dados do rack', 'error');
        }
      });
  }

  private subscribeToUpdates(): void {
    // Apartamentos
    this.rackDataService.apartamentos$
      .pipe(takeUntil(this.destroy$))
      .subscribe(apartamentos => {
        this.apartamentos = apartamentos;
        this.aplicarFiltro();
      });

    // Loading
    this.rackDataService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe(loading => this.loading = loading);

    // Connection state
    this.signalRService.connectionState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => this.connectionState = state);

    // SignalR events para notificações
    this.signalRService.novaReserva$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.showNotification(`Nova reserva: Apartamento ${data.apartamentoId}`, 'info');
        }
      });

    this.signalRService.checkInRealizado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.showNotification(`Check-in realizado: Apartamento ${data.apartamentoId}`, 'success');
        }
      });

    this.signalRService.checkOutRealizado$
      .pipe(takeUntil(this.destroy$))
      .subscribe(data => {
        if (data) {
          this.showNotification(`Check-out realizado: Apartamento ${data.apartamentoId}`, 'success');
        }
      });
  }

  aplicarFiltro(): void {
    if (!this.filtroStatus) {
      this.apartamentosFiltrados = [...this.apartamentos];
    } else {
      this.apartamentosFiltrados = this.apartamentos.filter(
        apt => apt.status === this.filtroStatus
      );
    }
  }

  recarregarRack(): void {
    this.carregarDadosIniciais();
  }

  // Ações dos apartamentos
  realizarCheckIn(apartamento: ApartamentoStatus): void {
    const hospedeNome = prompt('Nome do hóspede:');
    if (hospedeNome) {
      this.rackDataService.realizarCheckIn(apartamento.apartamentoId, hospedeNome)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => this.showNotification('Check-in realizado com sucesso', 'success'),
          error: () => this.showNotification('Erro ao realizar check-in', 'error')
        });
    }
  }

  realizarCheckOut(apartamento: ApartamentoStatus): void {
    if (confirm(`Confirma o check-out do apartamento ${apartamento.numero}?`)) {
      this.rackDataService.realizarCheckOut(apartamento.apartamentoId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => this.showNotification('Check-out realizado com sucesso', 'success'),
          error: () => this.showNotification('Erro ao realizar check-out', 'error')
        });
    }
  }

  marcarManutencao(apartamento: ApartamentoStatus): void {
    if (confirm(`Marcar apartamento ${apartamento.numero} em manutenção?`)) {
      this.rackDataService.alterarStatusApartamento(apartamento.apartamentoId, 'Manutencao')
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => this.showNotification('Apartamento marcado para manutenção', 'warning'),
          error: () => this.showNotification('Erro ao alterar status', 'error')
        });
    }
  }

  liberarApartamento(apartamento: ApartamentoStatus): void {
    if (confirm(`Liberar apartamento ${apartamento.numero}?`)) {
      this.rackDataService.alterarStatusApartamento(apartamento.apartamentoId, 'Livre')
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => this.showNotification('Apartamento liberado', 'success'),
          error: () => this.showNotification('Erro ao alterar status', 'error')
        });
    }
  }

  // Utility methods
  trackByApartamento(index: number, apartamento: ApartamentoStatus): string {
    return apartamento.apartamentoId;
  }

  trackByNotification(index: number, notification: any): number {
    return notification.id;
  }

  getConnectionClass(): string {
    switch (this.connectionState) {
      case signalR.HubConnectionState.Connected:
        return 'connected';
      case signalR.HubConnectionState.Reconnecting:
        return 'reconnecting';
      default:
        return 'disconnected';
    }
  }

  getConnectionText(): string {
    switch (this.connectionState) {
      case signalR.HubConnectionState.Connected:
        return 'Conectado';
      case signalR.HubConnectionState.Connecting:
        return 'Conectando...';
      case signalR.HubConnectionState.Reconnecting:
        return 'Reconectando...';
      case signalR.HubConnectionState.Disconnected:
        return 'Desconectado';
      default:
        return 'Indefinido';
    }
  }

  private showNotification(message: string, type: 'success' | 'info' | 'warning' | 'error'): void {
    const notification = {
      id: ++this.notificationId,
      message,
      type,
      fadeOut: false
    };
    
    this.notifications.push(notification);

    setTimeout(() => {
      notification.fadeOut = true;
      setTimeout(() => {
        const index = this.notifications.findIndex(n => n.id === notification.id);
        if (index > -1) {
          this.notifications.splice(index, 1);
        }
      }, 300);
    }, 5000);
  }
}
