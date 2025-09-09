using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncionarioCadastro.Models;
using Microsoft.EntityFrameworkCore;

namespace FuncionarioCadastro.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        // DbSets = Tabelas
        public DbSet<Funcionario> Funcionario {  get; set; }
        public DbSet<FuncionarioCNH> FuncionarioCNH {  get; set; }
        public DbSet<FuncionarioCTPS> FuncionarioCTPS {  get; set; }
        public DbSet<FuncionarioCurso> FuncionarioCurso {  get; set; }
        public DbSet<FuncionarioEndereco> FuncionarioEndereco {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PK
            modelBuilder.Entity<Funcionario>().HasKey(x => x.Id);
            modelBuilder.Entity<FuncionarioCNH>().HasKey(x => x.Id);
            modelBuilder.Entity<FuncionarioCTPS>().HasKey(x => x.IdFuncionario);
            modelBuilder.Entity<FuncionarioCurso>().HasKey(x => x.Id);
            modelBuilder.Entity<FuncionarioEndereco>().HasKey(x => x.Id);

            // 1:1
            modelBuilder.Entity<Funcionario>()
                .HasOne(f => f.CNH)
                .WithOne(c => c.Funcionario)
                .HasForeignKey<FuncionarioCNH>(c => c.IdFuncionario);

            modelBuilder.Entity<Funcionario>()
                .HasOne(f => f.CTPS)
                .WithOne(c => c.Funcionario)
                .HasForeignKey<FuncionarioCTPS>(c => c.IdFuncionario);

            // 1:N
            modelBuilder.Entity<Funcionario>()
                .HasMany(f => f.Curso)
                .WithOne(c => c.Funcionario)
                .HasForeignKey(c => c.IdFuncionario);

            modelBuilder.Entity<Funcionario>()
                .HasMany(f => f.Endereco)
                .WithOne(c => c.Funcionario)
                .HasForeignKey(c => c.IdFuncionario);
        }

    }
}
