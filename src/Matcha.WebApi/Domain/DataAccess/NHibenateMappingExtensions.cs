using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace Matcha.WebApi.Domain.DataAccess
{
    public static class NHibenateMappingExtensions
    {
        public static IEnumerable<Type> GetMappingTypes(Assembly domainAssembly)
        {
            var mappers = domainAssembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(
                    t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IClassMapper<>)));

            var subclassMappers = domainAssembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(
                    t =>
                        t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubclassMapper<>)));
            return mappers.Union(subclassMappers);
        }
    }
}