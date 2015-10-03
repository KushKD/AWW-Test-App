package com.dithp.aadhaar.aemc_hp;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONStringer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;


public class MainActivity extends Activity {

   private Button button_submit;
   private EditText editText_Aanganwadi_name, editText_phone_no, editText_total_enrollments, editText_issues;
    private TextView textView_Aanganwari , textView_IMEI;
    private String aanganwadi_Name, phoneNumber,totalEnrollments, issuesNfeedbacks ,date, deviceID = null;
    private int backButtonCount = 0;
    URL url;
    HttpURLConnection conn;
    StringBuilder sb = new StringBuilder();
    Boolean Flag_Initialize = false;

    @Override
    public void onBackPressed()
    {
        if(backButtonCount >= 1) {
            MainActivity.this.finish();
        }
        else
        {
            Toast.makeText(this, Constants.EXIT, Toast.LENGTH_SHORT).show();
            backButtonCount++;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
         Flag_Initialize = InitializeControls();

        if (Flag_Initialize){
            DateFormat df = new SimpleDateFormat("yyyy-MM-dd hh:mm:ss");
            String dateGetting = df.format(Calendar.getInstance().getTime());
            textView_Aanganwari.setText(dateGetting);
            TelephonyManager telephonyManager = (TelephonyManager)getSystemService(MainActivity.TELEPHONY_SERVICE);
            String deviceIDGetting =telephonyManager.getDeviceId();
            textView_IMEI.setText(deviceIDGetting);
            button_submit.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    getdata();
                }
            });

            editText_Aanganwadi_name.requestFocus();

        }else{
            Toast.makeText(getApplicationContext(),Constants.Error1,Toast.LENGTH_LONG).show();
        }


    }

    private void getdata() {

        aanganwadi_Name = editText_Aanganwadi_name.getText().toString().trim();
        phoneNumber = editText_phone_no.getText().toString().trim();
        totalEnrollments =editText_total_enrollments.getText().toString().trim();
        issuesNfeedbacks = editText_issues.getText().toString().trim();
        date = textView_Aanganwari.getText().toString().trim();
        deviceID = textView_IMEI.getText().toString().trim();

      if(phoneNumber.length()!=0){
          if(phoneNumber.length()== 10) {
              if(totalEnrollments.length()!=0) {
                  if(isOnline()) {
                      PostData pd = new PostData();
                      pd.execute(aanganwadi_Name, phoneNumber, totalEnrollments, issuesNfeedbacks, date, deviceID);
                  }else{
                      Toast.makeText(this, "Network isn't available", Toast.LENGTH_LONG).show();
                  }
              }else{
                  Toast.makeText(getApplicationContext(),Constants.Enroll ,Toast.LENGTH_LONG).show();
              }
          }else{
              Toast.makeText(getApplicationContext(),Constants.PError1,Toast.LENGTH_LONG).show();
          }
      }else{
          Toast.makeText(getApplicationContext(),Constants.PError2,Toast.LENGTH_LONG).show();
      }






    }

    private Boolean InitializeControls() {
        try{
        button_submit = (Button)findViewById(R.id.bt_submit_data);
        editText_Aanganwadi_name = (EditText)findViewById(R.id.et_aanganwadi_center_name);
        editText_phone_no = (EditText)findViewById(R.id.et_phone_number);
        editText_total_enrollments = (EditText)findViewById(R.id.et_total_number_of_enrollments);
        editText_issues = (EditText)findViewById(R.id.et_issuesnfeedbacks);
        textView_Aanganwari = (TextView)findViewById(R.id.et_aanganwadi_date);
        textView_IMEI = (TextView)findViewById(R.id.et_aanganwadi_IMEI);

        return true;
        }catch (Exception e){
            System.out.println(Constants.Error1 + e.getLocalizedMessage());
            return false;
        }
    }

    public class PostData extends AsyncTask<String,String,String>{

        private ProgressDialog dialog;
        @Override
        protected void onPreExecute() {
            super.onPreExecute();

            dialog = new ProgressDialog(MainActivity.this);
            dialog.setMessage(Constants.SERVER_MESSAGE);
            dialog.show();
            dialog.setCancelable(false);
        }

        @Override
        protected String doInBackground(String... param) {
            try {
                url=new URL(Constants.URL_BASE);
                conn = (HttpURLConnection)url.openConnection();
                conn.setDoOutput(true);
                conn.setRequestMethod("POST");
                conn.setUseCaches(false);
                conn.setConnectTimeout(10000);
                conn.setReadTimeout(10000);
                conn.setRequestProperty("Content-Type", "application/json");
                conn.connect();

                JSONStringer userJson = new JSONStringer()
                        .object().key("aww_user_details")
                        .object()
                        .key("Aanganwadi_Name").value(param[0])
                        .key("PhoneNumber").value(param[1])
                        .key("TotalEnrollments").value(param[2])
                        .key("Issues_Feedbacks").value(param[3])
                        .key("EntryDate").value(param[4])
                        .key("IMEINo").value(param[5])
                        .endObject()
                        .endObject();



               // System.out.println(userJson.toString());
                OutputStreamWriter out = new   OutputStreamWriter(conn.getOutputStream());
                out.write(userJson.toString());
                out.close();

                int HttpResult =conn.getResponseCode();
                if(HttpResult ==HttpURLConnection.HTTP_OK){
                    BufferedReader br = new BufferedReader(new InputStreamReader(conn.getInputStream(),"utf-8"));
                    String line = null;
                    while ((line = br.readLine()) != null) {
                        sb.append(line + "\n");
                    }
                    br.close();


                }else{
                    System.out.println(conn.getResponseMessage());
                }


            } catch (MalformedURLException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            } catch (JSONException e) {
                e.printStackTrace();
            }finally{
                if(conn!=null)
                    conn.disconnect();
            }
            return sb.toString();
        }

        @Override
        protected void onPostExecute(String result) {
            super.onPostExecute(result);

            JsonParser JP = new JsonParser();
            String finalResult = JP.POST(result);

            if(finalResult.equals(Constants.DATASENT)){
                clearData();
            dialog.dismiss();
            Toast.makeText(getApplicationContext(), finalResult, Toast.LENGTH_SHORT).show();
                Intent i = new Intent(MainActivity.this,EndScreen.class);
                startActivity(i);
            }
            else{
                dialog.dismiss();
                Toast.makeText(getApplicationContext(), finalResult, Toast.LENGTH_SHORT).show();
            }

        }
    }

    private void clearData() {
        editText_issues.setText("");
        editText_total_enrollments.setText("");
        editText_Aanganwadi_name.setText("");
        editText_phone_no.setText("");
    }

    protected boolean isOnline() {
        ConnectivityManager cm = (ConnectivityManager) getSystemService(MainActivity.CONNECTIVITY_SERVICE);
        NetworkInfo netInfo = cm.getActiveNetworkInfo();
        if (netInfo != null && netInfo.isConnectedOrConnecting()) {
            return true;
        } else {
            return false;
        }
    }
}
