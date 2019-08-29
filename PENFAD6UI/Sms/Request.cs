using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBankWebService.Utils
{
    public class Request
    {
        public String Url { get; set; }
        public Dictionary<String, String> Parameters { get; set; }
    }

}