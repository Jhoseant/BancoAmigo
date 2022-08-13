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
    public class UsuariosController : Controller
    {
        private BancoAmigoEntities db = new BancoAmigoEntities();

        // GET: Usuarios
        public ActionResult Index()
        {
            var ba_usuarios = db.ba_usuarios.Include(b => b.ba_persona).Include(b => b.ba_roles);
            return View(ba_usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_usuarios ba_usuarios = db.ba_usuarios.Find(id);
            if (ba_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(ba_usuarios);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.us_idpersona = new SelectList(db.ba_persona, "pe_idpersona", "pe_cedula");
            ViewBag.us_idrol = new SelectList(db.ba_roles, "ro_idrol", "ro_rol");
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios)
        {
            if (ModelState.IsValid)
            {
                ba_persona ba_persona = db.ba_persona.Find(ba_usuarios.us_idpersona);
                ba_roles ba_roles = db.ba_roles.Find(ba_usuarios.us_idrol);

                ba_usuarios.ba_persona = ba_persona;
                ba_usuarios.ba_roles = ba_roles;

                db.ba_usuarios.Add(ba_usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.us_idpersona = new SelectList(db.ba_persona, "pe_idpersona", "pe_cedula", ba_usuarios.us_idpersona);
            ViewBag.us_idrol = new SelectList(db.ba_roles, "ro_idrol", "ro_rol", ba_usuarios.us_idrol);
            return View(ba_usuarios);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_usuarios ba_usuarios = db.ba_usuarios.Find(id);
            if (ba_usuarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.us_idpersona = new SelectList(db.ba_persona, "pe_idpersona", "pe_cedula", ba_usuarios.us_idpersona);
            ViewBag.us_idrol = new SelectList(db.ba_roles, "ro_idrol", "ro_rol", ba_usuarios.us_idrol);
            return View(ba_usuarios);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ba_usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.us_idpersona = new SelectList(db.ba_persona, "pe_idpersona", "pe_cedula", ba_usuarios.us_idpersona);
            ViewBag.us_idrol = new SelectList(db.ba_roles, "ro_idrol", "ro_rol", ba_usuarios.us_idrol);
            return View(ba_usuarios);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_usuarios ba_usuarios = db.ba_usuarios.Find(id);
            if (ba_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(ba_usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ba_usuarios ba_usuarios = db.ba_usuarios.Find(id);
            db.ba_usuarios.Remove(ba_usuarios);
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
