using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Exporter.Models
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext() : base("Server")
        {
        }
    }
}