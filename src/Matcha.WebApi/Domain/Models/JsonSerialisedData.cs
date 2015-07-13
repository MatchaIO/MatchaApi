using System;
using System.Data;
using Newtonsoft.Json;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Matcha.WebApi.Domain.Models
{
    public class JsonSerialisedData<T> : IUserType
        where T : class
    {
        public SqlType[] SqlTypes
        {
            get { return new SqlType[] { new StringSqlType(SqlClientDriver.MaxSizeForLengthLimitedString + 1) }; }
        }

        public Type ReturnedType
        {
            get { return typeof(T); }
        }

        public bool IsMutable
        {
            get { return true; }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException(String.Format("Expected a single column, received {0}", String.Join(", ", names)));

            var serialisedValue = rs[names[0]] as string;

            return !string.IsNullOrWhiteSpace(serialisedValue)
                ? JsonConvert.DeserializeObject<T>(serialisedValue)
                : null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var parameter = (IDataParameter)cmd.Parameters[index];
            parameter.Value = (value != null)
                ? (object)JsonConvert.SerializeObject(value)
                : DBNull.Value;
        }

        public object DeepCopy(object value)
        {
            if (value == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return JsonConvert.SerializeObject(x) == JsonConvert.SerializeObject(y);
        }

        public int GetHashCode(object x)
        {
            if (x == null)
                return 0;

            return x.GetHashCode();
        }
    }
}