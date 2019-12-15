using GraphQL.Types;
using RealEstate.Types.Property;
using RealEstate.DataAccess.Repositories;

namespace RealEstate.API.Queries
{
    public class PropertyQuery : ObjectGraphType
    {
        public PropertyQuery(IPropertyRepository propertyRepository)
        {
            Field<ListGraphType<PropertyType>>(
                "propertiesAll",
                //resolve: context => propertyRepository.GetFields(context.SubFields));
                resolve: context => propertyRepository.GetAll());
            Field<ListGraphType<PropertyType>>(
                "properties",
                resolve: context => propertyRepository.GetFields(context.SubFields));

            Field<PropertyType>(
                "property", // has to be unique (can't be properties as it's already declared above)
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context => propertyRepository.GetById(context.GetArgument<int>("id")));
        }
    }
}