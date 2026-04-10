using Hotel.Application.Interfaces;
using Hotel.Domain.Common;
using Hotel.Domain.Entities;
using Hotel.Domain.Identity;
using Hotel.Infrastruture.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using static System.Console;
namespace Hotel.Infrastruture.Persistence.Context
{

    public class GhotelDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string> //DbContext
    {


        #region "Dbset"

        public DbSet<Serial> Serial { get; set; }
        public DbSet<DocumentosVenda> DocumentosVendas { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<AppMenu> AppMenu { get; set; }
        
        public DbSet<MenuRole> MenuRole { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; } //adicionei esta tabela para gerenciar o meu dos relatórios
        public DbSet<AppConfig> AppConfig { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Acesso> Acessos { get; set; }
        public DbSet<Apartamentos> Apartamentos { get; set; }
        public DbSet<ApartamentosReservado> ApartamentosReservados { get; set; }
        public DbSet<Checkins> Checkins { get; set; }
        public DbSet<Caixa> Caixas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Despertador> Despertadores { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<FacturaDividida> FacturaDivididas { get; set; }
        public DbSet<FacturaEmpresa> FacturaEmpresas { get; set; }
        public DbSet<Governanca> Governancas { get; set; }
        public DbSet<Hospedagem> Hospedagems { get; set; }
        public DbSet<Hospede> Hospedes { get; set; }
        public DbSet<ItemPedido> ItemPedidos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<PontoDeVenda> PonteDeVendas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<TipoApartamento> TipoApartamentos { get; set; }
        public DbSet<TipoGovernanca> TipoGovernancas { get; set; }
        public DbSet<TipoHospedagem> TipoHospedagens { get; set; }
        public DbSet<TipoPagamento> TipoPagamentos { get; set; }
        public DbSet<TipoRecibo> TipoRecibos { get; set; }
        public DbSet<ApplicationUser> Utilizadores { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<PlanoDeConta> PlanoDeContas { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Produtos> Produtos { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<Tax> Tax { get; set; }
        public DbSet<TaxAccountingBasis> TaxAccountingBasis { get; set; }
        public DbSet<TaxExemptionReason> TaxExemptionReasons { get; set; }
        public DbSet<TaxTableEntry> TaxTableEntry { get; set; }
        public DbSet<TaxTypes> TaxTypes { get; set; }
        public DbSet<TipoProduto> TipoProdutos { get; set; }
        public DbSet<Lavandaria> lavandarias { get; set; }
        public DbSet<LavandariaItem> lavandariaItens { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<LancamentoCaixa> LancamentoCaixas { get; set; }
        public DbSet<MobiliaApartamento> MobiliaApartamentos { get; set; }
        public DbSet<MobiliaTipoApartamento> MobiliaTipoApartamentos { get; set; }
        public DbSet<MotivoViagem> MotivoViagens { get; set; }
         public DbSet<MotivoTransferencia> MotivoTransferencias { get; set; }
        public DbSet<Patrimonio> Patrimonios { get; set; }
        public DbSet<ProdutoStock> ProdutoStocks { get; set; }
        public DbSet<TransferenciaQuarto> TransferenciaQuartos { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }

        // ✅ NOVOS DBSETS PARA MENSAGENS
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<MessageNotification> MessageNotifications { get; set; }
        public DbSet<OnlineStatus> OnlineStatuses { get; set; }

        public DbSet<EmpresaSaldo> EmpresaSaldos { get; set; }
        public DbSet<EmpresaSaldoMovimento> EmpresaSaldoMovimentos { get; set; }
        public DbSet<ContaReceber> ContasReceber { get; set; }
        public DbSet<ContaPagar> ContasPagar { get; set; }

        #endregion
        readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;

        public GhotelDbContext(DbContextOptions<GhotelDbContext> dbContextOptions, ITenantService tenantService, IConfiguration configuration) : base(dbContextOptions)
        {
            _tenantService = tenantService;
            _configuration = configuration;
        }

        public GhotelDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (_tenantService?.GetCurrentTenant().ConnectionString != null)
            {
                var currentTenant = _tenantService.GetCurrentTenant();
                optionsBuilder.UseSqlServer(currentTenant.ConnectionString)
                .LogTo(WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();
            }
            else
            {
                // Configuração alternativa, se _tenantService for nulo
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("GHotelBbContext"))
                .LogTo(WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();
            }

            base.OnConfiguring(optionsBuilder);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //  modelBuilder.HasSequence<int> ("Id");

            // Renomear a tabela padrão AspNetUsers para Utilizador
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Utilizador");
            });

            modelBuilder.Entity<MenuItem>()
        .HasOne(m => m.Parent)
        .WithMany(m => m.Children)
        .HasForeignKey(m => m.ParentId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Produtos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId);

            /*    modelBuilder.Entity<TransferenciaQuarto>()
                  .HasOne(t => t.Apartamentos)
                  .WithMany(h => h.TransferenciaQuartosOrigem)
                  .HasForeignKey(t => t.QuartoId)
                  .OnDelete(DeleteBehavior.Restrict);

               modelBuilder.Entity<TransferenciaQuarto>()
                  .Property(t => t.TipoTransferencia)
                  .IsRequired()
                  .HasConversion<string>() // ✅ Armazena como string no banco
                  .HasMaxLength(20); */





            /* modelBuilder.Entity(e => e.TipoTransferencia)
                       .IsRequired()
                       .HasConversion<string>() // ✅ Armazena como string no banco
                       .HasMaxLength(20);  */
            /*    modelBuilder.Entity<TransferenciaQuarto>()
                   .HasOne(t => t.ApartamentoDestino)
                   .WithMany(h => h.TransferenciaQuartosDestino)
                   .HasForeignKey(t => t.QuartoDestinoId)
                   .OnDelete(DeleteBehavior.Restrict);  */
            /* 
                    modelBuilder.Entity<TransferenciaQuarto>()
                        .HasOne(t => t.MotivoTransferencia)
                        .WithMany(m => m.Transferencias)
                        .HasForeignKey(t => t.MotivoTransferenciaId)
                        .OnDelete(DeleteBehavior.Restrict); 

                    modelBuilder.Entity<TransferenciaQuarto>()
                        .HasOne(t => t.Checkins)
                        .WithMany(c => c.Transferencias)
                        .HasForeignKey(t => t.CheckinId)
                        .OnDelete(DeleteBehavior.Restrict);
                    modelBuilder.Entity<TransferenciaQuarto>()
                        .HasOne(t => t.Utilizador)
                        .WithMany(c => c.TransferenciaQuartos)
                        .HasForeignKey(t => t.UtilizadorId)
                        .OnDelete(DeleteBehavior.Restrict); */


            // ✅ Configuração CORRETA para TransferenciaQuarto


            modelBuilder.Entity<TransferenciaQuarto>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.ToTable("TransferenciaQuartos");



                    // ✅ Configuração das propriedades
                    entity.Property(e => e.CheckinId)
                        .IsRequired();

                    entity.Property(e => e.QuartoId)
                        .IsRequired();

                    entity.Property(e => e.MotivoTransferenciaId)
                        .IsRequired();

                    entity.Property(e => e.TipoTransferencia)
                        .IsRequired()
                        .HasConversion<string>()
                        .HasMaxLength(20);

                    entity.Property(e => e.ManterPreco)
                        .IsRequired()
                        .HasDefaultValue(true);

                    entity.Property(e => e.ValorDiaria)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                    entity.Property(e => e.DataEntrada)
                        .IsRequired();

                    entity.Property(e => e.DataSaida)
                        .IsRequired();

                    entity.Property(e => e.DataTransferencia)
                        .IsRequired();

                    entity.Property(e => e.Observacao)
                        .HasMaxLength(500);

                    entity.Property(e => e.UtilizadorId)
                        .IsRequired()
                        .HasMaxLength(450);

                    // ✅ Relacionamentos corretos - UM POR UM
                    entity.HasOne(e => e.Checkins)
                        .WithMany(c => c.Transferencias)
                        .HasForeignKey(e => e.CheckinId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Apartamentos)
                        .WithMany() // Sem propriedade de navegação reversa
                        .HasForeignKey(e => e.QuartoId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.MotivoTransferencia)
                        .WithMany(m => m.Transferencias)
                        .HasForeignKey(e => e.MotivoTransferenciaId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Utilizador)
                        .WithMany() // Sem propriedade de navegação reversa
                        .HasForeignKey(e => e.UtilizadorId)
                        .OnDelete(DeleteBehavior.Restrict);
                    modelBuilder.Entity<TransferenciaQuarto>()
       .Property(t => t.TipoTransferencia)
       .IsRequired()
       .HasConversion<string>() // ✅ Armazena como string no banco
       .HasMaxLength(20);

                });

            #region "Configurações de Mensagens"
            // ✅ CONFIGURAÇÃO DA ENTIDADE MESSAGE
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Messages");

                // Propriedades
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.ConversationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.MessageType)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.Property(e => e.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ✅ RELACIONAMENTOS EXPLÍCITOS PARA EVITAR COLUNAS EXTRAS
                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Messages_Sender");

                entity.HasOne(e => e.Receiver)
                    .WithMany()
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Messages_Receiver");

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .HasPrincipalKey(c => c.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Messages_Conversation");

                // Índices
                entity.HasIndex(e => e.ConversationId)
                    .HasDatabaseName("IX_Messages_ConversationId");
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName("IX_Messages_Timestamp");
                entity.HasIndex(e => new { e.SenderId, e.ReceiverId })
                    .HasDatabaseName("IX_Messages_SenderId_ReceiverId");
                entity.HasIndex(e => new { e.ReceiverId, e.IsRead })
                    .HasDatabaseName("IX_Messages_ReceiverId_IsRead");
            });

            // ✅ CONFIGURAÇÃO DA ENTIDADE CONVERSATION
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Conversations");

                // Propriedades
                entity.Property(e => e.ConversationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UnreadCount)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.UpdatedAt)
                    .IsRequired();

                // ✅ RELACIONAMENTO COM LAST MESSAGE
                entity.HasOne(e => e.LastMessage)
                    .WithMany()
                    .HasForeignKey(e => e.LastMessageId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Conversations_LastMessage");

                // ✅ RELACIONAMENTO COM MESSAGES (Inverso do acima)
                entity.HasMany(e => e.Messages)
                    .WithOne(m => m.Conversation)
                    .HasForeignKey(m => m.ConversationId)
                    .HasPrincipalKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ✅ RELACIONAMENTO COM PARTICIPANTS
                entity.HasMany(e => e.Participants)
                    .WithOne(p => p.Conversation)
                    .HasForeignKey(p => p.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.ConversationId)
                    .IsUnique()
                    .HasDatabaseName("IX_Conversations_ConversationId");
                entity.HasIndex(e => e.UpdatedAt)
                    .HasDatabaseName("IX_Conversations_UpdatedAt");
            });

            // ✅ CONFIGURAÇÃO DA ENTIDADE CONVERSATION PARTICIPANT
            modelBuilder.Entity<ConversationParticipant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("ConversationParticipants");

                // Propriedades
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.JoinedAt)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ✅ RELACIONAMENTOS EXPLÍCITOS
                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ConversationParticipants_Conversation");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ConversationParticipants_User");

