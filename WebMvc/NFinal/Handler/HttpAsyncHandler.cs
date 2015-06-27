using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;


namespace NFinal.Handler
{
    public abstract class HttpAsyncHandler : IHttpAsyncHandler, IAsyncResult
    {
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            _callback = cb;
            _context = context;
            _completed = false;
            _state = this;


            ThreadPool.QueueUserWorkItem(new WaitCallback(DoProcess), this);
            return this;
        }


        public void EndProcessRequest(IAsyncResult result)
        {


        }


        public bool IsReusable
        {
            get { return false; }
        }


        public abstract void BeginProcess(HttpContext context);


        public void EndProcess()
        {
            //防止多次进行多次EndProcess  


            if (!_completed)
            {
                try
                {
                    _completed = true;
                    if (_callback != null)
                    {
                        _callback(this);
                    }
                }
                catch (Exception) { }
            }
        }


        private static void DoProcess(object state)
        {
            HttpAsyncHandler handler = (HttpAsyncHandler)state;
            handler.BeginProcess(handler._context);
        }


        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }


        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;


        public object AsyncState
        {
            get { return _state; }
        }


        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }


        public bool CompletedSynchronously
        {
            get { return false; }
        }


        public bool IsCompleted
        {
            get { return _completed; }
        }
    }
}