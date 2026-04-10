import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface RackNotification {
  type: string;
  message: string;
  data?: any;
  timestamp: Date;
}

export interface ConnectionStatus {
  isConnected: boolean;
  connectionId?: string;
  status: 'connected' | 'disconnected' | 'connecting' | 'reconnecting' | 'error';
}

export interface CheckinData {
  type: 'CHECKIN';
  checkinId: number;
  apartamentoIds: number[];
  apartamentosCodigos: string[];
  dataEntrada: string;
  hospedes: Array<{
    id: number;
    nome: string;
    telefone: string;
  }>;
  timestamp: Date;
}

export interface CheckoutData {
  type: 'CHECKOUT';
  checkinId: number;
  apartamentoIds: number[];
  apartamentosCodigos: string[];
  dataSaida: string;
  hospedes: Array<{
    id: number;
    nome: string;
    telefone: string;
  }>;
  timestamp: Date;
}

export interface ApartmentStatusData {
  type: 'APARTMENT_STATUS_CHANGE';
  apartamentoId: number;
  codigo: string;
  observacao: string;
  situacao: string;
  checkinsId?: number;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class RackHubService {
  
  private hubConnection: HubConnection | null = null;
  
  // Subjects para observáveis
  private connectionStatusSubject = new BehaviorSubject<ConnectionStatus>({
    isConnected: false,
    status: 'disconnected'
  });

  private notificationsSubject = new BehaviorSubject<RackNotification[]>([]);
  private checkinSubject = new BehaviorSubject<CheckinData | null>(null);
  private checkoutSubject = new BehaviorSubject<CheckoutData | null>(null);
  private apartmentStatusSubject = new BehaviorSubject<ApartmentStatusData | null>(null);

  // Observáveis públicos
  public connectionStatus$: Observable<ConnectionStatus> = this.connectionStatusSubject.asObservable();
  public notifications$: Observable<RackNotification[]> = this.notificationsSubject.asObservable();
  public checkins$: Observable<CheckinData | null> = this.checkinSubject.asObservable();
  public checkouts$: Observable<CheckoutData | null> = this.checkoutSubject.asObservable();
  public apartmentStatus$: Observable<ApartmentStatusData | null> = this.apartmentStatusSubject.asObservable();

  constructor() {
    this.createConnection();
  }

  /**
   * Cria a conexão SignalR
   */
  private createConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/rackHub`, {
        accessTokenFactory: () => this.getAuthToken()
      })
      .withAutomaticReconnect([0, 2000, 10000, 30000])
      .configureLogging(LogLevel.Information)
      .build();

    this.setupEventHandlers();
    this.setupConnectionEvents();
  }

  /**
   * Obtém o token de autenticação
   */
  private getAuthToken(): string {
    // Adapte conforme seu sistema de autenticação
    return localStorage.getItem('authToken') || sessionStorage.getItem('token') || '';
  }

  /**
   * Configura os manipuladores de eventos do SignalR
   */
  private setupEventHandlers(): void {
    if (!this.hubConnection) return;

    // ==========================================
    // EVENTOS DE CHECK-IN/CHECK-OUT
    // ==========================================
    
    this.hubConnection.on('CheckinUpdate', (data: CheckinData) => {
      console.log('🟢 Novo Check-in recebido:', data);
      this.checkinSubject.next(data);
      this.addNotification({
        type: 'CHECKIN',
        message: `Check-in realizado nos apartamentos: ${data.apartamentosCodigos.join(', ')}`,
        data: data,
        timestamp: new Date()
      });
    });

    this.hubConnection.on('CheckoutUpdate', (data: CheckoutData) => {
      console.log('🔴 Check-out recebido:', data);
      this.checkoutSubject.next(data);
      this.addNotification({
        type: 'CHECKOUT',
        message: `Check-out realizado nos apartamentos: ${data.apartamentosCodigos.join(', ')}`,
        data: data,
        timestamp: new Date()
      });
    });

    // ==========================================
    // EVENTOS DE APARTAMENTOS
    // ==========================================
    
    this.hubConnection.on('ApartmentStatusUpdate', (data: ApartmentStatusData) => {
      console.log('🏨 Status do apartamento alterado:', data);
      this.apartmentStatusSubject.next(data);
      this.addNotification({
        type: 'APARTMENT_STATUS',
        message: `Apartamento ${data.codigo}: ${data.situacao}`,
        data: data,
        timestamp: new Date()
      });
    });

    this.hubConnection.on('ApartamentosOcupadosUpdate', (data: any) => {
      console.log('📋 Lista de apartamentos ocupados atualizada:', data);
      this.addNotification({
        type: 'OCCUPIED_APARTMENTS',
        message: `${data.Count} apartamentos ocupados`,
        data: data,
        timestamp: new Date()
      });
    });

    // ==========================================
    // EVENTOS DE HOSPEDAGEM E PAGAMENTOS
    // ==========================================
    
    this.hubConnection.on('HospedagemUpdate', (data: any) => {
      console.log('📝 Hospedagem atualizada:', data);
      this.addNotification({
        type: 'HOSPEDAGEM',
        message: `Hospedagem atualizada (ID: ${data.HospedagemId})`,
        data: data,
        timestamp: new Date()
      });
    });

    this.hubConnection.on('PaymentUpdate', (data: any) => {
      console.log('💰 Pagamento recebido:', data);
      this.addNotification({
        type: 'PAYMENT',
        message: `Pagamento de R$ ${data.Valor?.toFixed(2)} recebido`,
        data: data,
        timestamp: new Date()
      });
    });

    // ==========================================
    // EVENTOS DE DASHBOARD
    // ==========================================
    
    this.hubConnection.on('DashboardMetricsUpdate', (data: any) => {
      console.log('📊 Métricas do dashboard atualizadas:', data);
      this.addNotification({
        type: 'DASHBOARD',
        message: 'Métricas do dashboard atualizadas',
        data: data,
        timestamp: new Date()
      });
    });

    this.hubConnection.on('GeneralRackUpdate', (data: any) => {
      console.log('🔄 Atualização geral do rack:', data);
      this.addNotification({
        type: 'RACK_UPDATE',
        message: 'Rack atualizado',
        data: data,
        timestamp: new Date()
      });
    });

    // ==========================================
    // EVENTOS DE SISTEMA
    // ==========================================
    
    this.hubConnection.on('ErrorNotification', (data: any) => {
      console.error('❌ Erro:', data);
      this.addNotification({
        type: 'ERROR',
        message: data.Message || 'Erro no sistema',
        data: data,
        timestamp: new Date()
      });
    });

    this.hubConnection.on('InfoNotification', (data: any) => {
      console.log('ℹ️ Informação:', data);
      this.addNotification({
        type: 'INFO',
        message: data.Message || 'Informação do sistema',
        data: data,
        timestamp: new Date()
      });
    });

    // ==========================================
    // EVENTOS DE GRUPOS
    // ==========================================
    
    this.hubConnection.on('JoinedGroup', (groupName: string) => {
      console.log(`✅ Entrou no grupo: ${groupName}`);
    });

    this.hubConnection.on('LeftGroup', (groupName: string) => {
      console.log(`❌ Saiu do grupo: ${groupName}`);
    });

    this.hubConnection.on('TestMessage', (message: string) => {
      console.log(`🧪 Mensagem de teste: ${message}`);
      this.addNotification({
        type: 'TEST',
        message: `Teste: ${message}`,
        timestamp: new Date()
      });
    });
  }

  /**
   * Configura eventos de conexão
   */
  private setupConnectionEvents(): void {
    if (!this.hubConnection) return;

    this.hubConnection.onreconnecting((error) => {
      console.warn('🔄 Reconectando...', error);
      this.updateConnectionStatus({
        isConnected: false,
        status: 'reconnecting'
      });
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('✅ Reconectado!', connectionId);
      this.updateConnectionStatus({
        isConnected: true,
        connectionId: connectionId,
        status: 'connected'
      });
    });

    this.hubConnection.onclose((error) => {
      console.error('❌ Conexão fechada', error);
      this.updateConnectionStatus({
        isConnected: false,
        status: 'disconnected'
      });
    });
  }

  /**
   * Inicia a conexão
   */
  public async startConnection(): Promise<void> {
    if (!this.hubConnection) {
      this.createConnection();
    }

    if (this.hubConnection?.state === 'Disconnected') {
      try {
        this.updateConnectionStatus({
          isConnected: false,
          status: 'connecting'
        });

        await this.hubConnection.start();
        
        this.updateConnectionStatus({
          isConnected: true,
          connectionId: this.hubConnection.connectionId || undefined,
          status: 'connected'
        });

        console.log('✅ Conectado ao RackHub!');
        
        // Entrar automaticamente no grupo geral
        await this.joinGroup('RackGroup');
        
      } catch (error) {
        console.error('❌ Erro ao conectar:', error);
        this.updateConnectionStatus({
          isConnected: false,
          status: 'error'
        });
        throw error;
      }
    }
  }

  /**
   * Para a conexão
   */
  public async stopConnection(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      try {
        await this.hubConnection.stop();
        console.log('🔌 Desconectado do RackHub');
      } catch (error) {
        console.error('❌ Erro ao desconectar:', error);
      }
    }
  }

  /**
   * Entra num grupo
   */
  public async joinGroup(groupName: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      try {
        await this.hubConnection.invoke('JoinGroup', groupName);
        console.log(`✅ Entrou no grupo: ${groupName}`);
      } catch (error) {
        console.error(`❌ Erro ao entrar no grupo ${groupName}:`, error);
      }
    }
  }

  /**
   * Sai de um grupo
   */
  public async leaveGroup(groupName: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      try {
        await this.hubConnection.invoke('LeaveGroup', groupName);
        console.log(`❌ Saiu do grupo: ${groupName}`);
      } catch (error) {
        console.error(`❌ Erro ao sair do grupo ${groupName}:`, error);
      }
    }
  }

  /**
   * Envia mensagem de teste
   */
  public async sendTestMessage(message: string): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      try {
        await this.hubConnection.invoke('SendTestMessage', message);
      } catch (error) {
        console.error('❌ Erro ao enviar mensagem de teste:', error);
      }
    }
  }

  /**
   * Obtém o status da conexão atual
   */
  public getConnectionStatus(): ConnectionStatus {
    return this.connectionStatusSubject.value;
  }

  /**
   * Verifica se está conectado
   */
  public isConnected(): boolean {
    return this.hubConnection?.state === 'Connected' && this.connectionStatusSubject.value.isConnected;
  }

  /**
   * Limpa as notificações
   */
  public clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  /**
   * Obtém as notificações atuais
   */
  public getNotifications(): RackNotification[] {
    return this.notificationsSubject.value;
  }

  // ==========================================
  // MÉTODOS PRIVADOS
  // ==========================================

  private updateConnectionStatus(status: ConnectionStatus): void {
    this.connectionStatusSubject.next(status);
  }

  private addNotification(notification: RackNotification): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = [notification, ...currentNotifications];
    
    // Manter apenas as últimas 50 notificações
    if (updatedNotifications.length > 50) {
      updatedNotifications.splice(50);
    }
    
    this.notificationsSubject.next(updatedNotifications);
  }

  /**
   * Cleanup quando o serviço for destruído
   */
  ngOnDestroy(): void {
    this.stopConnection();
  }
}
