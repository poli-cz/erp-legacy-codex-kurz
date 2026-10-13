using System.Collections.Generic;

namespace Erp.Framework
{
    /// <summary>
    /// Výsledek business operace. Business chyby se v ERP NEHLÁSÍ výjimkou, ale přes ErpResult.Fail
    /// s kódem chyby (MZD-xxx mzdy, DPH-xxx DPH, DAT-xxx data). Výjimka = chyba programu.
    /// </summary>
    public class ErpResult
    {
        protected ErpResult(bool success, string errorCode, string message)
        {
            Success = success;
            ErrorCode = errorCode;
            Message = message;
            Warnings = new List<string>();
        }

        public bool Success { get; private set; }
        public string ErrorCode { get; private set; }
        public string Message { get; private set; }

        /// <summary>Upozornění, která výpočet nezastaví (zobrazí se uživateli ve žlutém pruhu).</summary>
        public List<string> Warnings { get; private set; }

        public static ErpResult Ok()
        {
            return new ErpResult(true, null, null);
        }

        public static ErpResult Fail(string errorCode, string message)
        {
            Trace.Log(TraceLevel.Warning, "ErpResult", errorCode + ": " + message);
            return new ErpResult(false, errorCode, message);
        }

        public override string ToString()
        {
            return Success ? "OK" : ErrorCode + ": " + Message;
        }
    }

    public class ErpResult<T> : ErpResult
    {
        private ErpResult(bool success, string errorCode, string message, T value)
            : base(success, errorCode, message)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public static ErpResult<T> Ok(T value)
        {
            return new ErpResult<T>(true, null, null, value);
        }

        public static new ErpResult<T> Fail(string errorCode, string message)
        {
            Trace.Log(TraceLevel.Warning, "ErpResult", errorCode + ": " + message);
            return new ErpResult<T>(false, errorCode, message, default(T));
        }
    }
}
