using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FuncionarioCadastroWeb.Servicos
{
    public static class SvcSelectList
    {
        public static SelectList GetUFs(string selectedValue = null)
        {
            var itens = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione", Value = "" },
                new SelectListItem { Text = "AC", Value = "AC" },
                new SelectListItem { Text = "AL", Value = "AL" },
                new SelectListItem { Text = "AP", Value = "AP" },
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "BA", Value = "BA" },
                new SelectListItem { Text = "CE", Value = "CE" },
                new SelectListItem { Text = "DF", Value = "DF" },
                new SelectListItem { Text = "ES", Value = "ES" },
                new SelectListItem { Text = "GO", Value = "GO" },
                new SelectListItem { Text = "MA", Value = "MA" },
                new SelectListItem { Text = "MT", Value = "MT" },
                new SelectListItem { Text = "MS", Value = "MS" },
                new SelectListItem { Text = "MG", Value = "MG" },
                new SelectListItem { Text = "PA", Value = "PA" },
                new SelectListItem { Text = "PB", Value = "PB" },
                new SelectListItem { Text = "PR", Value = "PR" },
                new SelectListItem { Text = "PE", Value = "PE" },
                new SelectListItem { Text = "PI", Value = "PI" },
                new SelectListItem { Text = "RJ", Value = "RJ" },
                new SelectListItem { Text = "RN", Value = "RN" },
                new SelectListItem { Text = "RS", Value = "RS" },
                new SelectListItem { Text = "RO", Value = "RO" },
                new SelectListItem { Text = "RR", Value = "RR" },
                new SelectListItem { Text = "SC", Value = "SC" },
                new SelectListItem { Text = "SP", Value = "SP" },
                new SelectListItem { Text = "SE", Value = "SE" },
                new SelectListItem { Text = "TO", Value = "TO" }
            };
            return new SelectList(itens, "Value", "Text", selectedValue);
        }

        public static SelectList GetCTPSTipo(string selectedValue = null)
        {
            var itens = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione", Value = "" },
                new SelectListItem { Text = "CPF", Value = "CPF" },
                new SelectListItem { Text = "CTPS", Value = "CTPS" }
            };
            return new SelectList(itens, "Value", "Text", selectedValue);
        }

        public static SelectList GetTipoCurso(string selectedValue = null)
        {
            var itens = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione", Value = "" },
                new SelectListItem { Text = "Técnico", Value = "Técnico" },
                new SelectListItem { Text = "Graduação", Value = "Graduação" },
                new SelectListItem { Text = "Pós-Graduação", Value = "Pós-Graduação" },
                new SelectListItem { Text = "Outro", Value = "Outro" }
            };
            return new SelectList(itens, "Value", "Text", selectedValue);
        }

        public static SelectList GetCNHCategorias(string selectedValue = null)
        {
            var itens = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione", Value = "" },
                new SelectListItem { Text = "A", Value = "A" },
                new SelectListItem { Text = "B", Value = "B" },
                new SelectListItem { Text = "C", Value = "C" },
                new SelectListItem { Text = "D", Value = "D" },
                new SelectListItem { Text = "E", Value = "E" }
            };
            return new SelectList(itens, "Value", "Text", selectedValue);
        }
    }
}
