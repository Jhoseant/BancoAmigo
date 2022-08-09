using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Banco_Amigo.Models;

namespace Banco_Amigo.Controllers
{
    public class RolesController : Controller
    {
        private BancoAmigoEntities db = new BancoAmigoEntities();

        // GET: Roles
        public ActionResult Index()
        {
            return View(db.ba_roles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_roles ba_roles = db.ba_roles.Find(id);
            if (ba_roles == null)
            {
                return HttpNotFound();
            }
            return View(ba_roles);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ro_idrol,ro_rol,ro_estado")] ba_roles ba_roles)
        {
            if (ModelState.IsValid)
            {
                db.ba_roles.Add(ba_roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ba_roles);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_roles ba_roles = db.ba_roles.Find(id);
            if (ba_roles == null)
            {
                return HttpNotFound();
            }
            return View(ba_roles);
        }

        // POST: Roles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ro_idrol,ro_rol,ro_estado")] ba_roles ba_roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ba_roles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ba_roles);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_roles ba_roles = db.ba_roles.Find(id);
            if (ba_roles == null)
            {
                return HttpNotFound();
            }
            return View(ba_roles);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ba_roles ba_roles = db.ba_roles.Find(id);
            db.ba_roles.Remove(ba_roles);
            db.SaveChanges();
            return RedirectToAction("Index");
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
