using Models;
using Models.Local;
using MongoDB.Bson;

namespace Services.DataServices
{
    public class PaymentTypeService
    {

        internal readonly RealmService _realmService;
               
        public PaymentTypeService(RealmService realmService)
        {
            _realmService = realmService;
        }

        public async Task SaveNewPaymentType(PaymentType paymentType)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            await _realmService.Realm!.WriteAsync(() =>
            {
                _realmService.Realm.Add(paymentType, false);
            });
        }

        public async Task UpdatePaymentType(PaymentType paymentType)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            await _realmService.Realm!.WriteAsync(() =>
            {
                _realmService.Realm.Add(paymentType, true);
            });
        }

        public async Task DeletePaymentType(ObjectId paymentTypeId)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            var paymentType = _realmService.Realm!.All<PaymentType>()
                .FirstOrDefault(record => record.Id == paymentTypeId);

            if(paymentType is null) { return; }

            await _realmService.Realm.WriteAsync(() =>
            {
                _realmService.Realm.Remove(paymentType);
            });
        }

        public async Task<PaymentType?> GetPaymentTypeById(ObjectId id)
        {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<PaymentType>()
                .FirstOrDefault(record => record.Id == id);
        }

        public async Task<IQueryable<PaymentType>> GetAllPaymentTypes() {
            if (_realmService.Realm is null) await _realmService.InitializeAsync();

            return _realmService.Realm!.All<PaymentType>()
                .OrderBy(record => record.Name);
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
