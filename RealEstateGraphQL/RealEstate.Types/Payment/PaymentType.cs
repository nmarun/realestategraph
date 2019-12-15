using GraphQL.Types;

namespace RealEstate.Types.Payment
{
    public class PaymentType : ObjectGraphType<Database.Models.Payment>
    {
        public PaymentType()
        {
            Field(x => x.Id);
            Field(x => x.Value, nullable: true);
            Field(x => x.DateCreated, nullable: true);
            Field(x => x.DateOverdue, nullable: true);
            Field(x => x.Paid, nullable: true);
        }
    }
}
