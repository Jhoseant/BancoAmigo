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
    public class LoginController : Controller
    {
        private BancoAmigoEntities db = new BancoAmigoEntities();

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
                if (Request.Form["ForgetPassword"] != null)
                {
                    //string str = Request.Params["us_usuario"];
                    var usuario = db.ba_usuarios.Where(x => x.us_usuario == ba_usuarios.us_usuario).ToList();

                    if (usuario.Count > 0)
                    {
                        foreach (ba_usuarios i in usuario)
                        {
                            return RedirectToAction("ForgetPassword", "Login", new { id = i.us_idusuario });
                            //var Pregunta = db.ba_preguntas.Where(x => x.pr_idpregunta == i.ru_idpregunta).FirstOrDefault();
                        }
                        return RedirectToAction("ForgetPassword", "Login", new { id = 2 });
                    }
                    else {
                        ViewData["Mensaje"] = "Ingrese un usuario Correcto";
                        return View(ba_usuarios);
                    }
                    
                }
                //var Result = db.ba_respuestausuario.Where(x => x.ru_idpregunta == r.ru_idpregunta);
                var login = db.ba_usuarios.Where(x => x.us_usuario == ba_usuarios.us_usuario)
                                            .Where(x => x.us_clave == ba_usuarios.us_clave).ToList();

                //var preguntas = db.Database.SqlQuery<ba_preguntas>("select top 3 * from ba_preguntas where pr_idpregunta in (select ru_idpregunta from ba_respuestausuario where ru_idusuario = @p0)and pr_estado = 'A' order by NEWID()", id,).ToList();

                if (login.Count > 0)
                {
                    Session["Usuario"] = ba_usuarios;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Mensaje"] = "Usuario y/o clave incorrectos.";
                }
            }
            return View(ba_usuarios);
        }

        // GET: Login/NewUser
        public ActionResult NewUser()
        {
            ViewBag.pregunta1 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta2 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta3 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta4 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            ViewBag.pregunta5 = new SelectList(db.ba_preguntas, "pr_idpregunta", "pr_pregunta");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewUser([Bind(Include = "pe_idpersona,pe_cedula,pe_nombre,pe_apellido,pe_fecha_nacimiento,pe_direccion,pe_sexo,pe_correo,pe_estado")] ba_persona Persona,
            string txt_usuario, string txt_clave, string txt_ConfirmarClave,
            string pregunta1, string pregunta2, string pregunta3, string pregunta4, string pregunta5,
            string txt_respuesta1, string txt_respuesta2, string txt_respuesta3, string txt_respuesta4, string txt_respuesta5)
        {
            if (ModelState.IsValid)
            {
                //valida que la contraseña sea igual
                if (txt_clave != txt_ConfirmarClave)
                {
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
                return RedirectToAction("Index", "Home");
            }
            return View(Persona);
        }


        // GET: Login/ForgetPassword/5
        public ActionResult ForgetPassword(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var respuestas = db.ba_respuestausuario.SqlQuery("select * from ba_respuestausuario where ru_idusuario = 2").ToList();
            var Respuestas = db.ba_respuestausuario.Where(x => x.ru_idusuario == id).ToList();
            if (Respuestas == null)
            {
                return HttpNotFound();
            }
            else {
                foreach (ba_respuestausuario i in Respuestas)
                {
                    var Pregunta = db.ba_preguntas.Where(x => x.pr_idpregunta == i.ru_idpregunta).FirstOrDefault();
                }
            }

            var preguntas = db.Database.SqlQuery<ba_preguntas>("select top 3 * from ba_preguntas where pr_idpregunta in (select ru_idpregunta from ba_respuestausuario where ru_idusuario = @p0)and pr_estado = 'A' order by NEWID()",id).ToList();

            //List<ba_preguntas> listaPreguntas = new List<ba_preguntas>();
            List<ba_respuestausuario> listaRespuestas = new List<ba_respuestausuario>();

            int num_preguntas = 3;
            foreach (ba_preguntas i in preguntas)
            {
                if (num_preguntas == 3) {
                    ViewBag.lbl_pregunta1 = i.pr_pregunta;
                    //listaPreguntas.Add(new ba_preguntas { pr_idpregunta = i.pr_idpregunta, pr_pregunta = i.pr_pregunta });
                    listaRespuestas.Add(new ba_respuestausuario {ru_idpregunta = i.pr_idpregunta, ru_idusuario = id.Value });
                }
                else {
                    if (num_preguntas == 2) {
                        ViewBag.lbl_pregunta2 = i.pr_pregunta;
                        //listaPreguntas.Add(new ba_preguntas { pr_idpregunta = i.pr_idpregunta, pr_pregunta = i.pr_pregunta });
                        listaRespuestas.Add(new ba_respuestausuario { ru_idpregunta = i.pr_idpregunta, ru_idusuario = id.Value });
                    }
                    else {
                        if (num_preguntas == 1)
                        {
                            ViewBag.lbl_pregunta3 = i.pr_pregunta;
                            //listaPreguntas.Add(new ba_preguntas { pr_idpregunta = i.pr_idpregunta, pr_pregunta = i.pr_pregunta });
                            listaRespuestas.Add(new ba_respuestausuario { ru_idpregunta = i.pr_idpregunta, ru_idusuario = id.Value });
                        }
                    }
                }
                num_preguntas--;
            }
            //TempData["listaPreguntas"] = listaPreguntas;
            TempData["listaRespuestas"] = listaRespuestas;
            return View(listaRespuestas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword([Bind(Include = "pr_idpregunta,pr_pregunta,pr_estado")] ba_preguntas ba_preguntas,
            string txt_respuesta1, string txt_respuesta2, string txt_respuesta3)
        {
            if (ModelState.IsValid)
            {

                //var listaPreguntas = TempData["listaPreguntas"] as IEnumerable<ba_preguntas>;
                var listaRespuestas= TempData["listaRespuestas"] as IEnumerable<ba_respuestausuario>;

                int num_preguntas = 3;
                foreach (ba_respuestausuario r in listaRespuestas)
                {
                    if (num_preguntas == 3)
                        r.ru_respuesta = txt_respuesta1;
                    else
                    {
                        if (num_preguntas == 2)
                            r.ru_respuesta = txt_respuesta2;
                        else
                        {
                            if (num_preguntas == 1)
                                r.ru_respuesta = txt_respuesta3;
                        }
                    }
                    num_preguntas--;
                }
                int ErrorRespuesta = 0;
                int idUsuario = 0;
                foreach (ba_respuestausuario r in listaRespuestas)
                {
                    var Result = db.ba_respuestausuario.Where(x => x.ru_idpregunta == r.ru_idpregunta)
                                                        .Where(x => x.ru_respuesta == r.ru_respuesta)
                                                        .Where(x => x.ru_idusuario == r.ru_idusuario).ToList();
                    if (Result == null)
                    {
                        return HttpNotFound();
                    }
                    if (Result.Count() == 0) {
                        ErrorRespuesta++;
                        ViewData["Mensaje"] = "Una pregunta falló";
                        return RedirectToAction("ForgetPassword");
                    }
                    idUsuario = r.ru_idusuario;
                }
                return RedirectToAction("ChangePassword", "Login", new { id = idUsuario });
            }
            return View();
        }

        // GET: Usuarios/Edit/5
        public ActionResult ChangePassword(int? id)
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
        public ActionResult ChangePassword([Bind(Include = "us_idusuario,us_idpersona,us_idrol,us_usuario,us_clave,us_fecha_registro,us_fecha_modificacion,us_estado")] ba_usuarios ba_usuarios,
                                string txt_nuevaclave, string txt_confirmarclave)
        {
            if (ModelState.IsValid)
            {
                if (txt_nuevaclave == txt_confirmarclave)
                {
                    int updateclave = db.Database.ExecuteSqlCommand("update ba_usuarios set us_clave = @p0 where us_idusuario = @p1", txt_nuevaclave, ba_usuarios.us_idusuario);
                    return RedirectToAction("Index", "Home");
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

    }
}
