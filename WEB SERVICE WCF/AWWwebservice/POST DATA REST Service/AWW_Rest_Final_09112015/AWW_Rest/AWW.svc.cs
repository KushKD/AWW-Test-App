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
using System.Net.Mail;


namespace AWW_Rest
{
 
    public class AWW : IAWW
    {

              //Constants and Variables
                static Int32 count = 0;
                List<Users> UserList = new List<Users>();


            // Aanganwadi Methords All are up and working..... 
            #region LOGIN is WORKING
        public string Login(string Aadhaar)
        {
            return GetMobileNumber(Aadhaar);
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

        private Boolean SaveData(string OTP, string Mobile)
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

        #endregion  
            #region Check OTP IS WORKING
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
            #region SAVE AWW REPORTING DETAILS
            public string Save_AWW_Details(AWW_User_Details aww_user_details)
            {

                if (aww_user_details != null)
                {


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
                        return " Details not inserted successfully";
                    }
                    con.Close();


                }
                else
                {
                    return "Object is null";
                }

                return "Bad Request";
            }

        #endregion


            //HPSRDH Application Functions
            #region HPSRDH SIGNIN FUNCTION WORKING
            public bool CheckUser(string username, string password, string IMEI)
            {
                return ConfirmLogin(username, password, IMEI);
            }

