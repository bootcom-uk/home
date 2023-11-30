using Models;
using Models.Local;

namespace Services.DataServices
{
    public class PaymentTypeService
    {

        internal readonly RealmService _realmService;
               
        public PaymentTypeService(RealmService realmService)
        {
            _realmService = realmService;
        }

        public List<SelectablePaymentType> GetSelectablePaymentTypes(bool displayArchivedRecords)
        {
            var payments = _realmService.Realm.All<PaymentType>()
                 .ToList()
                 .Select(record => new SelectablePaymentType
                 {
                     Active = !record.HavePaymentsEnded,
                     Id = record.Id,
                     Name = record.Name,
                     PaymentAmount = record.DefaultPaymentAmount,
                     IsResourceRequired = record.IsResourceRequired
                 });

                if (!displayArchivedRecords)
                {
                    payments = payments.Where(record => record.Active == true);
                }

            payments = payments
            .OrderBy(record => record.Name);

            return payments.ToList();
        }

    }
}
