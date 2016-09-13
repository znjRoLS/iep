using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iep_newer.Models;

namespace iep_newer.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        const int AUCTIONS_PER_PAGE = 12;
        public ActionResult Index(string search_terms, int? page)
        {
            IEnumerable<Auction> auctions = db.Auctions;

            if (search_terms != null)
            {
                IEnumerable<string> tokens = search_terms.Split();

                auctions = auctions.Where(auction =>
                {
                    foreach (string token in tokens)
                    {
                        if (auction.name.Contains(token))
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            auctions = auctions.OrderByDescending(auction => auction.created);

            ViewBag.search_terms = search_terms;
            ViewBag.pages = Math.Ceiling((double)auctions.Count() / AUCTIONS_PER_PAGE);

            auctions = auctions.Skip(AUCTIONS_PER_PAGE * (page ?? default(int))).Take(AUCTIONS_PER_PAGE);

            return View(auctions);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}