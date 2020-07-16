using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net.Http;

namespace clientcert
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var host = "https://webapp.ewebsites.net";
            var endpoint = "/api/values";          
            string CertificatePath = @"C:\Users\Documents\ClientCertificate.cer";          
            string filetoUpload = @"c:\users\Documents\fieltopost.json"; //This is the file that will be passed as part of the actual POST.  Make sure its grater than 100 KB
            string postData = File.ReadAllText(filetoUpload);
            StringContent myStringContent = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");


            //Scenario 1: Here the POST call is made directly by passing the client certificate. Since the POST Size is huge (~100KB), the POST call hangs forever. 
            var _clientHandler = new HttpClientHandler();
            _clientHandler.ClientCertificates.Add(new X509Certificate2(CertificatePath, ""));
            HttpClient client = new HttpClient(_clientHandler);
            client.BaseAddress = new Uri(host);
            var httppostresponseTask =  client.PostAsync(host + endpoint, myStringContent);
           


            //Scenario 2: Here the POST call is made after making an HEAD call by passing the client certificate. 
            var _clientHandlersuccess = new HttpClientHandler();
            _clientHandlersuccess.ClientCertificates.Add(new X509Certificate2(CertificatePath, ""));
            HttpClient clientSuccess = new HttpClient(_clientHandlersuccess);
            clientSuccess.BaseAddress = new Uri(host);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, endpoint);
            HttpResponseMessage response = await clientSuccess.SendAsync(request);//First Make a HEAD request
            if (response.IsSuccessStatusCode)
            {
                //If the Head Request is sucessfull then make a POST call.
                var httpResponseMessage = await clientSuccess.PostAsync(host + endpoint, myStringContent);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Scenario 2: The HTTP POST call succeeded with client cert authentication");
                }
            }

            //This call never completes and hang forever. 
            var httppostresponse = await httppostresponseTask;
            if (httppostresponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Scenario 1: The HTTP POST call succeeded with client cert authentication");
            }

            Console.ReadLine();

        }
    }
}
