using GraphQL.Types;
using RealEstate.DataAccess.Repositories.Contracts;
using RealEstate.Types.Payment;

namespace RealEstate.Types.Property
{
    public class PropertyType : ObjectGraphType<Database.Models.Property>
    {
        public PropertyType()
        {
            Field(x => x.Id);
            Field(x => x.Name, nullable: true);
            Field(x => x.Value, nullable: true);
            Field(x => x.City, nullable: true);
            Field(x => x.Family, nullable: true);
            Field(x => x.Street, nullable: true);
            Field<ListGraphType<PaymentType>>("payments");
            //Field<ListGraphType<PaymentType>>("payments",
            //    arguments: new QueryArguments(new QueryArgument<IntGraphType> {Name = "last"}),
            //    resolve: context =>
            //    {
            //        var lastItemsFilter = context.GetArgument<int?>("last");
            //        return lastItemsFilter != null
            //            ? paymentRepository.GetAllForProperty(context.Source.Id, lastItemsFilter.Value)
            //            : paymentRepository.GetAllForProperty(context.Source.Id);
            //    });
        }
    }
}