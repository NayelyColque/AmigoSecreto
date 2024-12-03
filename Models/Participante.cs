using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmigoSecreto.Models
{
    public class Participante
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Presente { get; set; }
        public string Mensagem { get; set; }
        public object Id { get; internal set; }
    }
}