            private Boolean ConfirmLogin(String UserName, String Password, String IMEI)
            {
                Boolean Successful = false;
                try
                {

                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                    //SqlConnection con = new SqlConnection(connectionString);
                    if (CheckForLogin(UserName) == false)
                    {

                        SqlCommand cmd = new SqlCommand("Select top 1 ID,roll,UEmail from tbl_AdhLogin where EncEmail =@Email and EncPwd=@Pass and IsActive='true'", con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Email", UserName);
                        cmd.Parameters.AddWithValue("@Pass", Password);
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        Boolean logIn = false;
                        DataTable dt = new DataTable();
                        try
                        {
                            dt.Load(cmd.ExecuteReader());
                            if (dt.Rows.Count > 0)
                            {


                                logIn = true;
                                //Session["UID"] = dt.Rows[0]["UEmail"].ToString(); //dt.Rows[0]["ID"].ToString();
                                //Session["Roll"] = dt.Rows[0]["roll"].ToString();
                                //Session["UName"] = dt.Rows[0]["UEmail"].ToString();
                                //HiddenField2.Value = dt.Rows[0]["UEmail"].ToString();
                                MailSend(UserName, IMEI);

                                //lblMsz.Text = "LogIn Successfulpdatel";


                            }
                            else
                            {
                                /// Amit Singh 05 June 2015--- User will not Disabled from Mobile Client
                                //count = count + 1;
                                //if (count == 4)
                                //{
                                //    lblMsz.Text = "You have left  with only One attempt Otherwise your account has been locked for 24 hours.";
                                //    txtUserName.Text = "";
                                //    txtPassword.Text = "";
                                //    txt_Captcha.Text = "";
                                //    string returnValue = Convert.ToString(RandomNumber(10000, 99999));
                                //    lblcatcha.Text = Convert.ToString(returnValue);
                                //    Session["randomStr1"] = lblcatcha.Text;
                                //}
                                //else if (count >= 5)
                                //{

                                //    //SendOPT(getMobile(txtUserName.Text.Trim()));
                                //    // UpdateRecords();
                                //    DisableUser();
                                //    count = 0;
                                //    Session["UID"] = txtUserName.Text.Trim();
                                //    //pnlOTP.Visible = true;
                                //    //pnlLogin.Enabled = false;
                                //    lblMsz.Text = "You have exceeded the maximum allowed atempts, hence your account has been locked for 24 hours. Please contact administrator at: " + GetAdminstrator() + ".";
                                //    txtUserName.Text = "";
                                //    txtPassword.Text = "";
                                //    txt_Captcha.Text = "";
                                //    string returnValue = Convert.ToString(RandomNumber(10000, 99999));
                                //    lblcatcha.Text = Convert.ToString(returnValue);
                                //    Session["randomStr1"] = lblcatcha.Text;
                                //}
                                //else
                                //{
                                //    lblMsz.Text = "Wrong UserName/Password";
                                //    txtUserName.Text = "";
                                //    txtPassword.Text = "";
                                //    txt_Captcha.Text = "";
                                //    string returnValue = Convert.ToString(RandomNumber(10000, 99999));
                                //    lblcatcha.Text = Convert.ToString(returnValue);
                                //    Session["randomStr1"] = lblcatcha.Text;
                                //}
                            }

                        }
                        catch
                        { }
                        finally
                        {
                            cmd.Dispose();
                            con.Close();
                        }
                        if (logIn == true)
                        {
                            if (checkvalidation(UserName) <= 60)
                            {
                                if (dt.Rows[0]["roll"].ToString() == "1")
                                {
                                    UpDatePreviouslogin(UserName);
                                    Successful = true;
                                    // Response.Redirect("adminUserAuthentication.aspx");
                                }
                                if (CheckValid(UserName) == true)
                                {
                                    if (dt.Rows[0]["roll"].ToString() != "1")
                                    {
                                        // MailSend(txtUserName.Text.Trim());
                                        UpDatePreviouslogin(UserName);
                                        Successful = true;
                                        // Response.Redirect("Default.aspx");
                                    }

                                }
                                else
                                {
                                    //lblMsz.Text = "Login Time period expired please contact the administrator at " + GetAdminstrator() + ".";
                                    //string returnValue = Convert.ToString(RandomNumber(10000, 99999));
                                    //lblcatcha.Text = Convert.ToString(returnValue);
                                    //Session["randomStr1"] = lblcatcha.Text;
                                }
                            }

                            else
                            {
                                //Session["OldPWd"] = "1";
                                //ScriptManager.RegisterStartupScript(this, GetType(), "AAdhaar Search", "alert('Your Password 3 months older.Please update the password for accessing the Aadhaar Search Portal!!');", true);
                                SendMAil(dt.Rows[0]["UEmail"].ToString());
                                //Response.Redirect("ChangPassword.aspx");
                                //string returnValue = Convert.ToString(RandomNumber(10000, 99999));
                                //lblcatcha.Text = Convert.ToString(returnValue);
                                //Session["randomStr1"] = lblcatcha.Text;

                            }
                        }
                    }
                    else
                    {

                    }

                }



                catch (Exception ex) { throw ex; }
                return Successful;
            }

            private bool CheckForLogin(String Username)
            {
                Boolean Status = false;
                SqlDataAdapter adp = new SqlDataAdapter("Select UName from tbl_AdhLogin where IsLogedIn='true' and EncEmail=@Email", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                adp.SelectCommand.Parameters.AddWithValue("@Email", Username);
                DataTable dt = new DataTable();
                try
                {
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Status = true;
                    }

                }
                catch (Exception ex)
                { throw ex; }
                finally
                {
                    adp.Dispose();
                    dt.Dispose();
                }
                return Status;
            }

            private void MailSend(String UserName, String IMEI)
            {

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress("hpuid@hp.gov.in");
                mail.To.Add("hpuid@hp.gov.in");
                String Body = " User with Email ID:<b>" + UserName + "</b> has loged in at Himachal Aadhaar Online Search Portal on <b>" + DateTime.Now.ToString() + "</b> from Mobile Device with IMEI: <b>" + IMEI + "</b>. <br/>This is automatice mail send by Himachal Aadhaar Online Search Portal";

                mail.Subject = "Aadhaar Search Login Details";
                mail.IsBodyHtml = true;
                mail.Body = Body;
                SmtpClient smtp = new SmtpClient("10.241.8.51", 25);
                smtp.Credentials = new NetworkCredential("hpuid", "test@123");
                try
                {
                    smtp.Send(mail);
                }
                catch
                { }
            }

            private Int32 checkvalidation(string uname)
            {
                Int32 LastLogindate = 0;
                SqlDataAdapter adp = new SqlDataAdapter("select DATEDIFF(DD,LastPasswordchangedate,GETDATE()) as LastPasswordchangedate FROM tbl_AdhLogin where EncEmail=@Email and isActive='true'", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                adp.SelectCommand.Parameters.AddWithValue("@Email", uname);
                DataTable dt = new DataTable();
                try
                {
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        LastLogindate = Convert.ToInt32(dt.Rows[0]["LastPasswordchangedate"]);

                    }
                }
                catch (Exception ex) { throw ex; }
                finally { adp.Dispose(); dt.Dispose(); }
                return LastLogindate;
            }

            private void UpDatePreviouslogin(String UserName)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                SqlCommand cmd = new SqlCommand(" Update tbl_AdhLogin set IsLogedIn='true' where EncEmail=@ID ", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ID", UserName.Trim());
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                }
                try
                {
                    cmd.ExecuteNonQuery();



                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose(); con.Close();
                }

            }

