using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iep_newer.Models;
using Microsoft.AspNet.Identity;

namespace iep_newer.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        const int AUCTIONS_PER_PAGE = 12;


        public ActionResult Index(string search_terms, int? min_price, int? max_price, Auction.State? search_state, bool? search_self, int? page)
        {
            logger.Debug("Auction index");     

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = db.Users.Find(User.Identity.GetUserId()).Name;
                logger.Debug("Auction index username " + ViewBag.UserName);
            }

            IEnumerable<Auction> auctions = db.Auctions;

            foreach (Auction auction in auctions)
            {
                logger.Debug("Auction updating close status for auction " + auction.name);
                if (auction.state != Auction.State.OPEN)
                    continue;

                if (auction.opened.Value.AddSeconds(auction.duration) <= DateTime.Now)
                {
                    if (auction.Bids.Count > 0)
                        auction.state = Auction.State.SOLD;
                    else
                        auction.state = Auction.State.EXPIRED;

                    auction.closed = DateTime.Now;
                }
            }

            if (search_terms != null)
            {
                logger.Debug("Auction index search terms " + search_terms);

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

            if (search_self != null && search_self == true)
            {
                auctions = auctions.Where(
                    auction => auction.state == Auction.State.SOLD
                    );
                auctions = auctions.Where(
                    auction => auction.last_bid_user_id == User.Identity.GetUserId()
                    );
            }

            else if (search_state != null && search_state != Auction.State.ALL)
            {
                logger.Debug("Auction index search state " + search_state);
                auctions = auctions.Where(
                    auction => auction.state == search_state
                    );
            }

            if (min_price != null)
            {
                logger.Debug("Auction index min_price " + min_price);
                auctions = auctions.Where(
                    auction => auction.current_price >= min_price
                    );
            }

            if (max_price != null)
            {
                logger.Debug("Auction index max_price " + max_price);
                auctions = auctions.Where(
                    auction => auction.current_price <= max_price
                    );
            }

            auctions = auctions.OrderByDescending(auction => auction.created);

            ViewBag.search_terms = search_terms;
            ViewBag.min_price = min_price;
            ViewBag.max_price = max_price;
            ViewBag.search_state = search_state;
            ViewBag.search_self = search_self;
            ViewBag.pages = Math.Ceiling((double)auctions.Count() / AUCTIONS_PER_PAGE);

            logger.Debug("Auction index page " + page);

            auctions = auctions.Skip(AUCTIONS_PER_PAGE * (page ?? default(int))).Take(AUCTIONS_PER_PAGE);
            
            db.SaveChanges();

            return View(auctions);
        }
        
    }
}