package com.dithp.aadhaar.aemc_hp;
  import java.io.File;
  import java.io.FileInputStream;
  import java.io.FileOutputStream;
  import java.io.IOException;
  import java.nio.channels.FileChannel;
  import java.util.ArrayList;
  import java.util.Date;
  import java.util.List;
  import android.content.ContentValues;
  import android.content.Context;
  import android.database.Cursor;
  import android.database.sqlite.SQLiteDatabase;
  import android.database.sqlite.SQLiteOpenHelper;
  import android.os.Environment;
  import android.util.Log;
  import android.widget.Toast;

public class DatabaseHandler extends SQLiteOpenHelper {

    boolean bool = false;

    // All Static variables
    // Database Version
    private static final int DATABASE_VERSION = 1;

    // Database Name
    private static final String DATABASE_NAME = "Reporting";

    // Contacts table name
    private static final String TABLE_REPORTING = "Reporting_Data";

    // Contacts Table Columns names
    private static final String KEY_ID = "id";
    private static final String ANGANWARI_NAME = "name";
    private static final String KEY_PH_NO = "phone_number";
    private static final String TOTAL_ENROLLMENTS = "total_enrollments";
    private static final String ISSUESnFEEDBACKS = "issuesNfeedbacks";
    private static final String DATE = "date";
    private static final String DEVICE_ID = "device_id";

    public DatabaseHandler(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    // Creating Tables
    @Override
    public void onCreate(SQLiteDatabase db) {
        String CREATE_CONTACTS_TABLE = "CREATE TABLE " + TABLE_REPORTING + "("
                + KEY_ID + " INTEGER PRIMARY KEY,"
                + ANGANWARI_NAME + " TEXT,"
                + KEY_PH_NO + " TEXT,"
                + TOTAL_ENROLLMENTS + " TEXT,"
                + ISSUESnFEEDBACKS + " TEXT,"
                + DATE + " TEXT,"
                + DEVICE_ID + " TEXT" + ")";
        db.execSQL(CREATE_CONTACTS_TABLE);
    }

    // Upgrading database
    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        // Drop older table if existed
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_REPORTING);

        // Create tables again
        onCreate(db);
    }

    /**
     * All CRUD(Create, Read, Update, Delete) Operations
     */

    void addContact(String aanganwadi_Name ,String phoneNumber, String totalEnrollments, String issuesNfeedbacks, String date ,String deviceID) {
        SQLiteDatabase db = this.getWritableDatabase();

        ContentValues values = new ContentValues();
        values.put(ANGANWARI_NAME, aanganwadi_Name); // Contact Name
        values.put(KEY_PH_NO, phoneNumber); // Contact Phone
        values.put(TOTAL_ENROLLMENTS,totalEnrollments); //Total Enrollments
        values.put(ISSUESnFEEDBACKS,issuesNfeedbacks); //Issues n feedbacks
        values.put(DATE,date); //DATE
        values.put(DEVICE_ID,deviceID);

        // Inserting Row
        db.insert(TABLE_REPORTING, null, values);
        db.close(); // Closing database connection

        try{
            exportDatabse(DATABASE_NAME);
        }catch (Exception e){
            Log.d("Got Error ..",e.getLocalizedMessage());
        }
    }

    // Adding new contact
   /* void addContact(Contact contact) {
        SQLiteDatabase db = this.getWritableDatabase();

        ContentValues values = new ContentValues();
        values.put(KEY_NAME, contact.getName()); // Contact Name
        values.put(KEY_PH_NO, contact.getPhoneNumber()); // Contact Phone

        // Inserting Row
        db.insert(TABLE_CONTACTS, null, values);
        db.close(); // Closing database connection
    }*/

    // Getting single contact
   /* Contact getContact(int id) {
        SQLiteDatabase db = this.getReadableDatabase();

        Cursor cursor = db.query(TABLE_REPORTING, new String[] { KEY_ID,
                        KEY_NAME, KEY_PH_NO }, KEY_ID + "=?",
                new String[] { String.valueOf(id) }, null, null, null, null);
        if (cursor != null)
            cursor.moveToFirst();

        Contact contact = new Contact(Integer.parseInt(cursor.getString(0)),
                cursor.getString(1), cursor.getString(2));
        // return contact
        return contact;
    }*/

    // Getting All Contacts
/*    public List<Contact> getAllContacts() {
        List<Contact> contactList = new ArrayList<Contact>();
        // Select All Query
        String selectQuery = "SELECT  * FROM " + TABLE_CONTACTS;

        SQLiteDatabase db = this.getWritableDatabase();
        Cursor cursor = db.rawQuery(selectQuery, null);

        // looping through all rows and adding to list
        if (cursor.moveToFirst()) {
            do {
                Contact contact = new Contact();
                contact.setID(Integer.parseInt(cursor.getString(0)));
                contact.setName(cursor.getString(1));
                contact.setPhoneNumber(cursor.getString(2));
                // Adding contact to list
                contactList.add(contact);
            } while (cursor.moveToNext());
        }

        // return contact list
        return contactList;
    }*/

    // Updating single contact
  /*  public int updateContact(Contact contact) {
        SQLiteDatabase db = this.getWritableDatabase();

        ContentValues values = new ContentValues();
        values.put(KEY_NAME, contact.getName());
        values.put(KEY_PH_NO, contact.getPhoneNumber());

        // updating row
        return db.update(TABLE_CONTACTS, values, KEY_ID + " = ?",
                new String[] { String.valueOf(contact.getID()) });
    }*/

    // Deleting single contact
  /*  public void deleteContact(Contact contact) {
        SQLiteDatabase db = this.getWritableDatabase();
        db.delete(TABLE_CONTACTS, KEY_ID + " = ?",
                new String[] { String.valueOf(contact.getID()) });
        db.close();
    }*/


    // Getting contacts Count
 /*   public int getContactsCount() {
        String countQuery = "SELECT  * FROM " + TABLE_CONTACTS;
        SQLiteDatabase db = this.getReadableDatabase();
        Cursor cursor = db.rawQuery(countQuery, null);
        cursor.close();

        // return count
        return cursor.getCount();
    }*/

    public void exportDatabse(String databaseName) {
        try {
            File sd = Environment.getExternalStorageDirectory();
            File data = Environment.getDataDirectory();

            if (sd.canWrite()) {
                String currentDBPath = "//data//"+DatabaseHandler.class.getPackage().getName()+"//databases//"+databaseName+"";
                String backupDBPath = "iconn.db";
                File currentDB = new File(data, currentDBPath);
                File backupDB = new File(sd, backupDBPath);

                if (currentDB.exists()) {
                    FileChannel src = new FileInputStream(currentDB).getChannel();
                    FileChannel dst = new FileOutputStream(backupDB).getChannel();
                    dst.transferFrom(src, 0, src.size());
                    src.close();
                    dst.close();
                }
                else{
                    Log.d("Error","No Idea");
                }
            }else{
                Log.d("Error","No Idea 2");
            }
        } catch (Exception e) {

        }
    }
}


