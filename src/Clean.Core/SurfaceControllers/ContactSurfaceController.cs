using Clean.Core.ViewModels;
using System.Net.Mail;
using System.Reflection;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Clean.Core.SurfaceControllers
{
    public class ContactController : SurfaceController
    {
        [HttpGet]
        public ActionResult RenderForm()
        {
            ContactViewModel model = new ContactViewModel();
            return PartialView("/Views/Partials/Contact/contactForm.cshtml", model);
        }

        [HttpPost]
        public ActionResult RenderForm(ContactViewModel model)
        {
            return PartialView("/Views/Partials/Contact/contactForm.cshtml", model);
        }

        [HttpPost]
        public ActionResult SubmitForm(ContactViewModel model)
        {
            bool success = false;
            if (ModelState.IsValid)
            {
                success = SendEmail(model);
            }
            return PartialView($"/Views/Partials/Contact/{(success ? "success" : "error")}.cshtml");
        }

        public bool SendEmail(ContactViewModel model)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();

                string toAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["ContactEmailTo"];
                string fromAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["ContactEmailFrom"];
                message.Subject = string.Format("Enquiry from: {0} - {1}", model.Name, model.Email);
                message.Body = model.Message;
                message.To.Add(new MailAddress(toAddress, toAddress));
                message.From = new MailAddress(fromAddress, fromAddress);

                client.Send(message);
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod().DeclaringType, "Contact Form Error", ex);
                return false;
            }
        }
    }
}
