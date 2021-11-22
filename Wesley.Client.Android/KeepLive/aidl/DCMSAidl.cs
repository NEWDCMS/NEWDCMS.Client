using System;
using Boolean = System.Boolean;
using String = System.String;
using List = Android.Runtime.JavaList;
using Map = Android.Runtime.JavaDictionary;

namespace DCMS.Client.Droid.KeepLive
{
	/// <summary>
	/// Aidl Ïà»¥»½ÐÑ·þÎñ
	/// </summary>
	public interface IGuardAidl : global::Android.OS.IInterface
	{
		void WakeUp (String title, String discription, int iconRes);
	}

	public abstract class IGuardAidlStub : global::Android.OS.Binder, global::Android.OS.IInterface, IGuardAidl
	{
		const string descriptor = "DCMS.Client.Droid.KeepLive.DCMSAidl";
		public IGuardAidlStub ()
		{
			this.AttachInterface (this, descriptor);
		}

		public static IGuardAidl AsInterface (global::Android.OS.IBinder obj)
		{
			if (obj == null)
				return null;
			var iin = (global::Android.OS.IInterface) obj.QueryLocalInterface (descriptor);
			if (iin != null && iin is IGuardAidl aidl)
				return aidl;
			return new Proxy (obj);
		}

		public global::Android.OS.IBinder AsBinder ()
		{
			return this;
		}

		protected override bool OnTransact (int code, global::Android.OS.Parcel data, global::Android.OS.Parcel reply, int flags)
		{
			switch (code) {
			case global::Android.OS.BinderConsts.InterfaceTransaction:
				reply.WriteString (descriptor);
				return true;

			case TransactionWakeUp: {
				data.EnforceInterface (descriptor);
				String arg0 = default;
				arg0 = data.ReadString ();
				String arg1 = default;
				arg1 = data.ReadString ();
				int arg2 = default;
				arg2 = data.ReadInt ();
				this.WakeUp (arg0, arg1, arg2);
				reply.WriteNoException ();
				data.WriteString (arg0);
				data.WriteString (arg1);
				data.WriteInt (arg2);
				return true;
				}

			}
			return base.OnTransact (code, data, reply, flags);
		}

		public class Proxy : Java.Lang.Object, IGuardAidl
		{
			global::Android.OS.IBinder remote;

			public Proxy (global::Android.OS.IBinder remote)
			{
				this.remote = remote;
			}

			public global::Android.OS.IBinder AsBinder ()
			{
				return remote;
			}

			public string GetInterfaceDescriptor ()
			{
				return descriptor;
			}

			public void WakeUp (String title, String discription, int iconRes)
			{
				global::Android.OS.Parcel __data = global::Android.OS.Parcel.Obtain ();

				global::Android.OS.Parcel __reply = global::Android.OS.Parcel.Obtain ();

				try {
					__data.WriteInterfaceToken (descriptor);
					__data.WriteString (title);
					__data.WriteString (discription);
					__data.WriteInt (iconRes);
					remote.Transact (IGuardAidlStub.TransactionWakeUp, __data, __reply, 0);
					__reply.ReadException ();

				} finally {
					__data.Recycle ();
				}

			}
		}

		internal const int TransactionWakeUp = global::Android.OS.Binder.InterfaceConsts.FirstCallTransaction + 0;

		public abstract void WakeUp (String title, String discription, int iconRes);

	}
}
