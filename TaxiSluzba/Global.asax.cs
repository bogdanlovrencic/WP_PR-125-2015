using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaxiSluzba.Models;

namespace TaxiSluzba
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ovde ucitati iz fajla dispecere 
            string tmp = "";
            string[] line;
            Pol pol;

            string path = "~/App_Data/Dispeceri.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while ((tmp = sr.ReadLine()) != null)
                {
                    line = tmp.Split('_');
                    if (line[4] == "MUSKI")
                        pol = Pol.MUSKI;
                    else
                        pol = Pol.ZENSKI;

                    Dispecer dispecer = new Dispecer(line[0], line[1], line[2], line[3], pol, line[5], line[6], line[7]);
                    DataBase.registrovaniKorisnici.Add(line[0], dispecer);
                }

                sr.Close();

                UcitajBazu();
            }

        }

        public void UcitajBazu()
        {
            string temp = "";
            string[] read;

            string path = "~/App_Data/RegistrovaniKorisnici.txt";
            path = HostingEnvironment.MapPath(path);

            if (File.Exists(path))
            {
                StreamReader srUser = new StreamReader(path);
                while ((temp = srUser.ReadLine()) != "")
                {
                    read = temp.Split('_');
                    Pol pol;

                    if (read[5] == "MUSKI")
                        pol = Pol.MUSKI;
                    else
                        pol = Pol.ZENSKI;

                    Uloga uloga;

                    if (read[8] == "MUSTERIJA")
                        uloga = Uloga.MUSTERIJA;
                    else if (read[8] == "VOZAC")
                        uloga = Uloga.VOZAC;
                    else
                        uloga = Uloga.DISPECER;

                    Korisnik k = new Korisnik(read[0], read[1], read[2], read[3], pol, read[4], read[6], read[7]);
                    k.Uloga = uloga;

                    if (!DataBase.registrovaniKorisnici.ContainsKey(read[2]))
                    {
                        DataBase.registrovaniKorisnici.Add(k.KorisnickoIme, k);
                    }
                }

                srUser.Close();
            }

        }
    }
}