            private bool CheckValid(String userName)
            {
                bool valid = false;
                //SqlDataAdapter adp = new SqlDataAdapter("Select DATEDIFF(hh,convert(datetime,'" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "'),DATEAdd(HH,VlaidHours,flogindate)) [Hours],VlaidHours,flogindate[Fdate] from tbl_AdhLogin where EncEmail='" + txtUserName.Text.Trim() + "' and roll<>1", ConfigurationManager.ConnectionStrings["UID"].ConnectionString);
                SqlDataAdapter adp = new SqlDataAdapter("Select DATEDIFF(hh,convert(datetime,'" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "'),DATEAdd(HH,VlaidHours,flogindate)) [Hours],VlaidHours,flogindate[Fdate] from tbl_AdhLogin where EncEmail=@Email and roll<>1", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                adp.SelectCommand.Parameters.AddWithValue("@Email", userName);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Fdate"].ToString() == "")
                    {
                        if (Convert.ToInt32(dt.Rows[0]["VlaidHours"].ToString()) > 0)
                        {
                            UpDateFlogin(userName);

                            valid = true;
                        }
                    }
                    else if (Convert.ToInt32(dt.Rows[0]["Hours"].ToString()) > 0)
                    {
                        updateLastLogin(userName);
                        valid = true;

                    }
                }
                return valid;

            }

            private void SendMAil(string passwordChangeReminder)
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress("hpuid@hp.gov.in");
                mail.To.Add(passwordChangeReminder);
                String Body = "Your Password 3 months older.Please update the password for accessing the Aadhaar Search Portal!!";

                mail.Subject = "Aadhaar Search Login Details";
                mail.IsBodyHtml = true;
                mail.Body = Body;
                SmtpClient smtp = new SmtpClient("10.241.8.51", 25);
                smtp.Credentials = new NetworkCredential("hpuid", "Cat.Mat@123");
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                { throw ex; }
            }

