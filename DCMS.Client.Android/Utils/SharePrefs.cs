using Android.Annotation;
using Android.Content;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace DCMS.Client.Droid.Utils
{
    public class SharePrefs
    {
        private ISharedPreferences mSP;
        private ISharedPreferencesEditor editor;
        public Context context;
        public static SharePrefs mInstance;

        public static void Init(Context context, string prefsname, FileCreationMode mode)
        {
            mInstance = new SharePrefs
            {
                context = context,
                mSP = context.GetSharedPreferences(prefsname, mode)
            };
            mInstance.editor = mInstance.mSP.Edit();
        }

        private SharePrefs()
        {
        }

        public static SharePrefs Get()
        {
            if (mInstance == null)
            {
                mInstance = new SharePrefs();
            }
            return mInstance;
        }

        public bool GetBoolean(string key, bool defaultVal)
        {
            return this.mSP.GetBoolean(key, defaultVal);
        }
        public bool GetBoolean(string key)
        {
            return this.mSP.GetBoolean(key, false);
        }

        public string GetString(string key, string defaultVal)
        {
            return this.mSP.GetString(key, defaultVal);
        }
        public string GetString(string key)
        {
            return this.mSP.GetString(key, null);
        }

        public int GetInt(string key, int defaultVal)
        {
            return this.mSP.GetInt(key, defaultVal);
        }
        public int GetInt(string key)
        {
            return this.mSP.GetInt(key, 0);
        }

        public float GetFloat(string key, float defaultVal)
        {
            return this.mSP.GetFloat(key, defaultVal);
        }
        public float GetFloat(string key)
        {
            return this.mSP.GetFloat(key, 0f);
        }

        public long GetLong(string key, long defaultVal)
        {
            return this.mSP.GetLong(key, defaultVal);
        }
        public long GetLong(string key)
        {
            return this.mSP.GetLong(key, 01);
        }

        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public ICollection<string> GetStringSet(string key, ICollection<string> defaultVal)
        {
            return this.mSP.GetStringSet(key, defaultVal);
        }
        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public ICollection<string> GetStringSet(string key)
        {
            return this.mSP.GetStringSet(key, null);
        }

        public IDictionary<string, object> GetAll()
        {
            return this.mSP.All;
        }
        public bool Exists(string key)
        {
            return mSP.Contains(key);
        }

        public SharePrefs PutString(string key, string value)
        {
            editor.PutString(key, value);
            editor.Commit();
            return this;
        }
        public SharePrefs PutInt(string key, int value)
        {
            editor.PutInt(key, value);
            editor.Commit();
            return this;
        }
        public SharePrefs PutFloat(string key, float value)
        {
            editor.PutFloat(key, value);
            editor.Commit();
            return this;
        }
        public SharePrefs PutLong(string key, long value)
        {
            editor.PutLong(key, value);
            editor.Commit();
            return this;
        }
        public SharePrefs PutBoolean(string key, bool value)
        {
            editor.PutBoolean(key, value);
            editor.Commit();
            return this;
        }

        public void Commit()
        {
            editor.Commit();
        }

        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public SharePrefs PutStringSet(string key, ICollection<string> value)
        {
            editor.PutStringSet(key, value);
            editor.Commit();
            return this;
        }


        public void PutObject(string key, Java.Lang.Object obj)
        {
            System.IO.MemoryStream baos = new System.IO.MemoryStream();
            ObjectOutputStream outStream = null;
            try
            {
                outStream = new ObjectOutputStream(baos);
                outStream.WriteObject(obj);
                string objectVal = ASCIIEncoding.ASCII.GetString(Base64.Encode(baos.ToArray(), Base64Flags.Default));
                editor.PutString(key, objectVal);
                editor.Commit();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                try
                {
                    if (baos != null)
                    {
                        baos.Close();
                    }
                    if (outStream != null)
                    {
                        outStream.Close();
                    }
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }
        public T GetObject<T>(string key) where T : Java.Lang.Object
        {
            if (mSP.Contains(key))
            {
                string objectVal = mSP.GetString(key, null);
                byte[] buffer = Base64.Decode(objectVal, Base64Flags.Default);
                System.IO.MemoryStream bais = new System.IO.MemoryStream(buffer);
                ObjectInputStream ois = null;
                try
                {
                    ois = new ObjectInputStream(bais);
                    T t = (T)ois.ReadObject();
                    return t;
                }
                catch (StreamCorruptedException e)
                {
                    e.PrintStackTrace();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
                catch (ClassNotFoundException e)
                {
                    e.PrintStackTrace();
                }
                finally
                {
                    try
                    {
                        if (bais != null)
                        {
                            bais.Close();
                        }
                        if (ois != null)
                        {
                            ois.Close();
                        }
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            return null;
        }

        public void PutObject<T>(string key, T obj)
        {
            try
            {
                var value = JsonConvert.SerializeObject(obj);
                editor.PutString(key, value);
                editor.Commit();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        public List<T> GetObjects<T>(string key)
        {
            try
            {
                if (mSP.Contains(key))
                {
                    string objectVal = mSP.GetString(key, null);
                    var value = JsonConvert.DeserializeObject<List<T>>(objectVal);
                    return value;
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return new List<T>();
            }
        }

        public SharePrefs Remove(string key)
        {
            editor.Remove(key);
            editor.Commit();
            return this;
        }
        public SharePrefs RemoveAll()
        {
            editor.Clear();
            editor.Commit();
            return this;
        }
    }
}