using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Komentar
    {
        public string Opis { get; set; }
        public DateTime DatumObjave { get; set; }
        public Korisnik korisnik { get; set; }
        public Voznja Voznja { get; set; }
        public OcenaVoznje OcenaVoznje { get; set; }

        public Komentar() { }

        public Komentar(string opis, DateTime datumVreme, Korisnik korisnik, Voznja voznja, OcenaVoznje ocenaVoznje)
        {
            this.Opis = opis;
            this.DatumObjave = datumVreme;
            this.korisnik = korisnik;
            this.Voznja = voznja;
            this.OcenaVoznje = ocenaVoznje;
        }
    }
}