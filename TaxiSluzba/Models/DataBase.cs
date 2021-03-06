﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace TaxiSluzba.Models
{
    public class DataBase
    {
        public static Dictionary<string, Korisnik> registrovaniKorisnici = new Dictionary<string, Korisnik>();
        public static Dictionary<string, Vozac> vozaci = new Dictionary<string, Vozac>();
        public static Dictionary<string, Voznja> sveVoznje = new Dictionary<string, Voznja>();
        public static Dictionary<string, Voznja> voznjeNepoznatihDispecera = new Dictionary<string, Voznja>();
        public static Dictionary<string, Voznja> neuspesneVoznje = new Dictionary<string, Voznja>();
        public static Dictionary<string, Vozac> slobodniVozaci = new Dictionary<string, Vozac>();
        public static Dictionary<string, Voznja> voznjeNaCekanju = new Dictionary<string, Voznja>();

        public static void UpisiRegKorisnike()
        {
            string upis = "";

            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.DISPECER)
                    {
                        upis += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Jmbg + "_" + k.Pol.ToString() + "_" + k.KontaktTelefon + "_" + k.Email + "_" + k.Uloga.ToString() + "\n");
                    }
                }
                sw.WriteLine(upis);
                sw.Close();
            }
            else
            {
               
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.DISPECER)
                    {
                        upis += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Jmbg + "_" + k.Pol.ToString() + "_" + k.KontaktTelefon + "_" + k.Email + "_" + k.Uloga.ToString() + "\n");
                    }
                }
                sw.WriteLine(upis);
                sw.Close();
            }
        }

        public static void UpisiVoznje()
        {
            string upis = "";

            string path = "~/App_Data/SveVoznje.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Voznja v in sveVoznje.Values)
                {
                    upis += string.Format(v.DatumIvremePorudz.ToString() + "|" + v.DolaznaLokacija.Adress.Ulica + "|" + v.DolaznaLokacija.Adress.Broj + "|" + v.DolaznaLokacija.Adress.Grad +
                        "|" + v.DolaznaLokacija.Adress.PostanskiBroj + "|" + v.TipAuta.ToString() + "|" + v.Musterija.KorisnickoIme + "|"
                        + v.Odrediste.Adress.Ulica + "|" + v.Odrediste.Adress.Broj + "|" + v.Odrediste.Adress.Grad + "|" + v.Odrediste.Adress.PostanskiBroj + "|"
                        + v.dispecer.KorisnickoIme + "|" + v.vozac.KorisnickoIme + "|" + v.Iznos + "|"
                        + v.komentar.Opis + "|" + v.komentar.OcenaVoznje.ToString() + "|" + v.komentar.DatumObjave.ToString() + "|" + v.Status.ToString() + "\n");
                }

                sw.WriteLine(upis);
                sw.Close();
            }

          


        }

        public static void UpisiVozace()
        {
            string s = "";

            string path = "~/App_Data/Vozaci.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                foreach (Vozac vozac in vozaci.Values)
                {
                    s += string.Format(vozac.KorisnickoIme + "_" + vozac.Lozinka + "_" + vozac.Ime + "_" + vozac.Prezime + "_" + vozac.Pol.ToString() + "_" + vozac.Jmbg + "_" + vozac.KontaktTelefon + "_" + vozac.Email
                       + "_" + vozac.Auto.TaxiId + "|" + vozac.Auto.BrRegOznake + "|" + vozac.Auto.Godiste + "|" + vozac.Auto.tipAutomobila.ToString() + "_"
                       + vozac.Lokacija.Adress.Ulica + "|" + vozac.Lokacija.Adress.Broj + "|" + vozac.Lokacija.Adress.Grad + "|" + vozac.Lokacija.Adress.PostanskiBroj + "_" + vozac.Uloga.ToString() + "\n");
                }

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(s);
                sw.Close();
            }
        }

        public static void AzurirajPodatkeMusterije()
        {
            string upis = "";

            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (Korisnik k in registrovaniKorisnici.Values)
                {
                    if (k.Uloga != Uloga.DISPECER)
                    {
                        upis += string.Format(k.Ime + "_" + k.Prezime + "_" + k.KorisnickoIme + "_" + k.Lozinka + "_" + k.Jmbg + "_" + k.Pol.ToString() + "_" + k.KontaktTelefon + "_" + k.Email + "_" + k.Uloga.ToString() + "\n");
                    }
                }
                sw.WriteLine(upis);
                sw.Close();


            }
        }
    }
}