using System;
using Android.OS;
using Android.Runtime;

namespace DCMS.Client.Droid.KeepLive
{
    public interface GuardAidl
    {
        //相互唤醒服务
        void wakeUp(String title, String discription, int iconRes);
    }

    //public class DCMSAidl : Java.Lang.Object, IParcelable
    //{
    //    //void basicTypes(int anInt, long aLong, boolean aBoolean, float aFloat,double aDouble, String aString);

    //    //void wakeUp(String title, String discription, int iconRes);

    //    public int DescribeContents()
    //    {
    //        return 0;
    //    }

    //    public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
    //    {
            
    //    }
    //}

}