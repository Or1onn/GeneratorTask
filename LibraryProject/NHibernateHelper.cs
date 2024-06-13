using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using System.Reflection;

namespace LibraryProject;

public class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    private static ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
            {
                var configuration = new Configuration();
                configuration.Configure(); // Использует hibernate.cfg.xml
                var mapper = new ModelMapper();
                var assembly = Assembly.GetExecutingAssembly();
                mapper.AddMappings(assembly.GetExportedTypes());
                configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
                _sessionFactory = configuration.BuildSessionFactory();
            }

            return _sessionFactory;
        }
    }

    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
}