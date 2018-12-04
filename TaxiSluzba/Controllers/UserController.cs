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

        [HttpPost]
        public ActionResult Preusmeri()
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    return View("Musterija", kor);
                }
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    return View("Vozac", DataBase.vozaci[kor.KorisnickoIme]);
                }
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DISPECER)
                {
                    return View("Dispecer");
                }
            }

            ViewBag.ErrorMessage = "Nesto ne valja!";
            return View("Izmena");
        }

        [HttpPost]
        public ActionResult IzmenaPodataka(string username, string pass, string ime, string prezime, string pol, string jmbg, string telefon, string email)
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
                    DataBase.registrovaniKorisnici[username].Lozinka = pass;
                    DataBase.registrovaniKorisnici[username].Ime = ime;
                    DataBase.registrovaniKorisnici[username].Prezime = prezime;
                    Pol p;
                    if (pol == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;
                    DataBase.registrovaniKorisnici[username].Pol = p;
                    DataBase.registrovaniKorisnici[username].Jmbg = jmbg;
                    DataBase.registrovaniKorisnici[username].KontaktTelefon = telefon;
                    DataBase.registrovaniKorisnici[username].Email = email;

                    ViewBag.Message = "Podaci su uspesno izmenjeni.";
                    return View("Izmena");
                }

            }

            ViewBag.Message = "Podaci nisu izmenjeni, desila se neka greska!";
            return View("Izmena");

        }

        [HttpPost]
        public ActionResult IzmenaVoznje(string ulica,string broj,string grad,string postanskiBroj,string tipAuta,string username,string datumVoznje)
        {
            Korisnik kor =(Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor= new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == username && k.Uloga == Uloga.MUSTERIJA)
                {
                    TipAutomobila tipAutomobila;

                    if (tipAuta == "KOMBI")
                        tipAutomobila = TipAutomobila.KOMBI;                  
                    else
                        tipAutomobila = TipAutomobila.PUTNICKI_AUTOMOBIL;

                    foreach (Voznja v in DataBase.registrovaniKorisnici[username].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            v.DolaznaLokacija.Adress.Ulica = ulica;
                            v.DolaznaLokacija.Adress.Broj = broj;
                            v.DolaznaLokacija.Adress.Grad = grad;
                            v.DolaznaLokacija.Adress.PostanskiBroj = postanskiBroj;
                            v.TipAuta = tipAutomobila;
                            AzurirajVoznju(v, k.KorisnickoIme);
                        }
                    }

                    DataBase.UpisiVoznje();

                    ViewBag.Message = "Voznja je uspesno izmenjena.";
                    return View("Izmena");
                }
            }

            ViewBag.Message = "Voznja nije izmenjena, dogodila se greska.";
            return View("Izmena");
        }


        public Voznja PopuniPolja(Voznja v)
        {
            if (v.dispecer == null)
            {
                v.dispecer = new Dispecer("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");
            }

            if (v.Musterija == null)
            {
                v.Musterija = new Musterija("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");
            }

            if (v.Odrediste == null)
            {
                v.Odrediste = new Lokacija(1,1,new Adresa("-","-","-","-"));
            }

            if (v.vozac == null)
            {
                Korisnik k = new Korisnik("-","-","-","-",Pol.MUSKI,"-","-","-");
                Lokacija l = new Lokacija(1,1,new Adresa("-","-","-","-"));
                Automobil a = new Automobil(new Vozac(), "-", "-","-", TipAutomobila.PUTNICKI_AUTOMOBIL);
                v.vozac = new Vozac(k,l,a);
            }

            if (v.komentar == null)
            {
                v.komentar = new Komentar("-",DateTime.Now,v.Musterija,v,OcenaVoznje.NULA);
            }

            if (v.Iznos == null)
            {
                v.Iznos = "400";
            }

            return v;
        }

        [HttpPost]
        public ActionResult PoruciVoznju(string korisnickoIme, string ulica, string broj, string grad, string postanskiBroj, string tipPrevoza)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];

            if (korisnik == null)
            {
                korisnik = new Korisnik();
                Session["korisnik"] = korisnik;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == korisnik.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    Adresa adresa = new Adresa(ulica, broj, grad, postanskiBroj);
                    Lokacija lok = new Lokacija(1, 1, adresa);
                    TipAutomobila tipAuto;
                    if (tipPrevoza == "KOMBI")
                    {
                        tipAuto = TipAutomobila.KOMBI;
                    }
                    else
                    {
                        tipAuto = TipAutomobila.PUTNICKI_AUTOMOBIL;
                    }

                    Musterija m = new Musterija(DataBase.registrovaniKorisnici[korisnickoIme].Ime, DataBase.registrovaniKorisnici[korisnickoIme].Prezime, DataBase.registrovaniKorisnici[korisnickoIme].KorisnickoIme, DataBase.registrovaniKorisnici[korisnickoIme].Lozinka, DataBase.registrovaniKorisnici[korisnickoIme].Pol, DataBase.registrovaniKorisnici[korisnickoIme].Jmbg, DataBase.registrovaniKorisnici[korisnickoIme].KontaktTelefon, DataBase.registrovaniKorisnici[korisnickoIme].Email);
                    Voznja voznja = new Voznja(DateTime.Now, lok, tipAuto, m);
                    voznja.Status = StatusVoznje.KREIRANA_NA_CEKANJU;
                    voznja.komentar = new Komentar("-", DateTime.Now, DataBase.registrovaniKorisnici[korisnickoIme], voznja, OcenaVoznje.ČETIRI);

                    DataBase.registrovaniKorisnici[korisnickoIme].voznje.Add(voznja);
                    AzurirajVoznju(voznja, k.KorisnickoIme);

                    DataBase.UpisiVoznje();

                    ViewBag.Message = "Voznja je uspesno porucena!";
                    return View("Izmena");

                }
            }

            ViewBag.Message = "Voznja nije porucena desila se greska!";
            return View("Izmena");

        }

        public void AzurirajVoznju(Voznja voznja, string korisnik)
        {
            Voznja retVoznja = new Voznja();
            retVoznja = PopuniPolja(voznja);
           
            if (DataBase.sveVoznje.ContainsKey(voznja.DatumIvremePorudz.ToString()))
                DataBase.sveVoznje[voznja.DatumIvremePorudz.ToString()] = retVoznja;
            else
                DataBase.sveVoznje.Add(voznja.DatumIvremePorudz.ToString(), retVoznja);
        }

        [HttpPost]
        public ActionResult MusterijaMenjaVoznju(string korisnickoIme, string datumVoznje)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];

            if (korisnik == null)
            {
                korisnik = new Korisnik();
                Session["korisnik"] = korisnik;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == korisnik.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    Voznja voznja = new Voznja();
                    foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            voznja = v;
                            AzurirajVoznju(voznja,korisnik.KorisnickoIme);
                        }
                    }

                    return View("MusterijaMenjaVoznju", voznja);
                }
            }

            ViewBag.Message = "Voznja nije promenjena, desila se greska!";
            return View("Izmena");

        }

        [HttpPost]
        public ActionResult MusterijaOtkazujeVoznju(string korisnickoIme,string datumVoznje)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    Voznja voznja = new Voznja();
                    foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            v.Status = StatusVoznje.OTKAZANA;
                            voznja = v;
                            AzurirajVoznju(v, kor.KorisnickoIme);
                        }
                    }

                    return View("Komentar",voznja);
                }
            }

            ViewBag.ErrorMessage = "Nemoguce je otkazati voznju,desila se greska!";
            return View("Greska");

        }

        [HttpPost]
        public ActionResult KomentarMusterije(string komentar,string ocena,string datumVoznje,string usernameMusterije)
        {
            Korisnik kor =(Korisnik) Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    foreach (Voznja v in DataBase.registrovaniKorisnici[usernameMusterije].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            if (ocena == null)
                            {
                                Komentar kom = new Komentar(komentar, DateTime.Now, DataBase.registrovaniKorisnici[usernameMusterije], v, OcenaVoznje.NULA);
                                v.komentar = kom;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }

                            else
                            {
                                OcenaVoznje o;

                                if (ocena == "JEDAN")
                                {
                                    o = OcenaVoznje.JEDAN;
                                }
                                else if (ocena == "DVA")
                                {
                                    o = OcenaVoznje.DVA;
                                }
                                else if (ocena == "TRI")
                                {
                                    o = OcenaVoznje.TRI;
                                }
                                else if (ocena == "CETIRI")
                                {
                                    o = OcenaVoznje.ČETIRI;
                                }
                                else 
                                {
                                    o = OcenaVoznje.PET;
                                }

                                Komentar ko = new Komentar(komentar,DateTime.Now,DataBase.registrovaniKorisnici[usernameMusterije],v,o);
                                v.komentar = ko;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }
                        }
                    }

                    DataBase.UpisiVoznje();
                    ViewBag.Message = "Komenat je uspesno postavljen.";
                    return View("Izmena");
                }
            }

            ViewBag.Message = "Nije moguce ostaviti komentar!";
            return View("Izmena");

        }

        [HttpPost]
        public ActionResult KomentarUspesno(string korisnickoIme,string datumVoznje)
        {
            Korisnik kor =(Korisnik) Session["korisnik"];

            if(kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.MUSTERIJA)
                {
                    Voznja voznja = new Voznja();

                    foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            voznja = v;
                            AzurirajVoznju(voznja, kor.KorisnickoIme);
                        }
                    }

                    return View("Komentar", voznja);
                }
            }

            ViewBag.Message = "Nije moguce ostaviti komentar,desila se greska";
            return View("Izmena");

        }

       

        [HttpPost]
        public ActionResult Sortiranje(string sortirajPo, string korisnickoIme, string korisnickoImeVozaca)
        {
            List<Voznja> voznje = new List<Voznja>();
            List<Voznja> povVoznje = new List<Voznja>();
            List<Voznja> sortirane = new List<Voznja>();

            if (korisnickoIme != null)
            {
                foreach (Voznja v in voznje)
                {
                    povVoznje.Add(v);
                }

                if (sortirajPo == "datumu")
                {
                    sortirane = povVoznje.OrderBy(o => o.DatumIvremePorudz).ToList();
                }
                else
                {
                    sortirane = povVoznje.OrderBy(o => o.komentar.OcenaVoznje).ToList();
                }
            }

            else if (korisnickoImeVozaca != null)
            {
                voznje = DataBase.vozaci[korisnickoImeVozaca].voznje;

                foreach (Voznja v in voznje)
                {
                    if (v.komentar == null)
                    {
                        v.komentar = new Komentar("-", DateTime.Now, v.Musterija, v, OcenaVoznje.NULA);
                    }

                    if (v.vozac.KorisnickoIme == korisnickoImeVozaca)
                    {
                        povVoznje.Add(v);
                    }
                }

                if (sortirajPo == "datumu")
                    sortirane = povVoznje.OrderBy(o => o.DatumIvremePorudz).ToList();
                else
                    sortirane = povVoznje.OrderBy(o => o.komentar.OcenaVoznje).ToList();
            }

            else
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    voznje.Add(v);
                }

                if (sortirajPo == "datumu")
                    sortirane = voznje.OrderBy(o => o.DatumIvremePorudz).ToList();
                else
                    sortirane = voznje.OrderBy(o => o.komentar.OcenaVoznje).ToList();
            }

            return View("RezultatSortiranja", sortirane);
        }

        [HttpPost]
        public ActionResult Filtriranje(string korisnickoIme, string statusVoznje, string korisnickoImeVozaca)
        {
            List<Voznja> voznje = new List<Voznja>();

            if (korisnickoIme != null && korisnickoIme != "-")
            {
                foreach (Voznja v in DataBase.vozaci[korisnickoImeVozaca].voznje)
                {
                    if (v.Musterija.KorisnickoIme == korisnickoIme)
                        if (v.Status.ToString() == statusVoznje)
                            voznje.Add(v);
                }
            }
            else if (korisnickoImeVozaca != null)
            {
                foreach (Voznja v in DataBase.vozaci[korisnickoImeVozaca].voznje)
                {
                    if (v.komentar == null)
                        v.komentar = new Komentar("-", DateTime.Now, v.Musterija, v, OcenaVoznje.NULA);

                    if (v.vozac.KorisnickoIme == korisnickoImeVozaca)
                        if (v.Status.ToString() == statusVoznje)
                            voznje.Add(v);
                }
            }
            else
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.Status.ToString() == statusVoznje)
                    {
                        PopuniPolja(v);
                        voznje.Add(v);
                    }
                }
            }

            return View("RezultatFiltriranja",voznje);
        }

        [HttpPost]
        public ActionResult PretragaMusterija(string datumOd, string datumDo, string ocenaOd, string ocenaDo,string cenaOd,string cenaDo, string korisnickoIme)
        {
            List<Voznja> voznje = new List<Voznja>();

            if (datumOd != "" && datumDo != "")
            {
                string[] splitovano = datumOd.Split(' ', '.', ':');
                DateTime DatumOd = new DateTime(Int32.Parse(splitovano[2]), Int32.Parse(splitovano[1]), Int32.Parse(splitovano[0]), Int32.Parse(splitovano[4]), Int32.Parse(splitovano[5]), Int32.Parse(splitovano[6]));

                splitovano = datumDo.Split(' ', ',', ':');
                DateTime DatumDo = new DateTime(Int32.Parse(splitovano[2]), Int32.Parse(splitovano[1]), Int32.Parse(splitovano[0]), Int32.Parse(splitovano[4]), Int32.Parse(splitovano[5]), Int32.Parse(splitovano[6]));

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.DatumIvremePorudz > DatumOd && v.DatumIvremePorudz < DatumDo)
                    {
                        voznje.Add(v);
                    }
                }
            }

            else if (datumOd != "")
            {
                string[] splitovano = datumOd.Split(' ', '.', ':');
                DateTime DatumOd = new DateTime(Int32.Parse(splitovano[2]), Int32.Parse(splitovano[1]), Int32.Parse(splitovano[0]), Int32.Parse(splitovano[4]), Int32.Parse(splitovano[5]), Int32.Parse(splitovano[6]));

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.DatumIvremePorudz > DatumOd)
                    {
                        voznje.Add(v);
                    }
                }
            }

            else if (datumDo != "")
            {
                string []splitovano = datumDo.Split(' ', ',', ':');
                DateTime DatumDo = new DateTime(Int32.Parse(splitovano[2]), Int32.Parse(splitovano[1]), Int32.Parse(splitovano[0]), Int32.Parse(splitovano[4]), Int32.Parse(splitovano[5]), Int32.Parse(splitovano[6]));

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.DatumIvremePorudz < DatumDo)
                    {
                        voznje.Add(v);
                    }
                }
            }

            if (ocenaOd != "NULA" && ocenaDo != "NULA")
            {
                OcenaVoznje ocenaVoznjeOd;
                OcenaVoznje ocenaVoznjeDo;

                if (ocenaOd == "JEDAN")
                    ocenaVoznjeOd = OcenaVoznje.JEDAN;
                else if (ocenaOd == "DVA")
                    ocenaVoznjeOd = OcenaVoznje.DVA;
                else if (ocenaOd == "TRI")
                    ocenaVoznjeOd = OcenaVoznje.TRI;
                else if (ocenaOd == "CETIRI")
                    ocenaVoznjeOd = OcenaVoznje.ČETIRI;
                else if (ocenaOd == "PET")
                    ocenaVoznjeOd = OcenaVoznje.PET;
                else
                    ocenaVoznjeOd = OcenaVoznje.NULA;


                if (ocenaDo == "JEDAN")
                    ocenaVoznjeDo = OcenaVoznje.JEDAN;
                else if (ocenaDo == "DVA")
                    ocenaVoznjeDo = OcenaVoznje.DVA;
                else if (ocenaDo == "TRI")
                    ocenaVoznjeDo = OcenaVoznje.TRI;
                else if (ocenaDo == "CETIRI")
                    ocenaVoznjeDo = OcenaVoznje.ČETIRI;
                else if (ocenaDo == "PET")
                    ocenaVoznjeDo = OcenaVoznje.PET;
                else
                    ocenaVoznjeDo = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.komentar.OcenaVoznje >= ocenaVoznjeOd && v.komentar.OcenaVoznje <= ocenaVoznjeDo)
                        voznje.Add(v);
                }

             
            }

            else if (ocenaOd != "NULA")
            {
                OcenaVoznje ocenaVoznjeOd;

                if (ocenaOd == "JEDAN")
                    ocenaVoznjeOd = OcenaVoznje.JEDAN;
                else if (ocenaOd == "DVA")
                    ocenaVoznjeOd = OcenaVoznje.DVA;
                else if (ocenaOd == "TRI")
                    ocenaVoznjeOd = OcenaVoznje.TRI;
                else if (ocenaOd == "CETIRI")
                    ocenaVoznjeOd = OcenaVoznje.ČETIRI;
                else if (ocenaOd == "PET")
                    ocenaVoznjeOd = OcenaVoznje.PET;
                else
                    ocenaVoznjeOd = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.komentar.OcenaVoznje >= ocenaVoznjeOd)
                        voznje.Add(v);
                }
            }

            else if (ocenaDo != "NULA")
            {
                OcenaVoznje ocenaVoznjeDo;

                if (ocenaDo == "JEDAN")
                    ocenaVoznjeDo = OcenaVoznje.JEDAN;
                else if (ocenaDo == "DVA")
                    ocenaVoznjeDo = OcenaVoznje.DVA;
                else if (ocenaDo == "TRI")
                    ocenaVoznjeDo = OcenaVoznje.TRI;
                else if (ocenaDo == "CETIRI")
                    ocenaVoznjeDo = OcenaVoznje.ČETIRI;
                else if (ocenaDo == "PET")
                    ocenaVoznjeDo = OcenaVoznje.PET;
                else
                    ocenaVoznjeDo = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (v.komentar.OcenaVoznje <= ocenaVoznjeDo)
                        voznje.Add(v);
                }
            }

            if (cenaOd != "" && cenaOd != "")
            {
                int cenaMin = Int32.Parse(cenaOd);
                int cenaMax = Int32.Parse(cenaDo);


                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (Int32.Parse(v.Iznos) >= cenaMin && Int32.Parse(v.Iznos) <= cenaMax)
                        voznje.Add(v);
                }
            }

            else if (cenaOd != "")
            {
                int cenaMin = Int32.Parse(cenaOd);

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (Int32.Parse(v.Iznos) >= cenaMin)
                        voznje.Add(v);
                }
            }

            else if (cenaDo != "")
            {
                int cenaMax = Int32.Parse(cenaDo);

                foreach (Voznja v in DataBase.registrovaniKorisnici[korisnickoIme].voznje)
                {
                    if (Int32.Parse(v.Iznos) <= cenaMax)
                        voznje.Add(v);
                }
            }

            return View("RezultatPretrage", voznje);
        }

        [HttpPost]
        public ActionResult KreirajVozaca(string username, string password, string ime, string prezime, string pol, string jmbg, string telefon, string email, string godisteAuta, string regAuta, string brAuta, string tipAuta)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DISPECER)
                {
                    Pol p;
                    if (pol == "MUSKI")
                        p = Pol.MUSKI;
                    else
                        p = Pol.ZENSKI;

                    Korisnik korisnik = new Korisnik(ime, prezime, username, password, p, jmbg, telefon, email);
                    korisnik.Uloga = Uloga.VOZAC;

                    TipAutomobila tip;
                    if (tipAuta == "PUTNICKI_AUTOMOBIL")
                        tip = TipAutomobila.PUTNICKI_AUTOMOBIL;
                    else
                        tip = TipAutomobila.KOMBI;

                    Automobil auto = new Automobil(null, godisteAuta, regAuta, brAuta, tip);
                    Vozac v = new Vozac(korisnik, null, auto);
                    auto.vozac = v;
                    v.Lokacija = new Lokacija(1, 1, new Adresa("-", "-", "-", "-"));

                    DataBase.vozaci.Add(v.KorisnickoIme, v);
                    DataBase.registrovaniKorisnici.Add(korisnik.KorisnickoIme, korisnik);

                    DataBase.UpisiVozace();
                    DataBase.UpisiRegKorisnike();

                    ViewBag.Message = "Vozac je uspesno kreiran.";

                    return View("Izmena");
                }
            }

            ViewBag.Message = "Vozac nije kreiran,desila se greska!";
            return View("Izmena");
        }

        [HttpPost]
        public ActionResult KreirajVoznju(string ulica, string broj, string grad, string postanskoBroj, string tipPrevoza, string izabraniVozac)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DISPECER)
                {
                    Adresa adresa = new Adresa(ulica, broj, grad, postanskoBroj);
                    Lokacija l = new Lokacija(1, 1, adresa);

                    TipAutomobila tip;
                    if (tipPrevoza == "KOMBI")
                        tip = TipAutomobila.KOMBI;
                    else
                        tip = TipAutomobila.PUTNICKI_AUTOMOBIL;

                    Voznja v = new Voznja(DateTime.Now, l, tip, null);

                    v.dispecer = (Dispecer)DataBase.registrovaniKorisnici[kor.KorisnickoIme];
                    v.dispecer.voznje.Add(v);
                    v.vozac = DataBase.vozaci[izabraniVozac];
                    v.Status = StatusVoznje.FORMIRANA;
                    v.Musterija = new Musterija("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");

                    AzurirajVoznju(v, kor.KorisnickoIme);

                    DataBase.vozaci[izabraniVozac].voznje.Add(v);

                    ViewBag.Message = "Voznja je uspesno kreirana.";
                    return View("Izmena");

                }
            }

            ViewBag.Message = "Voznja nije kreirana,desila se greska!";
            return View("Izmena");
        }

        [HttpPost]
        public ActionResult DodeliVoznjuVozacu(string voznja, string slobodanVozac)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.DISPECER)
                {
                    Voznja povVoznja = DataBase.sveVoznje[voznja];
                    povVoznja.Status = StatusVoznje.OBRADJENA;
                    povVoznja.dispecer = (Dispecer)DataBase.registrovaniKorisnici[kor.KorisnickoIme];
                    DataBase.registrovaniKorisnici[kor.KorisnickoIme].voznje.Add(povVoznja);

                    Vozac vozac = DataBase.vozaci[slobodanVozac];
                    vozac.voznje.Add(povVoznja);

                    povVoznja.vozac = vozac;

                    AzurirajVoznju(povVoznja, kor.KorisnickoIme);

                    ViewBag.Message = "Voznje je uspesno kreirana.";
                    return View("Izmena");
                }
            }

            ViewBag.Message = "Voznja nije kreirana,desila se greska!";
            return View("Izmena");
        }

        public ActionResult PretragaDispecer(string datumOd,string datumDo,string ocenaOd,string ocenaDo,string cenaOd,string cenaDo,string imeVozaca,string prezimeVozaca,string imeMusterije,string prezimeMusterije)
        {
            List<Voznja> voznje = new List<Voznja>();

            if (datumOd != "" && datumDo != "")
            {
                string[] splitted = datumOd.Split(' ', '.', ':');
                DateTime dateTimeOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]), Int32.Parse(splitted[4]), Int32.Parse(splitted[5]), Int32.Parse(splitted[6]));

                splitted = datumDo.Split(' ', '.', ':');
                DateTime dateTimeDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]), Int32.Parse(splitted[4]), Int32.Parse(splitted[5]), Int32.Parse(splitted[6]));

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.DatumIvremePorudz > dateTimeOd && v.DatumIvremePorudz < dateTimeDo)
                        voznje.Add(v);
                }
            }

            else if (datumOd != "")
            {
                string[] splitted = datumOd.Split(' ', '.', ':');
                DateTime dateTimeOd = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]), Int32.Parse(splitted[4]), Int32.Parse(splitted[5]), Int32.Parse(splitted[6]));

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.DatumIvremePorudz > dateTimeOd)
                        voznje.Add(v);
                }
            }

            else if (datumDo != "")
            {
                string[] splitted = datumDo.Split(' ', '.', ':');
                DateTime dateTimeDo = new DateTime(Int32.Parse(splitted[2]), Int32.Parse(splitted[1]), Int32.Parse(splitted[0]), Int32.Parse(splitted[4]), Int32.Parse(splitted[5]), Int32.Parse(splitted[6]));

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.DatumIvremePorudz < dateTimeDo)
                        voznje.Add(v);
                }
            }

            if (ocenaOd != "" && ocenaDo != "")
            {
                OcenaVoznje ocenaOD;
                OcenaVoznje ocenaDO;

                if (ocenaOd == "JEDAN")
                    ocenaOD = OcenaVoznje.JEDAN;
                else if (ocenaOd == "DVA")
                    ocenaOD = OcenaVoznje.DVA;
                else if (ocenaOd == "TRI")
                    ocenaOD = OcenaVoznje.TRI;
                else if (ocenaOd == "CETIRI")
                    ocenaOD = OcenaVoznje.ČETIRI;
                else if (ocenaOd == "PET")
                    ocenaOD = OcenaVoznje.PET;
                else
                    ocenaOD = OcenaVoznje.NULA;


                if (ocenaDo == "JEDAN")
                    ocenaDO = OcenaVoznje.JEDAN;
                else if (ocenaDo == "DVA")
                    ocenaDO = OcenaVoznje.DVA;
                else if (ocenaDo == "TRI")
                    ocenaDO = OcenaVoznje.TRI;
                else if (ocenaDo == "CETIRI")
                    ocenaDO = OcenaVoznje.ČETIRI;
                else if (ocenaDo == "PET")
                    ocenaDO = OcenaVoznje.PET;
                else
                    ocenaDO = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.komentar.OcenaVoznje >= ocenaOD && v.komentar.OcenaVoznje <= ocenaDO)
                        voznje.Add(v);
                }
            }

            else if (ocenaOd != "")
            {
                OcenaVoznje ocenaOD;

                if (ocenaOd == "JEDAN")
                    ocenaOD = OcenaVoznje.JEDAN;
                else if (ocenaOd == "DVA")
                    ocenaOD = OcenaVoznje.DVA;
                else if (ocenaOd == "TRI")
                    ocenaOD = OcenaVoznje.TRI;
                else if (ocenaOd == "CETIRI")
                    ocenaOD = OcenaVoznje.ČETIRI;
                else if (ocenaOd == "PET")
                    ocenaOD = OcenaVoznje.PET;
                else
                    ocenaOD = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.komentar.OcenaVoznje >= ocenaOD)
                        voznje.Add(v);
                }
            }

            else if (ocenaDo != "")
            {
                OcenaVoznje ocenaDO;

                if (ocenaDo == "JEDAN")
                    ocenaDO = OcenaVoznje.JEDAN;
                else if (ocenaDo == "DVA")
                    ocenaDO = OcenaVoznje.DVA;
                else if (ocenaDo == "TRI")
                    ocenaDO = OcenaVoznje.TRI;
                else if (ocenaDo == "CETIRI")
                    ocenaDO = OcenaVoznje.ČETIRI;
                else if (ocenaDo == "PET")
                    ocenaDO = OcenaVoznje.PET;
                else
                    ocenaDO = OcenaVoznje.NULA;

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.komentar.OcenaVoznje <= ocenaDO)
                        voznje.Add(v);
                }
            }

            if (cenaOd != "" && cenaDo != "")
            {
                int cenaMin = Int32.Parse(cenaOd);
                int cenaMax = Int32.Parse(cenaDo);

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (Int32.Parse(v.Iznos) >= cenaMin && Int32.Parse(v.Iznos) <= cenaMax)
                        voznje.Add(v);
                }
            }

            else if (cenaOd != "")
            {
                int cenaMin = Int32.Parse(cenaOd);

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (Int32.Parse(v.Iznos) >= cenaMin)
                        voznje.Add(v);
                }
            }
            else if (cenaDo != "")
            {
                int cenaMax = Int32.Parse(cenaDo);

                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (Int32.Parse(v.Iznos) <= cenaMax)
                        voznje.Add(v);
                }
            }

            if (imeVozaca != "" && prezimeVozaca != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.vozac.Ime == imeVozaca && v.vozac.Prezime == prezimeVozaca)
                        voznje.Add(v);
                }
            }

            else if (imeVozaca != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.vozac.Ime == imeVozaca)
                        voznje.Add(v);
                }
            }

            else if (prezimeVozaca != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.vozac.Prezime == prezimeVozaca)
                        voznje.Add(v);
                }
            }

            if (imeMusterije != "" && prezimeMusterije != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.Musterija.Ime == imeMusterije && v.Musterija.Prezime == prezimeMusterije)
                        voznje.Add(v);
                }
            }

            else if (imeMusterije != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.Musterija.Ime == imeMusterije)
                        voznje.Add(v);
                }
            }

            else if (prezimeMusterije != "")
            {
                foreach (Voznja v in DataBase.sveVoznje.Values)
                {
                    if (v.Musterija.Prezime == prezimeMusterije)
                        voznje.Add(v);
                }
            }

            return View("RezultatPretrage", voznje);
        }

        public ActionResult DetaljiVoznje(string korisickoImeVozaca, string datumVoznje)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme)
                {
                    if (k.Uloga == Uloga.VOZAC || k.Uloga == Uloga.DISPECER)
                    {
                        Voznja voznja = new Voznja();
                        Korisnik korisnik = new Korisnik("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");
                        Musterija m = new Musterija("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");
                        Adresa a = new Adresa("-", "-", "-", "-");
                        Lokacija lok = new Lokacija(1, 1, a);

                        voznja = DataBase.sveVoznje[datumVoznje];

                        if (voznja.dispecer == null)
                            voznja.dispecer = new Dispecer("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");

                        if (voznja.Iznos == null)
                            voznja.Iznos = "-";

                        if (voznja.komentar == null)
                            voznja.komentar = new Komentar("-", DateTime.Now, korisnik, voznja, OcenaVoznje.NULA);

                        if (voznja.Musterija == null)
                            voznja.Musterija = new Musterija("-", "-", "-", "-", Pol.MUSKI, "-", "-", "-");

                        if (voznja.Odrediste == null)
                            voznja.Odrediste = lok;

                        AzurirajVoznju(voznja, korisickoImeVozaca);

                        return View("DetaljiVoznje", voznja);
                    }
                }
            }

            ViewBag.Message = "Nije moguce prikazati detalje voznje,jer se desila greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult PocniSaVoznjom(string datumVoznje,string korisnickoImeVozaca)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    Voznja voznja = new Voznja();

                    foreach (Voznja v in DataBase.vozaci[korisnickoImeVozaca].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            DataBase.vozaci[korisnickoImeVozaca].slobodan = false;
                            voznja.Status = StatusVoznje.U_TOKU;
                            voznja = v;

                            AzurirajVoznju(voznja, kor.KorisnickoIme);
                        }
                    }

                    return View("PocniSaVoznjom",voznja);
                }
            }

            ViewBag.Message = "Nemouce je zapoceti voznju,desila se greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult PromeniLokacijuTaksiste(string ulica,string broj,string grad,string postBroj,string korisnickoImeVozaca)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    Adresa adresa = new Adresa(ulica, broj, grad, postBroj);
                    Lokacija l = new Lokacija(2,4,adresa);
                    DataBase.vozaci[korisnickoImeVozaca].Lokacija = l;

                    if (DataBase.slobodniVozaci.ContainsKey(korisnickoImeVozaca))
                        DataBase.slobodniVozaci[korisnickoImeVozaca].Lokacija = l;

                    ViewBag.Message = "Lokacija taksiste je uspesno promenjena.";
                    return View("Izmena");
                }
            }

            ViewBag.Message = "Nije moguce promeniti lokaciju taksiste,jer se dogodila neka greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult PreuzmiVoznju(string voznja,string korisnickoImeVozaca)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    Voznja vo = new Voznja();

                    foreach (Korisnik ko in DataBase.registrovaniKorisnici.Values)
                    {
                        if (ko.Uloga == Uloga.MUSTERIJA)
                        {
                            foreach (Voznja v in ko.voznje)
                            {
                                if (v.DatumIvremePorudz.ToString() == voznja)
                                {
                                    if(v.Status == StatusVoznje.KREIRANA_NA_CEKANJU || v.Status == StatusVoznje.FORMIRANA)
                                    {
                                        v.Status = StatusVoznje.PRIHVACENA;
                                        vo = v;
                                        v.vozac = DataBase.vozaci[korisnickoImeVozaca];
                                        DataBase.voznjeNaCekanju.Remove(v.DatumIvremePorudz.ToString());
                                        DataBase.vozaci[korisnickoImeVozaca].voznje.Add(vo);
                                        AzurirajVoznju(vo, kor.KorisnickoIme);
                                    }
                                }
                            }
                        }
                    }

                    DataBase.slobodniVozaci.Remove(korisnickoImeVozaca);

                    return View("PocniSaVoznjom", vo);
                }
            }

            ViewBag.Message = "Nije moguce preuzeti voznju,dogodila se greska!";
            return View("Greska");           
        }

        [HttpPost]
        public ActionResult UspesnaVoznja(string datumVoznje,string korImeMusterija, string korImeVozac)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    Voznja vo = new Voznja();
                    bool postojiMusterija = false;

                    if (korImeMusterija != "-")
                    {
                        postojiMusterija = true;

                        foreach (Voznja v in DataBase.registrovaniKorisnici[korImeMusterija].voznje)
                        {
                            if (v.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                v.Status = StatusVoznje.USPESNA;
                                vo = v;

                                AzurirajVoznju(vo, kor.KorisnickoIme);
                            }
                        }
                    }

                    foreach(Voznja v in DataBase.vozaci[korImeVozac].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            if (v.dispecer == null)
                            {
                                v.dispecer = new Dispecer("-","-","-","-",Pol.MUSKI,"-","-","-");
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }

                            v.Status = StatusVoznje.USPESNA;

                            if (!postojiMusterija)
                            {
                                vo = v;
                                AzurirajVoznju(vo, kor.KorisnickoIme);
                            }
                        }
                    }

                    return View("UspesnaVoznja", vo);
                }
            }

            ViewBag.Message = "Dogodila se greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult NeuspesnaVoznja(string datumVoznje,string korImeMusterija,string korImeVozac)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    Voznja v = new Voznja();

                    if (korImeMusterija != "-")
                    {
                        foreach (Voznja vo in DataBase.registrovaniKorisnici[korImeMusterija].voznje)
                        {
                            if (vo.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                v = vo;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }
                        }
                    }
                    else
                    {
                        foreach (Voznja voznja in DataBase.sveVoznje.Values)
                        {
                            if (voznja.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                v = voznja;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }
                        }
                    }

                    return View("KomentarVozacaNeuspesnaVoznja", v);
                }
            }

            ViewBag.Message = "Nije moguce ostaviti komentar,desila se greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult ZavrsiVoznju(string ulica,string broj,string grad,string postBroj,string iznos,string datumVoznje,string usernameMusterija,string usernameVozac)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    if (usernameMusterija != "-")
                    {
                        foreach (Voznja v in DataBase.registrovaniKorisnici[usernameMusterija].voznje)
                        {
                            if (v.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                Adresa a = new Adresa(ulica,broj,grad,postBroj);
                                Lokacija lok = new Lokacija(2,4,a);
                                v.Odrediste = lok;
                                v.Iznos = iznos;

                                if (v.dispecer.KorisnickoIme == "-")
                                    DataBase.voznjeNepoznatihDispecera.Add(v.DatumIvremePorudz.ToString(), v);

                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }
                        }
                    }

                    foreach (Voznja v in DataBase.vozaci[usernameVozac].voznje)
                    {
                        if (v.DatumIvremePorudz.ToString() == datumVoznje)
                        {
                            Adresa a = new Adresa(ulica,broj,grad,postBroj);
                            Lokacija lok = new Lokacija(2, 4, a);
                            v.Odrediste = lok;
                            v.Iznos = iznos;
                            AzurirajVoznju(v, kor.KorisnickoIme);
                        }
                    }

                    DataBase.UpisiVoznje();

                    ViewBag.Message = "Voznja je uspesno izvrsena.";
                    return View("Izmena");
                }
            }

            ViewBag.Message = "Voznja nije zavrsena,desila se greska!";
            return View("Greska");
        }

        [HttpPost]
        public ActionResult KomentarVozac(string comment, string datumVoznje, string usernameMusterija)
        {
            Korisnik kor = (Korisnik)Session["korisnik"];

            if (kor == null)
            {
                kor = new Korisnik();
                Session["korisnik"] = kor;
            }

            foreach (Korisnik k in DataBase.registrovaniKorisnici.Values)
            {
                if (k.KorisnickoIme == kor.KorisnickoIme && k.Uloga == Uloga.VOZAC)
                {
                    if (usernameMusterija != "-")
                    {
                        foreach (Voznja v in DataBase.registrovaniKorisnici[usernameMusterija].voznje)
                        {
                            if (v.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                v.komentar = new Komentar(comment, DateTime.Now, v.Musterija, v, OcenaVoznje.JEDAN);
                                v.Status = StatusVoznje.NEUSPESNA;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                            }
                        }
                    }
                    else
                    {
                        foreach (Voznja v in DataBase.neuspesneVoznje.Values)
                        {
                            if (v.DatumIvremePorudz.ToString() == datumVoznje)
                            {
                                Komentar kom = new Komentar(comment, DateTime.Now, v.Musterija, v, OcenaVoznje.JEDAN);
                                v.komentar = kom;
                                v.Status = StatusVoznje.NEUSPESNA;
                                AzurirajVoznju(v, kor.KorisnickoIme);
                                DataBase.slobodniVozaci.Add(v.vozac.KorisnickoIme, v.vozac);
                            }
                        }
                    }

                    DataBase.UpisiVoznje();

                    ViewBag.Message = "Voznja je neuspesno zavrsena!";
                    return View("Greska");
                }
            }

            ViewBag.Message = "Nije moguce postaviti komentar,desila se neka greska!";
            return View("Greska");
        }

        [HttpPost]
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

        public ActionResult LogOut()
        {
            Session.Abandon();
            Korisnik k = new Korisnik();
            Session["korisnik"] = k;
            return View("Login");
        }
    }
}