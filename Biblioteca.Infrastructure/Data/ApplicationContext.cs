using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    public DbSet<AutorEntity> Autor { get; set; }
    public DbSet<CategoriaEntity> Categoria { get; set; }
    public DbSet<LivroEntity> Livro { get; set; }
    public DbSet<EmprestimoEntity> Emprestimo { get; set; }
    public DbSet<UserEntity> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutorEntity>(e =>
        {
            e.ToTable("tb_bib_autor");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.Nacionalidade).HasMaxLength(80);
        });

        modelBuilder.Entity<CategoriaEntity>(e =>
        {
            e.ToTable("tb_bib_categoria");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(80);
        });

        modelBuilder.Entity<LivroEntity>(e =>
        {
            e.ToTable("tb_bib_livro");
            e.HasKey(x => x.Id);
            e.Property(x => x.Isbn).IsRequired().HasMaxLength(20);
            e.Property(x => x.Titulo).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Isbn).IsUnique().HasDatabaseName("IDX_bib_livro_isbn");
            e.HasIndex(x => new { x.Titulo, x.AutorId }).HasDatabaseName("IDX_bib_livro_titulo_autor");
            e.HasOne(x => x.Autor).WithMany(a => a.Livros).HasForeignKey(x => x.AutorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Categoria).WithMany(c => c.Livros).HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserEntity>(e =>
        {
            e.ToTable("tb_bib_usuario");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(200);
            e.Property(x => x.Role).IsRequired().HasMaxLength(30);
            e.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IDX_bib_usuario_email");
        });

        modelBuilder.Entity<EmprestimoEntity>(e =>
        {
            e.ToTable("tb_bib_emprestimo");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.LivroId, x.DataDevolucao }).HasDatabaseName("IDX_bib_emprestimo_livro_devol");
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Livro).WithMany().HasForeignKey(x => x.LivroId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
