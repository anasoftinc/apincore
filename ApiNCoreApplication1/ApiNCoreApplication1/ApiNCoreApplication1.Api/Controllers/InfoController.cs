using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


namespace ApiNCoreApplication1.Api.Controllers
{
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {

        public IConfiguration Configuration { get; }
        public InfoController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //get info
        [AllowAnonymous]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult ApiInfo()
        {

            var migration = Configuration["ConnectionStrings:UseMigrationService"];
            var seed = Configuration["ConnectionStrings:UseSeedService"];
            var memorydb = Configuration["ConnectionStrings:UseInMemoryDatabase"];
            var connstring = Configuration["ConnectionStrings:ApiNCoreApplication1DB"];

            var controlers = MvcHelper.GetControllerMethodsNames();
            return Content("<html><head><link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css' integrity='sha384-PsH8R72JQ3SOdhVi3uxftmaW6Vc51MKb0q5P2rRUpPvrszuE4W1povHYgTpBfshb' crossorigin='anonymous'><link rel='stylesheet' href='https://use.fontawesome.com/releases/v5.3.1/css/all.css' integrity='sha384-mzrmE5qonljUremFsqc01SB46JvROS7bZs3IO2EmfFsd15uHvIt+Y8vEf7N7fWAU' crossorigin='anonymous'></head><body>" +

                "<div class='jumbotron'>" +
                "<h1><i class='fab fa-centercode' fa-2x></i>  ApiNCoreApplication1 Api</h1>" +
                "<h4>v.1.5</h4>" +
                 "NET.Core Api REST service started!<br>" +
                 "appsettings.json configuration:<br>" +
                 "<ul><li>UseMigrationService:" + migration + "</li>" +
                 "<li>UseSeedService:" + seed + "</li>" +
                 "<li>UseInMemoryDatabase:" + memorydb + "</li>" +
                 "<li>ConnectionStrings:" + connstring + "</li></ul>" +
                "</div>" +

                "<div class='row'>" +

                "<div class='col-md-3'>" +
                "<h3>API controlers and methods</h3>" +
                "<ul>" + controlers + "</ul>" +
                "<p></p>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API services and patterns</h3>" +
                "<p><ul><li>Dependency Injection (Net Core feature) </li><li>Repository and Unit of Work Patterns</li><li>Generic services</li><li>Automapper</li><li>Sync and Async calls</li><li>Generic exception handler</li><li>Serilog logging with Console and File sinks</li><li>Seed from json objects</li><li>JWT authorization and authentication</li></ul>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API projects</h3>" +
                "<ul><li>Api</li><li>Domain</li><li>Entity</li></ul>" +
                "</div>" +

                "</div>" +
                "</body></html>"
               , "text/html");

        }

    }

    public class MvcHelper
    {
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        public static string GetControllerMethodsNames()
        {
            List<Type> cmdtypes = GetSubClasses<ControllerBase>();
            var controlersInfo = "";
            foreach (Type ctrl in cmdtypes)
            {
                var methodsInfo = "";
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                MemberInfo[] methodName = ctrl.GetMethods(flags);
                foreach (MemberInfo method in methodName)
                {
                    if (method.DeclaringType.ToString() == ctrl.UnderlyingSystemType.ToString())
                        methodsInfo += "<li><i>" + method.Name.ToString() + "</i></li>";
                }
                controlersInfo += "<li>" + ctrl.Name.Replace("Controller", "") + "<ul>" + methodsInfo + "</ul></li>";
            }
            return controlersInfo;
        }
    }

}
