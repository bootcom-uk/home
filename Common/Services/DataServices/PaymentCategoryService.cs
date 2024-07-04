using Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DataServices
{
    public class PaymentCategoryService
    {

        internal RealmService _realmService;

        public PaymentCategoryService(RealmService realmService)
        {
            _realmService = realmService;
        }

        public async Task<PaymentCategory?> GetPaymentCategory(ObjectId? id)
        {
            if (_realmService.Realm is null)
            {
                await _realmService.InitializeAsync();
            }

            if (id is null) return null;

            return _realmService.Realm!.All<PaymentCategory>()
                .FirstOrDefault(record => record.Id == id);
        }

        public async Task<IQueryable<PaymentCategory>> GetAllPaymentCategories() {
            if (_realmService.Realm is null)
            {
                await _realmService.InitializeAsync();
            }
            return _realmService.Realm!.All<PaymentCategory>()
                .OrderBy(record => record.Name);
        }

        public async Task<double?> OutstandingHouseholdBillsForPeriod(PaymentPeriod paymentPeriod)
        {

            if (_realmService.Realm is null)
            {
                await _realmService.InitializeAsync();
            }

            var paymentCategory = _realmService.Realm!.All<PaymentCategory>()
                .FirstOrDefault(record => record.OriginalId == 1);
            var paymentTypes = _realmService.Realm.All<PaymentType>()
                .Where(record => record.PaymentCategoryId == paymentCategory && record.HavePaymentsEnded == false)
                .ToList();
            var paidPayments = _realmService.Realm.All<Payments>()
                .Where(record => record.StartDate >= paymentPeriod.DateFrom && record.EndDate <= paymentPeriod.DateTo)
                .ToList()
                .Where(record => record.PaymentTypeId != null && paymentTypes.Contains(record.PaymentTypeId))
                .ToList();
            var payments = paidPayments
                .Where(record => record.PaymentTypeId != null && paymentTypes.Contains(record.PaymentTypeId))
                .Select(record => record.PaymentTypeId)
                .Distinct();
            var currentOutstanding = paymentTypes.Except(payments)
                .Select(record => record!.DefaultPaymentAmount)
                .Sum();

            return currentOutstanding;
        }

    }
}
