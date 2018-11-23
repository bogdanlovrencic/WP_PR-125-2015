using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Vozac : Korisnik
    {
        public Lokacija  Lokacija{ get; set; }
        public Automobil Auto { get; set; }
        public bool slobodan;

        public Vozac(Korisnik korisnik,Lokacija lokacija,Automobil auto)
        {
            Ime = korisnik.Ime;
            Prezime = korisnik.Prezime;
            KorisnickoIme = korisnik.KorisnickoIme;
            Lozinka = korisnik.Lozinka;
            Pol = korisnik.Pol;
            Jmbg = korisnik.Jmbg;
            KontaktTelefon = korisnik.KontaktTelefon;
            Email = korisnik.Email;
            Uloga =Uloga.VOZAC;
            Lokacija = lokacija;
            Auto = auto;
            voznje = new List<Voznja>();
            slobodan = true;


        }

       
    }
}