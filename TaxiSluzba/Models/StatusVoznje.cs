using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiSluzba.Models
{
    public enum StatusVoznje
    {
        KREIRANA_NA_CEKANJU,
        OTKAZANA,
        FORMIRANA,
        OBRADJENA,
        U_TOKU,
        PRIHVACENA,
        NEUSPESNA,
        USPESNA
    }
}