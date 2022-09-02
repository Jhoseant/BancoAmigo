﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios)
        {
            if (ModelState.IsValid)
            {
                //var_tipo_modelo nom_var inicializar_la_variable_tipo_modelo
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
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios,
                                string txt_nuevaclave, string txt_confirmarclave )
        {
            if (ModelState.IsValid)
            {
                if (txt_nuevaclave == txt_confirmarclave)
                {
                    int updateclave = db.Database.ExecuteSqlCommand("update ba_usuarios set us_clave = @p0 where us_idusuario = @p1", txt_nuevaclave, ba_usuarios.us_idusuario);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Mensaje"] = "La nueva clave no coincide.";
                }                
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

        //GET
        public ActionResult Login()
        {
            ba_usuarios ba_usuarios = new ba_usuarios();
            return View(ba_usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios)
        {
            if (ModelState.IsValid)
            {
                //var Result = db.ba_respuestausuario.Where(x => x.ru_idpregunta == r.ru_idpregunta);
                var usuario = db.ba_usuarios.Where(x => x.us_usuario == ba_usuarios.us_usuario)
                                            .Where(x => x.us_clave == ba_usuarios.us_clave).ToList();

                //var preguntas = db.Database.SqlQuery<ba_preguntas>("select top 3 * from ba_preguntas where pr_idpregunta in (select ru_idpregunta from ba_respuestausuario where ru_idusuario = @p0)and pr_estado = 'A' order by NEWID()", id,).ToList();

                if (usuario.Count>0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Mensaje"] = "Usuario y/o clave incorrectos.";
                }
            }
            return View(ba_usuarios);
        }
    }
}
