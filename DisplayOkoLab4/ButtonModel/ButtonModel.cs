using System;
using System.Collections.Generic;

namespace VirtualDisplay
{
    public class ButtonModel
    {
        public int ID { get; set; }
        public int BtnId { get; set; }
        public string BtnDate { get; set; }
        public string BtnName { get; set; }
        //public string BtnService { get; set; }

        public string BtnFunction { get; set; }
        public string BtnStatus { get; set; }
        public byte BtnKeyCode { get; set; }


        public IList<ButtonModel> GetData(String bstr, String estr)
        {
            List<ButtonModel> genericList = new List<ButtonModel>();
            SqliteDataAccess db = new SqliteDataAccess();
            genericList = db.ShowButtons(bstr, estr);

            /*
            //тестирование            
            ButtonModel buttonObj;
            for (int i = 0; i < 12345; i++) //You can make this number anything 
                                            //you can think of (and your processor can handle).
            {
                buttonObj = new ButtonModel
                {
                    BtnId = 1 + i,
                    BtnName = "Middle " + i,
                    BtnDate =
            .Now.ToString(),                    
                };

                genericList.Add(buttonObj);

            }
            */           

            return genericList;
        }

    }
}