                // Índices
                entity.HasIndex(e => new { e.ConversationId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("IX_ConversationParticipants_ConversationUser");
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_ConversationParticipants_UserId");
            });

            // ✅ CONFIGURAÇÃO DA ENTIDADE MESSAGE NOTIFICATION
            modelBuilder.Entity<MessageNotification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("MessageNotifications");

                // Propriedades
                entity.Property(e => e.SenderId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ReceiverId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.IsNew)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                // ✅ RELACIONAMENTOS EXPLÍCITOS
                entity.HasOne(e => e.Message)
                    .WithMany()
                    .HasForeignKey(e => e.MessageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MessageNotifications_Message");

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MessageNotifications_Sender");

                entity.HasOne(e => e.Receiver)
                    .WithMany()
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_MessageNotifications_Receiver");

                // Índices
                entity.HasIndex(e => e.ReceiverId)
                    .HasDatabaseName("IX_MessageNotifications_ReceiverId");
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName("IX_MessageNotifications_Timestamp");
                entity.HasIndex(e => new { e.ReceiverId, e.IsRead })
                    .HasDatabaseName("IX_MessageNotifications_ReceiverId_IsRead");
            });

            // ✅ CONFIGURAÇÃO DA ENTIDADE ONLINE STATUS
            modelBuilder.Entity<OnlineStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("OnlineStatuses");

                // Propriedades
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.IsOnline)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.LastSeen)
                    .IsRequired();

                entity.Property(e => e.ConnectionId)
                    .HasMaxLength(100);

                // ✅ RELACIONAMENTO EXPLÍCITO
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OnlineStatuses_User");

                // Índices
                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_OnlineStatuses_UserId");
                entity.HasIndex(e => e.IsOnline)
                    .HasDatabaseName("IX_OnlineStatuses_IsOnline");
            });

            #endregion

            //cria as configurações de tansferência de quarto
            modelBuilder.Entity<MotivoTransferencia>().HasData(
                new MotivoTransferencia { Id = 1, Descricao = "Manutenção" },
                new MotivoTransferencia { Id = 2, Descricao = "Upgrade" },
                new MotivoTransferencia { Id = 3, Descricao = "Preferência do Cliente" },
                new MotivoTransferencia { Id = 4, Descricao = "Outros" }
            );

            #region "Configuration"
            // Opcional: renomear outras tabelas Identity
            modelBuilder.Entity<IdentityRole>(entity => entity.ToTable("Role"));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UtilizadorRoles"));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UtilizadorClaims"));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UtilizadorLogins"));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UtilizadorTokens"));


 // Configuração ContaReceber
            modelBuilder.Entity<ContaReceber>(entity =>
            {
                entity.HasOne(cr => cr.Empresa)
                    .WithMany()
                    .HasForeignKey(cr => cr.EmpresaId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();

                entity.HasOne(cr => cr.Checkins)
                    .WithMany()
                    .HasForeignKey(cr => cr.CheckinsId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);


            });

            // Configuração ContaPagar
            modelBuilder.Entity<ContaPagar>(entity =>
            {
                entity.HasOne(cp => cp.Empresa)
                    .WithMany()
                    .HasForeignKey(cp => cp.EmpresaId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);

                entity.Ignore(cp => cp.Empresa);
            });



            modelBuilder.Entity<IdentityRole>().HasData(
                            new IdentityRole { Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                            new IdentityRole { Id = "2", Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
                            new IdentityRole { Id = "3", Name = "Hotel", NormalizedName = "HOTEL" },
                            new IdentityRole { Id = "4", Name = "Finanças", NormalizedName = "FINANÇAS" },
                            new IdentityRole { Id = "5", Name = "Lavandaria", NormalizedName = "LAVANDARIA" }
                        );
            modelBuilder.Entity<TipoRecibo>().HasData
            (
                new TipoRecibo { Id = 1, Descricao = "Recibo Grande", RPT = "CrReciboNovo.rpt" },
                new TipoRecibo { Id = 2, Descricao = "Recibo Pequeno", RPT = "CrReciboP.rpt" },
                new TipoRecibo { Id = 3, Descricao = "Ticket", RPT = "CrReciboP2.rpt" }
            );

            modelBuilder.Entity<DocumentosVenda>().HasData(
                   new DocumentosVenda { Documento = "AA", Descricao = "Alienação de activo", Diario = null, PagarReceber = null, TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "AC", Descricao = "Aviso de cobrança", Diario = "54", PagarReceber = "R", TipoConta = "CCT", Estado = "PEN" },
                   new DocumentosVenda { Documento = "AF", Descricao = "Factura/recibo (autofacturação)", Diario = null, PagarReceber = "R", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "AR", Descricao = "Aviso de cobrança/recibo", Diario = null, PagarReceber = "R", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "DA", Descricao = "Devoluçao de activo", Diario = null, PagarReceber = null, TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "FG", Descricao = "Factura global", Diario = "52", PagarReceber = "R", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "FR", Descricao = "Factura/recibo", Diario = "51", PagarReceber = "P", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "FS", Descricao = "Factura genérica", Diario = "51", PagarReceber = "R", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "FT", Descricao = "Factura", Diario = "51", PagarReceber = "P", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "GR", Descricao = "Guia remessa", Diario = null, PagarReceber = "R", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "NC", Descricao = "Nota Crédito", Diario = "55", PagarReceber = "P", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "ND", Descricao = "Nota Débito", Diario = "55", PagarReceber = "R", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "RE", Descricao = "Recibo", Diario = null, PagarReceber = "R", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "TD", Descricao = "Talão de devolução", Diario = null, PagarReceber = null, TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "TS", Descricao = "Talão de serviços prestados", Diario = "56", PagarReceber = "R", TipoConta = "CCC", Estado = "PEN" },
                   new DocumentosVenda { Documento = "TV", Descricao = "Talão de venda", Diario = "1", PagarReceber = "R", TipoConta = null, Estado = null },
                   new DocumentosVenda { Documento = "VD", Descricao = "Venda-a-Dinheiro", Diario = null, PagarReceber = "R", TipoConta = null, Estado = null }
               );


            modelBuilder.Ignore<BaseDomainEntity>();
            modelBuilder.ApplyConfiguration(new ApartamentoConfiguration());
            modelBuilder.ApplyConfiguration(new ApartamentoReservadoConfiguration());
            modelBuilder.ApplyConfiguration(new AppMenuConfiguration());
            modelBuilder.ApplyConfiguration(new MenuRoleConfiguration());
            modelBuilder.ApplyConfiguration(new CheckinConfiguration());
            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new CaixaConfiguration());
            modelBuilder.ApplyConfiguration(new CheckinConfiguration());
            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            modelBuilder.ApplyConfiguration(new ContaConfiguration());
            modelBuilder.ApplyConfiguration(new EmpresaConfiguration());
            modelBuilder.ApplyConfiguration(new FacturaDivididaConfiguration());
            modelBuilder.ApplyConfiguration(new FacturaEmpresaConfiguration());
            modelBuilder.ApplyConfiguration(new GovernancaConfiguration());
            modelBuilder.ApplyConfiguration(new HistoricoConfiguration());
            modelBuilder.ApplyConfiguration(new HospedagemConfiguration());
            modelBuilder.ApplyConfiguration(new HospedeConfiguration());
            modelBuilder.ApplyConfiguration(new LancamentoCaixaConfiguration());
            modelBuilder.ApplyConfiguration(new LavandariaConfiguration());
            modelBuilder.ApplyConfiguration(new LavandariaItemConfiguration());
            modelBuilder.ApplyConfiguration(new MobiliaApartamentoConfiguration());
            modelBuilder.ApplyConfiguration(new MobiliaTipoApartamentoConfiguration());
            modelBuilder.ApplyConfiguration(new MotivoViagemConfiguration());
            modelBuilder.ApplyConfiguration(new PagamentoConfiguration());
            modelBuilder.ApplyConfiguration(new PaisConfiguration());
            modelBuilder.ApplyConfiguration(new PatrimonioConfiguration());
            modelBuilder.ApplyConfiguration(new PedidoConfiguration());
            modelBuilder.ApplyConfiguration(new PerfilConfiguration());
            modelBuilder.ApplyConfiguration(new PlanoDeContaConfiguration());
            modelBuilder.ApplyConfiguration(new PontoDeVendaConfiguration());
            modelBuilder.ApplyConfiguration(new ProductTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
            modelBuilder.ApplyConfiguration(new ProdutoStockConfiguration());
            modelBuilder.ApplyConfiguration(new ReservaConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());
            modelBuilder.ApplyConfiguration(new TaxExemptionReasonConfiguration());
            modelBuilder.ApplyConfiguration(new TaxTableEntryConfiguration());
            modelBuilder.ApplyConfiguration(new TaxTypesConfiguration());
            modelBuilder.ApplyConfiguration(new TipoApartamentoConfiguration());
            modelBuilder.ApplyConfiguration(new TipoGovernancaConfiguration());
            modelBuilder.ApplyConfiguration(new TipoHospedagemConfiguration());
            modelBuilder.ApplyConfiguration(new TipoPagamentoConfiguration());
            modelBuilder.ApplyConfiguration(new AppConfigConfiguratin());
            modelBuilder.ApplyConfiguration(new TransferenciaConfiguration());
            modelBuilder.ApplyConfiguration(new EmpresaSaldoConfiguration());
            modelBuilder.ApplyConfiguration(new EmpresaSaldoMovimentoConfiguration());

            #endregion



        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseDomainEntity>())

            {
                entry.Entity.LastModifiedDate = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }


    }
}