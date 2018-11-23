using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Adresa
    {
        private string ulica;
        private string broj;
        private string pozivniBroj;
        private string grad;


        public Adresa() { }

        public Adresa(string ul, string br, string pozBr, string grad)
        {
            Ulica = ul;
            Broj = br;
            PozivniBroj = pozBr;
            this.Grad = grad;

        }

        public string Ulica
        {
            get
            {
                return ulica;
            }

            set
            {
                ulica = value;
            }
        }

        public string Broj
        {
            get
            {
                return broj;
            }

            set
            {
                broj = value;
            }
        }

        public string PozivniBroj
        {
            get
            {
                return pozivniBroj;
            }

            set
            {
                pozivniBroj = value;
            }
        }

        public string Grad
        {
            get
            {
                return grad;
            }

            set
            {
                grad = value;
            }
        }

       
    }
}