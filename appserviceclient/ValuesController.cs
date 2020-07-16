using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace appserviceclient
{
    public class ValuesController : ApiController
    {
        public string certHeader = "";
        public string errorString = "";
        private X509Certificate2 certificate = null;
        public string certThumbprint = "";
        public string certSubject = "";
        public string certIssuer = "";
        public string certSignatureAlg = "";
        public string certIssueDate = "";
        public string certExpiryDate = "";
        public bool isValidCert = false;


        [HttpHead]       
        public IHttpActionResult Head()
        {
            if (validatecert(base.Request.Headers))
                return Ok();
            else
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Certificate Invalid"));
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            if (validatecert(base.Request.Headers))
                return new string[] { "value1", "value2" };

            else
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Certificate Invalid"));

        }

        private bool validatecert(HttpRequestHeaders headers)
        {
            headers.TryGetValues("X-ARR-ClientCert", out IEnumerable<string> headerValues);
            var certHeader = headerValues.FirstOrDefault();           
            if (!String.IsNullOrEmpty(certHeader))
            {
                try
                {
                    byte[] clientCertBytes = Convert.FromBase64String(certHeader);
                    certificate = new X509Certificate2(clientCertBytes);
                    certSubject = certificate.Subject;
                    certIssuer = certificate.Issuer;
                    certThumbprint = certificate.Thumbprint;
                    certSignatureAlg = certificate.SignatureAlgorithm.FriendlyName;
                    certIssueDate = certificate.NotBefore.ToShortDateString() + " " + certificate.NotBefore.ToShortTimeString();
                    certExpiryDate = certificate.NotAfter.ToShortDateString() + " " + certificate.NotAfter.ToShortTimeString();
                }
                catch (Exception ex)
                {
                    errorString = ex.ToString();
                }
                finally
                {
                    isValidCert = IsValidClientCertificate();
                    
                }
            }
            else
            {
                return false;
            }

            return isValidCert;
        
        }

        private bool IsValidClientCertificate()
        {
           //DO your custom cert validation here 

            if (certificate.Thumbprint.Trim().ToLower() == "28134dc467335ba86a4f8bda86504c8b62068903") return true;

            return false;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public string Post([FromBody] string value)
        {
            if (validatecert(base.Request.Headers))
                return "success";

            else
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Certificate Invalid"));
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}