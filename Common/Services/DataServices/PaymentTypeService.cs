using Models;
using Models.Local;
using MongoDB.Bson;
using System.Data.Common;

namespace Services.DataServices
{
    public class PaymentTypeService(RealmService realmService, PaymentPeriodService paymentPeriodService)
    {

        public async Task SaveNewPaymentType(PaymentType paymentType)
        {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            await realmService.Realm!.WriteAsync(() =>
            {
                realmService.Realm.Add(paymentType, false);
            });
        }

        public async Task UpdatePaymentType(PaymentType paymentType)
        {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            await realmService.Realm!.WriteAsync(() =>
            {
                realmService.Realm.Add(paymentType, true);
            });
        }

        public async Task DeletePaymentType(ObjectId paymentTypeId)
        {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            var paymentType = realmService.Realm!.All<PaymentType>()
                .FirstOrDefault(record => record.Id == paymentTypeId);

            if(paymentType is null) { return; }

            await realmService.Realm.WriteAsync(() =>
            {
                realmService.Realm.Remove(paymentType);
            });
        }

        public async Task<PaymentType?> GetPaymentTypeById(ObjectId id)
        {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            return realmService.Realm!.All<PaymentType>()
                .FirstOrDefault(record => record.Id == id);
        }

        public async Task<IQueryable<PaymentType>> GetAllPaymentTypes() {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            return realmService.Realm!.All<PaymentType>()
                .OrderBy(record => record.Name);
        }

        public async Task<List<HouseholdBillSpending>?> GetAllHouseholdBillsForCurrentPaymentPeriod()
        {
            if (realmService.Realm is null) await realmService.InitializeAsync();

            var currentPaymentPeriod = paymentPeriodService.CurrentPaymentPeriod();

            if (currentPaymentPeriod == null)
            {
                return null;
            }

            var paymentCategory = realmService.Realm!.All<PaymentCategory>()
                .FirstOrDefault(record => record.Name == "Household Bills");

            var householdBillsPaymentTypes = realmService.Realm!.All<PaymentType>()
                .Where(record => record.PaymentCategoryId != null && record.PaymentCategoryId == paymentCategory)
                .ToList();
            
            var payments = realmService.Realm!.All<Payments>()
                .Where(record => record.StartDate >= currentPaymentPeriod.DateFrom && record.EndDate <= currentPaymentPeriod.DateTo)                
                .ToList()
                .Where(record => CanProcess(record, householdBillsPaymentTypes));

            var returnList = new List<HouseholdBillSpending>();

            foreach(var paymentType in householdBillsPaymentTypes)
            {
                returnList.Add(new()
                {
                    Name = paymentType.Name,
                    PaymentTypeActive = !paymentType.HavePaymentsEnded,
                    ExpectedAmount = paymentType.DefaultPaymentAmount,
                    AmountSpent = payments
                    .Where(record => record.PaymentTypeId != null && record.PaymentTypeId.Id == paymentType.Id)
                    .Select(record => record.AmountPaid)
                    .Where(record => record != null)    
                    .Sum(record => record)
                });
            }

            returnList.ForEach(record => record.ShowSingleAmountControl = (record.AmountSpent == record.ExpectedAmount));

            return returnList
                .Where(record => record.PaymentTypeActive || (!record.PaymentTypeActive && record.AmountSpent > 0))
                .OrderBy(record => record.Name)                
                .ToList();
        }

        private bool CanProcess(Payments payments,  List<PaymentType> paymentTypes) {
            if (payments.PaymentTypeId is null) return false;
            return paymentTypes.Select(record => record.Id).Contains(payments.PaymentTypeId!.Id);
        }

        public List<SelectablePaymentType> GetSelectablePaymentTypes(bool displayArchivedRecords)
        {
            var payments = realmService.Realm!.All<PaymentType>()
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
