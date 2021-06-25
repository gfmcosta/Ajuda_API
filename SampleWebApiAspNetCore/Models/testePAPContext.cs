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

            });

            modelBuilder.Entity<Marcacao>(entity => {
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

                entity.HasOne(d => d.PacienteNavigation)
                    .WithMany(p => p.Marcacao)
                    .HasForeignKey(d => d.IdPaciente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Marcacao_Paciente");

                entity.HasOne(d => d.FuncionarioNavigation)
                    .WithMany(p => p.Marcacao)
                    .HasForeignKey(d => d.IdFuncionario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Marcacao_Funcionario");

                //entity.HasOne(d => d.TecnicoNavigation)
                //    .WithMany(p => p.Marcacao)
                //    .HasForeignKey(d => d.IdTecnico)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Marcacao_Tecnico");
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

                entity.HasOne<Funcionario>(u => u.Funcionario)
                    .WithOne(f => f.UtilizadorNavigation)
                    .HasForeignKey<Funcionario>(f => f.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Funcionario_Utilizador");

                entity.HasOne<Paciente>(u => u.Paciente)
                    .WithOne(p => p.UtilizadorNavigation)
                    .HasForeignKey<Paciente>(p => p.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paciente_Utilizador");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
