using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Matcha.WebApi.Domain.DataAccess.NHibernateImpl
{
    public abstract class NHibernateAutofacModuleBase : Autofac.Module
    {
        private readonly Assembly _domainAssembly;

        protected NHibernateAutofacModuleBase(Assembly domainAssembly)
        {
            _domainAssembly = domainAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = GetType().Assembly;
            var publicInstances = assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract).ToArray();
            builder.RegisterTypes(publicInstances.Where(t => t.Name.Contains("Repository") && t.Namespace.Contains("NHibernate")).ToArray())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlEventPublisher>().AsImplementedInterfaces().SingleInstance();
           

            var modelMapper = new ModelMapper();
            modelMapper.AfterMapProperty += ModelMapper_AfterMapProperty;

            var mappers = NHibenateMappingExtensions.GetMappingTypes(_domainAssembly);
            modelMapper.AddMappings(mappers);
            var mapping = modelMapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            SetUpConfiguration(configuration);

            configuration.AddMapping(mapping);

            builder.RegisterInstance(configuration).AsSelf();

            builder.RegisterInstance(configuration.BuildSessionFactory())
                .As<ISessionFactory>();

            DefineSessionLifeStyle(builder.Register(c => c.Resolve<ISessionFactory>().OpenSession()).As<ISession>());
        }

        protected virtual void ModelMapper_AfterMapProperty(
            IModelInspector modelinspector,
            PropertyPath member,
            IPropertyMapper propertycustomizer)
        { }
        protected abstract void SetUpConfiguration(Configuration configuration);
        protected abstract void DefineSessionLifeStyle(IRegistrationBuilder<ISession, SimpleActivatorData, SingleRegistrationStyle> configuration);
    }
}