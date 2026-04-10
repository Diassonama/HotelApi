// ==============================================================================
// EXEMPLO DE REGISTRO DO SERVIÇO DASHBOARD NO PROGRAM.CS
// ==============================================================================

// No arquivo Program.cs, adicione esta linha após o registro dos outros serviços:

using Hotel.Application.Extensions;

// ... outros registros de serviços ...

// Registrar o serviço de Dashboard
builder.Services.AddDashboardServices();

// ==============================================================================
// EXEMPLO DE USO NO FRONTEND ANGULAR
// ==============================================================================

/* 
// dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = 'api/dashboard';

  constructor(private http: HttpClient) {}

  // Obter todas as métricas principais de uma vez
  getDashboardSummary(): Observable<any> {
    return this.http.get(`${this.baseUrl}/summary`);
  }

  // Métricas individuais
  getDashboardMetrics(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/metrics`);
  }

  getWeeklyData(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/weekly`);
  }

  getOccupancyData(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/occupancy`);
  }

  getRevenueData(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/revenue`);
  }

  getUpcomingReservations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/upcoming-reservations`);
  }
}

// dashboard.component.ts
export class DashboardComponent implements OnInit {
  
  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    // Carregar resumo completo
    this.dashboardService.getDashboardSummary().subscribe(
      data => {
        this.dashboardMetrics = this.processMetrics(data.metrics);
        this.createWeeklyChart(data.weeklyData);
        this.createOccupancyChart(data.occupancyData);
        this.upcomingReservations = data.upcomingReservations;
      },
      error => console.error('Erro ao carregar dados do dashboard:', error)
    );
  }

  private processMetrics(metrics: any[]): any {
    const result: any = {};
    metrics.forEach(metric => {
      result[metric.metric] = metric.valor;
    });
    return result;
  }
}
*/

// ==============================================================================
// ENDPOINTS DISPONÍVEIS
// ==============================================================================

/*
GET /api/dashboard/metrics                 - Métricas principais para cards
GET /api/dashboard/checkins-today         - Check-ins de hoje
GET /api/dashboard/checkouts-today        - Check-outs de hoje  
GET /api/dashboard/weekly                  - Dados semanais para gráfico
GET /api/dashboard/occupancy               - Dados de ocupação para gráfico doughnut
GET /api/dashboard/revenue                 - Receita anual para gráfico de linha
GET /api/dashboard/monthly-revenue         - Receita do mês atual
GET /api/dashboard/stats                   - Estatísticas gerais
GET /api/dashboard/apartments-distribution - Distribuição de apartamentos
GET /api/dashboard/top-apartments?count=10 - Top apartamentos mais utilizados
GET /api/dashboard/upcoming-checkouts?days=3 - Check-outs próximos
GET /api/dashboard/upcoming-reservations?count=5 - Reservas próximas
GET /api/dashboard/summary                 - Resumo completo (combinado)
*/

// ==============================================================================
// EXEMPLO DE RESPOSTA DOS ENDPOINTS
// ==============================================================================

/*
// GET /api/dashboard/metrics
[
  { "metric": "checkins_hoje", "valor": 5, "label": "check-ins hoje", "cor": "#28a745" },
  { "metric": "checkouts_hoje", "valor": 3, "label": "check-outs hoje", "cor": "#dc3545" },
  { "metric": "ocupacao_atual", "valor": 78, "label": "% ocupação", "cor": "#007bff" },
  { "metric": "receita_mes", "valor": 12500, "label": "receita mês (€)", "cor": "#ffc107" }
]

// GET /api/dashboard/weekly
[
  { "semanaLabel": "Semana 1", "semanaOrdem": 1, "totalCheckins": 25, "totalCheckouts": 20, "receitaCheckins": 5000, "receitaCheckouts": 4500 },
  { "semanaLabel": "Semana 2", "semanaOrdem": 2, "totalCheckins": 32, "totalCheckouts": 28, "receitaCheckins": 6400, "receitaCheckouts": 5600 },
  { "semanaLabel": "Semana 3", "semanaOrdem": 3, "totalCheckins": 28, "totalCheckouts": 25, "receitaCheckins": 5600, "receitaCheckouts": 5000 },
  { "semanaLabel": "Semana 4", "semanaOrdem": 4, "totalCheckins": 35, "totalCheckouts": 30, "receitaCheckins": 7000, "receitaCheckouts": 6000 }
]

// GET /api/dashboard/occupancy
[
  { "label": "Ocupados", "valor": 78, "percentual": 65.0, "cor": "#28a745" },
  { "label": "Disponíveis", "valor": 42, "percentual": 35.0, "cor": "#e9ecef" }
]
*/
