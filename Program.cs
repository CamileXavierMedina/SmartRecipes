using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRecipes.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURAÇÃO DOS SERVIÇOS (DI & INFRAESTRUTURA)
// ============================================================================

// Registo do Contexto do Banco de Dados usando SQLite local específico para SmartRecipes
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=smartrecipes.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilita a leitura do index.html dentro da pasta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// ============================================================================
// CONFIGURAÇÃO DO PIPELINE DE EXECUÇÃO (MIDDLEWARES)
// ============================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Inicialização automática do banco de dados (Criação e Carga Inicial)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
    DbSeeder.Seed(context);
}

app.Run();

// ============================================================================
// 1. CAMADA DE MODELS (ENTIDADES FÍSICAS DO BANCO)
// ============================================================================

namespace SmartRecipes.Models
{
    [Table("utilizadores")]
    public class Utilizador
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Propriedade de navegação para relacionamento físico
        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
    }

    [Table("categorias_culinarias")]
    public class CategoriaCulinaria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
    }

    [Table("receitas")]
    public class Receita
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column("ingredientes")]
        public string Ingredientes { get; set; } = string.Empty; // Lista de ingredientes em texto longo

        [Required]
        [Column("modo_preparo")]
        public string ModoPreparo { get; set; } = string.Empty; // Passo a passo em texto longo

        [Column("tempo_preparo_minutos")]
        public int TempoPreparoMinutos { get; set; }

        [Required]
        [Column("paladar")]
        public string Paladar { get; set; } = "SALGADO"; // SALGADO, DOCE

        [Required]
        [Column("temperatura")]
        public string Temperatura { get; set; } = "QUENTE"; // FRIA, QUENTE

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        [Column("utilizador_id")]
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador? Utilizador { get; set; }

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public CategoriaCulinaria? Categoria { get; set; }
    }
}

// ============================================================================
// 2. CAMADA DE DTOs (SERIALIZERS / DADOS DE ENTRADA E SAÍDA)
// ============================================================================

namespace SmartRecipes.DTOs
{
    public class CriarUtilizadorDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class CriarReceitaDto
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Ingredientes { get; set; } = string.Empty;

        [Required]
        public string ModoPreparo { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "O tempo de preparo deve ser de pelo menos 1 minuto.")]
        public int TempoPreparoMinutos { get; set; }

        [Required]
        public string Paladar { get; set; } = "SALGADO"; // SALGADO, DOCE

        [Required]
        public string Temperatura { get; set; } = "QUENTE"; // FRIA, QUENTE

        public int UtilizadorId { get; set; }
        public int CategoriaId { get; set; }
    }

    public class ExibirReceitaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Ingredientes { get; set; } = string.Empty;
        public string ModoPreparo { get; set; } = string.Empty;
        public int TempoPreparoMinutos { get; set; }
        public string Paladar { get; set; } = string.Empty;
        public string Temperatura { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }
}

// ============================================================================
// 3. CAMADA DE PERSISTÊNCIA (DATA CONTEXT E SEEDER DO SQLITE)
// ============================================================================

namespace SmartRecipes.Data
{
    using SmartRecipes.Models;

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Utilizador> Utilizadores { get; set; } = null!;
        public DbSet<CategoriaCulinaria> Categorias { get; set; } = null!;
        public DbSet<Receita> Receitas { get; set; } = null!;
    }

    public static class DbSeeder
    {
        public static void Seed(DataContext context)
        {
            // Criação de categorias culinárias padrão se o banco estiver vazio
            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new CategoriaCulinaria { Nome = "Massas" },
                    new CategoriaCulinaria { Nome = "Sobremesas" },
                    new CategoriaCulinaria { Nome = "Bebidas" },
                    new CategoriaCulinaria { Nome = "Carnes" },
                    new CategoriaCulinaria { Nome = "Sopas" }
                );
                context.SaveChanges();
            }

            // Criação de um utilizador inicial para testes
            if (!context.Utilizadores.Any())
            {
                context.Utilizadores.Add(new Utilizador
                {
                    Nome = "Chef Universitário",
                    Email = "chef@ipt.pt"
                });
                context.SaveChanges();
            }
        }
    }
}

// ============================================================================
// 4. CAMADA DE VIEWS E ROTAS (CONTROLLERS)
// ============================================================================

