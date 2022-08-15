using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
   public  class Trans
    {
     public  int  id ;
        public int amount ;
        public int transType;// (1 - "AUTH", 2 - "COMMIT", 3 - "REFUND")
        public int cardId ;
        public Card cardDetails;
    }


}
