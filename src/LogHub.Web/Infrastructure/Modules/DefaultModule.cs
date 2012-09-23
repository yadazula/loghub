using System.Linq;
using LogHub.Web.Infrastructure.Indexes;
using LogHub.Web.Infrastructure.Modules.Tasks;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace LogHub.Web.Infrastructure.Modules
{
  public class DefaultModule : NinjectModule
  {
    public override void Load()
    {
      Bind<IDocumentStore>()
        .ToMethod(_ =>
          {
            var store = new DocumentStore { ConnectionStringName = "RavenDB" }.Initialize();
            store.Conventions.SaveEnumsAsIntegers = true;
            store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.MonotonicRead;
            IndexCreation.CreateIndexes(typeof(LogMessage_Search).Assembly, store);
            return store;
          })
        .InSingletonScope();

      Bind<IDocumentSession>()
        .ToMethod(x => x.Kernel.Get<IDocumentStore>().OpenSession())
        .InRequestScope();
    }
  }
}