// rack-signalr.js
import * as signalR from "@microsoft/signalr";

class RackSignalRService {
    constructor() {
        this.connection = null;
        this.isConnected = false;
    }

    async iniciarConexao() {
        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/rackHub")
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .build();

            // Configurar eventos
            this.configurarEventos();

            // Iniciar conexão
            await this.connection.start();
            this.isConnected = true;
            console.log("Conectado ao RackHub SignalR");

            // Entrar no grupo do rack
            await this.connection.invoke("JoinRackGroup");

        } catch (error) {
            console.error("Erro ao conectar ao SignalR:", error);
            this.isConnected = false;
        }
    }

    configurarEventos() {
        // Atualização geral do rack
        this.connection.on("AtualizarRack", () => {
            console.log("Atualizando rack completo...");
            this.atualizarRackCompleto();
        });

        // Status de apartamento alterado
        this.connection.on("StatusApartamentoAlterado", (data) => {
            console.log("Status do apartamento alterado:", data);
            this.atualizarStatusApartamento(data.ApartamentoId, data.NovoStatus);
        });

        // Nova reserva
        this.connection.on("NovaReserva", (data) => {
            console.log("Nova reserva criada:", data);
            this.processarNovaReserva(data.ApartamentoId);
        });

        // Check-in realizado
        this.connection.on("CheckInRealizado", (data) => {
            console.log("Check-in realizado:", data);
            this.processarCheckIn(data.ApartamentoId);
        });

        // Check-out realizado
        this.connection.on("CheckOutRealizado", (data) => {
            console.log("Check-out realizado:", data);
            this.processarCheckOut(data.ApartamentoId);
        });

        // Eventos de conexão
        this.connection.onreconnecting(() => {
            console.log("Reconectando ao SignalR...");
            this.isConnected = false;
        });

        this.connection.onreconnected(() => {
            console.log("Reconectado ao SignalR");
            this.isConnected = true;
            this.connection.invoke("JoinRackGroup");
        });

        this.connection.onclose(() => {
            console.log("Conexão SignalR fechada");
            this.isConnected = false;
        });
    }

    // Métodos para atualizar a interface
    atualizarRackCompleto() {
        // Implementar lógica para recarregar todos os dados do rack
        // Exemplo: fazer uma nova requisição para a API e atualizar a tela
        location.reload(); // Solução simples, mas pode ser otimizada
    }

    atualizarStatusApartamento(apartamentoId, novoStatus) {
        // Implementar lógica para atualizar apenas um apartamento específico
        const apartamentoElement = document.querySelector(`[data-apartamento-id="${apartamentoId}"]`);
        if (apartamentoElement) {
            apartamentoElement.className = `apartamento ${novoStatus.toLowerCase()}`;
            apartamentoElement.querySelector('.status').textContent = novoStatus;
        }
    }

    processarNovaReserva(apartamentoId) {
        // Implementar lógica específica para nova reserva
        this.atualizarStatusApartamento(apartamentoId, "Reservado");
        this.mostrarNotificacao(`Nova reserva para apartamento ${apartamentoId}`);
    }

    processarCheckIn(apartamentoId) {
        // Implementar lógica específica para check-in
        this.atualizarStatusApartamento(apartamentoId, "Ocupado");
        this.mostrarNotificacao(`Check-in realizado no apartamento ${apartamentoId}`);
    }

    processarCheckOut(apartamentoId) {
        // Implementar lógica específica para check-out
        this.atualizarStatusApartamento(apartamentoId, "Livre");
        this.mostrarNotificacao(`Check-out realizado no apartamento ${apartamentoId}`);
    }

    mostrarNotificacao(mensagem) {
        // Implementar sistema de notificações
        console.log("Notificação:", mensagem);
        // Pode usar bibliotecas como Toastr, SweetAlert, etc.
    }

    async pararConexao() {
        if (this.connection && this.isConnected) {
            await this.connection.invoke("LeaveRackGroup");
            await this.connection.stop();
            this.isConnected = false;
        }
    }
}

// Exportar para uso global
window.RackSignalR = new RackSignalRService();

// Iniciar automaticamente quando a página carregar
document.addEventListener('DOMContentLoaded', () => {
    window.RackSignalR.iniciarConexao();
});

// Parar conexão quando a página for fechada
window.addEventListener('beforeunload', () => {
    window.RackSignalR.pararConexao();
});

export default RackSignalRService;
