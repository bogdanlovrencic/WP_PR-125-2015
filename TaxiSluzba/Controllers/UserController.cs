using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TaxiSluzba.Models;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;

namespace TaxiSluzba.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            List<Korisnik> korisnici = (List<Korisnik>)Session["korisnici"];
           
            if (korisnici == null)
            {
                korisnici = new List<Korisnik>();
                Session["korisnici"] = korisnici;
            }

            ViewBag.Registrovani = korisnici;

            return View();
        }

        [HttpPost]
        public ActionResult AddUser()
        {
           
            List<Korisnik> korisnici = (List<Korisnik>)Session["korisnici"];
            

            Korisnik korisnik = (Korisnik)Session["korisnik"];
          

            if(korisnik == null)
            {
                korisnik = new Korisnik();
                Session["korisnik"] = korisnik;
            }

            korisnik.Ime = Request["ime"];
            korisnik.Prezime = Request["prezime"];
            korisnik.KorisnickoIme = Request["korisnickoIme"];
            korisnik.Lozinka = Request["lozinka"];
            korisnik.Pol = (Pol)Enum.Parse(typeof(Pol),Request["pol"]);
            korisnik.Jmbg = Request["jmbg"];
            korisnik.KontaktTelefon = Request["telefon"];
            korisnik.Email = Request["email"];
           
            
            //validacija
            if (korisnik.Ime == "" || korisnik.Prezime == "" || korisnik.KorisnickoIme == "" || korisnik.Lozinka == "" || korisnik.Jmbg == "" || korisnik.KontaktTelefon == "" || korisnik.Email == "")
            {
                return RedirectToAction("Validation");
            }

            //upis registrovanog korisnika u bazu i u txt fajl

            if (!DataBase.registrovaniKorisnici.ContainsKey(korisnik.KorisnickoIme))
            {
                DataBase.registrovaniKorisnici.Add(korisnik.KorisnickoIme, korisnik);
                DataBase.UpisiRegKorisnike();
                ViewBag.Message = "Uspesno ste se registrovali!";
            }


            korisnici.Add(korisnik);
            ViewBag.RegistrovaniKor = korisnici;

            return View("RegistrationResult");
        }

        [HttpPost]
        public ActionResult Login(string user,string pass)
        {
            
            Korisnik korisnik = (Korisnik)Session["korisnik"];
          

            if (korisnik == null)
            {
                korisnik = new Korisnik();
                Session["korisnik"] = korisnik;
            }

            if (DataBase.registrovaniKorisnici.ContainsKey(user))
            {
                if (DataBase.registrovaniKorisnici[user].Lozinka == pass)
                {
                    if (DataBase.registrovaniKorisnici[user].Uloga == Uloga.DISPECER)
                    {
                        Session["korisnik"] = DataBase.registrovaniKorisnici[user];
                        return View("Dispecer");
                    }
                    else if (DataBase.registrovaniKorisnici[user].Uloga == Uloga.VOZAC)
                    {
                        Adresa adresa = new Adresa("", "", "", "");
                        Lokacija l = new Lokacija(1, 1, adresa);
                        DataBase.vozaci[user].Lokacija = l;
                        Vozac v = DataBase.vozaci[user];
                        Session["korisnik"] = DataBase.registrovaniKorisnici[user];

                        return View("Vozac", v);
                    }

                    else
                    {
                        Korisnik musterija = new Musterija();
                        musterija = DataBase.registrovaniKorisnici[user];
                        Session["korisnik"] = DataBase.registrovaniKorisnici[user];

                        return View("Musterija", musterija);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Pogresna lozinka!";
                    return View("LoginError");
                }     
            }
           
            else
            {
                ViewBag.ErrorMessage = "Niste u mogucnosti da se prijavite, jer niste registrovani!";
                return View("RegistrationError");
            }
          
        }

        //[HttpPost]
        //public ActionResult Preusmeri()
        //{
        //    Korisnik kor = (Korisnik)Session["korisnik"];

        //    if (kor == null)
        //    {
        //        kor = new Korisnik();
        //        Session["korisnik"] = kor;
        //    }

        //}

        public ActionResult Error()
        {
            ViewBag.ErrorMessage = "Neuspesno registrovanje, korisnik sa unetim korisnickim imenom vec postoji!";
            return View("RegistrationError");
        }

        public ActionResult Validation()
        {
            ViewBag.ErrorMessage = "Morate popuniti sva polja za registraciju!";
            return View("RegistrationValid");
        }

        public ActionResult prijava()
        {
            return View("Login");
        }

        public ActionResult LogOut() {

            Session.Abandon();
            Korisnik k = new Korisnik();
            Session["korisnik"] = k;
            return View("Login");
        }

        [HttpPost]
        public ActionResult IzmenaPodataka(string user, string pass, string ime, string prezime, string pol, string jmbg, string telefon, string email)
        {
            Korisnik k = (Korisnik)Session["korisnik"];

            if (k == null)
            {
                k = new Korisnik();
                Session["korisnik"] = k;
            }

            foreach (Korisnik korisnik in DataBase.registrovaniKorisnici.Values)
            {
                if (korisnik.KorisnickoIme == k.KorisnickoIme && korisnik.Uloga == Uloga.MUSTERIJA)
                {
                    DataBase.registrovaniKorisnici[user].Lozinka = pass;
                    DataBase.registrovaniKorisnici[user].Ime = ime;
                    DataBase.registrovaniKorisnici[user].Prezime = prezime;
                    Pol p;
                    if (pol == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;
                    DataBase.registrovaniKorisnici[user].Pol = p;
                    DataBase.registrovaniKorisnici[user].Jmbg = jmbg;
                    DataBase.registrovaniKorisnici[user].KontaktTelefon = telefon;
                    DataBase.registrovaniKorisnici[user].Email = email;

                    ViewBag.Message = "Podaci su uspesno izmenjeni.";
                    return View("Izmena");
                }
        
            }

            ViewBag.Message = "Podaci nisu izmenjeni, desila se neka greska!";
            return View("Izmena");
            
        }
       
    }
}