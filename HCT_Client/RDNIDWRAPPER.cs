using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;

namespace RDNIDWRAPPER
{
    internal static partial class DefineConstants
    {
        public const int NID_SUCCESS = 0;
        public const int NID_INTERNAL_ERROR = -1;
        public const int NID_INVALID_LICENSE = -2;
        public const int NID_READER_NOT_FOUND = -3;
        public const int NID_CONNECTION_ERROR = -4;
        public const int NID_GET_PHOTO_ERROR = -5;
        public const int NID_GET_DATA_ERROR = -6;
        public const int NID_INVALID_CARD = -7;
        public const int NID_UNKNOWN_CARD_VERSION = -8;
        public const int NID_DISCONNECTION_ERROR = -9;
        public const int NID_INIT_ERROR = -10;
        public const int NID_SUPPORTED_READER_NOT_FOUND = -11;
    }

    class RDNID
    {
        [DllImport("RDNIDlib.dll",
                EntryPoint = "OpenNIDLib", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 OpenNIDLib(byte[] _szReaders);

        [DllImport("RDNIDlib.dll")]
        public static extern Int32 CloseNIDLib();

        //        [DllImport("RDNIDlib.dll")]
        //        public static extern Int32 LoadNIDData();

        [DllImport("RDNIDlib.dll",
                EntryPoint = "getReaderListRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getReaderListRD([Out] byte[] _szReaders, int size);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "selectReaderRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr selectReaderRD(byte[] _szReaders);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "deselectReaderRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 deselectReaderRD(IntPtr obj);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "isCardInsertRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 isCardInsertRD(IntPtr obj);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "disconnectCardRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 disconnectCardRD(IntPtr obj);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "getNIDNumberRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getNIDNumberRD(IntPtr obj, [Out] byte[] strID);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "getNIDDataRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getNIDDataRD(IntPtr obj, [Out] byte[] strData, int sizeData);

        [DllImport("RDNIDlib.dll",
                EntryPoint = "getNIDPhotoRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getNIDPhotoRD(IntPtr obj, [Out] byte[] strData, out int sizeData);


        [DllImport("RDNIDlib.dll",
                EntryPoint = "getSoftwareInfoRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getSoftwareInfoRD([Out] byte[] strData);


        [DllImport("RDNIDlib.dll",
                EntryPoint = "getDLXInfoRD", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 getDLXInfoRD([Out] byte[] strData);

        static string aByteToString(byte[] b)
        {
            Encoding ut = Encoding.GetEncoding(874); // 874 for Thai langauge
            int i;
            for (i = 0; b[i] != 0; i++) ;

            string s = ut.GetString(b);
            s = s.Substring(0, i);
            return s;
        }

        static byte[] String2Byte(string s)
        {
            // Create two different encodings.
            Encoding ascii = Encoding.GetEncoding(874);
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(s);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            return asciiBytes;
        }


        public RDNID()
        {
        }

        ~RDNID()
        {
            CloseNIDLib();
        }

        static public Int32 OpenNIDLib(string LICfile)
        {
            byte[] _lic = String2Byte(LICfile);
            return OpenNIDLib(_lic);
        }

        static public String[] getReaderListRD()
        {
            byte[] szReaders = new byte[1024 * 2];
            int size = szReaders.Length;
            int numreader = RDNID.getReaderListRD(szReaders, size);
            if (numreader <= 0)
                return null;
            String s = RDNID.aByteToString(szReaders);
            string[] readerlist = s.Split(';');
            return readerlist;
        }

        IntPtr mCard;
        public IntPtr selectReader(String reader)
        {
            byte[] _reader = String2Byte(reader);
            mCard = RDNID.selectReaderRD(_reader);
            return mCard;
        }

        public Int32 deselectReader()
        {
            return RDNID.deselectReaderRD(mCard);
        }

        public Int32 isCardInsert()
        {
            return RDNID.isCardInsertRD(mCard);
        }
        public Int32 disconnectCard()
        {
            return RDNID.disconnectCardRD(mCard);
        }


        public string getNIDNumber()
        {
            byte[] id = new byte[30];
            if (RDNID.getNIDNumberRD(mCard, id) != DefineConstants.NID_SUCCESS)
                return "";
            String s = RDNID.aByteToString(id);
            return s;
        }


        public string getNIDData()
        {
            byte[] data = new byte[1024];
            if (RDNID.getNIDDataRD(mCard, data, data.Length) != DefineConstants.NID_SUCCESS)
                return "";
            String s = RDNID.aByteToString(data);
            return s;
        }


        public byte[] getNIDPicture()
        {
            byte[] data = new byte[1024 * 5];
            int imgsize = data.Length;
            if (RDNID.getNIDPhotoRD(mCard, data, out imgsize) != DefineConstants.NID_SUCCESS)
                return null;
            return data;
        }

        public string getSoftwareInfoRD()
        {
            byte[] data = new byte[1024];
            RDNID.getSoftwareInfoRD(data);
            String s = RDNID.aByteToString(data);
            return s;
        }

        public string getDLXInfoRD()
        {
            byte[] data = new byte[1024];
            RDNID.getDLXInfoRD(data);
            String s = RDNID.aByteToString(data);
            return s;
        }

    }
}