            private void UpDateFlogin(String UserName)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                //SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(" Update tbl_AdhLogin set Flogindate=@p , LastPasswordchangedate=@c where EncEmail=@ID ", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd.Parameters.AddWithValue("@c", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd.Parameters.AddWithValue("@ID", UserName.Trim());
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                }
                try
                {
                    cmd.ExecuteNonQuery();



                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose(); con.Close();
                }

            }

            private void updateLastLogin(String UserName)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                //SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("update tbl_AdhLogin  set LastLogindate=@p where EncEmail=@u ", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@p", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd.Parameters.AddWithValue("@u", UserName.Trim());
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }


                try
                {
                    cmd.ExecuteReader();
                }
                catch (Exception ex)
                { throw ex; }
                finally { cmd.Dispose(); con.Close(); con.Dispose(); }
            }

            #endregion
            #region HPSRDH SIGNOUT FUNCTION WORKING
            public bool LogOutUser(string username)
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                SqlCommand cmdLogout = new SqlCommand("update tbl_AdhLogin set IsLogedIn='false' where EncEmail=@u", con);
                cmdLogout.Parameters.Clear();
                cmdLogout.Parameters.AddWithValue("@u", username);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                try
                {
                    cmdLogout.ExecuteNonQuery();
                    return true;
                }
                catch { return false; }


            }
            #endregion
            #region HPSRDH GET BY AAdhaar

            public IEnumerable<Users> GetWithAadhaar(string Aadhaar)
            {
                return ReturnGetWithAadhaar(Aadhaar);
            }

            private IEnumerable<Users> ReturnGetWithAadhaar(string Aadhaar)
            {
                SqlDataReader reader = null;
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                SqlCommand adp = new SqlCommand(" SELECT top 10 [eid] [Enroll ID],[uid][Aadhaar],[name][Resident Name],convert(varchar(20),DOB,105)[DOB],[gender][Gender],[addr_careof][Care of] ,[addr_building][Address building],[addr_street][Addr Street],[addr_landmark][Addr Landmark],[addr_locality][Addr Locality] ,[addr_vtc_name] [Addr VTC],[addr_district_name] [Addr District] " +
                                                      " ,[addr_state_name],[addr_pincode],[res_gauardian_name],[res_addr_subdistrict_name],[res_addr_po_name] FROM EID_UID_MAPPING " +
                                                      " where uid='" + Aadhaar + "' ", Con);
                adp.CommandType = CommandType.Text;
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                try
                {
                    reader = adp.ExecuteReader();

                    Users user = null;
                    while (reader.Read())
                    {
                        user = new Users();
                        user.Enroll_ID = reader.GetValue(0).ToString();
                        user.Aadhaar = reader.GetValue(1).ToString();
                        user.Resident_Name = reader.GetValue(2).ToString();
                        user.DOB = reader.GetValue(3).ToString();
                        user.Gender = reader.GetValue(4).ToString();
                        user.Care_of = reader.GetValue(5).ToString();
                        user.Address_Building = reader.GetValue(6).ToString();
                        user.Addr_Street = reader.GetValue(7).ToString();
                        user.Addr_Landmark = reader.GetValue(8).ToString();
                        user.Addr_Locality = reader.GetValue(9).ToString();
                        user.Addr_VTC = reader.GetValue(10).ToString();
                        user.Addr_District = reader.GetValue(11).ToString();
                        user.addr_state_name = reader.GetValue(12).ToString();
                        user.addr_pincode = reader.GetValue(13).ToString();
                        user.res_gauardian_name = reader.GetValue(14).ToString();
                        user.res_addr_subdistrict_name = reader.GetValue(15).ToString();
                        user.res_addr_po_name = reader.GetValue(16).ToString();

                        UserList.Add(user);
                    }


                }
                catch (Exception ex) { throw ex; }
                finally { adp.Dispose(); }
                return UserList;
            }
        #endregion
            #region HPSRDH GET WITH EID

            public IEnumerable<Users> GetWithEID(string EID)
            {
                return ReturnGetWithEID(EID);
            }

            private IEnumerable<Users> ReturnGetWithEID(string eid)
            {
                SqlDataReader reader = null;
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                SqlCommand adp = new SqlCommand(" SELECT top 10 [eid] [Enroll ID],[uid][Aadhaar],[name][Resident Name],convert(varchar(20),DOB,105)[DOB],[gender][Gender],[addr_careof][Care of] ,[addr_building][Address building],[addr_street][Addr Street],[addr_landmark][Addr Landmark],[addr_locality][Addr Locality] ,[addr_vtc_name] [Addr VTC],[addr_district_name] [Addr District] " +
                                                      " ,[addr_state_name],[addr_pincode],[res_gauardian_name],[res_addr_subdistrict_name],[res_addr_po_name] FROM EID_UID_MAPPING " +
                                                      " where EID like'" + eid + "%' ", Con);
                adp.CommandType = CommandType.Text;
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                try
                {
                    reader = adp.ExecuteReader();

                    Users user = null;
                    while (reader.Read())
                    {
                        user = new Users();
                        user.Enroll_ID = reader.GetValue(0).ToString();
                        user.Aadhaar = reader.GetValue(1).ToString();
                        user.Resident_Name = reader.GetValue(2).ToString();
                        user.DOB = reader.GetValue(3).ToString();
                        user.Gender = reader.GetValue(4).ToString();
                        user.Care_of = reader.GetValue(5).ToString();
                        user.Address_Building = reader.GetValue(6).ToString();
                        user.Addr_Street = reader.GetValue(7).ToString();
                        user.Addr_Landmark = reader.GetValue(8).ToString();
                        user.Addr_Locality = reader.GetValue(9).ToString();
                        user.Addr_VTC = reader.GetValue(10).ToString();
                        user.Addr_District = reader.GetValue(11).ToString();
                        user.addr_state_name = reader.GetValue(12).ToString();
                        user.addr_pincode = reader.GetValue(13).ToString();
                        user.res_gauardian_name = reader.GetValue(14).ToString();
                        user.res_addr_subdistrict_name = reader.GetValue(15).ToString();
                        user.res_addr_po_name = reader.GetValue(16).ToString();

                        UserList.Add(user);
                    }


                }
                catch (Exception ex) { throw ex; }
                finally { adp.Dispose(); }
                return UserList;
            }
        #endregion
            #region  HPSRDH SEARCH WORKING
        public IEnumerable<Users>Search(SearchUsers HPSRDH_Search)
      {
          SqlDataReader reader = null;
          SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
          SqlCommand adp = new SqlCommand(" SELECT top 100 [eid] [Enroll ID],[uid][Aadhaar],[name][Resident Name],[dob][DOB],[gender][Gender],[addr_careof][Care of] ,[addr_building][Address building],[addr_street][Addr Street],[addr_landmark][Addr Landmark],[addr_locality][Addr Locality] ,[addr_vtc_name] [Addr VTC],[addr_district_name] [Addr District] " +
                                                " ,[addr_state_name],[addr_pincode],[res_gauardian_name],[res_addr_subdistrict_name],[res_addr_po_name] FROM EID_UID_MAPPING " +
                                                " where" + Returnwhare(HPSRDH_Search), Con);
          adp.CommandType = CommandType.Text;
          if (Con.State == ConnectionState.Closed)
          {
              Con.Open();
          }
          try
          {
              reader = adp.ExecuteReader();

              Users user = null;
              while (reader.Read())
              {
                  user = new Users();
                  user.Enroll_ID = reader.GetValue(0).ToString();
                  user.Aadhaar = reader.GetValue(1).ToString();
                  user.Resident_Name = reader.GetValue(2).ToString();
                  user.DOB = reader.GetValue(3).ToString();
                  user.Gender = reader.GetValue(4).ToString();
                  user.Care_of = reader.GetValue(5).ToString();
                  user.Address_Building = reader.GetValue(6).ToString();
                  user.Addr_Street = reader.GetValue(7).ToString();
                  user.Addr_Landmark = reader.GetValue(8).ToString();
                  user.Addr_Locality = reader.GetValue(9).ToString();
                  user.Addr_VTC = reader.GetValue(10).ToString();
                  user.Addr_District = reader.GetValue(11).ToString();
                  user.addr_state_name = reader.GetValue(12).ToString();
                  user.addr_pincode = reader.GetValue(13).ToString();
                  user.res_gauardian_name = reader.GetValue(14).ToString();
                  user.res_addr_subdistrict_name = reader.GetValue(15).ToString();
                  user.res_addr_po_name = reader.GetValue(16).ToString();

                  UserList.Add(user);
              }


          }
          catch (Exception ex) { throw ex; }
          finally { adp.Dispose(); }
          return UserList;
      }

         private string Returnwhare(SearchUsers HPSRDH_Search)
         {

             String Where = "";
             try
             {
                 if (HPSRDH_Search.District.ToString().Trim() != "")
                 {

                     if (Where.Length > 0)
                     {

                         Where = Where + " and addr_district_name like '%" + HPSRDH_Search.District.ToString().Trim() + "%'";

                     }
                     else
                     {

                         Where = " addr_district_name like '%" + HPSRDH_Search.District.ToString().Trim() + "%'";


                     }
                 }
                 if (HPSRDH_Search.Name.ToString().Trim() != "")
                 {
                     if (Where.Length > 0)
                     {
                         Where = Where + " and Name like '%" + HPSRDH_Search.Name.ToString().Trim() + "%'";

                     }
                     else
                     {

                         Where = " Name like '%" + HPSRDH_Search.Name.ToString().Trim() + "%'";


                     }
                 }
                 if (HPSRDH_Search.F_H_Name.ToString().Trim() != "")
                 {

                     if (Where.Length > 0)
                     {

                         Where = Where + " and addr_careof like '%" + HPSRDH_Search.F_H_Name.ToString().Trim() + "%'";

                     }
                     else
                     {

                         Where = " addr_careof like '%" + HPSRDH_Search.F_H_Name.ToString().Trim() + "%'";

                     }

                 }

                 if (HPSRDH_Search.DOB.ToString().Trim() != "")
                 {

                     //string date = DateTime.ParseExact(Gdate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy");
                     if (Where.Length > 0)
                     {
                         Where = Where + " and CONVERT(varchar(20),DOB,105)='" + HPSRDH_Search.DOB.ToString().Trim() + "'";
                     }
                     else
                     {
                         Where = "  CONVERT(varchar(20),DOB,105)='" + HPSRDH_Search.DOB.ToString().Trim() + "'";
                     }
                 }
                 if (HPSRDH_Search.Pincode.ToString().Trim() != "")
                 {
                     if (Where.Length > 0)
                     {

                         Where = Where + " and addr_pincode='" + HPSRDH_Search.Pincode.ToString().Trim() + "'";

                     }
                     else
                     {

                         Where = " addr_pincode ='" + HPSRDH_Search.Pincode.ToString().Trim() + "'";


                     }
                 }
             }
             catch
             { }

             return Where;
         }

      #endregion


         #region Mobile Seeding Aadhaar


         public string AadhaarSeedMobile(MobileAadhaarSeedSave Mobile_Seed)
         {
             try
             {
                 if (Mobile_Seed != null)
                 {
                     SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Aadhaar"].ConnectionString);
                     SqlCommand cmd = new SqlCommand("INSERT INTO tbl_UIDMobileInfo(AadhaarNumber,MobileNo,Email,Ocupation,MszReceivedFrom,ReceiveDateTime)VALUES(@Adr,@MobNO,@Email,@op,@ReciveFrom,@RDT)", con);
                     cmd.Connection = con;
                     cmd.CommandType = CommandType.Text;
                     cmd.Parameters.Clear();
                     cmd.Parameters.AddWithValue("@Adr", Mobile_Seed.AadhaarNo.ToString());
                     cmd.Parameters.AddWithValue("@MobNO", Mobile_Seed.MobileNo.ToString());
                     cmd.Parameters.AddWithValue("@EMail", Mobile_Seed.Email.ToString() != null ? Mobile_Seed.Email.ToString() : "");
                     cmd.Parameters.AddWithValue("@op", Mobile_Seed.Occupation);
                     cmd.Parameters.AddWithValue("@ReciveFrom", Mobile_Seed.SendingMobileNo);
                     cmd.Parameters.AddWithValue("@RDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                     if (con.State == ConnectionState.Closed)
                     {
                         con.Open();
                     }
                     try
                     {
                         cmd.ExecuteNonQuery();
                         return "Data Sent Successfully";
                     }
                     catch (Exception ex) { throw ex; }
                     finally
                     {
                         con.Close();
                         con.Dispose();
                         cmd.Dispose();
                     }
                     
                 }
                 else
                 {
                     return "Unable to save the data";
                 }
               
             }catch(Exception e){
                 return "Something really bad happened. Unable to save the Data";
             }
         }

         #endregion
    }
    }

    

