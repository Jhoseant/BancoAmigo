using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Banco_Amigo.Models;

namespace Banco_Amigo.Controllers
{
    public class PersonaController : Controller
    {
        private BancoAmigoEntities db = new BancoAmigoEntities();

        // GET: ba_persona
        public ActionResult Index()
        {
            return View(db.ba_persona.ToList());
        }

        // GET: ba_persona/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_persona ba_persona = db.ba_persona.Find(id);
            if (ba_persona == null)
            {
                return HttpNotFound();
            }
            return View(ba_persona);
        }

        // GET: ba_persona/Create
        public ActionResult Create()
        {
            ViewBag.pregunta1 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta2 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta3 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta4 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta5 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            return View();
        }

        // POST: ba_persona/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pe_idpersona,pe_cedula,pe_nombre,pe_apellido,pe_fecha_nacimiento,pe_direccion,pe_sexo,pe_correo,pe_estado")] ba_persona Persona, 
            string txt_usuario, string txt_clave, string txt_ConfirmarClave, 
            string pregunta1, string txt_respuesta1, string pregunta2, string txt_respuesta2, string pregunta3, string txt_respuesta3, string pregunta4, string txt_respuesta4, string pregunta5, string txt_respuesta5)
        {
            if (ModelState.IsValid)
            {
                //valida que la contraseña sea igual
                if (txt_clave != txt_ConfirmarClave) {
                    ViewData["Mensaje"] = "Las contraseñas no coinciden";
                    return View();
                }

                //Llama Sp de Validación de registro de usuario sp_ValidaRegistroUsuario.
                string cod = "";
                string mensaje = "";

                var codParameter = cod != null ?
                    new ObjectParameter("cod", cod) :
                    new ObjectParameter("cod", typeof(string));

                var mensajeParameter = mensaje != null ?
                    new ObjectParameter("mensaje", mensaje) :
                    new ObjectParameter("mensaje", typeof(string));

                int exec = db.sp_ValidaRegistroUsuario(Persona.pe_correo, txt_clave, codParameter, mensajeParameter);

                if (codParameter.Value.ToString() != "00")
                {
                    ViewData["Mensaje"] = mensajeParameter.Value.ToString();
                    return View();
                }

                //calcula el nuevo ID de persona
                int new_IDPersona = db.ba_persona.Count();
                new_IDPersona++;
                Persona.pe_idpersona = new_IDPersona;

                //Agrega el genero
                if (Persona.pe_sexo == "Masculino")
                    Persona.pe_sexo = "M";
                else if (Persona.pe_sexo == "Femenino")
                    Persona.pe_sexo = "F";
                else
                    Persona.pe_sexo = "O";

                //Estado por default
                Persona.pe_estado = "A";

                // calcula el nuevo ID de persona
                ba_usuarios Usuarios = new ba_usuarios();
                int new_IDUsuario = db.ba_usuarios.Count();
                new_IDUsuario++;
                Usuarios.us_idusuario = new_IDUsuario;

                //setea datos de usuario
                Usuarios.us_idpersona = new_IDPersona;
                Usuarios.us_idrol = 2;
                Usuarios.us_usuario = txt_usuario;
                Usuarios.us_clave = txt_clave;
                Usuarios.us_fecha_registro = DateTime.Now;
                Usuarios.us_estado = "A";
                
                //agrega y guarda en BD
                db.ba_persona.Add(Persona);
                db.ba_usuarios.Add(Usuarios);
                db.SaveChanges();

                //inserta preguntas y respuestas
                ba_respuestausuario RespuestasUsuarios = new ba_respuestausuario();

                List<ba_respuestausuario> respuestas = new List<ba_respuestausuario>();
                respuestas.Add(new ba_respuestausuario { ru_idpregunta = Int32.Parse(pregunta1), ru_respuesta = txt_respuesta1 });
                respuestas.Add(new ba_respuestausuario { ru_idpregunta = Int32.Parse(pregunta2), ru_respuesta = txt_respuesta2 });
                respuestas.Add(new ba_respuestausuario { ru_idpregunta = Int32.Parse(pregunta3), ru_respuesta = txt_respuesta3 });
                respuestas.Add(new ba_respuestausuario { ru_idpregunta = Int32.Parse(pregunta4), ru_respuesta = txt_respuesta4 });
                respuestas.Add(new ba_respuestausuario { ru_idpregunta = Int32.Parse(pregunta5), ru_respuesta = txt_respuesta5 });

                foreach (ba_respuestausuario i in respuestas)
                {
                    int new_IDRespuesta = db.ba_respuestausuario.Count();
                    new_IDRespuesta++;

                    i.ru_idrespuesta = new_IDRespuesta;
                    i.ru_idusuario = new_IDUsuario;
                    db.ba_respuestausuario.Add(i);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(Persona);
        }

        // GET: ba_persona/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_persona ba_persona = db.ba_persona.Find(id);
            if (ba_persona == null)
            {
                return HttpNotFound();
            }
            return View(ba_persona);
        }

        // POST: ba_persona/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pe_idpersona,pe_cedula,pe_nombre,pe_apellido,pe_fecha_nacimiento,pe_direccion,pe_sexo,pe_correo,pe_estado")] ba_persona ba_persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ba_persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ba_persona);
        }

        // GET: ba_persona/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ba_persona ba_persona = db.ba_persona.Find(id);
            if (ba_persona == null)
            {
                return HttpNotFound();
            }
            return View(ba_persona);
        }

        // POST: ba_persona/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ba_persona ba_persona = db.ba_persona.Find(id);
            db.ba_persona.Remove(ba_persona);
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
