using System.Runtime.Serialization;

namespace RestService
{
    public class User
    {
        [DataMember(Name = "Aadhaar")]
        public string uAadhaar { get; set; }

        //[DataMember(Name = "Pass")]
        //public string Pass { get; set; }

        //[DataMember(Name = "Device")]
        //public string Device { get; set; }

       
    }
    public class CheckOTP
    {
        [DataMember(Name = "Aadhaar")]
        public string uAadhaar { get; set; }
        [DataMember(Name = "OTP")]
        public string OTP { get; set; }
    }

   
}