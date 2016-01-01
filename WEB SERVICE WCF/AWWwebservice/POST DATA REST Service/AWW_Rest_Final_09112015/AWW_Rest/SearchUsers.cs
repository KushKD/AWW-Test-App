using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AWW_Rest
{
     [Serializable, DataContract(Name = "SearchUsers")]
    public class SearchUsers
     {
        [DataMember(Name = "District")]
        public string District { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "F_H_Name")]
        public string F_H_Name { get; set; }

        [DataMember(Name = "DOB")]
        public string DOB { get; set; }

        [DataMember(Name = "Pincode")]
        public string Pincode { get; set; }
    
}
    }

