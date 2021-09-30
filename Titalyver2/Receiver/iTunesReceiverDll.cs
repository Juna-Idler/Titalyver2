using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;


namespace Titalyver2
{
    public class iTunesReceiverDll
    {
        private Type iTunesReceiverType;
        public bool Load()
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom("iTunesReceiver.dll");
                Type type = assembly.GetType("iTunesReceiver.iTunesReceiver", true);

                iTunesReceiverType = type;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        public ITitalyverReceiver GetReceiver()
        {
            if (iTunesReceiverType == null)
                return null;
            try
            {
                return Activator.CreateInstance(iTunesReceiverType) as ITitalyverReceiver;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //アンロードは無い
    }
}
