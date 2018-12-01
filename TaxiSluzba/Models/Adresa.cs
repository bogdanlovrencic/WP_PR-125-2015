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
        private string postanskiBroj;
        private string grad;


        public Adresa() { }

        public Adresa(string ul, string br, string grad, string postBroj)
        {
            Ulica = ul;
            Broj = br;
            PostanskiBroj = postBroj;
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

        public string PostanskiBroj
        {
            get
            {
                return postanskiBroj;
            }

            set
            {
                postanskiBroj = value;
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