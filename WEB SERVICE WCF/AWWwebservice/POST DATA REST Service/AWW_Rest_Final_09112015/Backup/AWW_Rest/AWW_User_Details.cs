using System;
using System.Runtime.Serialization;

namespace RestService
{
    [Serializable, DataContract(Name = "AWW_User_Details")]
    public class AWW_User_Details
    {
        [DataMember(Name = "Aanganwadi_Name")]
        public string Aanganwadi_Name { get;set; }

        [DataMember(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "TotalEnrollments")]
        public string TotalEnrollments { get; set; } 

        [DataMember(Name = "Issues_Feedbacks")]
        public string Issues_Feedbacks { get; set; }

        [DataMember(Name = "EntryDate")]
        public string EntryDate { get; set; }

        [DataMember(Name = "IMEINo")]
        public string IMEINo { get; set; } 
    }


}