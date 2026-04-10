import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

export interface ApartamentoStatus {
  apartamentoId: string;
  numero: string;
  status: 'Livre' | 'Ocupado' | 'Reservado' | 'Manutencao';
  hospedeNome?: string;
  dataCheckIn?: Date;
  dataCheckOut?: Date;
}

export interface RackEventData {
  apartamentoId?: string;
  apartamentoIds?: Array<number | string>;
  apartamentosCodigos?: string[];
  novoStatus?: string;
  hospedeNome?: string;
  dataCheckIn?: Date;
  dataCheckOut?: Date;
  reservaId?: string;
  type?: string;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RackSignalRService {
  private connection: signalR.HubConnection | null = null;
  private connectionState = new BehaviorSubject<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
  
  // Observables para eventos específicos
  private rackAtualizado = new Subject<boolean>();
  private statusApartamentoAlterado = new Subject<RackEventData>();
  private novaReserva = new Subject<RackEventData>();
  private checkInRealizado = new Subject<RackEventData>();
  private checkOutRealizado = new Subject<RackEventData>();

  constructor() {
    this.iniciarConexao();
  }

  // Observables públicos
  get connectionState$(): Observable<signalR.HubConnectionState> {
    return this.connectionState.asObservable();
  }

  get rackAtualizado$(): Observable<boolean> {
    return this.rackAtualizado.asObservable();
  }

  get statusApartamentoAlterado$(): Observable<RackEventData | null> {
    return this.statusApartamentoAlterado.asObservable();
  }

  get novaReserva$(): Observable<RackEventData | null> {
    return this.novaReserva.asObservable();
  }

  get checkInRealizado$(): Observable<RackEventData | null> {
    return this.checkInRealizado.asObservable();
  }

  get checkOutRealizado$(): Observable<RackEventData | null> {
    return this.checkOutRealizado.asObservable();
  }

  private async iniciarConexao(): Promise<void> {
    try {
      this.connectionState.next(signalR.HubConnectionState.Connecting);

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl('/rackHub')
        .withAutomaticReconnect([0, 2000, 10000, 30000])
        .build();

      this.configurarEventos();
      this.configurarEventosConexao();

      await this.connection.start();
      this.connectionState.next(this.connection.state);
      
      console.log('Conectado ao RackHub SignalR');
      
      // Entrar no grupo do rack no backend
      await this.connection.invoke('JoinGroup', 'RackGroup');
      
    } catch (error) {
      console.error('Erro ao conectar ao SignalR:', error);
      this.connectionState.next(signalR.HubConnectionState.Disconnected);
    }
  }

  private configurarEventos(): void {
    if (!this.connection) return;

    // Evento de atualização geral (enviado em checkin/checkout/status)
    this.connection.on('RackUpdate', (data: any) => {
      console.log('Rack atualizado via SignalR', data);
      this.rackAtualizado.next(true);
    });

    this.connection.on('GeneralRackUpdate', (data: any) => {
      console.log('GeneralRackUpdate recebido', data);
      this.rackAtualizado.next(true);
    });

    // Status de apartamento alterado
    this.connection.on('ApartmentStatusUpdate', (data: any) => {
      const payload: RackEventData = {
        apartamentoId: this.toStringOrUndefined(data?.ApartamentoId),
        novoStatus: this.toStringOrUndefined(data?.Situacao),
        type: this.toStringOrUndefined(data?.Type),
        message: this.toStringOrUndefined(data?.Message)
      };

      console.log('Status do apartamento alterado:', data);
      this.statusApartamentoAlterado.next(payload);
    });

    // Check-in realizado
    this.connection.on('CheckinUpdate', (data: any) => {
      const payload: RackEventData = {
        apartamentoIds: (data?.ApartamentoIds ?? []).map((id: any) => id),
        apartamentosCodigos: data?.ApartamentosCodigos ?? [],
        dataCheckIn: data?.DataEntrada ? new Date(data.DataEntrada) : undefined,
        hospedeNome: data?.Hospedes?.[0]?.Nome,
        type: this.toStringOrUndefined(data?.Type)
      };

      if (payload.apartamentoIds && payload.apartamentoIds.length > 0) {
        payload.apartamentoId = String(payload.apartamentoIds[0]);
      }

      console.log('Check-in realizado:', data);
      this.checkInRealizado.next(payload);
    });

    // Check-out realizado
    this.connection.on('CheckoutUpdate', (data: any) => {
      const payload: RackEventData = {
        apartamentoIds: (data?.ApartamentoIds ?? []).map((id: any) => id),
        apartamentosCodigos: data?.ApartamentosCodigos ?? [],
        dataCheckOut: data?.DataSaida ? new Date(data.DataSaida) : undefined,
        hospedeNome: data?.Hospedes?.[0]?.Nome,
        type: this.toStringOrUndefined(data?.Type)
      };

      if (payload.apartamentoIds && payload.apartamentoIds.length > 0) {
        payload.apartamentoId = String(payload.apartamentoIds[0]);
      }

      console.log('Check-out realizado:', data);
      this.checkOutRealizado.next(payload);
    });

    // Mantido para compatibilidade com fluxos antigos
    this.connection.on('NovaReserva', (data: any) => {
      const payload: RackEventData = {
        apartamentoId: this.toStringOrUndefined(data?.apartamentoId ?? data?.ApartamentoId),
        reservaId: this.toStringOrUndefined(data?.reservaId ?? data?.ReservaId),
        type: this.toStringOrUndefined(data?.Type)
      };
      this.novaReserva.next(payload);
    });
  }

  private configurarEventosConexao(): void {
    if (!this.connection) return;

    this.connection.onreconnecting(() => {
      console.log('Tentando reconectar ao SignalR...');
      this.connectionState.next(signalR.HubConnectionState.Reconnecting);
    });

    this.connection.onreconnected(async () => {
      console.log('Reconectado ao SignalR');
      this.connectionState.next(this.connection!.state);
      
      // Reentrar no grupo após reconexão
      try {
        await this.connection!.invoke('JoinGroup', 'RackGroup');
      } catch (error) {
        console.error('Erro ao reentrar no grupo:', error);
      }
    });

    this.connection.onclose(() => {
      console.log('Conexão SignalR fechada');
      this.connectionState.next(signalR.HubConnectionState.Disconnected);
    });
  }

  // Método para desconectar manualmente
  async desconectar(): Promise<void> {
    if (this.connection) {
      try {
        if (this.connection.state === signalR.HubConnectionState.Connected) {
          await this.connection.invoke('LeaveGroup', 'RackGroup');
        }
        await this.connection.stop();
      } catch (error) {
        console.error('Erro ao desconectar:', error);
      }
    }
  }

  // Verificar se está conectado
  get isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  // Método para reconectar manualmente
  async reconectar(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Disconnected) {
      await this.iniciarConexao();
    }
  }

  private toStringOrUndefined(value: any): string | undefined {
    if (value === null || value === undefined) return undefined;
    const result = String(value).trim();
    return result.length > 0 ? result : undefined;
  }
}
