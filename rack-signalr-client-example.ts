// Exemplo de integração do RackHub SignalR para notificações em tempo real

import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

export class RackSignalRService {
    private connection: any;
    
    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl("/rackHub", {
                accessTokenFactory: () => this.getAuthToken()
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        this.setupEventHandlers();
    }

    private getAuthToken(): string {
        // Retorna o token JWT para autenticação
        return localStorage.getItem('authToken') || '';
    }

    private setupEventHandlers(): void {
        // ========================================
        // EVENTOS DE CHECK-IN/CHECK-OUT
        // ========================================
        
        this.connection.on("CheckinUpdate", (data: any) => {
            console.log("🔔 Novo Check-in:", data);
            this.showNotification(
                `✅ Check-in realizado`,
                `Apartamentos: ${data.ApartamentosCodigos.join(', ')} - ${data.Hospedes.length} hóspedes`,
                'success'
            );
            this.updateRackDisplay(data);
        });

        this.connection.on("CheckoutUpdate", (data: any) => {
            console.log("🔔 Check-out:", data);
            this.showNotification(
                `🚪 Check-out realizado`,
                `Apartamentos: ${data.ApartamentosCodigos.join(', ')}`,
                'info'
            );
            this.updateRackDisplay(data);
        });

        // ========================================
        // EVENTOS DE APARTAMENTOS
        // ========================================
        
        this.connection.on("ApartmentStatusUpdate", (data: any) => {
            console.log("🔔 Status do apartamento alterado:", data);
            this.showNotification(
                `🏨 Apartamento ${data.Codigo}`,
                `Status alterado para: ${data.Situacao}`,
                'warning'
            );
            this.updateApartmentStatus(data.ApartamentoId, data.Situacao, data.Codigo);
        });

        this.connection.on("ApartamentosOcupadosUpdate", (data: any) => {
            console.log("🔔 Lista de apartamentos ocupados atualizada:", data);
            this.updateOccupiedApartmentsList(data.Apartamentos);
        });

        // ========================================
        // EVENTOS DE HOSPEDAGEM E PAGAMENTOS
        // ========================================
        
        this.connection.on("HospedagemUpdate", (data: any) => {
            console.log("🔔 Hospedagem atualizada:", data);
            this.showNotification(
                `📝 Hospedagem atualizada`,
                `ID: ${data.HospedagemId} - Check-in: ${data.CheckinsId}`,
                'info'
            );
        });

        this.connection.on("PaymentUpdate", (data: any) => {
            console.log("🔔 Novo pagamento:", data);
            this.showNotification(
                `💰 Pagamento recebido`,
                `Valor: R$ ${data.Valor.toFixed(2)}`,
                'success'
            );
        });

        // ========================================
        // EVENTOS DE DASHBOARD E MÉTRICAS
        // ========================================
        
        this.connection.on("DashboardMetricsUpdate", (data: any) => {
            console.log("🔔 Métricas do dashboard atualizadas:", data);
            this.updateDashboardMetrics(data.Metrics);
        });

        this.connection.on("GeneralRackUpdate", (data: any) => {
            console.log("🔔 Atualização geral do rack:", data);
            this.refreshRackData();
        });

        // ========================================
        // EVENTOS DE SISTEMA
        // ========================================
        
        this.connection.on("ErrorNotification", (data: any) => {
            console.error("❌ Erro notificado:", data);
            this.showNotification(
                `❌ Erro`,
                data.Message,
                'error'
            );
        });

        this.connection.on("InfoNotification", (data: any) => {
            console.log("ℹ️ Informação:", data);
            this.showNotification(
                `ℹ️ Informação`,
                data.Message,
                'info'
            );
        });

        // ========================================
        // EVENTOS DE GRUPOS
        // ========================================
        
        this.connection.on("JoinedGroup", (groupName: string) => {
            console.log(`✅ Entrou no grupo: ${groupName}`);
        });

        this.connection.on("LeftGroup", (groupName: string) => {
            console.log(`❌ Saiu do grupo: ${groupName}`);
        });

        this.connection.on("TestMessage", (message: string) => {
            console.log(`🧪 Mensagem de teste: ${message}`);
        });

        // ========================================
        // EVENTOS DE CONEXÃO
        // ========================================
        
        this.connection.onreconnecting((error: any) => {
            console.warn("🔄 Reconectando ao RackHub...", error);
            this.showConnectionStatus('reconnecting');
        });

        this.connection.onreconnected(() => {
            console.log("✅ Reconectado ao RackHub!");
            this.showConnectionStatus('connected');
        });

        this.connection.onclose((error: any) => {
            console.error("❌ Conexão com RackHub fechada", error);
            this.showConnectionStatus('disconnected');
        });
    }

    // ========================================
    // MÉTODOS PÚBLICOS
    // ========================================
    
    public async start(): Promise<void> {
        try {
            await this.connection.start();
            console.log("✅ Conectado ao RackHub!");
            this.showConnectionStatus('connected');
        } catch (error) {
            console.error("❌ Erro ao conectar ao RackHub:", error);
            this.showConnectionStatus('error');
        }
    }

    public async stop(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            console.log("🔌 Desconectado do RackHub");
            this.showConnectionStatus('disconnected');
        }
    }

