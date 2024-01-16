using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Local;

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
                Name = "Chris"
            };
            Assert.IsFalse(string.IsNullOrWhiteSpace(selectablePayment.Name));
        }

    }
}
