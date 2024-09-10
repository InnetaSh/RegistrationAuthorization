using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationAuthorization
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Phone { get; set; }


        public int AccountId { get; set; }
        //public Account Account { get; set; }
    }
}