    public async joinGroup(groupName: string): Promise<void> {
        try {
            await this.connection.invoke("JoinGroup", groupName);
            console.log(`✅ Entrou no grupo: ${groupName}`);
        } catch (error) {
            console.error(`❌ Erro ao entrar no grupo ${groupName}:`, error);
        }
    }

    public async leaveGroup(groupName: string): Promise<void> {
        try {
            await this.connection.invoke("LeaveGroup", groupName);
            console.log(`❌ Saiu do grupo: ${groupName}`);
        } catch (error) {
            console.error(`❌ Erro ao sair do grupo ${groupName}:`, error);
        }
    }

    public async sendTestMessage(message: string): Promise<void> {
        try {
            await this.connection.invoke("SendTestMessage", message);
        } catch (error) {
            console.error("❌ Erro ao enviar mensagem de teste:", error);
        }
    }

    // ========================================
    // MÉTODOS PRIVADOS DE UI
    // ========================================
    
    private showNotification(title: string, message: string, type: 'success' | 'error' | 'warning' | 'info'): void {
        // Integração com sistema de notificações (ex: Toastr, SweetAlert, etc.)
        console.log(`${this.getIconForType(type)} ${title}: ${message}`);
        
        // Exemplo de integração com Toastr
        if (typeof toastr !== 'undefined') {
            toastr[type](message, title);
        }
        
        // Exemplo de integração com notificações do browser
        if (Notification.permission === 'granted') {
            new Notification(title, {
                body: message,
                icon: this.getIconUrlForType(type)
            });
        }
    }

    private getIconForType(type: string): string {
        const icons = {
            'success': '✅',
            'error': '❌',
            'warning': '⚠️',
            'info': 'ℹ️'
        };
        return icons[type as keyof typeof icons] || 'ℹ️';
    }

    private getIconUrlForType(type: string): string {
        return `/assets/icons/${type}.png`;
    }

    private showConnectionStatus(status: 'connected' | 'disconnected' | 'reconnecting' | 'error'): void {
        const statusElement = document.getElementById('signalr-status');
        if (statusElement) {
            statusElement.className = `status ${status}`;
            statusElement.textContent = this.getStatusText(status);
        }
    }

    private getStatusText(status: string): string {
        const texts = {
            'connected': '🟢 Conectado',
            'disconnected': '🔴 Desconectado',
            'reconnecting': '🟡 Reconectando...',
            'error': '❌ Erro de conexão'
        };
        return texts[status as keyof typeof texts] || status;
    }

