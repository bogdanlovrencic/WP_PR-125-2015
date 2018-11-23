using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Korisnik
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public  Pol Pol { get; set; }
        public string Jmbg { get; set; }
        public string KontaktTelefon { get; set; }
        public string Email { get; set; }
        public Uloga Uloga { get; set; }
        
        public List<Voznja> voznje { get; set; }


        public Korisnik()
        {
            Uloga = Uloga.MUSTERIJA;
            voznje = new List<Voznja>();
        }

        public Korisnik(string ime, string prezime, string korisnickoIme, string lozinka, Pol pol, string jmbg, string telefon, string email) {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Jmbg = jmbg;
            KontaktTelefon = telefon;
            Email = email;
            Uloga = Uloga.MUSTERIJA;
            voznje = new List<Voznja>();

            
        }
    }
}