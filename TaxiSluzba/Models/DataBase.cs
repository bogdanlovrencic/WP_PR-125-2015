using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class DataBase
    {
        public static Dictionary<string, Korisnik> registrovaniKorisnici = new Dictionary<string, Korisnik>();
        public static Dictionary<string, Vozac> vozaci = new Dictionary<string, Vozac>();

        public static void UpisiRegKorisnike()
        {
            string upis = "";
            if (File.Exists("D:\\fax\\III godina\\Web programiranje\\Projekat\\RegistrovaniKorisnici.txt"))
            {
                StreamWriter sw = new StreamWriter("D:\\fax\\III godina\\Web programiranje\\Projekat\\RegistrovaniKorisnici.txt");

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
                StreamWriter sw = new StreamWriter("D:\\fax\\III godina\\Web programiranje\\Projekat\\RegistrovaniKorisnici.txt");

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

        //public static void UpisiVozace()
        //{
        //    string s = "";
        //    string[] line;

        //    if (File.Exists(""))
        //    { }
        //}
    }
}