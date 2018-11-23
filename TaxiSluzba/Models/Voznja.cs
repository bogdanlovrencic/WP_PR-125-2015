using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Voznja
    {
        public DateTime DatumIvremePorudz { get; set; }
        public Lokacija DolaznaLokacija { get; set; }
        public TipAutomobila TipAuta { get; set; }
        public Musterija Musterija { get; set; }
        public Lokacija Odrediste { get; set; }
        public Dispecer dispecer { get; set; }
        public Vozac vozac { get; set; }
        public string Iznos { get; set; }
        public StatusVoznje Status { get; set; }
        public Komentar komentar { get; set; }

        public Voznja(DateTime datumVreme,Lokacija lokacijaZaTaxi,TipAutomobila tipAuta,Musterija musterija)
        {
            DatumIvremePorudz = datumVreme;
            DolaznaLokacija = lokacijaZaTaxi;
            TipAuta = tipAuta;
            Musterija = musterija;
           
        }
    }
}