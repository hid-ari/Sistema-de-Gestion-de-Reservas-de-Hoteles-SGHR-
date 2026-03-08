using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Data.Base
{
    public class OperationResult
    {
        public OperationResult(bool isSuccces, string message, dynamic? data) 
        {
            IsSuccess = isSuccces;
            Message = message;
            Data = Data;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public dynamic? Data { get; set; }

        public static OperationResult Success(string message, dynamic? data = null)
        {
            return new OperationResult(true, message, data);
        }

        public static OperationResult Failure(string message, dynamic? data = null)
        {
            return new OperationResult(false, message, data);
        }
    }
}
