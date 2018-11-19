package com.sdktoolbox.utils;

import android.app.Activity;
import android.content.ClipData;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Environment;
import android.os.PowerManager;
import android.provider.MediaStore;
import android.telephony.TelephonyManager;
import android.util.Base64;
import android.util.DisplayMetrics;
import android.util.Log;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.IntBuffer;
import java.util.Arrays;

import javax.microedition.khronos.opengles.GL10;

import static android.content.Context.WIFI_SERVICE;

/**
 * Created by yangbowen on 2018/11/6.
 */
public class Utils {

    private static PowerManager.WakeLock wakeLock;

    public static String getMetaData(Activity activity, String key) {
        ApplicationInfo appInfo;
        String msg = "";
        try {
            appInfo = activity.getPackageManager().getApplicationInfo(activity.getPackageName(),
                    PackageManager.GET_META_DATA);
            msg = appInfo.metaData.get(key).toString();
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }
        return msg;
    }

    public static String getDeviceId(Activity activity){
        try {
            TelephonyManager telephoneManager = (TelephonyManager) activity.getSystemService(Context.TELEPHONY_SERVICE);
            String deviceID = telephoneManager.getDeviceId();
            if (deviceID != null && !"".equals(deviceID)) {
                return deviceID;
            }
            deviceID = android.os.Build.SERIAL;
            if (deviceID != null && !"".equals(deviceID)) {
                return deviceID;
            }
            deviceID = InstallId.id(activity);
            if (deviceID != null && !"".equals(deviceID)) {
                return deviceID;
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return "";
    }

    public static String getApplicationName(Activity activity){
        PackageManager packageManager = null;
        ApplicationInfo applicationInfo = null;
        String applicationName = "";
        try {
            packageManager = activity.getApplicationContext().getPackageManager();
            applicationInfo = packageManager.getApplicationInfo(activity.getPackageName(), 0);
            applicationName = (String) packageManager.getApplicationLabel(applicationInfo);
        } catch (PackageManager.NameNotFoundException e) {
            applicationInfo = null;
            Log.e("[Utils]","getApplicationName" + e);

        }
        return applicationName;
    }

    public static String getClipBoardString(Activity activity){
        android.content.ClipboardManager clipboard = (android.content.ClipboardManager)(activity.getSystemService(Context.CLIPBOARD_SERVICE));
        ClipData primaryClip = clipboard.getPrimaryClip();
        String ret = "";
        if(primaryClip != null && primaryClip.getItemCount() > 0){
            CharSequence str = primaryClip.getItemAt(0).getText();
            ret = str != null ? str.toString() : "";
            byte[] encodeBase64;
            try {
                encodeBase64 = Base64.encode(ret.getBytes("utf-8"),0);
                ret = new String(encodeBase64);
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.getClipBoardString, 1,string2));;
            } catch (UnsupportedEncodingException e) {
                e.printStackTrace();
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.getClipBoardString, 1, ""));
            }

        }else{
            //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.getClipBoardString, 1, ""));
        }
        return ret;

    }

    public static String getUrlParam(String paramName){
        return "";
    }

    public static String getIp(){
        return "";
    }

    public static void vibrate(long milliseconds){

    }

    public static void vibrateAsPattern(long[] pattetn,boolean isRepeat){

    }

    public static void setClipBoardString(Activity activity,String text){
        android.content.ClipboardManager clipboard = (android.content.ClipboardManager)(activity.getSystemService(Context.CLIPBOARD_SERVICE));
        android.content.ClipData clip = android.content.ClipData.newPlainText("Copied Text", text);
        clipboard.setPrimaryClip(clip);
    }

    public static void keepScreenOn(Activity activity,boolean on){
        if (on) {
            PowerManager pm = (PowerManager) activity.getSystemService(Context.POWER_SERVICE);
            wakeLock = pm.newWakeLock(PowerManager.SCREEN_BRIGHT_WAKE_LOCK | PowerManager.ON_AFTER_RELEASE, "==KeepScreenOn==");
            wakeLock.acquire();
        } else {
            if (wakeLock != null) {
                wakeLock.release();
                wakeLock = null;
            }
        }
    }

    public static boolean isNetworkAvailable(Context context){
        ConnectivityManager connectivity = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (connectivity != null) {
            NetworkInfo info = connectivity.getActiveNetworkInfo();
            if (info != null && info.isConnected()) {
                // 当前网络是连接的
                if (info.getState() == NetworkInfo.State.CONNECTED) {
                    // 当前所连接的网络可用
                    return true;
                }
            }
        }
        return false;
    }

    public static int getNetType(Activity activity){
        ConnectivityManager cm = (ConnectivityManager) activity.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = cm.getActiveNetworkInfo();
        String[] network2G = new String[]{"GPRS","EDGE","CDMA","1xRTT","IDEN"};
        String[] network3G = new String[]{"UMTS","EVDO_0","EVDO_A","HSDPA","HSUPA","HSPA","EVDO_B","EHRPD","HSPAP","TD-SCDMA","WCDMA","CDMA2000"};
        if(networkInfo != null){
            if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                return 0;
            } else if (networkInfo.getType() == ConnectivityManager.TYPE_MOBILE) {
                String subTypeName = networkInfo.getSubtypeName();
                if(Arrays.binarySearch(network2G,subTypeName) >= 0){
                    return 1;
                } else if(Arrays.binarySearch(network3G,subTypeName) >= 0){
                    return 2;
                } else if(subTypeName.equals("LTE")){
                    return 3;
                }
            }else{
                return -1;
            }
        }else{
            return -1;
        }
        return -1;
    }

    public static int getWifiStrength(Activity activity){
        WifiManager wifiManager = (WifiManager) activity.getSystemService(WIFI_SERVICE);
        // WifiInfo wifiInfo = wifiManager.getConnectionInfo();
        WifiInfo info = wifiManager.getConnectionInfo();
        if (info.getBSSID() != null) {
            int strength = WifiManager.calculateSignalLevel(info.getRssi(), 4);
            String code = strength + "";
            //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.getSignalStrength, 1, code));
            return strength;
        }
        return 0;
    }

    public static int getSignalStrength(Activity activity){
        return 0;
    }

    public static void getBattery(Activity activity){

    }

    public static void saveImgToAblbum(String path,Activity activity) {
        File appDir = new File(Environment.getExternalStorageDirectory(),"Temp");

        if(!appDir.exists()){
            appDir.mkdir();
        }

        File pngFile = new File(path);

        if(pngFile.exists()){
            Bitmap bitmap = BitmapFactory.decodeFile(path);
            String fileName = System.currentTimeMillis() + ".png";
            File saveDst = new File(appDir,fileName);

            try {
                FileOutputStream fos = new FileOutputStream(saveDst);
                bitmap.compress(Bitmap.CompressFormat.PNG, 100, fos);
                fos.flush();
                fos.close();
            } catch (FileNotFoundException e) {
                e.printStackTrace();
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.saveAlbum, 0, ""));
            } catch (IOException e) {
                e.printStackTrace();
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.saveAlbum, 0, ""));
            }

            try {
                MediaStore.Images.Media.insertImage(activity.getContentResolver(), saveDst.getAbsolutePath(), fileName, null);
                Context context = activity.getBaseContext();
                context.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.parse("file://" + appDir)));
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.saveAlbum, 1, ""));
            } catch (FileNotFoundException e) {
                e.printStackTrace();
                //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.saveAlbum, 0, ""));
            }
        }

    }

    public static void installApk(final Activity activity,final String path){
        new Thread(new Runnable() {
            @Override
            public void run() {
                File file = new File(path);
                if(file.exists() && path.endsWith(".apk")){
                    Intent intent = new Intent();
                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    intent.setAction(android.content.Intent.ACTION_VIEW);
                    intent.setDataAndType(Uri.fromFile(file), "application/vnd.android.package-archive");
                    activity.startActivity(intent);
                }
            }
        }).start();
    }

    public static void downLoadFile(String downloadUrl,String saveFileName,String savePath){
        File dir = new File("/sdcard/" + savePath);
        if(!dir.exists()){
            dir.mkdir();
        }
        File file = new File(dir + "/" + saveFileName);

        try {
            URL url = new URL(downloadUrl);
            HttpURLConnection conn = (HttpURLConnection)url.openConnection();
            InputStream is = conn.getInputStream();
            int fileSize = conn.getContentLength();
            FileOutputStream fos = new FileOutputStream(file);
            byte[] buf = new byte[512];
            double count = 0;
            conn.connect();
            if(conn.getResponseCode() >= 400){

            }else{
                int numread;
                int old_persent = 0;
                while ((numread = is.read(buf)) != -1) {
                    fos.write(buf, 0, numread);
                    count += numread;
                    int persent = (int) (((float) (count) / (float) (fileSize)) * 100);
                    if (old_persent != persent) {
                        //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.downLoadApk, 1, String.valueOf(persent)));
                        old_persent = persent;
                    }

                    if (persent == 100) {
                        //ExtensionApi.callBackOnGLThread(this.bindMsg(ExtensionApi.downLoadApk, 2, ""));
                    }
                    conn.disconnect();
                    fos.close();
                    is.close();
                }
            }
        } catch (MalformedURLException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static Bitmap getScreenShotBitmap(Resources res,GL10 gl){
        DisplayMetrics displayMetrics = res.getDisplayMetrics();
        Bitmap bitmap = getBitmapFromGL(displayMetrics.widthPixels, displayMetrics.heightPixels, gl);
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 60, out);
        bitmap.recycle();
        BitmapFactory.Options newOpts = new BitmapFactory.Options();
        int be = 2;
        newOpts.inSampleSize = be;
        ByteArrayInputStream isBm = new ByteArrayInputStream(out.toByteArray());
        Bitmap retBitmap = BitmapFactory.decodeStream(isBm, null, null);
        return retBitmap;
    }

    private static Bitmap getBitmapFromGL(int w, int h, GL10 gl){
        int bitmapBuffer[] = new int[w * h];
        int bitmapSource[] = new int[w * h];
        IntBuffer intBuffer = IntBuffer.wrap(bitmapBuffer);
        intBuffer.position(0);
        gl.glReadPixels(0, 0, w, h, GL10.GL_RGBA, GL10.GL_UNSIGNED_BYTE, intBuffer);
        int offset1, offset2;
        for (int i = 0; i < h; i++) {
            offset1 = i * w;
            offset2 = (h - i - 1) * w;
            for (int j = 0; j < w; j++) {
                int texturePixel = bitmapBuffer[offset1 + j];
                int blue = (texturePixel >> 16) & 0xff;
                int red = (texturePixel << 16) & 0x00ff0000;
                int pixel = (texturePixel & 0xff00ff00) | red | blue;
                bitmapSource[offset2 + j] = pixel;
            }
        }
        return Bitmap.createBitmap(bitmapSource, w, h, Bitmap.Config.RGB_565);
    }

    //TODO 相机

    //TODO 联系人

    //TODO 陀螺仪

    //TODO 经纬度获取

    //
}
