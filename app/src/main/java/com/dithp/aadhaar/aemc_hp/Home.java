package com.dithp.aadhaar.aemc_hp;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.Toast;

public class Home extends Activity {

    LinearLayout bt_Aww, bt_Pec , bt_Csc;
    Boolean initialize_layout = false;

    String Header = null;
    String Color_Button = null;
    String Text_Color = null;
    String HeaderText = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_home);
        initialize_layout = Layout_Initialize();
        if(initialize_layout){

            /**
             * CSC Button Click
             */
            bt_Csc.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {

                    Header = "#9dbda9";
                   // Color_Button = "";
                   // Text_Color = "";
                    HeaderText = "CSC Login";

                    Intent intent = new Intent(Home.this,Login.class);
                    intent.putExtra("Header", Header);
                    intent.putExtra("Header_Text", HeaderText);
                   // intent.putExtra("Text_Color", Text_Color);
                    startActivity(intent);
                }
            });


            /**
             * Aww Button Click
             */
             bt_Aww.setOnClickListener(new View.OnClickListener() {
                 @Override
                 public void onClick(View v) {

                     Header = "#00ad96";
                     // Color_Button = "";
                     // Text_Color = "";
                     HeaderText = "Anganwadi Login";
                     Intent intent = new Intent(Home.this,Login.class);
                     intent.putExtra("Header", Header);
                     intent.putExtra("Header_Text", HeaderText);
                     // intent.putExtra("Text_Color", Text_Color);
                     startActivity(intent);
                 }
             });

            /**
             * Pec Button Click
             */
            bt_Pec.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    Header = "#1595a6";
                    HeaderText = "PEC Login";
                    // Color_Button = "";
                    // Text_Color = "";
                    Intent intent = new Intent(Home.this,Login.class);
                    intent.putExtra("Header", Header);
                    intent.putExtra("Header_Text", HeaderText);
                    // intent.putExtra("Text_Color", Text_Color);
                    startActivity(intent);
                }
            });




        }else{
            Toast.makeText(Home.this, "Problem in loading the Interface", Toast.LENGTH_SHORT).show();
        }




    }

    /**
     * Initialize the Layout
     * @return
     */
    private Boolean Layout_Initialize() {

        try{
            bt_Csc = (LinearLayout)findViewById(R.id.csc_click);
            bt_Aww = (LinearLayout)findViewById(R.id.aww_click);
            bt_Pec = (LinearLayout)findViewById(R.id.pec_click);
            return true;
        }catch(Exception e){
            return false;
        }

    }

}
