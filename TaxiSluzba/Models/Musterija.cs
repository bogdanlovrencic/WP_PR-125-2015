using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Musterija : Korisnik
    {
        public Musterija() { voznje = new List<Voznja>(); }

        public Musterija(string ime, string prezime, string korisnickoIme, string lozinka, Pol pol, string jmbg, string telefon, string email) : base()
        {
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