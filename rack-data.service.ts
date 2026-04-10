import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, combineLatest } from 'rxjs';
import { map, tap, switchMap } from 'rxjs/operators';
import { RackSignalRService, ApartamentoStatus, RackEventData } from './rack-signalr.service';

export interface ApartamentoResponse {
  id: string;
  numero: string;
  tipoApartamentoId: string;
  status: 'Livre' | 'Ocupado' | 'Reservado' | 'Manutencao';
  reservaAtual?: {
    id: string;
    hospedeNome: string;
    dataCheckIn: Date;
    dataCheckOut: Date;
  };
}

@Injectable({
  providedIn: 'root'
})
export class RackDataService {
  private apartamentosSubject = new BehaviorSubject<ApartamentoStatus[]>([]);
  private loading = new BehaviorSubject<boolean>(false);

  constructor(
    private http: HttpClient,
    private signalRService: RackSignalRService
  ) {
    this.initializeSignalRUpdates();
  }

  get apartamentos$(): Observable<ApartamentoStatus[]> {
    return this.apartamentosSubject.asObservable();
  }

  get loading$(): Observable<boolean> {
    return this.loading.asObservable();
  }

  // Carregar dados iniciais do rack
  carregarRack(): Observable<ApartamentoStatus[]> {
    this.loading.next(true);
    
    return this.http.get<ApartamentoResponse[]>('/api/apartamentos/rack')
      .pipe(
        map(response => this.mapToApartamentoStatus(response)),
        tap(apartamentos => {
          this.apartamentosSubject.next(apartamentos);
          this.loading.next(false);
        })
      );
  }

  // Recarregar apartamento específico
  private recarregarApartamento(apartamentoId: string): Observable<ApartamentoStatus> {
    return this.http.get<ApartamentoResponse>(`/api/apartamentos/${apartamentoId}/status`)
      .pipe(
        map(response => this.mapSingleApartamento(response))
      );
  }

  // Configurar atualizações em tempo real
  private initializeSignalRUpdates(): void {
    // Quando o rack for atualizado completamente
    this.signalRService.rackAtualizado$.subscribe(updated => {
      if (updated) {
        this.carregarRack().subscribe();
      }
    });

    // Quando status de apartamento específico for alterado
    this.signalRService.statusApartamentoAlterado$.subscribe(data => {
      if (data) {
        this.updateApartamentoLocal(data.apartamentoId, apartamento => {
          apartamento.status = data.novoStatus as any;
        });
      }
    });

    // Quando nova reserva for criada
    this.signalRService.novaReserva$.subscribe(data => {
      if (data) {
        // Recarregar apartamento específico para obter dados completos da reserva
        this.recarregarApartamento(data.apartamentoId).subscribe(apartamento => {
          this.updateApartamentoById(data.apartamentoId, apartamento);
        });
      }
    });

    // Quando check-in for realizado
    this.signalRService.checkInRealizado$.subscribe(data => {
      if (data) {
        this.updateApartamentoLocal(data.apartamentoId, apartamento => {
          apartamento.status = 'Ocupado';
          apartamento.hospedeNome = data.hospedeNome;
          apartamento.dataCheckIn = data.dataCheckIn;
        });
      }
    });

    // Quando check-out for realizado
    this.signalRService.checkOutRealizado$.subscribe(data => {
      if (data) {
        this.updateApartamentoLocal(data.apartamentoId, apartamento => {
          apartamento.status = 'Livre';
          apartamento.hospedeNome = undefined;
          apartamento.dataCheckIn = undefined;
          apartamento.dataCheckOut = data.dataCheckOut;
        });
      }
    });
  }

  // Atualizar apartamento localmente
  private updateApartamentoLocal(
    apartamentoId: string, 
    updateFn: (apartamento: ApartamentoStatus) => void
  ): void {
    const apartamentos = this.apartamentosSubject.value;
    const apartamento = apartamentos.find(apt => apt.apartamentoId === apartamentoId);
    
    if (apartamento) {
      updateFn(apartamento);
      this.apartamentosSubject.next([...apartamentos]);
    }
  }

  // Substituir apartamento por ID
  private updateApartamentoById(apartamentoId: string, novoApartamento: ApartamentoStatus): void {
    const apartamentos = this.apartamentosSubject.value;
    const index = apartamentos.findIndex(apt => apt.apartamentoId === apartamentoId);
    
    if (index !== -1) {
      apartamentos[index] = novoApartamento;
      this.apartamentosSubject.next([...apartamentos]);
    }
  }

  // Mapear resposta da API para modelo local
  private mapToApartamentoStatus(response: ApartamentoResponse[]): ApartamentoStatus[] {
    return response.map(item => this.mapSingleApartamento(item));
  }

  private mapSingleApartamento(item: ApartamentoResponse): ApartamentoStatus {
    return {
      apartamentoId: item.id,
      numero: item.numero,
      status: item.status,
      hospedeNome: item.reservaAtual?.hospedeNome,
      dataCheckIn: item.reservaAtual?.dataCheckIn,
      dataCheckOut: item.reservaAtual?.dataCheckOut
    };
  }

  // Métodos para ações específicas (opcional)
  realizarCheckIn(apartamentoId: string, hospedeNome: string): Observable<any> {
    return this.http.post(`/api/apartamentos/${apartamentoId}/check-in`, { hospedeNome });
  }

  realizarCheckOut(apartamentoId: string): Observable<any> {
    return this.http.post(`/api/apartamentos/${apartamentoId}/check-out`, {});
  }

  alterarStatusApartamento(apartamentoId: string, novoStatus: string): Observable<any> {
    return this.http.put(`/api/apartamentos/${apartamentoId}/status`, { status: novoStatus });
  }
}
