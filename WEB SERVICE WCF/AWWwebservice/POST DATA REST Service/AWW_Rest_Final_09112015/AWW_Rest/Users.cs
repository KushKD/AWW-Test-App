using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWW_Rest
{
    public class Users
    {
        public String Enroll_ID { get; set; }
        public String Aadhaar { get; set; }
        public String Resident_Name { get; set; }
        public String DOB { get; set; }
        public String Gender { get; set; }
        public String Care_of { get; set; }
        public String Address_Building { get; set; }
        public String Addr_Street { get; set; }
        public String Addr_Landmark { get; set; }
        public String Addr_Locality { get; set; }
        public String Addr_VTC { get; set; }
        public String Addr_District { get; set; }
        public String addr_state_name { get; set; }
        public String addr_pincode { get; set; }
        public String res_gauardian_name { get; set; }
        public String res_addr_subdistrict_name { get; set; }
        public String res_addr_po_name { get; set; }
    }
}