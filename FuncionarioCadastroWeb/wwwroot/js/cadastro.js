function addCurso() {
    let container = document.getElementById("cursos-container");
    let index = container.children.length;

    container.insertAdjacentHTML("beforeend",
        `<div class="curso-item d-flex gap-2 mb-2">
            <div class="mb-2">
                <input name="Curso[${index}].Curso" placeholder="Nome do curso" class="form-control" />
                <span class="text-danger" data-valmsg-for="Curso[${index}].Curso" data-valmsg-replace="true"></span>
            </div>

            <div class="mb-2">
                <select name="Curso[${index}].TipoCurso" class="form-select">
                    <option value="">Selecione</option>
                    <option value="Técnico">Técnico</option>
                    <option value="Graduação">Graduação</option>
                    <option value="Pós-Graduação">Pós-Graduação</option>
                    <option value="Outro">Outro</option>
                </select>
                <span class="text-danger" data-valmsg-for="Curso[${index}].TipoCurso" data-valmsg-replace="true"></span>
            </div>

            <button type="button" class="btn btn-danger h-100" onclick="removeItem(this, 'cursos')">Remover</button>
            
        </div>`);
}

function addEndereco() {
    let container = document.getElementById("enderecos-container");
    let index = container.children.length;

    container.insertAdjacentHTML("beforeend",
        `<div class="endereco-item d-flex flex-wrap gap-2 mb-2">
            <input name="Endereco[${index}].Cidade" placeholder="Cidade" class="form-control" />
            <input name="Endereco[${index}].Bairro" placeholder="Bairro" class="form-control" />
            <input name="Endereco[${index}].Logradouro" placeholder="Logradouro" class="form-control" />
            <input name="Endereco[${index}].Numero" placeholder="Número" class="form-control" />
            <input name="Endereco[${index}].Complemento" placeholder="Complemento" class="form-control" />
            <input name="Endereco[${index}].CEP" placeholder="CEP" class="form-control" oninput="this.value = aplicarMascaraCEP(this.value)" />
            <button type="button" class="btn btn-danger" onclick="removeItem(this, 'enderecos')">Remover</button>
        </div>`);
}

function addCategoria() {
    let container = document.getElementById("categorias-container");
    let index = container.children.length;

    // Limitar a 5 categorias
    if (index >= 5) {
        alert("Você só pode adicionar até 5 categorias.");
        return;
    }

    // Todas as categorias possíveis
    let todasCategorias = ["A", "B", "C", "D", "E"];

    // Capturar categorias já selecionadas
    let selecionadas = Array.from(container.querySelectorAll("select"))
        .map(s => s.value)
        .filter(v => v !== "");

    // Filtrar categorias não disponíveis
    let disponiveis = todasCategorias.filter(c => !selecionadas.includes(c));

    if (disponiveis.length === 0) {
        alert("Todas as categorias já foram selecionadas.");
        return;
    }

    // Montar opções do select
    let options = `<option value="">Selecione</option>`;
    disponiveis.forEach(c => {
        options += `<option value="${c}">${c}</option>`;
    });

    // Adicionar item no container
    container.insertAdjacentHTML("beforeend",
        `<div class="categoria-item d-flex gap-2 mb-2">
            <select name="CNH.Categoria[${index}]" class="form-select">
                ${options}
            </select>
            <button type="button" class="btn btn-danger" onclick="removeItem(this, 'categorias')">Remover</button>
        </div>`);
}

function removeItem(button, tipo) {
    let container = document.getElementById(tipo + "-container");
    button.parentElement.remove();

    // Garante que sempre tenha pelo menos 1 campo
    if (container.children.length === 0) {
        if (tipo === "cursos") addCurso();
        else if (tipo === "enderecos") addEndereco();
        else if (tipo === "categorias") addCategoria();
    }
}



// Apenas números
function onlyDigits(value) {
    return (value || "").replace(/\D/g, "");
}

// Máscaras
function aplicarMascaraCPF(value) {
    value = onlyDigits(value).slice(0, 11);
    return value
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
}

function aplicarMascaraCEP(value) {
    value = onlyDigits(value).slice(0, 8);
    return value.replace(/(\d{5})(\d)/, '$1-$2');
}
function aplicarMascaraCNH(value) {
    value = onlyDigits(value).slice(0, 11);
    return value;
}

function aplicarMascaraCTPS(value) {
    value = onlyDigits(value).slice(0, 11); // 7 + 4
    if (value.length <= 7) return value;
    return value.replace(/(\d{7})(\d{1,4})$/, "$1/$2");
}

function aplicarMascaraDocumento(value, tipo) {
    if (!value) return "";
    if (tipo === "CPF") return aplicarMascaraCPF(value);
    if (tipo === "CTPS") return aplicarMascaraCTPS(value);
    return value;
}

function removerMascara(value) {
    return onlyDigits(value);
}

document.addEventListener('DOMContentLoaded', function () {
    // CPF
    const cpfInput = document.getElementById('CPF');
    if (cpfInput) {
        if (cpfInput.value) cpfInput.value = aplicarMascaraCPF(cpfInput.value);
        cpfInput.addEventListener('input', function () {
            this.value = aplicarMascaraCPF(this.value);
        });
    }

    // CEP
    const cepInput = document.getElementById('CEP');
    if (cepInput) {
        if (cepInput.value) cepInput.value = aplicarMascaraCEP(cepInput.value);
        cepInput.addEventListener('input', function () {
            this.value = aplicarMascaraCEP(this.value);
        });
    }

    // CTPS
    const ctpsInput = document.getElementById('CTPS');
    const ctpsTipo = document.getElementById('CTPS-Tipo');
    if (ctpsInput && ctpsTipo) {
        if (ctpsInput.value) ctpsInput.value = aplicarMascaraDocumento(ctpsInput.value, ctpsTipo.value);
        ctpsInput.addEventListener('input', function () {
            this.value = aplicarMascaraDocumento(this.value, ctpsTipo.value);
        });
        ctpsTipo.addEventListener('change', function () {
            ctpsInput.value = aplicarMascaraDocumento(ctpsInput.value, this.value);
        });
    }

    // CNH
    const cnhInput = document.getElementById('CNH');
    if (cnhInput) {
        if (cnhInput.value) cnhInput.value = aplicarMascaraCNH(cnhInput.value);
        cnhInput.addEventListener('input', function () {
            this.value = aplicarMascaraCNH(this.value);
        });
    }
});


