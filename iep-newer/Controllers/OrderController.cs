using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iep_newer.Models;
using Microsoft.AspNet.Identity;

namespace iep_newer.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Order/New
        
        public int New()
        {
            logger.Debug("Order New");

            var order = new Order();

            var user = db.Users.Find(User.Identity.GetUserId());

            order.user = user;

            order.state = Order.OrderState.WAIT;

            order.created = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return order.id;
        }

        //public int NewFejk()
        //{
        //    var order = new Order();

        //    var user = db.Users.First();

        //    order.user = user;

        //    order.state = Order.OrderState.FAIL;

        //    order.created = DateTime.Now;

        //    db.Orders.Add(order);
        //    db.SaveChanges();

        //    return order.id;
        //}

        // GET: Order
        public ActionResult Index(string msg)
        {
            logger.Debug("Order Index msg " + msg + " userid " + User.Identity.GetUserId());
            var thisUser = User.Identity.GetUserId();
            var orders = db.Orders.Where(o => o.user_id == thisUser).Include(o => o.user);

            ViewBag.msg = msg;
            return View(orders.ToList());
        }

        //// GET: Order/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(order);
        //}

        // GET: Order/Finish

        //// GET: Order/Create
        //public ActionResult Create()
        //{
        //    //var user = db.Users.Find(User.Identity.GetUserId());
        //    //ViewBag.user_id = new SelectList(db.Users, "Id", "Name");
        //    return View();
        //}

        //// POST: Order/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,user_id,created,tokens,price")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Orders.Add(order);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    //ViewBag.user_id = new SelectList(db.Users, "Id", "Name", order.user_id);
        //    return View(order);
        //}

        //// GET: Order/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //ViewBag.user_id = new SelectList(db.Users, "Id", "Name", order.user_id);
        //    return View(order);
        //}

        //// POST: Order/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,user_id,created,tokens,price")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(order).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.user_id = new SelectList(db.Users, "Id", "Name", order.user_id);
        //    return View(order);
        //}

        //// GET: Order/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(order);
        //}

        //// POST: Order/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Order order = db.Orders.Find(id);
        //    db.Orders.Remove(order);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public HttpStatusCodeResult Result(int clientId, string status, int amount, double endUserPrice)
        {
            logger.Debug("Order Result clientId " + clientId + " status " + status + " amount " + amount + " endUserPrice " + endUserPrice);
            //Order order = db.Orders.Find(IDOrder);
            
            Order order = db.Orders.Find(clientId);

            order.tokens = amount;
            order.price = (float)endUserPrice;
            order.state = (status == "success") ? Order.OrderState.PASS : Order.OrderState.FAIL;
            order.user = order.user;
            
            if (order.state == Order.OrderState.PASS)
            {
                order.user.Tokens += amount;
            }

            db.SaveChanges();

            return new HttpStatusCodeResult(200);
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
