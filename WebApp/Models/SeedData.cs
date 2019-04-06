using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WebAppContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<WebAppContext>>()))
            {
                // Look for any movies.
                if (context.Entry.Any())
                {
                    return;   // DB has been seeded
                }

                context.Entry.AddRange(
                    new Entry
                    {
                        Journal = "2019-I",
                        Title = "Aujourd'hui, j'ai appris",
                        Pages = "6-7"
                    },
                    new Entry
                    {
                        Journal = "2019-I",
                        Title = "Generateur de rapport",
                        Pages = "20"
                    },
                    new Entry
                    {
                        Journal = "2019-I",
                        Title = "Point validation CMDB",
                        Pages = "52"
                    },
                    new Entry
                    {
                        Journal = "2018",
                        Title = "Aujourd'hui, j'ai appris",
                        Pages = "110-120"
                    },
                    new Entry
                    {
                        Journal = "2018",
                        Title = "Concert Orelsan",
                        Pages = "148"
                    },
                    new Entry
                    {
                        Journal = "2018",
                        Title = "Fête ING",
                        Pages = "112"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
