using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;
using System.Web;
using Notification.Processor.Models;
using System.Configuration;

namespace Notification.Processor
{
    public class NotificationTemplate
    {
        public static string EmailTemplate(NotificationModel notificationmodel)
        {
            var templateService = new TemplateService();
            string file  = System.IO.Directory.GetParent(@"../../").FullName + "\\Templates\\EmailTemplate.cshtml";
            return templateService.Parse(File.ReadAllText(file), notificationmodel, null, null);
        }
    }
}
