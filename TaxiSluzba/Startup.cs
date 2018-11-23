using Microsoft.Owin;
using Owin;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

[assembly: OwinStartupAttribute(typeof(TaxiSluzba.Startup))]
namespace TaxiSluzba
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }

     

       
    }
}
