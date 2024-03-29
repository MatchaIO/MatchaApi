﻿using System;
using NHibernate;

namespace Matcha.WebApi.Domain.DataAccess.NHibernateImpl
{
    public static class SessionHelper
    {
        public static void Delete<TEntity>(this ISession session, Guid id)
        {
            var queryString = string.Format("delete {0} where id = :id",
                typeof(TEntity));
            session.CreateQuery(queryString)
                .SetParameter("id", id)
                .ExecuteUpdate();
        }
    }
}