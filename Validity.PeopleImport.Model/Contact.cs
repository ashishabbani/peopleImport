using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validity.PeopleImport.Model
{
    public class Contact
    {
        public string First_Name { get; set; }
        public string ID { get; set; }
        public string Last_Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State_Long { get; set; }
        public string State { get; set; }
    }
}
