using System;
using System.Data;
using System.Reflection;
using Autofac.Builder;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.DataAccess.NHibernateImpl;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace Matcha.WebApi.Tests.NHibernateImpl
{
    public class SqliteFileStorageNHibernateModule : NHibernateAutofacModuleBase
    {
        private readonly string _dbFile = "testdatabase_" + Guid.NewGuid() + ".db";
        public SqliteFileStorageNHibernateModule(Assembly domainAssembly)
            : base(domainAssembly)
        { }

        protected override void SetUpConfiguration(Configuration configuration)
        {
            configuration.DataBaseIntegration(db =>
            {
                db.ConnectionProvider<DriverConnectionProvider>();
                db.Dialect<SQLiteDialect>();
                db.Driver<SQLite20Driver>();
                db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                db.IsolationLevel = IsolationLevel.ReadCommitted;
                db.ConnectionString = "Data Source=" + _dbFile + ";Version=3;New=True";
                db.Timeout = 10;
                db.BatchSize = 20;
                db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
                db.LogSqlInConsole = true;
            });
        }

        protected override void DefineSessionLifeStyle(IRegistrationBuilder<ISession, SimpleActivatorData, SingleRegistrationStyle> configuration)
        {
            configuration.OwnedByLifetimeScope();
        }
    }
}
