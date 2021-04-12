using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SampleWebApiAspNetCore.Models
{
    public partial class testePAPContext : DbContext
    {
        public testePAPContext()
        {
        }

        public testePAPContext(DbContextOptions<testePAPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Funcao> Funcao { get; set; }
        public virtual DbSet<Funcionario> Funcionario { get; set; }
        public virtual DbSet<FuncionarioMarcacao> FuncionarioMarcacao { get; set; }
        public virtual DbSet<Marcacao> Marcacao { get; set; }
        public virtual DbSet<Paciente> Paciente { get; set; }
        public virtual DbSet<Utilizador> Utilizador { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Funcao>(entity =>
            {
                entity.HasKey(e => e.IdFuncao);

                entity.ToTable("Funcao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.HasKey(e => e.IdFuncionario);

                entity.ToTable("Funcionario");

                entity.Property(e => e.Cc)
                    .IsRequired()
                    .HasMaxLength(9)
                    .HasColumnName("CC");

                entity.Property(e => e.DataNasc).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Nacionalidade).HasMaxLength(50);

                entity.Property(e => e.Nif)
                    .IsRequired()
                    .HasMaxLength(9)
                    .HasColumnName("NIF");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sexo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Telemovel)
                    .IsRequired()
                    .HasMaxLength(9);

                entity.HasOne(d => d.FuncaoNavigation)
                    .WithMany(p => p.Funcionarios)
                    .HasForeignKey(d => d.Funcao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Funcionario_Funcao");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Funcionarios)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Funcionario_Utilizador");
            });

            modelBuilder.Entity<FuncionarioMarcacao>(entity =>
            {
                entity.HasKey(e => e.IdFuncionarioMarcacao);

                entity.ToTable("Funcionario_Marcacao");

                entity.Property(e => e.IdFuncionarioMarcacao).HasColumnName("IdFuncionario_Marcacao");

                entity.HasOne(d => d.IdFuncionarioNavigation)
                    .WithMany(p => p.FuncionarioMarcacaos)
                    .HasForeignKey(d => d.IdFuncionario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Funcionario_Marcacao_Funcionario");

                entity.HasOne(d => d.IdMarcacaoNavigation)
                    .WithMany(p => p.FuncionarioMarcacaos)
                    .HasForeignKey(d => d.IdMarcacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Funcionario_Marcacao_Marcacao");
            });

            modelBuilder.Entity<Marcacao>(entity =>
            {
                entity.HasKey(e => e.IdMarcacao);

                entity.ToTable("Marcacao");

                entity.Property(e => e.Data).HasColumnType("date");

                entity.Property(e => e.Qrcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("QRCODE");

                entity.Property(e => e.Relatorio)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UltimaAtualizacao)
                    .HasColumnType("datetime")
                    .HasColumnName("Ultima_atualizacao");

                entity.HasOne(d => d.IdPacienteNavigation)
                    .WithMany(p => p.Marcacaos)
                    .HasForeignKey(d => d.IdPaciente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Marcacao_Paciente");
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.IdPaciente);

                entity.ToTable("Paciente");

                entity.Property(e => e.Cc)
                    .IsRequired()
                    .HasMaxLength(9)
                    .HasColumnName("CC");

                entity.Property(e => e.DataNasc).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Nacionalidade).HasMaxLength(50);

                entity.Property(e => e.Nif)
                    .IsRequired()
                    .HasMaxLength(9)
                    .HasColumnName("NIF");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sexo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Telemovel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Pacientes)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paciente_Utilizador");
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasKey(e => e.IdUtilizador);

                entity.ToTable("Utilizador");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Senha)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
