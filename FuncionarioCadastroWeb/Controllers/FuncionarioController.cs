using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FuncionarioCadastro.Data;
using FuncionarioCadastroWeb.ViewModels;

namespace FuncionarioCadastroWeb.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly Context _context;

        public FuncionarioController(Context context)
        {  
            _context = context; 
        }

        // Index
        public async Task<IActionResult> Index()
        {
            List<FuncionarioViewModel> ListaFuncionarios = new List<FuncionarioViewModel>();
            var funcionarios = await _context.Funcionario
                .Include(f => f.CNH)
                .Include(f =>  f.CTPS)
                .Include(f => f.Endereco)
                .Include(f => f.Curso)
                .ToListAsync();

            return View(funcionarios);
        }

        // GET Create
        public IActionResult Create() 
        {
            return View();
        }

        // POST Create
        [HttpPost]
        public async Task<IActionResult> Create(FuncionarioViewModel f)
        {
            if (ModelState.IsValid)
            {
                var funcionario = new FuncionarioViewModel
                {
                    Nome = f.Nome,
                    CPF = f.CPF,
                    DataNascimento = f.DataNascimento,
                    UF = f.UF,
                    Profissao = f.Profissao,
                    
                    CNH = new FuncionarioCNHViewModel
                    {
                        CNH = f.CNH.CNH,
                        Categoria = f.CNH.Categoria,
                        DataEmissao = f.CNH.DataEmissao,
                        Validade = f.CNH.Validade,
                    },

                    CTPS = new FuncionarioCTPSViewModel
                    {
                        CTPS = f.CTPS.CTPS,
                        Tipo = f.CTPS.Tipo,
                        DataEmissao = f.CTPS.DataEmissao
                    },

                    Curso = f.Curso?.Select(c => new FuncionarioCursoViewModel
                    {
                        Curso = c.Curso,
                        TipoCurso = c.Curso,
                    }).ToList(),

                    Endereco = f.Endereco.Select(e => new FuncionarioEnderecoViewModel
                    {
                        Cidade = e.Cidade,
                        Bairro = e.Bairro,
                        Logradouro = e.Logradouro,
                        Numero = e.Numero,
                        Complemento = e.Complemento,
                        CEP = e.CEP,
                    }).ToList()

                };

                _context.Add(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(f);
        }
    }
}
