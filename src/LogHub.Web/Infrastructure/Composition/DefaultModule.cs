﻿using LogHub.Core.Indexes;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace LogHub.Web.Infrastructure.Composition
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