using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDNIDWRAPPER;
using System.Drawing;

namespace HCT_Client
{
    public enum CardReaderMode
    { 
        NORMAL,
        HALF_BYPASS,
        FULL_BYPASS
    }

    public class CardReaderManager
    {
        private static CardReaderManager instance;
        RDNIDWRAPPER.RDNID mRDNIDWRAPPER = new RDNIDWRAPPER.RDNID();
        string[] cardReaderList;
        string NIDNum;
        string NIDData;
        public static string NO_CARD_ERROR = "NO_CARD_ERROR";
        public static string NO_READER_ERROR = "NO_READER_ERROR";
        public static string UNKNOWN_ERROR = "UNKNOWN_ERROR";
        public static CardReaderMode cardReaderMode = CardReaderMode.HALF_BYPASS;

        public CardReaderManager()
        {
            string fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\RDNIDLib.DLX";
        
            int nres = RDNID.OpenNIDLib(fileName);
            if (nres != 0)
            {
                String m;
                m = String.Format(" error no {0} ", nres);
                Util.printLine("CardReaderManager error : " + m);
            }
            cardReaderList = RDNID.getReaderListRD();
        }

        public static CardReaderManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardReaderManager();
                }
                return instance;
            }
        }

        public static void InitInstance()
        {
            if (instance == null)
            {
                instance = new CardReaderManager();
            }
        }

        private int ReadCard()
        {
            if (cardReaderList == null)
            {
                Util.printLine("CardReaderManager error in ReadCard : cardReaderList is null");
                return -2;
            }             
            
            String strTerminal = cardReaderList[0];
            IntPtr obj = mRDNIDWRAPPER.selectReader(strTerminal);
            Int32 nInsertCard = mRDNIDWRAPPER.isCardInsert();

            if (nInsertCard != 0)
            {
                String m;
                m = String.Format(" error no {0} ", nInsertCard);
                Util.printLine("CardReaderManager error in ReadCard : " + m);

                mRDNIDWRAPPER.disconnectCard();
                mRDNIDWRAPPER.deselectReader();
                return nInsertCard;
            }

            NIDNum = mRDNIDWRAPPER.getNIDNumber();      //Read NID Number
            NIDData = mRDNIDWRAPPER.getNIDData();

            //NOTE: uncomment the code below if you want to get photo data
            /*byte[] byteImage = mRDNIDWRAPPER.getNIDPicture();
            if (byteImage == null)
            {
                Util.printLine("CardReaderManager error in ReadCard : read photo error");
            }
            else
            {
                //m_picPhoto
                Image img = Image.FromStream(new MemoryStream(byteImage));
                Bitmap MyImage = new Bitmap(img, m_picPhoto.Width-2, m_picPhoto.Height-2);           
                m_picPhoto.Image = (Image)MyImage;
            }*/

            mRDNIDWRAPPER.disconnectCard();
            mRDNIDWRAPPER.deselectReader();
            return 0;
        }

        public static string ReadCardAndGetData()
        {
            int status = instance.ReadCard();
            switch (status)
            {
                case 0:
                    string result = instance.NIDData;
                    instance.NIDData = null;
                    return result;
                case -1:
                case -11:
                    return NO_CARD_ERROR;
                case -2:
                    return NO_READER_ERROR;
                default:
                    return UNKNOWN_ERROR;
            }
        }
    }
}
