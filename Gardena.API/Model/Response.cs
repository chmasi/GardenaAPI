using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.Net.Model
{
    public class Response<T>
    {
        private Response() { }

        /// <summary>
        /// Expected result from the service
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// True if request was sucesfully and returned with expected result
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Readable error string
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Error returned by service
        /// </summary>
        public string ServiceError { get; set; }

        /// <summary>
        /// Service returned status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Store data associated with the response
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// Create an empty response object
        /// </summary>
        /// <returns></returns>
        public static Response<T> CreateEmpty()
        {
            return new Response<T>();
        }

        /// <summary>
        /// Create a successful response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        public static Response<T> CreateSuccessful(T result, HttpStatusCode httpStatusCode)
        {
            return new Response<T> { Success = true, Result = result, HttpStatusCode = httpStatusCode };
        }


        /// <summary>
        /// Create an unsuccessful response
        /// </summary>
        /// <param name="serviceError"></param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        public static Response<T> CreateUnsuccessful(string serviceError, HttpStatusCode httpStatusCode)
        {
            return new Response<T> { Success = false, Error = "See ServiceError", ServiceError = serviceError, HttpStatusCode = httpStatusCode };
        }
    }
}
