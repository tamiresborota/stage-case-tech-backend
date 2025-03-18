using CaseStage.Areas;
using CaseStage.DetalheProcesso;
using CaseStage.Processos;
using Microsoft.EntityFrameworkCore;

namespace CaseStage.Data;

public class AppDbContext : DbContext
{
    public DbSet<Area> Areas { get; set; }
    public DbSet<Processo> Processos { get; set; }
    public DbSet<ProcessoDetalhe> ProcessosDetalhes { get; set; }
    public DbSet<FerramentaProcesso> FerramentasProcesso { get; set; }
    public DbSet<ResponsavelProcesso> ResponsaveisProcesso { get; set; }
    public DbSet<DocumentoProcesso> DocumentosProcesso { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=BancoDeDados.sqlite");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.EnableSensitiveDataLogging();
        
        base.OnConfiguring(optionsBuilder);    
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Processo>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            // Configuração da relação hierárquica
            entity.HasOne(p => p.ProcessoPai)
                .WithMany(p => p.SubProcessos)
                .HasForeignKey(p => p.ProcessoPaiId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            
            // Configuração da relação com área
            entity.HasOne(p => p.Area)
                .WithMany()
                .HasForeignKey(p => p.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Aplicação do soft delete
            entity.HasQueryFilter(p => !p.IsDeleted);
            
            // Configuração de ProcessoDetalhe
            modelBuilder.Entity<ProcessoDetalhe>()
                .HasOne(pd => pd.Processo)
                .WithOne()
                .HasForeignKey<ProcessoDetalhe>(pd => pd.ProcessoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configuração de FerramentaProcesso
            modelBuilder.Entity<FerramentaProcesso>()
                .HasOne(pt => pt.ProcessoDetalhe)
                .WithMany(pd => pd.Ferramentas)
                .HasForeignKey(pt => pt.ProcessoDetalheId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configuração de ResponsavelProcesso
            modelBuilder.Entity<ResponsavelProcesso>()
                .HasOne(pr => pr.ProcessoDetalhe)
                .WithMany(pd => pd.Responsaveis)
                .HasForeignKey(pr => pr.ProcessoDetalheId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configuração de DocumentoProcesso
            modelBuilder.Entity<DocumentoProcesso>()
                .HasOne(pd => pd.ProcessoDetalhe)
                .WithMany(detail => detail.Documentos)
                .HasForeignKey(pd => pd.ProcessoDetalheId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
        

    }

