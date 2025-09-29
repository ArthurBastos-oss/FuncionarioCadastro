using FuncionarioCadastro.Data;
using FuncionarioCadastro.Models;
using FuncionarioCadastroWeb.Servicos;
using FuncionarioCadastroWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<IActionResult> Index(string searchNome, string searchCPF, string searchProfissao, string searchUF)
        {

            //        var FabioBastos = await _context.Funcionario
            //.FromSqlRaw(@"
            //    SELECT f.*, c.* 
            //    FROM ""Funcionario"" AS f
            //    INNER JOIN ""FuncionarioCNH"" as c 
            //        ON f.""Id"" = c.""IdFuncionario""
            //    WHERE f.""Id"" = 3;")
            //.ToListAsync();

            var query = _context.Funcionario
             .Include(f => f.CNH)
             .Include(f => f.CTPS)
             .Include(f => f.Endereco)
             .Include(f => f.Curso)
             .AsQueryable();

            // Filtro por nome
            if (!string.IsNullOrWhiteSpace(searchNome))
                {
                query = query.Where(f => f.Nome.ToUpper().Contains(searchNome.ToUpper()));
            }

            // Filtro por CPF (sem mascara)
            if (!string.IsNullOrWhiteSpace(searchCPF))
            {
                var cpfSemMascara = searchCPF.Replace(".", "").Replace("-", "");
                query = query.Where(f => f.CPF.Contains(cpfSemMascara));
            }

            // Filtro por profissão
            if (!string.IsNullOrWhiteSpace(searchProfissao))
            {
                query = query.Where(f => f.Profissao.ToUpper().Contains(searchProfissao.ToUpper()));
            }

            // Filtro por UF
            if (!string.IsNullOrWhiteSpace(searchUF))
            {
                query = query.Where(f => f.UF.ToUpper().Contains(searchUF.ToUpper()));
            }

            var funcionarios = await query.ToListAsync();


            // Mapeamento Model -> ViewModel
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
                    CTPS = SvcFormatacao.MascaraCTPS(f.CTPS.CTPS, f.CTPS.Tipo),
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

            ViewBag.UFs = SvcSelectList.GetUFs(model.UF);
            ViewBag.CTPSTipo = SvcSelectList.GetCTPSTipo(model?.CTPS?.Tipo);
            ViewBag.TipoCurso = SvcSelectList.GetTipoCurso();
            ViewBag.CNHCategorias = SvcSelectList.GetCNHCategorias(model?.CNH?.Categoria?.FirstOrDefault());

            return View(model);
        }

        // POST Create
        [HttpPost]
        public IActionResult Create(FuncionarioViewModel f)
        {

            // Ignora validação de CNH se não preenchida
            if (string.IsNullOrEmpty(f.CNH?.CNH))
            {
                ModelState.Remove("CNH.CNH");
                ModelState.Remove("CNH.Categoria");
                ModelState.Remove("CNH.DataEmissao");
                ModelState.Remove("CNH.Validade");
            }
            else
            {
                // CNH: todos ou nenhum
                if (f.CNH != null)
                {
                    bool algumPreenchido =
                        !string.IsNullOrWhiteSpace(f.CNH.CNH) ||
                        (f.CNH.Categoria != null && f.CNH.Categoria.Any()) ||
                        f.CNH.DataEmissao != null ||
                        f.CNH.Validade != null;

                    bool todosPreenchidos =
                        !string.IsNullOrWhiteSpace(f.CNH.CNH) &&
                        f.CNH.Categoria != null && f.CNH.Categoria.Any() &&
                        f.CNH.DataEmissao != null &&
                        f.CNH.Validade != null;

                    if (algumPreenchido && !todosPreenchidos)
                    {
                        ModelState.AddModelError("CNH", "Se algum campo da CNH for preenchido, todos os campos devem ser preenchidos.");
                    }
                    else if (todosPreenchidos)
                    {
                        var hoje = DateTime.Today;

                        if (f.CNH.DataEmissao > hoje)
                            ModelState.AddModelError("CNH.DataEmissao", "A data de emissão da CNH não é válida.");

                        if (f.CNH.Validade < hoje)
                            ModelState.AddModelError("CNH.Validade", "A validade da CNH já expirou.");
                    }
                }

            }

            // normaliza Endereco: Numero => "s/n" se vazio; Complemento opcional
            if (f.Endereco != null)
            {
                for (int i = 0; i < f.Endereco.Count; i++)
                {
                    var end = f.Endereco[i];

                    // Se end.Numero for null ou vazio, define "s/n"
                    if (string.IsNullOrWhiteSpace(end.Numero))
                        end.Numero = "s/n";

                    // Complemento é opcional - normaliza para null se vazio (opcional)
                    if (string.IsNullOrWhiteSpace(end.Complemento))
                        end.Complemento = null;

                    // limpar erros no ModelState para as chaves da lista (se houver)
                    var numeroKey = $"Endereco[{i}].Numero";
                    var complementoKey = $"Endereco[{i}].Complemento";

                    if (ModelState.ContainsKey(numeroKey))
                    {
                        ModelState[numeroKey].Errors.Clear();
                        ModelState.Remove(numeroKey);
                    }

                    if (ModelState.ContainsKey(complementoKey))
                    {
                        ModelState[complementoKey].Errors.Clear();
                        ModelState.Remove(complementoKey);
                    }
                }
            }

            // Validação Data de Nascimento
            if (f.DataNascimento != null)
            {
                var hoje = DateTime.Today;

                if (f.DataNascimento > hoje)
                {
                    ModelState.AddModelError("DataNascimento", "A data de nascimento não pode ser no futuro.");
                }
            }

            // Validação de CPF
            if (!string.IsNullOrWhiteSpace(f.CPF))
            {
                var cpfNumeros = SvcFormatacao.RemoverMascara(f.CPF);

                if (!SvcValidacao.ValidarCPF(cpfNumeros))
                {
                    ModelState.AddModelError("CPF", "O CPF informado não é válido.");
                }
            }

            // Validação de CNH
            if (!string.IsNullOrWhiteSpace(f.CNH?.CNH))
            {
                if (SvcValidacao.ValidarCNH(f.CNH.CNH))
                {
                    ModelState.AddModelError("CNH.CNH", "O CNH informado não é válido.");
                }
            }

            // Validação de CTPS
            if (!string.IsNullOrWhiteSpace(f.CTPS?.CTPS))
            {
                var ctpsNumeros = SvcFormatacao.RemoverMascara(f.CTPS.CTPS);
                var cpfNumeros = SvcFormatacao.RemoverMascara(f.CPF);

                if (f.CTPS.Tipo == "CPF")
                {
                    if (ctpsNumeros != cpfNumeros)
                    {
                        ModelState.AddModelError("CTPS.CTPS", "O CPF e o CTPS não correspondem.");
                    }
                }
                else if (f.CTPS.Tipo == "CTPS")
                {
                    if (!SvcValidacao.ValidarCTPS(ctpsNumeros))
                    {
                        ModelState.AddModelError("CTPS.CTPS", "O número da CTPS deve conter exatamente 11 dígitos numéricos.");
                    }
                }
            }

            // Validação de CEP
            if (f.Endereco != null)
            {
                for (int i = 0; i < f.Endereco.Count; i++)
                {
                    var endereco = f.Endereco[i];

                    if (!string.IsNullOrWhiteSpace(endereco.CEP))
                    {
                        var cepNumeros = SvcFormatacao.RemoverMascara(endereco.CEP);

                        if (!SvcValidacao.ValidarCEP(cepNumeros))
                        {
                            ModelState.AddModelError($"Endereco[{i}].CEP", "O CEP informado não é válido.");
                        }
                    }
                }
            }


            if (!ModelState.IsValid)

            ViewBag.UFs = SvcSelectList.GetUFs(f.UF);
            ViewBag.CTPSTipo = SvcSelectList.GetCTPSTipo(f?.CTPS?.Tipo);
            ViewBag.TipoCurso = SvcSelectList.GetTipoCurso();
            ViewBag.CNHCategorias = SvcSelectList.GetCNHCategorias(f?.CNH?.Categoria?.FirstOrDefault());

            return View(f);

            // Mapeamento ViewModel → Model
            var funcionario = new Funcionario
            {
                Id = f.Id ?? 0,
                Nome = f.Nome,
                CPF = SvcFormatacao.RemoverMascara(f.CPF),
                DataNascimento = f.DataNascimento,
                UF = f.UF,
                Profissao = f.Profissao,
                CNH = !string.IsNullOrEmpty(f.CNH?.CNH) ? new FuncionarioCNH
                {
                    CNH = f.CNH.CNH,
                    Categoria = string.Join(",", f.CNH.Categoria?.ToArray() ?? Array.Empty<string>()),
                    DataEmissao = f.CNH.DataEmissao.Value,
                    Validade = f.CNH.Validade.Value
                } : null,
                CTPS = new FuncionarioCTPS
                {
                    CTPS = SvcFormatacao.RemoverMascara(f.CTPS.CTPS),
                    Tipo = f.CTPS.Tipo,
                    DataEmissao = f.CTPS.DataEmissao
                },
                Curso = f.Curso.Select(c => new FuncionarioCurso
                {
                    Curso = c.Curso,
                    TipoCurso = c.TipoCurso
                }).ToList(),
                Endereco = f.Endereco.Select(e => new FuncionarioEndereco
                {
                    Cidade = e.Cidade,
                    Bairro = e.Bairro,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    CEP = SvcFormatacao.RemoverMascara(e.CEP),
                }).ToList()
            };

            _context.Funcionario.Add(funcionario);
            _context.SaveChanges();

            // Atualizar a sequence para o próximo ID correto
            if (f.Id.HasValue)
            {
                var maxId = _context.Funcionario.Max(x => x.Id);
                _context.Database.ExecuteSqlRaw($"SELECT setval('\"Funcionario_Id_seq\"', {maxId}, true);");
            }

            return RedirectToAction("Index");
        }


        // Get Edit
        public async Task<IActionResult> Edit(int id)
        {
            var f = await _context.Funcionario
        .Include(f => f.CNH)
        .Include(f => f.CTPS)
        .Include(f => f.Curso)
        .Include(f => f.Endereco)
        .FirstOrDefaultAsync(f => f.Id == id);

            if (f == null)
                return NotFound();

            var viewModel = new FuncionarioViewModel
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
                    Categoria = f.CNH.Categoria?.Split(',') ?? Array.Empty<string>(),
                    DataEmissao = f.CNH.DataEmissao,
                    Validade = f.CNH.Validade
                } : null,

                CTPS = f.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = SvcFormatacao.MascaraCTPS(f.CTPS.CTPS, f.CTPS.Tipo),
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
            };

            ViewBag.UFs = SvcSelectList.GetUFs(f.UF);
            ViewBag.CTPSTipo = SvcSelectList.GetCTPSTipo(f?.CTPS?.Tipo);
            ViewBag.TipoCurso = SvcSelectList.GetTipoCurso();
            ViewBag.CNHCategorias = SvcSelectList.GetCNHCategorias(f?.CNH?.Categoria?.Split(',').FirstOrDefault());


            return View(viewModel);
        }

        // POST Edit
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FuncionarioViewModel f)
        {
            // Ignora validação de CNH se não preenchida
            if (string.IsNullOrEmpty(f.CNH?.CNH))
            {
                ModelState.Remove("CNH.CNH");
                ModelState.Remove("CNH.Categoria");
                ModelState.Remove("CNH.DataEmissao");
                ModelState.Remove("CNH.Validade");
            }
            else
            {
                // CNH: todos ou nenhum
                if (f.CNH != null)
                {
                    bool algumPreenchido =
                        !string.IsNullOrWhiteSpace(f.CNH.CNH) ||
                        (f.CNH.Categoria != null && f.CNH.Categoria.Any()) ||
                        f.CNH.DataEmissao != null ||
                        f.CNH.Validade != null;

                    bool todosPreenchidos =
                        !string.IsNullOrWhiteSpace(f.CNH.CNH) &&
                        f.CNH.Categoria != null && f.CNH.Categoria.Any() &&
                        f.CNH.DataEmissao != null &&
                        f.CNH.Validade != null;

                    if (algumPreenchido && !todosPreenchidos)
                    {
                        ModelState.AddModelError("CNH", "Se algum campo da CNH for preenchido, todos os campos devem ser preenchidos.");
                    }
                    else if (todosPreenchidos)
                    {
                        var hoje = DateTime.Today;

                        if (f.CNH.DataEmissao > hoje)
                            ModelState.AddModelError("CNH.DataEmissao", "A data de emissão da CNH não é válida.");

                        if (f.CNH.Validade < hoje)
                            ModelState.AddModelError("CNH.Validade", "A validade da CNH já expirou.");
                    }
                }

            }

            // normaliza Endereco: Numero => "s/n" se vazio; Complemento opcional
            if (f.Endereco != null)
            {
                for (int i = 0; i < f.Endereco.Count; i++)
                {
                    var end = f.Endereco[i];

                    // Se end.Numero for null ou vazio, define "s/n"
                    if (string.IsNullOrWhiteSpace(end.Numero))
                        end.Numero = "s/n";

                    // Complemento é opcional - normaliza para null se vazio (opcional)
                    if (string.IsNullOrWhiteSpace(end.Complemento))
                        end.Complemento = null;

                    // limpar erros no ModelState para as chaves da lista (se houver)
                    var numeroKey = $"Endereco[{i}].Numero";
                    var complementoKey = $"Endereco[{i}].Complemento";

                    if (ModelState.ContainsKey(numeroKey))
                    {
                        ModelState[numeroKey].Errors.Clear();
                        ModelState.Remove(numeroKey);
                    }

                    if (ModelState.ContainsKey(complementoKey))
                    {
                        ModelState[complementoKey].Errors.Clear();
                        ModelState.Remove(complementoKey);
                    }
                }
            }

            // Validação Data de Nascimento
            if (f.DataNascimento != null)
            {
                var hoje = DateTime.Today;

                if (f.DataNascimento > hoje)
                {
                    ModelState.AddModelError("DataNascimento", "A data de nascimento não pode ser no futuro.");
                }
            }

            // Validação de CPF
            if (!string.IsNullOrWhiteSpace(f.CPF))
            {
                var cpfNumeros = SvcFormatacao.RemoverMascara(f.CPF);

                if (!SvcValidacao.ValidarCPF(cpfNumeros))
                {
                    ModelState.AddModelError("CPF", "O CPF informado não é válido.");
                }
            }

            // Validação de CNH
            if (!string.IsNullOrWhiteSpace(f.CNH?.CNH))
            {
                if (SvcValidacao.ValidarCNH(f.CNH.CNH))
                {
                    ModelState.AddModelError("CNH.CNH", "O CNH informado não é válido.");
                }
            }

            // Validação de CTPS
            if (!string.IsNullOrWhiteSpace(f.CTPS?.CTPS))
            {
                var ctpsNumeros = SvcFormatacao.RemoverMascara(f.CTPS.CTPS);
                var cpfNumeros = SvcFormatacao.RemoverMascara(f.CPF);

                if (f.CTPS.Tipo == "CPF")
                {
                    if (ctpsNumeros != cpfNumeros)
                    {
                        ModelState.AddModelError("CTPS.CTPS", "O CPF e o CTPS não correspondem.");
                    }
                }
                else if (f.CTPS.Tipo == "CTPS")
                {
                    if (!SvcValidacao.ValidarCTPS(ctpsNumeros))
                    {
                        ModelState.AddModelError("CTPS.CTPS", "O número da CTPS deve conter exatamente 11 dígitos numéricos.");
                    }
                }
            }

            // Validação de CEP
            if (f.Endereco != null)
            {
                for (int i = 0; i < f.Endereco.Count; i++)
                {
                    var endereco = f.Endereco[i];

                    if (!string.IsNullOrWhiteSpace(endereco.CEP))
                    {
                        var cepNumeros = SvcFormatacao.RemoverMascara(endereco.CEP);

                        if (!SvcValidacao.ValidarCEP(cepNumeros))
                        {
                            ModelState.AddModelError($"Endereco[{i}].CEP", "O CEP informado não é válido.");
                        }
                    }
                }
            }

            if (!ModelState.IsValid)

                ViewBag.UFs = SvcSelectList.GetUFs(f.UF);
            ViewBag.CTPSTipo = SvcSelectList.GetCTPSTipo(f?.CTPS?.Tipo);
            ViewBag.TipoCurso = SvcSelectList.GetTipoCurso();
            ViewBag.CNHCategorias = SvcSelectList.GetCNHCategorias(f?.CNH?.Categoria?.FirstOrDefault());

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
            funcionario.CPF = SvcFormatacao.RemoverMascara(f.CPF);
            funcionario.DataNascimento = f.DataNascimento;
            funcionario.UF = f.UF;
            funcionario.Profissao = f.Profissao;

            // Atualiza ou cria CNH
            if (!string.IsNullOrEmpty(f.CNH?.CNH))
            {
                if (funcionario.CNH == null)
                    funcionario.CNH = new FuncionarioCNH();

                funcionario.CNH.CNH = f.CNH.CNH;
                funcionario.CNH.Categoria = string.Join(",", f.CNH.Categoria?.ToArray() ?? Array.Empty<string>());
                funcionario.CNH.DataEmissao = f.CNH.DataEmissao ?? DateTime.MinValue;
                funcionario.CNH.Validade = f.CNH.Validade ?? DateTime.MinValue;
            }
            else
            {
                funcionario.CNH = null;
            }

                // Atualiza 
                funcionario.CTPS.CTPS = SvcFormatacao.RemoverMascara(f.CTPS.CTPS);
                funcionario.CTPS.Tipo = f.CTPS.Tipo;
                funcionario.CTPS.DataEmissao = f.CTPS.DataEmissao;

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
                        enderecoExistente.CEP = SvcFormatacao.RemoverMascara(e.CEP);
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
                        CEP = SvcFormatacao.RemoverMascara(e.CEP)
                    });
                }
            }

            _context.Update(funcionario);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

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
                CPF = SvcFormatacao.MascaraCPF(f.CPF),
                DataNascimento = f.DataNascimento,
                UF = f.UF,
                Profissao = f.Profissao,
                CNH = f.CNH != null ? new FuncionarioCNHViewModel
                {
                    CNH = f.CNH.CNH,
                    Categoria = f.CNH.Categoria?.Split(',') ?? Array.Empty<string>(),
                    DataEmissao = f.CNH.DataEmissao,
                    Validade = f.CNH.Validade
                } : null,
                CTPS = f.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = SvcFormatacao.MascaraCTPS(f.CTPS.CTPS, f.CTPS.Tipo),
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
                    CEP = SvcFormatacao.MascaraCEP(e.CEP)
                }).ToList()
            };

            return View(viewModel);
        }

        // GET Delete
        public async Task<IActionResult> Delete(int id)
        {
            var funcionario = await _context.Funcionario
                .Include(f => f.CNH)
                .Include(f => f.CTPS)
                .Include(f => f.Curso)
                .Include(f => f.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            // Faz o mapeamento manual para o ViewModel
            var vm = new FuncionarioViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                CPF = SvcFormatacao.MascaraCPF(funcionario.CPF),
                DataNascimento = funcionario.DataNascimento,
                UF = funcionario.UF,
                Profissao = funcionario.Profissao,
                CNH = funcionario.CNH != null ? new FuncionarioCNHViewModel
                {
                    CNH = funcionario.CNH.CNH,
                    Categoria = funcionario.CNH.Categoria?.Split(',').ToArray(),
                    Validade = funcionario.CNH.Validade
                } : null,
                CTPS = funcionario.CTPS != null ? new FuncionarioCTPSViewModel
                {
                    CTPS = SvcFormatacao.MascaraCTPS(funcionario.CTPS.CTPS, funcionario.CTPS.Tipo),
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
                    CEP = SvcFormatacao.MascaraCEP(e.CEP)
                }).ToList()
            };

            return View(vm);
        }


        // POST Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionario
        .Include(f => f.CNH)
        .Include(f => f.CTPS)
        .Include(f => f.Curso)
        .Include(f => f.Endereco)
        .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
                return NotFound();

            if (funcionario.CNH != null)
                _context.FuncionarioCNH.Remove(funcionario.CNH);

            if (funcionario.CTPS != null)
                _context.FuncionarioCTPS.Remove(funcionario.CTPS);

            if (funcionario.Curso.Any())
                _context.FuncionarioCurso.RemoveRange(funcionario.Curso);

            if (funcionario.Endereco.Any())
                _context.FuncionarioEndereco.RemoveRange(funcionario.Endereco);

            _context.Funcionario.Remove(funcionario);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}