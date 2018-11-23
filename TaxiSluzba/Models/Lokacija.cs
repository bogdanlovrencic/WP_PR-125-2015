using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public class Lokacija
    {
        public double xKordinata { get; set; }
        public double yKordinata { get; set; }
        public Adresa adresa { get; set; }

      
        public Lokacija(double xKordinata, double yKordinata, Adresa adresa)
        {
            this.xKordinata = xKordinata;
            this.yKordinata = yKordinata;
            this.adresa = adresa;
        }

        public double XKordinata
        {
            get { return xKordinata; }
            set { xKordinata = value; }
        }
        public double YKordinata
        {
            get { return yKordinata; }
            set { yKordinata = value; }
        }
        public Adresa Adress
        {
            get { return adresa; }
            set { adresa = value; }
        }
    }

    
}