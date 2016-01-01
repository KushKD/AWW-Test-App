using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RestService;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;


namespace AWW_Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AWW" in code, svc and config file together.
    public class AWW : IAWW
    {
 
        List<AWW_User_Details> test;
        //public string JSONData(string id)
        //{
        //    return id;
        //}
        public Boolean CheckLoging(String UserId, String Pass, String IMEI)
        {

            ActivityClass actLog = new ActivityClass();
            return actLog.ConfirmLogin(UserId, Pass, IMEI);
        }

        public String GetInfo(String Name, String Father, String DOB, String P)
        {

            ActivityClass actLog = new ActivityClass();
            return actLog.ConfirmLogin(UserId, Pass, IMEI);
        }

        public string Login(string Aadhaar)
        {
            return GetMobileNumber(Aadhaar);
        }

        private string GeneratOTP()
        {

            
            var chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 6)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result.ToString();


        }

        private string GetMobileNumber(string Aadhaar)
        {
           
            SqlDataAdapter adp = new SqlDataAdapter("Select top 1 Mobile_Number from AWW_AWWMaster where REPLACE(AadhaarNo,'-','')='" + Aadhaar + "'", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string OTP = GeneratOTP();
                    if (SaveData(OTP, dt.Rows[0]["Mobile_Number"].ToString()) == true)
                    {
                        OTPBySMS(dt.Rows[0]["Mobile_Number"].ToString(), OTP);
                        return "OTP has been sent on registered mobile number";
                    }
                    else
                    {
                        return "Unable to generate OTP Please try again after some time";
                    }


                }
                else
                {
                    return "Aadhaar Number is not registered";
                }
            }
            catch
            {
                return "Something Went Wrong";
            }

            return "Bad Request";


        }

        private Boolean SaveData(string OTP,string Mobile)
        {
            
            Boolean Bool = false;
            try
            {
               
                try
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd = new SqlCommand("insert into Aww_tbl_OTP(MobileNo,OTP,IsActive,Datetime) values(@Mobile,@OTP,@A,@Date)", con);
                    cmd.Parameters.AddWithValue("@Mobile", Mobile);
                    cmd.Parameters.AddWithValue("@OTP", OTP);
                    cmd.Parameters.AddWithValue("@A", true.ToString());
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                   

                    int result = cmd.ExecuteNonQuery();
                    if (result == 1)
                    {
                        Bool = true;
                    }
                    else
                    {
                        Bool = false;
                    }

                }
                catch (Exception er)
                {
                    Bool = false;
                }
                
            }
            catch (Exception er)
            {
                Bool = false;

            }
            return Bool;
        }

        private void OTPBySMS(string Mobile, string OTP)
        {
           
           
            Stream dataStream;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://msdgweb.mgov.gov.in/esms/sendsmsrequest");
            request.ProtocolVersion = HttpVersion.Version10;

            ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
            request.Method = "POST";
            try
            {
                string smsservicetype = "bulkmsg";
                string query = "username=" + HttpUtility.UrlEncode("hpgovt") +
                    "&password=" + HttpUtility.UrlEncode("hpdit@1234") +
                    "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
                    "&content=" + HttpUtility.UrlEncode("Your OTP  is: " + OTP + ". Valid for 2 hours only.") +
                    "&bulkmobno=" + HttpUtility.UrlEncode(Mobile) +
                    "&senderid=" + HttpUtility.UrlEncode("hpgovt");
                byte[] byteArray = Encoding.ASCII.GetBytes(query);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();
                WebResponse response = request.GetResponse();
                string Status = ((HttpWebResponse)response).StatusDescription;
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
            catch
            {
                // ScriptManager.RegisterStartupScript(//(this, GetType(), "Online Self Seeding", "alert('"+er.Message.ToString()+"');", true);
            }
        }


        public string Save_AWW_Details(AWW_User_Details aww_user_details)
        {

            if (aww_user_details != null) { 

           
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlCommand cmd = new SqlCommand("insert into AWW_DATA(Aanganwadi_Name,PhoneNumber,TotalEnrollments,Issues_Feedbacks,EntryDate,IMEINo) values(@Aanganwadi_Name,@PhoneNumber,@TotalEnrollments,@Issues_Feedbacks,@EntryDate,@IMEINo)", con);
            cmd.Parameters.AddWithValue("@Aanganwadi_Name", aww_user_details.Aanganwadi_Name);
            cmd.Parameters.AddWithValue("@PhoneNumber", aww_user_details.PhoneNumber);
            cmd.Parameters.AddWithValue("@TotalEnrollments", aww_user_details.TotalEnrollments);
            cmd.Parameters.AddWithValue("@Issues_Feedbacks", aww_user_details.Issues_Feedbacks);
            cmd.Parameters.AddWithValue("@EntryDate", aww_user_details.EntryDate);
            cmd.Parameters.AddWithValue("@IMEINo", aww_user_details.IMEINo);

                int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
               return "Details Sent Succcessfully";
            }
            else
            {
                return  " Details not inserted successfully";
            }
            con.Close();

           
        }else{
          return "Object is null";
        }

            return "Bad Request";
        }

        //public string XMLData(string id)
        //{
        //    return id;
        //}

        #region IRestServiceImpl Members


        public string CheckOTP(string Aadhaar, string OTP)
        {
            return CheckLogIn(Aadhaar, OTP);
        }

        private string CheckLogIn(string Aadhaar,string OTP)
        {
            string Exist = "Unsuccessful";
            try {
                DataTable dt = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("Select *  from Aww_tbl_OTP O inner join AWW_AWWMaster  A on A.Mobile_Number=O.MobileNo  where REPLACE(A.AadhaarNo,'-','')='" + Aadhaar + "' and O.OTP='" + OTP + "'", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);

                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Exist = "Successful";
                    return Exist;
                }
                else
                {
                    return Exist;
                }
            }catch(Exception e)
            {
                return Exist;
            }
           
        }
        #endregion
    }
}

    