    // ========================================
    // MÉTODOS DE ATUALIZAÇÃO DE UI
    // ========================================
    
    private updateRackDisplay(data: any): void {
        // Atualizar visualização do rack
        console.log("🔄 Atualizando display do rack...", data);
        
        // Exemplo: atualizar cores dos apartamentos
        if (data.ApartamentoIds) {
            data.ApartamentoIds.forEach((id: number) => {
                const apartmentElement = document.querySelector(`[data-apartment-id="${id}"]`);
                if (apartmentElement) {
                    this.updateApartmentVisualStatus(apartmentElement, data.Type);
                }
            });
        }
    }

    private updateApartmentStatus(apartmentId: number, status: string, codigo: string): void {
        console.log(`🏨 Atualizando status do apartamento ${codigo}: ${status}`);
        
        const apartmentElement = document.querySelector(`[data-apartment-id="${apartmentId}"]`);
        if (apartmentElement) {
            apartmentElement.setAttribute('data-status', status.toLowerCase());
            apartmentElement.classList.remove('livre', 'ocupado', 'manutencao', 'limpeza');
            apartmentElement.classList.add(this.getStatusClass(status));
        }
    }

    private updateOccupiedApartmentsList(apartments: any[]): void {
        console.log("📋 Atualizando lista de apartamentos ocupados...", apartments);
        
        const listElement = document.getElementById('occupied-apartments-list');
        if (listElement) {
            listElement.innerHTML = apartments.map(apt => 
                `<div class="apartment-item">
                    <span class="apartment-code">${apt.Codigo}</span>
                    <span class="guest-name">${apt.Hospede}</span>
                    <span class="checkout-info">${apt.Checkout}</span>
                </div>`
            ).join('');
        }
    }

    private updateDashboardMetrics(metrics: any): void {
        console.log("📊 Atualizando métricas do dashboard...", metrics);
        
        // Atualizar elementos do dashboard
        if (metrics) {
            Object.keys(metrics).forEach(key => {
                const element = document.getElementById(`metric-${key.toLowerCase()}`);
                if (element) {
                    element.textContent = metrics[key];
                }
            });
        }
    }

    private refreshRackData(): void {
        console.log("🔄 Atualizando dados gerais do rack...");
        
        // Disparar evento customizado para outros componentes
        window.dispatchEvent(new CustomEvent('rackDataRefresh'));
        
        // Ou chamar método específico do componente
        if (typeof this.onRackRefresh === 'function') {
            this.onRackRefresh();
        }
    }

    private updateApartmentVisualStatus(element: Element, eventType: string): void {
        switch (eventType) {
            case 'CHECKIN':
                element.classList.add('checkin-animation');
                setTimeout(() => element.classList.remove('checkin-animation'), 2000);
                break;
            case 'CHECKOUT':
                element.classList.add('checkout-animation');
                setTimeout(() => element.classList.remove('checkout-animation'), 2000);
                break;
        }
    }

    private getStatusClass(status: string): string {
        const statusMap: { [key: string]: string } = {
            'Livre': 'livre',
            'Ocupado': 'ocupado',
            'Manutencao': 'manutencao',
            'Limpeza': 'limpeza',
            'Bloqueado': 'bloqueado'
        };
        return statusMap[status] || 'desconhecido';
    }

    // ========================================
    // CALLBACK CUSTOMIZÁVEL
    // ========================================
    
    public onRackRefresh?: () => void;
}

// ========================================
// USO PRÁTICO
// ========================================

// Inicialização do serviço
const rackService = new RackSignalRService();

// Configurar callback customizado
rackService.onRackRefresh = () => {
    console.log("🔄 Callback personalizado: atualizando interface...");
    // Sua lógica personalizada aqui
};

// Conectar ao hub
rackService.start();

// Entrar em grupos específicos
rackService.joinGroup("Floor1");
rackService.joinGroup("VIP");

// Exemplo de uso em componente Angular
export { RackSignalRService };
