using System;
using System.Collections.Generic;

#nullable disable

namespace PorraGironaWeb.DataLayer
{
    public partial class Penye
    {
        public Penye()
        {
            Penyistes = new HashSet<Penyiste>();
        }

        public int Idpenya { get; set; }
        public string Nom { get; set; }

        public virtual ICollection<Penyiste> Penyistes { get; set; }
    }
}
