using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logit.Tests
{
    public class MockRequestContext2 : IRequestContext
    {
        public string AbsoluteUri
        {
            get { return "jalla"; }
        }

        public string CompressionType
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, System.Net.Cookie> Cookies
        {
            get { throw new NotImplementedException(); }
        }

        public EndpointAttributes EndpointAttributes
        {
            get { throw new NotImplementedException(); }
        }

        public IFile[] Files
        {
            get { throw new NotImplementedException(); }
        }

        public T Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public string GetHeader(string headerName)
        {
            throw new NotImplementedException();
        }

        public string IpAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string PathInfo
        {
            get { throw new NotImplementedException(); }
        }

        public IRequestAttributes RequestAttributes
        {
            get { throw new NotImplementedException(); }
        }

        public string ResponseContentType
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
