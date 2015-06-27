using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace NFinal.Common.SMS.Open189
{
    public class BaoMingTemplate:Template
    {
        public BaoMingTemplate(string tplId)
            : base(tplId)
        { }
        public bool SendSMS(string acceptor_tel, string name, string phone)
        {
            NameValueCollection nvc=new NameValueCollection();
            nvc.Add("name", name);
            nvc.Add("phone", phone);
            return Send(acceptor_tel, tplId, nvc,"");
        }
    }
}