namespace SmartRecipes.Controllers
{
    using SmartRecipes.Data;
    using SmartRecipes.DTOs;
    using SmartRecipes.Models;

    [ApiController]
    [Route("api/[controller]")] // Definição de Rotas: api/utilizadores
    public class UtilizadoresController : ControllerBase
    {
        private readonly DataContext _context;

        public UtilizadoresController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarUtilizadorDto dto)
        {
            if (await _context.Utilizadores.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Este e-mail já está registrado.");

            var utilizador = new Utilizador
            {
                Nome = dto.Nome,
                Email = dto.Email
            };

            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ObterPorId), new { id = utilizador.Id }, utilizador);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos() => Ok(await _context.Utilizadores.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var user = await _context.Utilizadores.FindAsync(id);
            return user == null ? NotFound() : Ok(user);
        }
    }

    [ApiController]
    [Route("api/[controller]")] // Definição de Rotas: api/receitas
    public class ReceitasController : ControllerBase
    {
        private readonly DataContext _context;

        public ReceitasController(DataContext context)
        {
            _context = context;
        }

        // CADASTRAR RECEITA (POST)
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarReceitaDto dto)
        {
            var utilizadorExiste = await _context.Utilizadores.AnyAsync(u => u.Id == dto.UtilizadorId);
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

            if (!utilizadorExiste || !categoriaExiste)
                return BadRequest("O Utilizador ou a Categoria informados não existem no sistema.");

            var receita = new Receita
            {
                Titulo = dto.Titulo,
                Ingredientes = dto.Ingredientes,
                ModoPreparo = dto.ModoPreparo,
                TempoPreparoMinutos = dto.TempoPreparoMinutos,
                Paladar = dto.Paladar.ToUpper(),
                Temperatura = dto.Temperatura.ToUpper(),
                UtilizadorId = dto.UtilizadorId,
                CategoriaId = dto.CategoriaId
            };

            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = receita.Id }, receita);
        }

        // LISTAR TODAS AS RECEITAS COM RELACIONAMENTOS (GET)
        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var receitas = await _context.Receitas
                .Include(r => r.Utilizador)
                .Include(r => r.Categoria)
                .Select(r => new ExibirReceitaDto
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Ingredientes = r.Ingredientes,
                    ModoPreparo = r.ModoPreparo,
                    TempoPreparoMinutos = r.TempoPreparoMinutos,
                    Paladar = r.Paladar,
                    Temperatura = r.Temperatura,
                    Autor = r.Utilizador != null ? r.Utilizador.Nome : "Anônimo",
                    Categoria = r.Categoria != null ? r.Categoria.Nome : "Sem Categoria"
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // OBTER POR ID (GET)
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Utilizador)
                .Include(r => r.Categoria)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null) return NotFound("Receita não encontrada.");

            var dto = new ExibirReceitaDto
            {
                Id = receita.Id,
                Titulo = receita.Titulo,
                Ingredientes = receita.Ingredientes,
                ModoPreparo = receita.ModoPreparo,
                TempoPreparoMinutos = receita.TempoPreparoMinutos,
                Paladar = receita.Paladar,
                Temperatura = receita.Temperatura,
                Autor = receita.Utilizador?.Nome ?? "Anônimo",
                Categoria = receita.Categoria?.Nome ?? "Sem Categoria"
            };

            return Ok(dto);
        }

        // EDITAR RECEITA (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CriarReceitaDto dto)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null) return NotFound("Receita não encontrada.");

            var utilizadorExiste = await _context.Utilizadores.AnyAsync(u => u.Id == dto.UtilizadorId);
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

            if (!utilizadorExiste || !categoriaExiste)
                return BadRequest("Utilizador ou Categoria informados são inválidos.");

            receita.Titulo = dto.Titulo;
            receita.Ingredientes = dto.Ingredientes;
            receita.ModoPreparo = dto.ModoPreparo;
            receita.TempoPreparoMinutos = dto.TempoPreparoMinutos;
            receita.Paladar = dto.Paladar.ToUpper();
            receita.Temperatura = dto.Temperatura.ToUpper();
            receita.UtilizadorId = dto.UtilizadorId;
            receita.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // EXCLUIR RECEITA (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null) return NotFound("Receita não encontrada.");

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}