using FuncionarioCadastro.Data;
using FuncionarioCadastro.Models;
using FuncionarioCadastroWeb.Servicos;
using FuncionarioCadastroWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var funcionarios = await _context.Funcionario
                .Include(f => f.CNH)
                .Include(f => f.CTPS)
                .Include(f => f.Endereco)
                .Include(f => f.Curso)
                .ToListAsync();

            // Mapeamento Model → ViewModel
            var viewModel = funcionarios.Select(f => new FuncionarioViewModel
            {
                Id = f.Id,
                Nome = f.Nome,
                CPF = SvcFormatacao.MascaraCPF(f.CPF),
                DataNascimento = f.DataNascimento,
                UF = f.UF,
                Profissao = f.Profissao,

                CNH = f.CNH != null ? new FuncionarioCNHViewModel
                {
                    CNH = f.CNH.CNH,
                    Categoria = f.CNH.Categoria?.Split(','),
                    DataEmissao = f.CNH.DataEmissao,
                    Validade = f.CNH.Validade
                } : null,

                CTPS = f.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = f.CTPS.CTPS,
                    Tipo = f.CTPS.Tipo,
                    DataEmissao = f.CTPS.DataEmissao
                } : null,

                Curso = f.Curso.Select(c => new FuncionarioCursoViewModel
                {
                    Curso = c.Curso,
                    TipoCurso = c.TipoCurso
                }).ToList(),

                Endereco = f.Endereco.Select(e => new FuncionarioEnderecoViewModel
                {
                    Cidade = e.Cidade,
                    Bairro = e.Bairro,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    CEP = SvcFormatacao.MascaraCEP(e.CEP)
                }).ToList()

            }).ToList();

            return View(viewModel);
        }

        // GET Create
        public IActionResult Create() 
        {
            var model = new FuncionarioViewModel
            {
                CNH = new FuncionarioCNHViewModel(),
                CTPS = new FuncionarioCTPSViewModel(),
                Curso = new List<FuncionarioCursoViewModel>
                {
                    new FuncionarioCursoViewModel() // 1 campo pronto
                },
                Endereco = new List<FuncionarioEnderecoViewModel>
                {
                    new FuncionarioEnderecoViewModel() // 1 campo pronto
                },
            };

            ViewBag.UFs = new SelectList(new[] {
                "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
                "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
            });

            ViewBag.CTPSTipo = new SelectList(new[] { "CPF", "CTPS" });

            ViewBag.TipoCurso = new SelectList(new[] { "Selecione", "Técnico", "Graduação", "Pós-Graduação" });

            ViewBag.CNHCategorias = new MultiSelectList(new[] { "A", "B", "C", "D", "E" });


            return View(model);
        }

        // POST Create
        [HttpPost]
        public IActionResult Create(FuncionarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Mapeamento ViewModel → Model
            var funcionario = new Funcionario
            {
                Nome = model.Nome,
                CPF = model.CPF,
                DataNascimento = model.DataNascimento,
                UF = model.UF,
                Profissao = model.Profissao,
                CNH = new FuncionarioCNH
                {
                    CNH = model.CNH.CNH,
                    Categoria = model.CNH.Categoria != null ? string.Join(",", model.CNH.Categoria) : null,
                    DataEmissao = model.CNH.DataEmissao,
                    Validade = model.CNH.Validade
                },
                CTPS = new FuncionarioCTPS
                {
                    CTPS = model.CTPS.CTPS,
                    Tipo = model.CTPS.Tipo,
                    DataEmissao = model.CTPS.DataEmissao
                },
                Curso = model.Curso.Select(c => new FuncionarioCurso
                {
                    Curso = c.Curso,
                    TipoCurso = c.TipoCurso
                }).ToList(),
                Endereco = model.Endereco.Select(e => new FuncionarioEndereco
                {
                    Cidade = e.Cidade,
                    Bairro = e.Bairro,
                    CEP = e.CEP,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento
                }).ToList()
            };

            _context.Funcionario.Add(funcionario);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // Get Edit
        public async Task<IActionResult> Edit(int id)
        {
            var funcionario = await _context.Funcionario
        .Include(f => f.CNH)
        .Include(f => f.CTPS)
        .Include(f => f.Curso)
        .Include(f => f.Endereco)
        .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
                return NotFound();

            var viewModel = new FuncionarioViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                CPF = Convert.ToUInt64(funcionario.CPF).ToString(@"000.000.000-00"), // aplica máscara
                DataNascimento = funcionario.DataNascimento,
                UF = funcionario.UF,
                Profissao = funcionario.Profissao,

                CNH = funcionario.CNH != null ? new FuncionarioCNHViewModel
                {
                    CNH = funcionario.CNH.CNH,
                    Categoria = funcionario.CNH.Categoria?.Split(','),
                    DataEmissao = funcionario.CNH.DataEmissao,
                    Validade = funcionario.CNH.Validade
                } : null,

                CTPS = funcionario.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = funcionario.CTPS.CTPS,
                    Tipo = funcionario.CTPS.Tipo,
                    DataEmissao = funcionario.CTPS.DataEmissao
                } : null,

                Curso = funcionario.Curso?.Select(c => new FuncionarioCursoViewModel
                {
                    Curso = c.Curso,
                    TipoCurso = c.TipoCurso
                }).ToList(),

                Endereco = funcionario.Endereco?.Select(e => new FuncionarioEnderecoViewModel
                {
                    Cidade = e.Cidade,
                    Bairro = e.Bairro,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    CEP = Convert.ToUInt64(e.CEP).ToString(@"00000-000")
                }).ToList()
            };

            // SelectLists para a view
            ViewBag.UFs = new SelectList(new[] {
                "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
                "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
            }, viewModel.UF);

            ViewBag.CTPSTipo = new SelectList(new[] { "CPF", "CTPS" }, viewModel.CTPS?.Tipo);

            ViewBag.TipoCurso = new SelectList(new[] { "Técnico", "Graduação", "Pós-Graduação" });

            ViewBag.CNHCategorias = new MultiSelectList(new[] { "A", "B", "C", "D", "E" }, viewModel?.CNH?.Categoria);

            return View(viewModel);
        }

        // POST Edit
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FuncionarioViewModel f)
        {
            if (!ModelState.IsValid)
                return View(f);

            var funcionario = await _context.Funcionario
                .Include(f => f.CNH)
                .Include(f => f.CTPS)
                .Include(f => f.Curso)
                .Include(f => f.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
                return NotFound();

            // Atualizar campos
            funcionario.Nome = f.Nome;
            funcionario.CPF = f.CPF?.Replace(".", "").Replace("-", ""); // remove máscara
            funcionario.DataNascimento = f.DataNascimento;
            funcionario.UF = f.UF?.ToUpper();
            funcionario.Profissao = f.Profissao;

            // Atualiza ou cria CNH
            if (f.CNH != null)
            {
                if (funcionario.CNH == null)
                    funcionario.CNH = new FuncionarioCNH();

                funcionario.CNH.CNH = f.CNH.CNH;
                funcionario.CNH.Categoria = f.CNH.Categoria != null ? string.Join(",", f.CNH.Categoria) : null;
                funcionario.CNH.DataEmissao = f.CNH.DataEmissao;
                funcionario.CNH.Validade = f.CNH.Validade;
            }

            // Atualiza ou cria CTPS
            if (f.CTPS != null)
            {
                if (funcionario.CTPS == null)
                    funcionario.CTPS = new FuncionarioCTPS();

                funcionario.CTPS.CTPS = f.CTPS.CTPS;
                funcionario.CTPS.Tipo = f.CTPS.Tipo;
                funcionario.CTPS.DataEmissao = f.CTPS.DataEmissao;
            }

            // Atualiza Cursos
            // Remove cursos antigos que não existem mais
            var cursosIds = f.Curso?.Where(c => c.Id != 0).Select(c => c.Id).ToList() ?? new List<int>();
            funcionario.Curso.RemoveAll(c => !cursosIds.Contains(c.Id));

            // Atualiza ou adiciona cursos
            foreach (var c in f.Curso ?? new List<FuncionarioCursoViewModel>())
            {
                if (c.Id != 0)
                {
                    var cursoExistente = funcionario.Curso.FirstOrDefault(x => x.Id == c.Id);
                    if (cursoExistente != null)
                    {
                        cursoExistente.Curso = c.Curso;
                        cursoExistente.TipoCurso = c.TipoCurso;
                    }
                }
                else
                {
                    funcionario.Curso.Add(new FuncionarioCurso
                    {
                        Curso = c.Curso,
                        TipoCurso = c.TipoCurso
                    });
                }
            }

            // Atualiza Endereços (mesma lógica)
            var enderecosIds = f.Endereco?.Where(e => e.Id != 0).Select(e => e.Id).ToList() ?? new List<int>();
            funcionario.Endereco.RemoveAll(e => !enderecosIds.Contains(e.Id));

            foreach (var e in f.Endereco ?? new List<FuncionarioEnderecoViewModel>())
            {
                if (e.Id != 0)
                {
                    var enderecoExistente = funcionario.Endereco.FirstOrDefault(x => x.Id == e.Id);
                    if (enderecoExistente != null)
                    {
                        enderecoExistente.Cidade = e.Cidade;
                        enderecoExistente.Bairro = e.Bairro;
                        enderecoExistente.Logradouro = e.Logradouro;
                        enderecoExistente.Numero = e.Numero;
                        enderecoExistente.Complemento = e.Complemento;
                        enderecoExistente.CEP = e.CEP;
                    }
                }
                else
                {
                    funcionario.Endereco.Add(new FuncionarioEndereco
                    {
                        Cidade = e.Cidade,
                        Bairro = e.Bairro,
                        Logradouro = e.Logradouro,
                        Numero = e.Numero,
                        Complemento = e.Complemento,
                        CEP = e.CEP
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Details
        public async Task<IActionResult> Details(int id)
        {
            var f = await _context.Funcionario
                .Include(fu => fu.CNH)
                .Include(fu => fu.CTPS)
                .Include(fu => fu.Curso)
                .Include(fu => fu.Endereco)
                .FirstOrDefaultAsync(fu => fu.Id == id);

            if (f == null) return NotFound();

            var viewModel = new FuncionarioViewModel
            {
                Id = f.Id,
                Nome = f.Nome,
                CPF = f.CPF,
                DataNascimento = f.DataNascimento,
                UF = f.UF,
                Profissao = f.Profissao,
                CNH = f.CNH != null ? new FuncionarioCNHViewModel
                {
                    CNH = f.CNH.CNH,
                    Categoria = f.CNH.Categoria?.Split(','),
                    DataEmissao = f.CNH.DataEmissao,
                    Validade = f.CNH.Validade
                } : null,
                CTPS = f.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = f.CTPS.CTPS,
                    Tipo = f.CTPS.Tipo,
                    DataEmissao = f.CTPS.DataEmissao
                } : null,
                Curso = f.Curso?.Select(c => new FuncionarioCursoViewModel
                {
                    Curso = c.Curso,
                    TipoCurso = c.TipoCurso
                }).ToList(),
                Endereco = f.Endereco?.Select(e => new FuncionarioEnderecoViewModel
                {
                    Cidade = e.Cidade,
                    Bairro = e.Bairro,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    CEP = e.CEP
                }).ToList()
            };

            return View(viewModel);
        }

        // GET Delete
        public async Task<IActionResult> Delete(int id)
        {
            var f = await _context.Funcionario.FindAsync(id);
            if (f == null) return NotFound();
            return View(f);
        }

        // POST Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var f = await _context.Funcionario.FindAsync(id);
            if (f == null) return NotFound();

            _context.Funcionario.Remove(f);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
