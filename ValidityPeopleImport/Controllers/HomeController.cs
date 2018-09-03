using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Validity.PeopleImport.Model;
using Validity.PeopleImport.Processes;

namespace ValidityPeopleImport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase uploadFile)
        {

            if (uploadFile.ContentLength >0)
            {
                var fileName = Path.GetFileName(uploadFile.FileName);
                var path = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
                uploadFile.SaveAs(path);
                ViewBag.Message  = "File has been uploaded successfully";

                DedupDetector dedupDetector = new DedupDetector();
                dedupDetector.FindDuplicateAndNonDuplicate(path);
                List<Contact> duplicateContactList = JsonConvert.DeserializeObject<List<Contact>>(dedupDetector.DuplicateJson);
                List<Contact> nonDuplicateContactList = JsonConvert.DeserializeObject<List<Contact>>(dedupDetector.NonDuplicateJson);

                ViewBag.DuplicateTable = ConvertToHtmlView(duplicateContactList,"Potiential Duplicates");
                ViewBag.NonduplicateTable = ConvertToHtmlView(nonDuplicateContactList, "None Duplicates");

                ModelState.Clear();
            }
            else
            {
                ViewBag.Message = "File upload failed";
            }

            return View("Index");
        }

        private string ConvertToHtmlView(List<Contact> listOfContacts, string text)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<html>");
            htmlBuilder.Append("<head>");
            htmlBuilder.Append("<title>");
            htmlBuilder.Append("Page-");
            htmlBuilder.Append(Guid.NewGuid().ToString());
            htmlBuilder.Append("</title>");
            htmlBuilder.Append("</head>");
            htmlBuilder.Append("<body>");
            htmlBuilder.Append("</br>");
            htmlBuilder.Append(text);
            htmlBuilder.Append("</br>");
            htmlBuilder.Append("<ul>");
            foreach (Contact c in listOfContacts)
            {
                htmlBuilder.Append("<li>");
                htmlBuilder.Append(c.First_Name);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Last_Name);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Company);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Email);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Address1);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Address2);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.City);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.State_Long);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.State);
                htmlBuilder.Append(", ");
                htmlBuilder.Append(c.Phone);
                htmlBuilder.Append("</li>");
            }
            htmlBuilder.Append("</br>");
            htmlBuilder.Append("</body>");
            htmlBuilder.Append("</html>");

            return htmlBuilder.ToString();
            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}