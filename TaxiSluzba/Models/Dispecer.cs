using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Dispecer : Korisnik
    {
        public Dispecer(string korisnickoIme,string lozinka, string ime, string prezime, Pol pol,string jmbg,string telefon,string email) {

            this.KorisnickoIme = korisnickoIme;
            this.Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Jmbg = jmbg;
            KontaktTelefon = telefon;
            Email = email;
            Uloga = Uloga.DISPECER;
        }

        public Vozac KreirajVozaca(Vozac vozac)
        {
            return vozac;
        }
    }
}