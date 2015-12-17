package com.dithp.aadhaar.aemc_hp;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.graphics.Color;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

import java.net.HttpURLConnection;
import java.net.URL;

public class Login extends Activity {

    private int backButtonCount = 0;
    Boolean Flag_Initialize = false;
    private Button button_submit, button_getOTP;
    private EditText editText_aadhaarLogin, editText_otpLogin;
    private LinearLayout l1;
    private TextView tv_header;
    private  String aadhaar , otp = null;
    RelativeLayout Login_Header;

    URL url;
    HttpURLConnection conn;
    StringBuilder sb = new StringBuilder();
    String HeaderText;
    String HeaderColor;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        Bundle bundle = getIntent().getExtras();
        HeaderColor = bundle.getString("Header");
         HeaderText = bundle.getString("Header_Text");

        Flag_Initialize = InitializeControls();
        if (Flag_Initialize){
            Login_Header.setBackgroundColor(Color.parseColor(HeaderColor));
            button_submit.setBackgroundColor(Color.parseColor(HeaderColor));
            button_getOTP.setBackgroundColor(Color.parseColor(HeaderColor));

            editText_aadhaarLogin.setHintTextColor(Color.parseColor("#000000"));
            editText_otpLogin.setHintTextColor(Color.parseColor("#000000"));
            tv_header.setText(HeaderText);

            editText_otpLogin.setEnabled(false);

            button_getOTP.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    getAadhaar();
                }
            });

            button_submit.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    getOtpandAadhaa();
                }
            });

        }else{
            Toast.makeText(getApplicationContext(),Constants.Error1,Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public void onBackPressed()
    {
        if(backButtonCount >= 1) {
            Login.this.finish();
        }
        else
        {
            Toast.makeText(this, Constants.EXIT, Toast.LENGTH_SHORT).show();
            backButtonCount++;
        }
    }

    protected boolean isOnline() {
        ConnectivityManager cm = (ConnectivityManager) getSystemService(Login.CONNECTIVITY_SERVICE);
        NetworkInfo netInfo = cm.getActiveNetworkInfo();
        if (netInfo != null && netInfo.isConnectedOrConnecting()) {
            return true;
        } else {
            return false;
        }
    }

    private void getOtpandAadhaa() {

        otp = editText_otpLogin.getText().toString().trim();
        String aadhaar_a = editText_aadhaarLogin.getText().toString().trim();
        if(!otp.isEmpty()){
            if(otp.length()== 6){
                if(isOnline()) {
                    OTP_Async OA = new OTP_Async();
                    OA.execute(aadhaar_a, otp);
                 } else {
                Toast.makeText(this, "Network isn't available", Toast.LENGTH_LONG).show();
            }
            }else{
                Toast.makeText(getApplicationContext(),Constants.OTPError1,Toast.LENGTH_LONG).show();
            }
        }else{
           Toast.makeText(getApplicationContext(),Constants.OTPError2,Toast.LENGTH_LONG).show();
        }

    }
    private void getAadhaar() {

        aadhaar = editText_aadhaarLogin.getText().toString().trim();


        if(!aadhaar.isEmpty() ){
            if(aadhaar.length() == 12 ){

if(isOnline()){
                    Login_Async LA  = new Login_Async();
                    LA.execute(aadhaar);}
             else {
                Toast.makeText(this, "Network isn't available", Toast.LENGTH_LONG).show();
            }



            }else{
                Toast.makeText(getApplicationContext(), Constants.AError1,Toast.LENGTH_LONG).show();
            }

        }else{
            Toast.makeText(getApplicationContext(),Constants.AError2,Toast.LENGTH_LONG).show();
        }
    }

    private Boolean InitializeControls() {
        try{
            button_submit = (Button)findViewById(R.id.bt_login);
            button_getOTP = (Button)findViewById(R.id.bt_getotp);
            editText_aadhaarLogin = (EditText)findViewById(R.id.et_aadhaar);
            editText_otpLogin = (EditText)findViewById(R.id.et_otp);
            l1 = (LinearLayout)findViewById(R.id.otp);
            Login_Header = (RelativeLayout)findViewById(R.id.header_login);
            tv_header = (TextView)findViewById(R.id.tvheader);


            return true;
        }catch (Exception e){
            System.out.println(Constants.Error1 + e.getLocalizedMessage());
            return false;
        }
    }

    class Login_Async extends AsyncTask<String,String,String>{

        String url = null;
        private ProgressDialog dialog;

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            dialog = new ProgressDialog(Login.this);
            dialog.setMessage(Constants.SERVER_MESSAGE);
            dialog.show();
            dialog.setCancelable(false);
        }

        @Override
        protected String doInBackground(String... params) {

            String aadhaar = params[0];
            StringBuilder sb = new StringBuilder();
            sb.append(Constants.url_Generic);
            sb.append(Constants.url_Delemetre);
            sb.append(Constants.methord_Login);
            sb.append(Constants.url_Delemetre);
            sb.append(aadhaar);
            url = sb.toString();

            HttpManager jParser = new HttpManager();
            String result  = jParser.Login(url);

            return result;


        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            JsonParser JP = new JsonParser();

            String finalResult = JP.ParseString(s);

            if(finalResult.equalsIgnoreCase(Constants.Login_Success)){
                dialog.dismiss();
                Toast.makeText(getApplicationContext(), finalResult, Toast.LENGTH_SHORT).show();
                editText_aadhaarLogin.setEnabled(false);
                editText_otpLogin.setEnabled(true);
               }
            else{
                dialog.dismiss();
                Toast.makeText(getApplicationContext(), finalResult, Toast.LENGTH_SHORT).show();
            }
        }
    }

    class OTP_Async extends AsyncTask<String,String,String>{

        private ProgressDialog dialog;
        String url2 = null;
        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            dialog = new ProgressDialog(Login.this);
            dialog.setMessage(Constants.SERVER_MESSAGE);
            dialog.show();
            dialog.setCancelable(false);
        }

        @Override
        protected String doInBackground(String... params) {
            String aadhaar = params[0];
            String otp = params[1];
            StringBuilder sb = new StringBuilder();
            sb.append(Constants.url_Generic);
            sb.append(Constants.url_Delemetre);
            sb.append(Constants.methord_otp);
            sb.append(Constants.url_Delemetre);
            sb.append(aadhaar);
            sb.append(Constants.url_Delemetre);
            sb.append(otp);
            url2 = sb.toString();
            HttpManager jParser = new HttpManager();
            String result  = jParser.Login(url2);

            return result;
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);

            JsonParser JP = new JsonParser();

            String finalResult = JP.ParseStringOTP(s);

            if(finalResult.equalsIgnoreCase(Constants.OTP_Successfull)){
                 dialog.dismiss();
                 Intent i = new Intent(Login.this,MainActivity.class);
                 i.putExtra("Color",  HeaderColor );


                 startActivity(i);
                 Login.this.finish();


            } else{
                dialog.dismiss();
                Toast.makeText(getApplicationContext(), finalResult, Toast.LENGTH_SHORT).show();
            }


        }
    }

}








