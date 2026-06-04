using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLogin.Model
{
    public class Usuario
    {
        public int id { get; set; }
        public string? login { get; set; }
        public string? senha { get; set; }
        public DateTime ultimologin { get; set; }
    }
}