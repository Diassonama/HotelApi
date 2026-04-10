import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { 
  RackHubService, 
  RackNotification, 
  ConnectionStatus, 
  CheckinData, 
  CheckoutData, 
  ApartmentStatusData 
} from '../services/rack-hub.service';

@Component({
  selector: 'app-rack-dashboard',
  templateUrl: './rack-dashboard.component.html',
  styleUrls: ['./rack-dashboard.component.scss']
})
export class RackDashboardComponent implements OnInit, OnDestroy {

  // Estado da conexão
  connectionStatus: ConnectionStatus = { isConnected: false, status: 'disconnected' };
  
  // Notificações
  notifications: RackNotification[] = [];
  
  // Dados mais recentes
  latestCheckin: CheckinData | null = null;
  latestCheckout: CheckoutData | null = null;
  latestApartmentStatus: ApartmentStatusData | null = null;

  // Estatísticas
  stats = {
    totalCheckins: 0,
    totalCheckouts: 0,
    totalStatusChanges: 0,
    totalNotifications: 0
  };

  // Subscriptions
  private subscriptions: Subscription[] = [];

  // Estado dos apartamentos (simulado)
  apartments = [
    { id: 1, codigo: '101', status: 'Livre', lastUpdate: new Date() },
    { id: 2, codigo: '102', status: 'Ocupado', lastUpdate: new Date() },
    { id: 3, codigo: '103', status: 'Limpeza', lastUpdate: new Date() },
    { id: 4, codigo: '201', status: 'Livre', lastUpdate: new Date() },
    { id: 5, codigo: '202', status: 'Manutencao', lastUpdate: new Date() },
    { id: 6, codigo: '203', status: 'Ocupado', lastUpdate: new Date() }
  ];

  constructor(private rackHubService: RackHubService) {}

  ngOnInit(): void {
    this.initializeSignalR();
    this.subscribeToEvents();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.rackHubService.stopConnection();
  }

  /**
   * Inicializa a conexão SignalR
   */
  private async initializeSignalR(): Promise<void> {
    try {
      await this.rackHubService.startConnection();
    } catch (error) {
      console.error('Erro ao inicializar SignalR:', error);
    }
  }

  /**
   * Subscreve aos eventos do RackHub
   */
  private subscribeToEvents(): void {
    
    // Status da conexão
    const connectionSub = this.rackHubService.connectionStatus$.subscribe(
      (status: ConnectionStatus) => {
        this.connectionStatus = status;
        console.log('Status da conexão:', status);
      }
    );
    this.subscriptions.push(connectionSub);

    // Notificações
    const notificationsSub = this.rackHubService.notifications$.subscribe(
      (notifications: RackNotification[]) => {
        this.notifications = notifications;
        this.stats.totalNotifications = notifications.length;
      }
    );
    this.subscriptions.push(notificationsSub);

    // Check-ins
    const checkinSub = this.rackHubService.checkins$.subscribe(
      (checkin: CheckinData | null) => {
        if (checkin) {
          this.latestCheckin = checkin;
          this.stats.totalCheckins++;
          this.updateApartmentStatus(checkin.apartamentoIds, 'Ocupado');
          this.showToast(`✅ Check-in: ${checkin.apartamentosCodigos.join(', ')}`, 'success');
        }
      }
    );
    this.subscriptions.push(checkinSub);

    // Check-outs
    const checkoutSub = this.rackHubService.checkouts$.subscribe(
      (checkout: CheckoutData | null) => {
        if (checkout) {
          this.latestCheckout = checkout;
          this.stats.totalCheckouts++;
          this.updateApartmentStatus(checkout.apartamentoIds, 'Livre');
          this.showToast(`🚪 Check-out: ${checkout.apartamentosCodigos.join(', ')}`, 'info');
        }
      }
    );
    this.subscriptions.push(checkoutSub);

    // Status dos apartamentos
    const apartmentSub = this.rackHubService.apartmentStatus$.subscribe(
      (status: ApartmentStatusData | null) => {
        if (status) {
          this.latestApartmentStatus = status;
          this.stats.totalStatusChanges++;
          this.updateSingleApartmentStatus(status.apartamentoId, status.situacao);
          this.showToast(`🏨 ${status.codigo}: ${status.situacao}`, 'warning');
        }
      }
    );
    this.subscriptions.push(apartmentSub);
  }

