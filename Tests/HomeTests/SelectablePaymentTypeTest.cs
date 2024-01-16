using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Local;
using MongoDB.Bson;

namespace HomeTests
{
    [TestFixture]
    public class SelectablePaymentTypeTest
    {

        [Test()]
        public void IsNameOk()
        {
            var selectablePayment = new SelectablePaymentType()
            {
                Name = "Chris",
                Active = true,
                IsResourceRequired = true,
                PaymentAmount = 100,
                Id = ObjectId.GenerateNewId()
            };
            Assert.That(string.IsNullOrWhiteSpace(selectablePayment.Name), Is.False);
        }

    }
}
