using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AWW_Rest
{
    [Serializable, DataContract(Name = "MobileAadhaarSeedSave")]
    public class MobileAadhaarSeedSave
    {
        [DataMember(Name = "AadhaarNo")]
        public string AadhaarNo { get; set; }

        [DataMember(Name = "MobileNo")]
        public string MobileNo { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "Occupation")]
        public string Occupation { get; set; }

        [DataMember(Name = "SendingMobileNo")]
        public string SendingMobileNo { get; set; }
    }
}