  /**
   * Atualiza o status de múltiplos apartamentos
   */
  private updateApartmentStatus(apartmentIds: number[], newStatus: string): void {
    apartmentIds.forEach(id => {
      const apartment = this.apartments.find(apt => apt.id === id);
      if (apartment) {
        apartment.status = newStatus;
        apartment.lastUpdate = new Date();
      }
    });
  }

  /**
   * Atualiza o status de um apartamento específico
   */
  private updateSingleApartmentStatus(apartmentId: number, newStatus: string): void {
    const apartment = this.apartments.find(apt => apt.id === apartmentId);
    if (apartment) {
      apartment.status = newStatus;
      apartment.lastUpdate = new Date();
    }
  }

  /**
   * Mostra toast notification (implementar conforme sua biblioteca de UI)
   */
  private showToast(message: string, type: 'success' | 'info' | 'warning' | 'error'): void {
    // Exemplo com ngx-toastr ou similar
    console.log(`Toast [${type}]: ${message}`);
    
    // Se você usar ngx-toastr:
    // this.toastr[type](message);
    
    // Se você usar PrimeNG:
    // this.messageService.add({severity: type, summary: 'Rack Hub', detail: message});
  }

  // ==========================================
  // MÉTODOS PÚBLICOS PARA O TEMPLATE
  // ==========================================

  /**
   * Reconecta ao SignalR
   */
  public async reconnect(): Promise<void> {
    try {
      await this.rackHubService.stopConnection();
      await this.rackHubService.startConnection();
    } catch (error) {
      console.error('Erro ao reconectar:', error);
    }
  }

  /**
   * Envia mensagem de teste
   */
  public async sendTestMessage(): Promise<void> {
    const message = `Teste do dashboard - ${new Date().toLocaleTimeString()}`;
    await this.rackHubService.sendTestMessage(message);
  }

  /**
   * Entra em um grupo específico
   */
  public async joinGroup(groupName: string): Promise<void> {
    await this.rackHubService.joinGroup(groupName);
  }

  /**
   * Sai de um grupo
   */
  public async leaveGroup(groupName: string): Promise<void> {
    await this.rackHubService.leaveGroup(groupName);
  }

  /**
   * Limpa as notificações
   */
  public clearNotifications(): void {
    this.rackHubService.clearNotifications();
  }

  /**
   * Obtém a classe CSS para o status da conexão
   */
  public getConnectionStatusClass(): string {
    switch (this.connectionStatus.status) {
      case 'connected': return 'status-connected';
      case 'connecting': return 'status-connecting';
      case 'reconnecting': return 'status-reconnecting';
      case 'disconnected': return 'status-disconnected';
      case 'error': return 'status-error';
      default: return 'status-unknown';
    }
  }

  /**
   * Obtém a classe CSS para o status do apartamento
   */
  public getApartmentStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'livre': return 'apartment-free';
      case 'ocupado': return 'apartment-occupied';
      case 'limpeza': return 'apartment-cleaning';
      case 'manutencao': return 'apartment-maintenance';
      case 'bloqueado': return 'apartment-blocked';
      default: return 'apartment-unknown';
    }
  }

  /**
   * Obtém o ícone para o tipo de notificação
   */
  public getNotificationIcon(type: string): string {
    switch (type) {
      case 'CHECKIN': return '✅';
      case 'CHECKOUT': return '🚪';
      case 'APARTMENT_STATUS': return '🏨';
      case 'PAYMENT': return '💰';
      case 'ERROR': return '❌';
      case 'INFO': return 'ℹ️';
      case 'TEST': return '🧪';
      default: return '📢';
    }
  }

  /**
   * Formata a data para exibição
   */
  public formatDate(date: Date): string {
    return new Intl.DateTimeFormat('pt-PT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).format(new Date(date));
  }

  /**
   * Obtém o texto do status da conexão
   */
  public getConnectionStatusText(): string {
    switch (this.connectionStatus.status) {
      case 'connected': return '🟢 Conectado';
      case 'connecting': return '🟡 Conectando...';
      case 'reconnecting': return '🟡 Reconectando...';
      case 'disconnected': return '🔴 Desconectado';
      case 'error': return '❌ Erro';
      default: return '⚪ Desconhecido';
    }
  }

  /**
   * Verifica se pode enviar comandos
   */
  public canSendCommands(): boolean {
    return this.connectionStatus.isConnected && this.connectionStatus.status === 'connected';
  }
}
