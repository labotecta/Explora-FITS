using System;
using System.Text;
using System.IO;

namespace ExploraFits
{
    class Idioma
    {
        public static readonly int NUMIDIOMAS = 2;
        public static readonly int NUMMSGIDIOMAS = 111;
        public static int lengua;       // 0 = Español, 1 = Inglés
        public static string[,] msg = new string[NUMIDIOMAS, NUMMSGIDIOMAS];
        public static int LeeMensajes(string s)
        {
            FileStream f;
            StreamReader srtmp;
            try
            {
                f = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.Read);
                srtmp = new StreamReader(f, Encoding.UTF8);
            }
            catch { return 0; }
            int ni;
            int nm;
            int id1;
            int id2;
            string sid;
            string sms;
            while (srtmp.EndOfStream == false)
            {
                sid = srtmp.ReadLine().Trim();
                if (sid.Length > 0 && sid.StartsWith("//") == false)
                {
                    id1 = sid.IndexOf(';');
                    if (id1 == -1)
                    {
                        f.Close();
                        return 1;
                    }
                    id2 = sid.IndexOf(';', id1 + 1);
                    if (id2 == -1)
                    {
                        f.Close();
                        return 2;
                    }
                    try
                    {
                        ni = Convert.ToInt32(sid.Substring(0, id1++));
                        nm = Convert.ToInt32(sid[id1..id2]);
                        if (ni >= Idioma.NUMIDIOMAS || nm >= Idioma.NUMMSGIDIOMAS)
                        {
                            f.Close();
                            return 3;
                        }
                        sms = sid[(id2 + 1)..].Trim();
                        id1 = sms.IndexOf('"') + 1;
                        if (id1 == 0)
                        {
                            f.Close();
                            return 4;
                        }
                        id2 = sms.Length - 1;
                        if (id2 < id1)
                        {
                            f.Close();
                            return 5;
                        }
                        Idioma.msg[ni, nm] = id2 == id1 ? string.Empty : sms[id1..id2];
                    }
                    catch
                    {
                        f.Close();
                        return 6;
                    }
                }
            }
            f.Close();
            for (ni = 0; ni < Idioma.NUMIDIOMAS; ni++)
            {
                for (nm = 0; nm < Idioma.NUMMSGIDIOMAS; nm++)
                {
                    if (Idioma.msg[ni, nm] == null) return 7;
                }
            }
            return -1;
        }
    }
}
