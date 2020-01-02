﻿using Abp.Dependency;
using GraphQL;
using GraphQL.Types;
using Zinlo.Queries.Container;

namespace Zinlo.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IDependencyResolver resolver) :
            base(resolver)
        {
            Query = resolver.Resolve<QueryContainer>();
        }
    }
}