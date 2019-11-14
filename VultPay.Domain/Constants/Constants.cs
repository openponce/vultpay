using System;
using System.Collections.Generic;
using System.Text;

namespace VultPay.Domain.Constants
{
    public class CONSTANTS
    {
        //Appsettings
        public const string APP_SETTINGS_FILE_LOCATION_UNIX = @"/home/user/data/pay_app/appsettings/appsettings.json";
        public const string APP_SETTINGS_FILE_LOCATION_WINDOWS = @"C:\data\pay_app\appsettings\appsettings.json";
        //Messages
        public const string FRAUD_ANALYSIS_MESSAGE_PENDING_REVIEW = "Fraud Analysis - Transaction pending/review!";
        public const string FRAUD_ANALYSIS_MESSAGE_DENIED = "Fraud Analysis - Transaction denied!";
        //
        public const string LOG_MESSAGE_INIT_FRAUD_ANALYSIS = "Fraud Analysis - initialized";
        public const string LOG_MESSAGE_APPLYING_RULE_FRAUD_ANALYSIS = "Fraud Analysis - applying rules";
        public const string LOG_MESSAGE_RESULT_RULE_FRAUD_ANALYSIS = "Fraud Analysis - result of rules";
        public const string LOG_MESSAGE_TRANSACTION_RESULT = "Transaction Result";
        public const string LOG_MESSAGE_TRANSACTION_ERROR = "Transaction Error";
    }

    public class DocumentsFromBrazil
    {
        public const string CPF = "CPF";
        public const string CNPJ = "CNPJ";
    }
}
