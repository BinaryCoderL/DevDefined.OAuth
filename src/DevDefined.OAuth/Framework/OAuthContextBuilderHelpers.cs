#region License

// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

namespace DevDefined.OAuth.Framework
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;

    public class OAuthContextBuilderHelpers
    {
        protected static NameValueCollection GetCleanedNameValueCollection(NameValueCollection requestQueryString)
        {
            var nvc = new NameValueCollection(requestQueryString);

            if (nvc.HasKeys())
            {
                nvc.Remove(null);
            }

            return nvc;
        }

        protected static Uri CleanUri(Uri uri)
        {
            // this is a fix for OpenSocial platforms sometimes appending an empty query string parameter
            // to their url.

            string originalUrl = uri.OriginalString;
            return originalUrl.EndsWith("&") ? new Uri(originalUrl.Substring(0, originalUrl.Length - 1)) : uri;
        }

        protected static NameValueCollection CollectCookies(WebRequest request)
        {
            return CollectCookiesFromHeaderString(request.Headers[HttpRequestHeader.Cookie]);
        }

        protected static NameValueCollection CollectCookies(HttpRequestBase request)
        {
            return CollectCookiesFromHeaderString(request.Headers["Set-Cookie"]);
        }

        protected static NameValueCollection CollectCookiesFromHeaderString(string cookieHeader)
        {
            var cookieCollection = new NameValueCollection();

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                string[] cookies = cookieHeader.Split(';');
                foreach (string cookie in cookies)
                {
                    //Remove the trailing and Leading white spaces
                    string strCookie = cookie.Trim();

                    var reg = new Regex(@"^(\S*)=(\S*)$");
                    if (reg.IsMatch(strCookie))
                    {
                        Match match = reg.Match(strCookie);
                        if (match.Groups.Count > 2)
                        {
                            cookieCollection.Add(match.Groups[1].Value,
                                HttpUtility.UrlDecode(match.Groups[2].Value).Replace(' ', '+'));
                            //HACK: find out why + is coming in as " "
                        }
                    }
                }
            }

            return cookieCollection;
        }

        protected static void ParseAuthorizationHeader(NameValueCollection headers, OAuthContext context)
        {
            if (headers.AllKeys.Contains("Authorization"))
            {
                context.AuthorizationHeaderParameters = UriUtility.GetHeaderParameters(headers["Authorization"]).ToNameValueCollection();
                context.UseAuthorizationHeader = true;
            }
        }
    }
}