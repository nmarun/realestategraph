using Microsoft.Extensions.DependencyInjection;
using RealEstate.API.Mutations;
using RealEstate.API.Queries;
using System;

namespace RealEstate.API.Schema
{
    public class RealEstateSchema : GraphQL.Types.Schema
    {
        public RealEstateSchema(IServiceProvider serviceProvider) : base()
        {
            Query = serviceProvider.GetRequiredService<PropertyQuery>();
            Mutation = serviceProvider.GetRequiredService<PropertyMutation>();
            //Query = resolver.Resolve<PropertyQuery>();
            //Mutation = resolver.Resolve<PropertyMutation>();
        }
    }
}
