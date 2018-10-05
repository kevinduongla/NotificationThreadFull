using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Processor.Models
{
    public class NotificationModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public double? CurrentExpense { get; set; }

    }
}
