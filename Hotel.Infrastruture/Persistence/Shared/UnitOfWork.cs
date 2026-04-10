using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Apartamentos.Queries;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Repositories;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hotel.Infrastruture.Persistence.Shared
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly GhotelDbContext context;
        private readonly UsuarioLogado _usuarioLogado;
         private readonly IHttpContextAccessor _httpContextAccessor; // ✅ ADICIONAR

        private IDbContextTransaction  _transaction;
        private readonly Dictionary<Type , object> _repositories;
        public IApartamentoRepository Apartamento { get; }
        public ITipoApartamentoRepository TipoApartamento { get; }
        public ICheckinRepository checkins {get;}
        public IHospedagemRepository Hospedagem{get;}
        public IClienteRepository  clientes {get;}
        public IHospedeRepository hospedes{get;}
        public ICaixaRepository caixa{get;}
        public IPagamentoRepository pagamentos {get;}
        public ILancamentoCaixaRepository  lancamentoCaixa {get;}
        public IFacturaEmpresaRepository Factura{get;}
        public IMenuRepository Menu {get;}
        public IParamRepository Param {get;}
        public IMenuRoleRepository MenuRole {get;}
        public IEmpresaRepository Empresa{get;}
        public IPaisRepository Paises{get;}
        public ITipoHospedagemRepository TipoHospedagem { get;}
        public IHistoricoRepository historico { get;}
        public ITipoPagamentoRepository TipoPagamentos{get; set;}
        public IMotivoViagemRepository MotivoViagem{get;}
        public IReservaRepository Reservas{get;}
        public IApartamentoReservadoRepository apartamentoReservado{get;}
        public ISeriesRepository series  { get;  }
        public IAppConfigRepository AppConfig { get;  }
        public ITipoReciboRepository TipoRecibo { get;  }
        public  ITaxTableEntryRepository  TaxTableEntry { get;  }
        public  ITaxExemptionReasonRepository  TaxExemptionReason { get;  }
        public  ITaxAccountingRepository  TaxAccountingBase { get;  }
 private readonly IServiceProvider _serviceProvider;
        public IPlanoDeContaRepository PlanoDeConta { get; }
        public ITransferenciaQuartoRepository TransferenciaQuartos { get; }
        public IMotivoTransferenciaRepository MotivoTransferencia { get; }
         public IProdutoRepository Produto {get;}
        public IProdutoStockRepository ProdutoStock {get;}

        public ILancamentoCaixaRepository lancamentoCaixas { get; }
        public IUtilizadorRepository Utilizadores { get; } 

 public IMessageRepository Messages { get; }
  public IConversationRepository Conversations { get; }
  public IMessageNotificationRepository MessageNotifications { get; }
  public IOnlineStatusRepository OnlineStatuses { get; }
  public IConversationParticipantRepository ConversationParticipants { get; }
  public ITransferenciaRepository Transferencias { get; }
  public IParamRepository   Parametros { get; }
  public IEmpresaSaldoRepository EmpresaSaldo { get; }

  public IContaPagarRepository ContasPagar { get; }
  public IContaReceberRepository ContasReceber { get; }
  public IPlanoDeContaRepository PlanoDeContas { get; }
  public IPedidoRepository Pedidos { get; }
    
        //  public ISeriesRepository Series {get;}
        // public IRoleRepository Perfil{get;}

        public UnitOfWork(GhotelDbContext context, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, UsuarioLogado usuarioLogado)
        {
            this.context = context;
            Apartamento = new ApartamentoRepository(context, context);
            TipoApartamento = new TipoApartamentoRepository(context);
            checkins = new CheckinRepository(context);
            Hospedagem = new HospedagemRepository(context);
            hospedes = new HospedeRepository(context);
            clientes = new ClienteRepository(context);
            pagamentos = new PagamentoRepository(context);
            caixa = new CaixaRepository(context, _httpContextAccessor, _usuarioLogado);
            Factura = new FacturaEmpresaRepository(context);
            lancamentoCaixa = new LancamentoCaixaRepository(context);
            Menu = new MenuRepository(context);
            MenuRole = new MenuRoleRepository(context);
            Param = new ParamRepository(context);
            Empresa = new EmpresaRepository(context);
            Paises = new PaisRepository(context);
            TipoHospedagem = new TipoHospedagemRepository(context);
            MotivoViagem = new MotivoViagemRepository(context);
            TipoPagamentos = new TipoPagamentoRepository(context);
            historico = new HistoricoRepository(context);
            Reservas = new ReservaRepository(context);
            apartamentoReservado = new ApartamentosReservadoRepository(context);
            series = new SeriesRepository(context);
            AppConfig = new AppConfigRepository(context);
            TipoRecibo = new TipoReciboRepository(context);
            TaxTableEntry = new TaxTableEntryRepository(context);
            TaxExemptionReason = new TaxExemptionReasonRepository(context);
            TaxAccountingBase = new TaxAccountingBasisRepository(context);

            PlanoDeConta = new PlanoDeContaRepository(context);
            TransferenciaQuartos = new TransferenciaQuartoRepository(context);
            MotivoTransferencia = new MotivoTransferenciaRepository(context);
            Produto = new ProdutoRepository(context, context);
            ProdutoStock = new ProdutoStockRepository(context);

            ConversationParticipants = new ConversationParticipantRepository(context);
            Messages = new MessageRepository(context);
            Conversations = new ConversationRepository(context);
            MessageNotifications = new MessageNotificationRepository(context);
            OnlineStatuses = new OnlineStatusRepository(context);
            Transferencias = new TransferenciaRepository(context);
            Parametros = new ParamRepository(context);
            lancamentoCaixas = new LancamentoCaixaRepository(context);
            Utilizadores = new UtilizadorRepository(context, context);
            EmpresaSaldo = new EmpresaSaldoRepository(context);
            ContasPagar = new ContaPagarRepository(context);
            ContasReceber = new ContaReceberRepository(context);
            PlanoDeContas = new PlanoDeContaRepository(context);
            Pedidos = new PedidoRepository(context);
            // ✅ VERIFICAR SE HTTPCONTEXTACCESSOR FOI INJETADO
            var logger = serviceProvider.GetService<ILogger<UnitOfWork>>();
            logger?.LogInformation("UnitOfWork - HttpContextAccessor: {Status}",
                _httpContextAccessor == null ? "NULL" : "OK");

            //   Series = new SeriesRepository(context);

            //  Perfil = new RoleRepository(context);

            _repositories = new Dictionary<Type, object>();
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            _usuarioLogado = usuarioLogado;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
      

     //   public IApartamentoRepository Apartamento => _apartamento ??= new ApartamentoRepository(context);
     //      //  private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        public async Task<int> Save()
        {
           return await context.SaveChangesAsync();
        }
         public async Task BeginTransactionAsync()
        {
           _transaction =  await context.Database.BeginTransactionAsync();
        }

        public IRepositoryBase<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))  
             {
                return _repositories[typeof(T)] as IRepositoryBase<T>;
            }
            var Repository = new RepositoryBase<T>(context);
            _repositories.Add(typeof(T), Repository);
            return Repository;
        }

        public async Task CommitAsync()
        {
            
            try
            {
                await context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch 
            {
                
                await _transaction.RollbackAsync();
            }
           /*  finally 
            {
                
                await _transaction.DisposeAsync();
                _transaction = null;
            } */

        }

        public async Task RollBackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
