using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iep_newer;
using iep_newer.Models;
using Microsoft.AspNet.Identity;

namespace iep_newer.Controllers
{
    public class AuctionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void updateAuctionClosed(ref Auction auction)
        {
            logger.Debug("Auction updateAuctionClosed for " + auction.name);
            if (auction.closed < DateTime.Now)
            {
                if (auction.Bids.Count > 0)
                {
                    auction.state = Auction.State.SOLD;
                }
                else
                {
                    auction.state = Auction.State.EXPIRED;
                }
            }
        }

        // GET: Auction/Open
        public HttpStatusCodeResult Open(int id)
        {
            logger.Debug("Auction Open " + id);
            Auction auction = db.Auctions.Find(id);

            auction.opened = DateTime.Now;
            auction.state = Auction.State.OPEN;
            auction.closed = DateTime.Now.AddSeconds(auction.duration);

            db.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        // GET: Auction/Bid

        public DateTime Bid(int id)
        {
            logger.Debug("Auction Bid " + id);
            Bid bid = new Models.Bid();

            var user = db.Users.Find(User.Identity.GetUserId());

            if (user.Tokens == 0)
            {
                //return new HttpStatusCodeResult(400);
                return DateTime.MinValue;
            }

            Auction auction = db.Auctions.Find(id);

            updateAuctionClosed(ref auction);

            if (auction.state != Auction.State.OPEN)
            {
                //return new HttpStatusCodeResult(400);
                return DateTime.MinValue;
            }

            if (auction.closed - DateTime.Now < TimeSpan.FromSeconds(10))
            {
                auction.closed = DateTime.Now.AddSeconds(10);
            }

            bid.auction = auction;

            //auction.Bids.Add(bid);
            auction.last_bid_user = user;

            bid.user = user;

            bid.created = DateTime.Now;

            bid.auction.current_price += 1;
            bid.user.Tokens -= 1;

            db.Bids.Add(bid);
            db.SaveChanges();

            //return new HttpStatusCodeResult(200);
            return bid.created;
        }

        // GET: Auction/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            logger.Debug("Auction Create Get ");
            return View();
        }

        // POST: Auction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "name,duration,starting_price,image_file")] Auction auction)
        {
            logger.Debug("Auction Create Post ");
            auction.created = DateTime.Now;
            logger.Debug("Auction Create Post created " + auction.created);
            auction.current_price = auction.starting_price;
            logger.Debug("Auction Create Post starting_price " + auction.starting_price);
            auction.image = new byte[auction.image_file.ContentLength];
            auction.image_file.InputStream.Read(auction.image, 0, auction.image.Length);
            auction.state = Auction.State.READY;

            if (ModelState.IsValid)
            {
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(auction);
        }

        // GET: Auction/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            logger.Debug("Auction Edit Get ");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "id,name,duration,starting_price,image_file")] Auction auction)
        {
            Auction auctionDB = db.Auctions.Find(auction.id);

            auctionDB.name = auction.name;
            auctionDB.duration = auction.duration;
            logger.Debug("Auction Edit Post ");
            auctionDB.created = DateTime.Now;
            logger.Debug("Auction Edit Post created " + auction.created);
            auctionDB.current_price = auctionDB.starting_price = auction.starting_price;
            logger.Debug("Auction Edit Post starting_price " + auction.starting_price);
            auctionDB.image_file = auction.image_file;
            auctionDB.image = new byte[auctionDB.image_file.ContentLength];
            auctionDB.image_file.InputStream.Read(auctionDB.image, 0, auctionDB.image.Length);
            
            if (ModelState.IsValid)
            {
                db.Entry(auctionDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(auction);
        }

        // GET: Auction/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            db.Auctions.Remove(auction);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // GET: Auction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            ViewBag.closed = auction.closed;

            return View(auction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
