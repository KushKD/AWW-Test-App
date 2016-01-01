using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using RestService;
namespace AWW_Rest
{
    public class ActivityClass
    {
 public string XMLData(string id, string name)
        {
            string name2 = id + name;
            return "You requested product " + name2;
        }

        public string JSONData(string id, string age, string school)
        {
            string details = id + age + school;
            return "You requested product " + details;
        }

        public bool CheckUser(string username, string password, string IMEI)
        {
            return ConfirmLogin(username, password, IMEI);
        }


        private void updatecolumn()
        {
            SqlDataAdapter adp = new SqlDataAdapter("select UPassowrd , UEmail  from dbo.tbl_AdhLogin", ConfigurationManager.ConnectionStrings["UID"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
                            //SqlConnection con = new SqlConnection(connectionString);
                            SqlCommand cmd = new SqlCommand(" Update tbl_AdhLogin set EncPwd='" + dt.Rows[0]["UPassowrd"].ToString() + "' where UEmail='" + dt.Rows[0]["UEmail"].ToString() + "' ", con);

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();

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
                                throw;
                            }
                            finally
                            {
                                cmd.Dispose(); con.Close();
                            }

                        }
                        catch { }
                        finally { dt.Dispose(); adp.Dispose(); }
                    }

                }

            }
            catch { }
            finally { dt.Dispose(); adp.Dispose(); }

        }
        private String Encryption(String Value)
        {
            string value1 = Convert.ToBase64String(Encoding.Unicode.GetBytes(Convert.ToBase64String(Encoding.Unicode.GetBytes(Value))));
            return value1;
        }
        String path;
        public string GetLanIPAddress()
        {
            //Get the Host Name
            string stringHostName = Dns.GetHostName();
            //Get The Ip Host Entry
            IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
            //Get The Ip Address From The Ip Host Entry Address List
            IPAddress[] arrIpAddress = ipHostEntries.AddressList;
            return arrIpAddress[arrIpAddress.Length - 1].ToString();
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
        private void MailSendOTP(String UserName, String OTP)
        {

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new MailAddress("hpuid@hp.gov.in");
            mail.To.Add("hpuid@hp.gov.in");
            String Body = " Your One Time Pin for Aadhaar Search is:<b>" + OTP + "</b>. <br/>This is automatice mail send by Himachal Aadhaar Online Search Portal";

            mail.Subject = "Aadhaar Search Login Details";
            mail.IsBodyHtml = true;
            mail.Body = Body;
            SmtpClient smtp = new SmtpClient("10.241.8.51", 25);
            smtp.Credentials = new NetworkCredential("hpuid", "Cat.Mat@123");
            try
            {
                smtp.Send(mail);
            }
            catch
            { }
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
                throw;
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
            catch
            { }
            finally { cmd.Dispose(); con.Close(); con.Dispose(); }
        }
        static Int32 count = 0;
        private void DisableUser(String UserName)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
            //SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("update tbl_AdhLogin  set IsActive='False' ,deactivedate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where EncEmail=@u ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@u", UserName.Trim());
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }


            try
            {
                cmd.ExecuteReader();
            }
            catch
            { }
            finally { cmd.Dispose(); con.Close(); con.Dispose(); }
        }
        private String GetAdminstrator()
        {
            String admin = "";
            SqlDataAdapter adp = new SqlDataAdapter("Select UEmail from tbl_AdhLogin where Roll='1'", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        admin += dt.Rows[i]["UEmail"].ToString().Trim() + ", ";
                    }
                    admin = admin.Substring(0, admin.Length - 2);
                }

            }
            catch { }
            finally { dt.Dispose(); adp.Dispose(); }
            return admin;
        }
        //String connectionString = "Data Source=10.241.9.54;Initial Catalog=Aadhaar;Integrated Security=True;Max Pool Size=400";//providerName="System.Data.SqlClient";
        /// Amit Singh Date 06 June 2015-----Users are not supposed to get activated from Mobile Divice so OTP is not required.
        /* private string GenerateOPT()
         {
             string charPool = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
             StringBuilder rs = new StringBuilder();
             Random rnd = new Random();
             int length = 6;
             for (int i = 0; i < length; i++)
             {
                 rs.Append(charPool[(int)(rnd.NextDouble() * charPool.Length)]);
             }
             return rs.ToString();

         }

         private void UpdateOTP(String Email, String Otp)
         {
             SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UID"].ConnectionString);
             SqlConnection con = new SqlConnection(connectionString);
             SqlCommand cmd = new SqlCommand(" Update tbl_AdhLogin set OTP=@p where UEmail=@ID and IsActive='true'", con);

             cmd.CommandType = CommandType.Text;
             cmd.Parameters.Clear();
             cmd.Parameters.AddWithValue("@p", Otp);
             cmd.Parameters.AddWithValue("@ID", Email);
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
                 throw;
             }
             finally
             {
                 cmd.Dispose(); con.Close();
             }


         }

         private void ActivateUser(String ID)
         {
             SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UID"].ConnectionString);
             //SqlConnection con = new SqlConnection(connectionString);
             SqlCommand cmd = new SqlCommand(" Update tbl_AdhLogin set isActive=1 where ID=@ID ", con);
             cmd.CommandType = CommandType.Text;
             cmd.Parameters.Clear();
             cmd.Parameters.AddWithValue("@ID", ID);
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
                 throw;
             }
             finally
             {
                 cmd.Dispose(); con.Close();
             }
         }

         private void SendOPT(String Mobile)
         {
             String Otp = GenerateOPT();
             UpdateOTP(txtUserName.Text.Trim(), Otp);
             Stream dataStream;

             HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://msdgweb.mgov.gov.in/esms/sendsmsrequest");
             request.ProtocolVersion = HttpVersion.Version10;

             ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
             request.Method = "POST";
             try
             {
                 String smsservicetype = "bulkmsg";
                 String query = "username=" + HttpUtility.UrlEncode("hpgovt") +
                     "&password=" + HttpUtility.UrlEncode("hpdit@1234") +
                     "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
                     "&content=" + HttpUtility.UrlEncode("Your One Time Pin for Aadhaar Search is:" + Otp) +
                     "&bulkmobno=" + HttpUtility.UrlEncode(Mobile) +
                     "&senderid=" + HttpUtility.UrlEncode("hpgovt");
                 byte[] byteArray = Encoding.ASCII.GetBytes(query);
                 request.ContentType = "application/x-www-form-urlencoded";
                 request.ContentLength = byteArray.Length;
                 dataStream = request.GetRequestStream();
                 dataStream.Write(byteArray, 0, byteArray.Length);

                 dataStream.Close();
                 WebResponse response = request.GetResponse();
                 String Status = ((HttpWebResponse)response).StatusDescription;
                 dataStream = response.GetResponseStream();
                 StreamReader reader = new StreamReader(dataStream);
                 string responseFromServer = reader.ReadToEnd();
                 reader.Close();
                 dataStream.Close();
                 MailSendOTP(txtUserName.Text, Otp);
             }
             catch
             {
                 // ScriptManager.RegisterStartupScript(//(this, GetType(), "Online Self Seeding", "alert('"+er.Message.ToString()+"');", true);
             }
         }*/

        public Boolean ConfirmLogin(String UserName, String Password, String IMEI)
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

            }



            catch (Exception ex) { }
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
            catch
            { }
            finally
            {
                adp.Dispose();
                dt.Dispose();
            }
            return Status;
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
                throw;
            }
            finally
            {
                cmd.Dispose(); con.Close();
            }

        }

        #region ILogOut Members

        public bool LogOutUser(string username)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["UID"].ConnectionString);
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
            catch { }
            finally { adp.Dispose(); dt.Dispose(); }
            return LastLogindate;
        }

        private string getMobile(string p)
        {
            String Mobile = "";
            SqlDataAdapter adp = new SqlDataAdapter("Select Umobile from tbl_AdhLogin where UEmail='" + p.Trim() + "' and isActive='true'", ConfigurationManager.ConnectionStrings["AWWs"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Mobile = dt.Rows[0]["Umobile"].ToString();
                }
            }
            catch { }
            finally { adp.Dispose(); dt.Dispose(); }
            return Mobile;
        }


    }
}
