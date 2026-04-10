using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
  public interface IUnitOfWork : IDisposable
  {

    IRepositoryBase<T> GetRepository<T>() where T : class;
    public IApartamentoRepository Apartamento { get; }
    public ITipoApartamentoRepository TipoApartamento { get; }
    public ICheckinRepository checkins { get; }
    public IHospedagemRepository Hospedagem { get; }
    public IClienteRepository clientes { get; }
    public IHospedeRepository hospedes { get; }
    public ICaixaRepository caixa { get; }
    public IPagamentoRepository pagamentos { get; }
    public ILancamentoCaixaRepository lancamentoCaixa { get; }
    public IMenuRepository Menu { get; }
    public IMenuRoleRepository MenuRole { get; }
    public IParamRepository Param { get; }
    public IEmpresaRepository Empresa { get; }
    public IPaisRepository Paises{get;}
    public IHistoricoRepository historico { get;}
    public ITipoHospedagemRepository TipoHospedagem { get; }
    public IMotivoViagemRepository MotivoViagem { get; }
    public ITipoPagamentoRepository TipoPagamentos{get; set;}
    public IApartamentoReservadoRepository apartamentoReservado{get;}
    public ISeriesRepository series  { get;  }
    public IAppConfigRepository AppConfig { get;  }
    public ITipoReciboRepository TipoRecibo { get;  }
    public  ITaxTableEntryRepository  TaxTableEntry { get;  }
    public  ITaxExemptionReasonRepository  TaxExemptionReason { get;  }
    public  ITaxAccountingRepository  TaxAccountingBase { get;  }
    public ITransferenciaQuartoRepository TransferenciaQuartos { get; }
    public IMotivoTransferenciaRepository MotivoTransferencia { get; }
    public IProdutoRepository Produto {get;}
    public IProdutoStockRepository ProdutoStock {get;}
      public IUtilizadorRepository Utilizadores { get; } 

    
    // public ISeriesRepository Series {get;}
    // public IRoleRepository Perfil { get; }
  public IFacturaEmpresaRepository Factura { get; }
  public IReservaRepository Reservas{get;}
  public IPlanoDeContaRepository PlanoDeConta { get; }
  public IMessageRepository Messages { get; }
  public IConversationRepository Conversations { get; }
  public IMessageNotificationRepository MessageNotifications { get; }
  public IOnlineStatusRepository OnlineStatuses { get; }
  public IConversationParticipantRepository ConversationParticipants { get; }
  public ITransferenciaRepository Transferencias { get; }
   public IParamRepository   Parametros { get; }
  public ILancamentoCaixaRepository lancamentoCaixas { get; } 

  public IEmpresaSaldoRepository EmpresaSaldo { get; }
    public IContaPagarRepository ContasPagar { get; }
  public IContaReceberRepository ContasReceber { get; }
    public IPlanoDeContaRepository PlanoDeContas { get; }
  public IPedidoRepository Pedidos { get; }

    Task<int> Save();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollBackAsync();
  }
}