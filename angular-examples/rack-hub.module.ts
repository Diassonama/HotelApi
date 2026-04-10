import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

// Componentes
import { RackDashboardComponent } from './components/rack-dashboard.component';

// Serviços
import { RackHubService } from './services/rack-hub.service';

// Routing (opcional)
import { RouterModule, Routes } from '@angular/router';

// Definir rotas (se necessário)
const routes: Routes = [
  {
    path: '',
    component: RackDashboardComponent
  },
  {
    path: 'dashboard',
    component: RackDashboardComponent
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];

@NgModule({
  declarations: [
    RackDashboardComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forChild(routes)
  ],
  providers: [
    RackHubService
  ],
  exports: [
    RackDashboardComponent
  ]
})
export class RackHubModule { 

  /**
   * Configuração para o módulo raiz
   * Use RackHubModule.forRoot() no seu AppModule
   */
  static forRoot() {
    return {
      ngModule: RackHubModule,
      providers: [
        RackHubService
      ]
    };
  }

  /**
   * Configuração para módulos feature
   * Use RackHubModule.forFeature() em outros módulos
   */
  static forFeature() {
    return {
      ngModule: RackHubModule,
      providers: []
    };
  }
}
