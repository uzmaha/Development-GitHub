using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace P_CMS.Controllers
{
    [RoutePrefix("menu")]
    [Route("{action=Index}")]
    public class RoutingTestController : Controller
    {
        // GET: RoutingTest
        public ActionResult Index()
        {
            return View();
        }
        //menu/2
       [Route("menu/{id:int:min(100)}")]
        public ActionResult IndexById(int id)
        {
            ViewBag.PassedId = id;
            return View();
        }
       [Route("menu/{id:alpha}")]
       public ActionResult RouteByLetters(string id)
       {
           ViewBag.PassedId = id;
           return View();
       }

       [Route("menu/{id:bool}")]
       public ActionResult RouteByBit(bool id)
       {
           ViewBag.PassedId = id;
           return View();
       }
    }
}