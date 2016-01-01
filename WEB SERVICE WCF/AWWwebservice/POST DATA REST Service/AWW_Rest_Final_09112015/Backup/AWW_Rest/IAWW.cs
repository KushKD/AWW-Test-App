using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using RestService;

namespace AWW_Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAWW" in both code and config file together.
    [ServiceContract]
    public interface IAWW
    {
        //Search Data
        //[OperationContract]
        //Boolean CheckLoging(String UserId, String Pass, String IMEI);
        ////Savd data        
        //[OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "saveDetails")]
        //string Save_AWW_Details(AWW_User_Details aww_user_details);


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "login/{Aadhaar}")]
        string Login(string Aadhaar);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "otp/{useraadhaar}/{OTP}")]
        //string CheckOTP(string useraadhaar, string OTP);


        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "signin/{username}/{password}/{IMEI}")]
        //Boolean CheckUser(string username, string password, string IMEI);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "signout/{username}")]
        //Boolean LogOutUser(string username);

        


    }
}
