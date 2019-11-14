using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;

using System.Threading.Tasks;
using VultPay.Domain.Enums;
using MongoDB.Driver;

namespace VultPay.DataBase.Core
{
    public sealed class RepositoryMongoDB
    {
        public static IMongoDatabase Configure(IConfiguration configuration, string DataBase)
        {
            MongoClient Instance = new MongoClient(configuration.GetConnectionString("MongoConnection").ToString());
            return Instance.GetDatabase(DataBase);
        }
    }
}