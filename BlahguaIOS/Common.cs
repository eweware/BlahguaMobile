using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Text.RegularExpressions;

namespace BlahguaMobile.IOS
{

    //Synsoft on 17 July 2014
   public static class Common
    {
        //Synsoft on 17 July 2014
       public static bool IsValidEmail(this String email)
       {
           string MatchEmailPattern =
          @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
   + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
   + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
   + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

           if (email != null)
           {
               return Regex.IsMatch(email, MatchEmailPattern);
           }
           else
           {
               return false;
           }
       }

    }
}