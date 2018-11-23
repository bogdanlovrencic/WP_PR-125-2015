using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Automobil
    {
        public Vozac vozac;
        public string Godiste { get; set; }
        public string BrRegOznake { get; set; }
        public int TaxiId { get; set; }
        public TipAutomobila tipAutomobila { get; set; }

        public Automobil(){
           
        }

        public Automobil(Vozac v,string god,string regOznaka,int id,TipAutomobila tip)
        {
            vozac = v;
            Godiste = god;
            BrRegOznake = regOznaka;
            TaxiId = id;
            tipAutomobila = tip;
        }
    }
}