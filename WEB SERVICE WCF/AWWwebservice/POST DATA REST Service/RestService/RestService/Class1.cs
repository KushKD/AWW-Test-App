using System;
using System.Configuration;
using System.Collections.Generic;

using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Mail;


namespace GenricHelperClass
{

    public class SQLHelper
    {
        String username, Password, SchoolName;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
        public String prop_UName
        {
            get { return username; }
            set { username = value; }
        }
        public String prop_Pwd
        {
            get { return Password; }
            set { Password = value; }
        }
        public DataTable SearchInData()
        {
            
            SqlCommand cmd = new SqlCommand("sp_Login", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Uname", username.ToString().Trim());
            cmd.Parameters.AddWithValue("@pwd", Password.ToString().Trim());
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            //SqlDataAdapter adp = new SqlDataAdapter("SELECT UName,Pwd,Role FROM tbl_Login where UName=@Uname and Pwd=@pwd", ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            //adp.SelectCommand.Parameters.AddWithValue("@Uname", username);
            //adp.SelectCommand.Parameters.AddWithValue("@pwd", Password);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {

                return dt;
            }
            return dt;
        }

        public DataTable ReturnAllRecords(String Table)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select * from " + Table, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable ReturnAllRecordswithColumn(String Table,String Column)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select "+Column+" from " + Table, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable ReturnAllRecords(String Table, String Orderby)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select * from " + Table + " Order by " + Orderby, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable ReturnAllRecordswhere(String Table, String WhereCondition)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select * from " + Table + " Where " + WhereCondition, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public Boolean UpdateStatus(String Table, String[] Values, String whereCondition)
        {
            Boolean Complete = false;
            String parameter = "";
            for (int i = 0; i < Values.Length; i++)
            {
                parameter += Values[i].ToString() + ",";
            }
            parameter = parameter.Substring(0, parameter.Length - 1);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(" Update " + Table + " set " + parameter + " where " + whereCondition, con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try { cmd.ExecuteNonQuery(); Complete = true; }
            catch { }
            finally { cmd.Dispose(); con.Close(); con.Dispose(); }
            return Complete;
        }
        
        public DataTable ReturnwithQuery(String WhereCondition)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select O.Org_name,N.NodalOfficer,O.Org_Website from [tbl_OrgTable] O inner join [tbl_NodalOfficer] N on O.Org_name=N.DepartmentName where N.[MobNo]='" + WhereCondition + "'", ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }
        public DataTable ReturnAllRecords(String Table, String WhereCondition, String OrderBy)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter("Select * from " + Table + " Where " + WhereCondition + "  order by " + OrderBy, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            try
            {
                adp.Fill(dt);

            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public Boolean SaveData(String Table, String Fields, String[] Values)
        {
            Boolean Complete = false;
            String Query = " INSERT INTO " + Table + " ( " + Fields + " ) VALUES ( ";
            String Parameters = "";
            for (int i = 0; i < Values.Length; i++)
            {
                Parameters += "@" + i.ToString() + ",";
            }
            Parameters = Parameters.Substring(0, Parameters.Length - 1);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(Query + Parameters + " ) ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            for (int i = 0; i < Values.Length; i++)
            {
                cmd.Parameters.AddWithValue("@" + i.ToString(), Values[i].ToString());
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            { cmd.ExecuteNonQuery(); Complete = true; }
            catch { }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return Complete;
        }

        public Boolean SaveData(String Table, String Fields, String[] Values, SqlTransaction trans)
        {
            Boolean Complete = false;
            String Query = " INSERT INTO " + Table + " ( " + Fields + " ) VALUES ( ";
            String Parameters = "";
            for (int i = 0; i < Values.Length; i++)
            {
                Parameters += "@" + i.ToString() + ",";
            }
            Parameters = Parameters.Substring(0, Parameters.Length - 1);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(Query + Parameters + " ) ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Transaction = trans;
            cmd.Parameters.Clear();
            for (int i = 0; i < Values.Length; i++)
            {
                cmd.Parameters.AddWithValue("@" + i.ToString(), Values[i].ToString());
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            { cmd.ExecuteNonQuery(); Complete = true; }
            catch { }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return Complete;
        }

        public Boolean UpdateStatus(String[] Values, String whereCondition)
        {
            Boolean Complete = false;
            String parameter = "";
            for (int i = 0; i < Values.Length; i++)
            {
                parameter += Values[i].ToString() + ",";
            }
            parameter = parameter.Substring(0, parameter.Length - 1);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
           // SqlCommand cmd = new SqlCommand(" Update " + Table + " set " + parameter + " where " + whereCondition, con);
            SqlCommand cmd = new SqlCommand("  Update O set " + parameter + " FROM tbl_OrgTable O  inner join tbl_NodalOfficer N on N.DepartmentName=O.Org_name   where N.[MobNo]='" + whereCondition + "'", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try { cmd.ExecuteNonQuery(); Complete = true; }
            catch { }
            finally { cmd.Dispose(); con.Close(); con.Dispose(); }
            return Complete;
        }

        public Boolean UpdateStatus(String Table, String[] Values, String whereCondition, SqlTransaction trans)
        {
            Boolean Complete = false;
            String parameter = "";
            for (int i = 0; i < Values.Length; i++)
            {
                parameter += Values[i].ToString() + ",";
            }
            parameter = parameter.Substring(0, parameter.Length - 1);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(" Update " + Table + " set " + parameter + " where " + whereCondition, con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Transaction = trans;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try { cmd.ExecuteNonQuery(); Complete = true; }
            catch { }
            finally { cmd.Dispose(); con.Close(); con.Dispose(); } return Complete;
        }

        public Boolean DeleteTemp(String Table, String whereCondition)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand("Delete from " + Table + " where " + whereCondition, con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            Boolean deleted = false;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                cmd.ExecuteNonQuery();
                deleted = true;
            }
            catch
            { }
            finally
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }
            return deleted;

        }

        public Boolean DeleteTemp(String Table, String whereCondition, SqlTransaction trans)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand("Delete from " + Table + " where " + whereCondition, con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Transaction = trans;
            Boolean deleted = false;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            try
            {
                cmd.ExecuteNonQuery();
                deleted = true;
            }
            catch
            { }
            finally
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }
            return deleted;

        }

        public String ReturnCount(String TableName, String ColumnName)
        {
            String Count = "0";
            SqlDataAdapter adp = new SqlDataAdapter("Select count( " + ColumnName + " )[Count] from " + TableName, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Count = dt.Rows[0]["Count"].ToString();
                }
            }
            catch
            {


            }
            finally
            {
                adp.Dispose();
                dt.Dispose();
            }
            return Count;
        }

        public String ReturnCount(String TableName, String ColumnName, String Where)
        {
            String Count = "0";
            SqlDataAdapter adp = new SqlDataAdapter("Select count( " + ColumnName + " )[Count] from " + TableName + " Where " + Where, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Count = dt.Rows[0]["Count"].ToString();
                }
            }
            catch
            {


            }
            finally
            {
                adp.Dispose();
                dt.Dispose();
            }
            return Count;
        }

        public String ReturnCount(String TableName, String ColumnName, String Where, SqlTransaction trans)
        {
            String Count = "0";
            SqlDataAdapter adp = new SqlDataAdapter("Select count( " + ColumnName + " )[Count] from " + TableName + " Where " + Where, ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            adp.SelectCommand.Transaction = trans;
            DataTable dt = new DataTable();
            try
            {
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Count = dt.Rows[0]["Count"].ToString();
                }
            }
            catch
            {


            }
            finally
            {
                adp.Dispose();
                dt.Dispose();
            }
            return Count;
        }



        public DataTable ReturnAllRecordswhereCompinfo(string CompInfo)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("sp_returnrecordwhere", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@compno", CompInfo);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            try
            {
                adp.Fill(dt);
            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable ReturnAllRecordswhereStatus(string statusid)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("sp_returnrecordwhereStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@compno", statusid);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            try
            {
                adp.Fill(dt);
            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable ReturnAllRecordswhereStatus1(string Compno)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("sp_returnrecordwhereStatus1", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@compno", Compno);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            try
            {
                adp.Fill(dt);
            }
            catch
            { }
            finally
            { adp.Dispose(); }
            return dt;
        }

        public DataTable  CheckLogIn(string pwd)
        {
            Boolean Exist = false;
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(" Select * from tbl_OTP as otp  inner join tbl_NodalOfficer as aa on aa.MobNo=otp.MobileNo where otp.OTP='" + pwd + "' and otp.MobileNo ='" + HttpContext.Current.Session["MobNo"] + "' and otp.IsActive='true' order by otp.TransID desc", ConfigurationManager.ConnectionStrings["OfficeDirectoryConnectionString"].ConnectionString);
            
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {

                return dt;
            }
            return dt;
        }
     
    }

}
