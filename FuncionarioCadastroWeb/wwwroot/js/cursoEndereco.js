function addCurso() {
    let container = document.getElementById("cursos-container");
    let index = container.children.length;
    container.insertAdjacentHTML("beforeend",
        `<div class="curso-item">
            <input name="Curso[${index}].Curso" placeholder="Nome do curso" />
            <select name="Curso[${index}].TipoCurso">
                <option value="">Selecione</option>
                <option value="Técnico">Técnico</option>
                <option value="Graduação">Graduação</option>
                <option value="Pós-Graduação">Pós-Graduação</option>
                <option value="Outro">Outro</option>
            </select>
            <button type="button" onclick="removeItem(this, 'cursos')">Remover</button>
        </div>`);
}

function addEndereco() {
    let container = document.getElementById("enderecos-container");
    let index = container.children.length;
    container.insertAdjacentHTML("beforeend",
        `<div class="endereco-item">
            <input name="Endereco[${index}].Cidade" placeholder="Cidade" />
            <input name="Endereco[${index}].Bairro" placeholder="Bairro" />
            <input name="Endereco[${index}].Logradouro" placeholder="Logradouro" />
            <input name="Endereco[${index}].Numero" placeholder="Número" />
            <input name="Endereco[${index}].Complemento" placeholder="Complemento" />
            <input name="Endereco[${index}].CEP" placeholder="CEP" />
            <button type="button" onclick="removeItem(this, 'enderecos')">Remover</button>
        </div>`);
}

function removeItem(button, tipo) {
    let container = document.getElementById(tipo + "-container");
    button.parentElement.remove();

    if (container.children.length === 0) {
        if (tipo === "cursos") addCurso();
        else if (tipo === "enderecos") addEndereco();
    }
}