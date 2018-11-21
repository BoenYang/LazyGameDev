package com.sdktoolbox.wechat;

import android.app.Activity;
import android.content.Context;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

import com.tencent.mm.sdk.modelbase.BaseReq;
import com.tencent.mm.sdk.modelbase.BaseResp;
import com.tencent.mm.sdk.modelmsg.SendAuth;
import com.tencent.mm.sdk.modelmsg.SendMessageToWX;
import com.tencent.mm.sdk.modelmsg.WXImageObject;
import com.tencent.mm.sdk.modelmsg.WXMediaMessage;
import com.tencent.mm.sdk.modelmsg.WXWebpageObject;
import com.tencent.mm.sdk.modelpay.PayReq;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.sdk.openapi.WXAPIFactory;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.File;

/**
 * Created by yangbowen on 2018/11/6.
 */
public class Wechat implements IWXAPIEventHandler{

    private static final int THUMB_SIZE = 256;

    private static final String TIME_LINE = "timeline";

    private IWXAPI api = null;

    private Resources res = null;

    private int iconId;

    private Activity activity;

    public void init(Activity activity,String appId,int iconId){
        this.iconId = iconId;
        this.api =  WXAPIFactory.createWXAPI(activity,appId,false);
        api.registerApp(appId);
        res = activity.getResources();
    }

    public void Login(){
        SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo";
        req.state = "dzadn";
        this.api.sendReq(req);
    }

    public void shareImage(String shareTo,String imagePath){
        Bitmap bmp = null;
        File imageFile = new File(imagePath);
        if(imageFile.exists()){
            bmp = BitmapFactory.decodeFile(imagePath);
        }

        if(bmp != null){
            WXImageObject imgObj = new WXImageObject(bmp);
            WXMediaMessage msgObj = new WXMediaMessage();
            msgObj.mediaObject = imgObj;

            //压缩得到缩略图
            int w = bmp.getWidth();
            Bitmap thumpBmp = Bitmap.createScaledBitmap(bmp,w,THUMB_SIZE,true);
            msgObj.thumbData = this.bmpToByteArray(thumpBmp,true);

            SendMessageToWX.Req req = new SendMessageToWX.Req();
            req.transaction = this.buildTransaction("img");
            req.message = msgObj;
            if (shareTo.equals(TIME_LINE)) {
                req.scene = SendMessageToWX.Req.WXSceneTimeline;
                Log.i("[Wechat Share Image]", "call WXSceneTimeline--->");
            } else {
                req.scene = SendMessageToWX.Req.WXSceneSession;
                Log.i("[Wechat Share Image]", "call WXSceneSession--->");
            }
            api.sendReq(req);
            bmp.recycle();
        }
    }

    public void shareText(String shareTo,String title,String message,String url){
        WXWebpageObject webpage = new WXWebpageObject();
        webpage.webpageUrl = url;
        WXMediaMessage msg = new WXMediaMessage(webpage);
        msg.title = title;
        msg.description = message;
        Bitmap thumb = BitmapFactory.decodeResource(res,iconId);
        msg.thumbData = this.bmpToByteArray(thumb, true);
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = String.valueOf(System.currentTimeMillis());
        req.message = msg;
        if (shareTo.equals("timeline")) {
            req.scene = SendMessageToWX.Req.WXSceneTimeline;
            Log.i("[Wechat Share Image]", "call WXSceneTimeline--->");
        } else {
            req.scene = SendMessageToWX.Req.WXSceneSession;
            Log.i("[Wechat Share Image]", "call WXSceneSession--->");
        }
        api.sendReq(req);
    }

    public void pay(String orderInfo){
        try {
            JSONObject data = new JSONObject(orderInfo.substring(orderInfo.indexOf("{"), orderInfo.lastIndexOf("}") + 1));
            try{
                Log.i("[wechat pay]", orderInfo);
                PayReq req = new PayReq();
                req.appId = data.getString("appid");
                Log.i("[wechat pay]", req.appId);
                req.partnerId = data.getString("partnerid");
                Log.i("[wechat pay]", req.partnerId);
                req.prepayId = data.getString("prepayid");
                Log.i("[wechat pay]", req.prepayId);
                req.packageValue = data.getString("package");
                Log.i("[wechat pay]", req.packageValue);
                req.nonceStr = data.getString("noncestr");
                Log.i("[wechat pay]", req.nonceStr);
                req.timeStamp = data.getString("timestamp");
                Log.i("[wechat pay]", req.timeStamp);
                req.sign = data.getString("sign");
                this.api.registerApp(req.appId);
                this.api.handleIntent(activity.getIntent(), this);
                this.api.sendReq(req);
            } catch (Exception e) {
                Log.e("[wechat pay]", e.toString(), e);
            }
        } catch (JSONException e) {
            e.printStackTrace();
            Log.e("[wechat pay]", e.toString(), e);
        }
    }

    public boolean isWechatInstall(){
        return this.api.isWXAppInstalled();
    }

    private String buildTransaction(final String type) {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }

    private byte[] bmpToByteArray(final Bitmap bmp, final boolean needRecycle) {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        bmp.compress(Bitmap.CompressFormat.JPEG, 60, output);
        if (needRecycle) {
            bmp.recycle();
        }

        byte[] result = output.toByteArray();
        try {
            output.close();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return result;
    }

    @Override
    public void onReq(BaseReq baseReq) {

    }

    @Override
    public void onResp(BaseResp baseResp) {

    }
}
