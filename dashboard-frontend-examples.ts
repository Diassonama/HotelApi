// ==============================================================================
// EXEMPLOS DE CONSUMO DAS QUERIES NO FRONTEND
// ==============================================================================

// Interface para os dados dos gráficos
interface ChartData {
  labels: string[];
  datasets: any[];
}

interface DashboardMetrics {
  checkins_hoje: number;
  checkouts_hoje: number;
  ocupacao_atual: number;
  receita_mes: number;
}

interface OccupancyData {
  label: string;
  valor: number;
  percentual: number;
  cor: string;
}

interface WeeklyData {
  semanaLabel: string;
  semanaOrdem: number;
  totalCheckins: number;
  totalCheckouts: number;
  receitaCheckins: number;
  receitaCheckouts: number;
}

interface RevenueData {
  label: string;
  mesNumero: number;
  receita: number;
  totalCheckouts: number;
  corLinha: string;
  corFundo: string;
}

// ==============================================================================
// SERVIÇO PARA CONSUMIR AS QUERIES
// ==============================================================================

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = 'api/dashboard'; // Ajuste conforme sua API

  constructor(private http: HttpClient) {}

  // Métricas principais para os cards
  getDashboardMetrics(): Observable<DashboardMetrics> {
    return this.http.get<any[]>(`${this.baseUrl}/metrics`).pipe(
      map(data => {
        const metrics: any = {};
        data.forEach(item => {
          metrics[item.metric] = item.valor;
        });
        return metrics as DashboardMetrics;
      })
    );
  }

  // Dados para gráfico de ocupação (doughnut)
  getOccupancyData(): Observable<OccupancyData[]> {
    return this.http.get<OccupancyData[]>(`${this.baseUrl}/occupancy`);
  }

  // Dados para gráfico semanal (bar chart)
  getWeeklyData(): Observable<WeeklyData[]> {
    return this.http.get<WeeklyData[]>(`${this.baseUrl}/weekly`);
  }

  // Dados para gráfico de receita (line chart)
  getRevenueData(): Observable<RevenueData[]> {
    return this.http.get<RevenueData[]>(`${this.baseUrl}/revenue`);
  }

  // Reservas próximas para lista
  getUpcomingReservations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/upcoming-reservations`);
  }
}

// ==============================================================================
// COMPONENTE ATUALIZADO PARA USAR OS DADOS REAIS
// ==============================================================================

export class DashboardComponent implements OnInit {
  occupancyChart: Chart;
  revenueChart: Chart;
  monthlyChart: Chart;

  dashboardMetrics: DashboardMetrics = {
    checkins_hoje: 0,
    checkouts_hoje: 0,
    ocupacao_atual: 0,
    receita_mes: 0
  };

  upcomingReservations: any[] = [];

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    // Carregar métricas principais
    this.dashboardService.getDashboardMetrics().subscribe(
      metrics => {
        this.dashboardMetrics = metrics;
      }
    );

    // Carregar reservas próximas
    this.dashboardService.getUpcomingReservations().subscribe(
      reservations => {
        this.upcomingReservations = reservations;
      }
    );

    // Carregar dados dos gráficos
    this.loadCharts();
  }

  loadCharts() {
    this.createOccupancyChart();
    this.createRevenueChart();
    this.createMonthlyChart();
  }

  createOccupancyChart() {
    this.dashboardService.getOccupancyData().subscribe(data => {
      const ctx = document.getElementById('occupancyChart') as HTMLCanvasElement;
      this.occupancyChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
          labels: data.map(item => item.label),
          datasets: [{
            data: data.map(item => item.valor),
            backgroundColor: data.map(item => item.cor),
            borderWidth: 0
          }]
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              display: true,
              labels: {
                color: '#2d3748',
                font: {
                  weight: 'bold',
                  size: 14
                }
              }
            },
            tooltip: {
              bodyColor: '#2d3748',
              titleColor: '#2d3748',
              backgroundColor: 'rgba(255, 255, 255, 0.95)',
              borderColor: '#2d3748',
              borderWidth: 1,
              callbacks: {
                label: function(context: any) {
                  const item = data[context.dataIndex];
                  return `${item.label}: ${item.valor} (${item.percentual}%)`;
                }
              }
            }
          }
        }
      });
    });
  }

  createRevenueChart() {
    this.dashboardService.getRevenueData().subscribe(data => {
      const ctx = document.getElementById('revenueChart') as HTMLCanvasElement;
      this.revenueChart = new Chart(ctx, {
        type: 'line',
        data: {
          labels: data.map(item => item.label),
          datasets: [{
            label: 'Receita (€)',
            data: data.map(item => item.receita),
            borderColor: data[0]?.corLinha || '#007bff',
            backgroundColor: data[0]?.corFundo || 'rgba(0, 123, 255, 0.1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4
          }]
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              labels: {
                color: '#2d3748',
                font: {
                  weight: 'bold',
                  size: 14
                }
              }
            },
            tooltip: {
              bodyColor: '#2d3748',
              titleColor: '#2d3748',
              backgroundColor: 'rgba(255, 255, 255, 0.95)',
              borderColor: '#2d3748',
              borderWidth: 1,
              callbacks: {
                label: function(context: any) {
                  const item = data[context.dataIndex];
                  return `Receita: €${item.receita.toLocaleString()} (${item.totalCheckouts} check-outs)`;
                }
              }
            }
          },
          scales: {
            x: {
              ticks: {
                color: '#2d3748',
                font: {
                  weight: 'bold'
                }
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: '#2d3748',
                font: {
                  weight: 'bold'
                },
                callback: function(value: any) {
                  return '€' + value.toLocaleString();
                }
              }
            }
          }
        }
      });
    });
  }

  createMonthlyChart() {
    this.dashboardService.getWeeklyData().subscribe(data => {
      const ctx = document.getElementById('monthlyChart') as HTMLCanvasElement;
      this.monthlyChart = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: data.map(item => item.semanaLabel),
          datasets: [{
            label: 'Check-ins',
            data: data.map(item => item.totalCheckins),
            backgroundColor: '#28a745'
          }, {
            label: 'Check-outs',
            data: data.map(item => item.totalCheckouts),
            backgroundColor: '#dc3545'
          }]
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              labels: {
                color: '#2d3748',
                font: {
                  weight: 'bold',
                  size: 14
                }
              }
            },
            tooltip: {
              bodyColor: '#2d3748',
              titleColor: '#2d3748',
              backgroundColor: 'rgba(255, 255, 255, 0.95)',
              borderColor: '#2d3748',
              borderWidth: 1,
              callbacks: {
                afterLabel: function(context: any) {
                  const item = data[context.dataIndex];
                  if (context.datasetIndex === 0) {
                    return `Receita Check-ins: €${item.receitaCheckins.toLocaleString()}`;
                  } else {
                    return `Receita Check-outs: €${item.receitaCheckouts.toLocaleString()}`;
                  }
                }
              }
            }
          },
          scales: {
            x: {
              ticks: {
                color: '#2d3748',
                font: {
                  weight: 'bold'
                }
              }
            },
            y: {
              beginAtZero: true,
              ticks: {
                color: '#2d3748',
                font: {
                  weight: 'bold'
                }
              }
            }
          }
        }
      });
    });
  }

  getStatusClass(status: string): string {
    switch(status) {
      case 'Confirmado': return 'badge-warning';
      case 'Check-in': return 'badge-success';
      case 'Check-out': return 'badge-secondary';
      case 'Em andamento': return 'badge-primary';
      default: return 'badge-primary';
    }
  }

  // Método para atualizar os dados periodicamente
  refreshData() {
    this.loadDashboardData();
  }
}

// ==============================================================================
// TEMPLATE HTML EXEMPLO PARA OS CARDS
// ==============================================================================

/*
<div class="row">
  <div class="col-xl-3 col-md-6 mb-4">
    <div class="card border-left-success shadow h-100 py-2">
      <div class="card-body">
        <div class="row no-gutters align-items-center">
          <div class="col mr-2">
            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
              Check-ins Hoje
            </div>
            <div class="h5 mb-0 font-weight-bold text-gray-800">
              {{ dashboardMetrics.checkins_hoje }}
            </div>
          </div>
          <div class="col-auto">
            <i class="fas fa-sign-in-alt fa-2x text-gray-300"></i>
          </div>
        </div>
      </div>
    </div>
  </div>
  
  <div class="col-xl-3 col-md-6 mb-4">
    <div class="card border-left-danger shadow h-100 py-2">
      <div class="card-body">
        <div class="row no-gutters align-items-center">
          <div class="col mr-2">
            <div class="text-xs font-weight-bold text-danger text-uppercase mb-1">
              Check-outs Hoje
            </div>
            <div class="h5 mb-0 font-weight-bold text-gray-800">
              {{ dashboardMetrics.checkouts_hoje }}
            </div>
          </div>
          <div class="col-auto">
            <i class="fas fa-sign-out-alt fa-2x text-gray-300"></i>
          </div>
        </div>
      </div>
    </div>
  </div>
  
  <div class="col-xl-3 col-md-6 mb-4">
    <div class="card border-left-info shadow h-100 py-2">
      <div class="card-body">
        <div class="row no-gutters align-items-center">
          <div class="col mr-2">
            <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
              Taxa de Ocupação
            </div>
            <div class="row no-gutters align-items-center">
              <div class="col-auto">
                <div class="h5 mb-0 mr-3 font-weight-bold text-gray-800">
                  {{ dashboardMetrics.ocupacao_atual }}%
                </div>
              </div>
              <div class="col">
                <div class="progress progress-sm mr-2">
                  <div class="progress-bar bg-info" 
                       role="progressbar" 
                       [style.width.%]="dashboardMetrics.ocupacao_atual">
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="col-auto">
            <i class="fas fa-bed fa-2x text-gray-300"></i>
          </div>
        </div>
      </div>
    </div>
  </div>
  
  <div class="col-xl-3 col-md-6 mb-4">
    <div class="card border-left-warning shadow h-100 py-2">
      <div class="card-body">
        <div class="row no-gutters align-items-center">
          <div class="col mr-2">
            <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
              Receita do Mês
            </div>
            <div class="h5 mb-0 font-weight-bold text-gray-800">
              €{{ dashboardMetrics.receita_mes | number:'1.0-0' }}
            </div>
          </div>
          <div class="col-auto">
            <i class="fas fa-euro-sign fa-2x text-gray-300"></i>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
*/
