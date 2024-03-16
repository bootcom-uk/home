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

        internal SelectablePaymentType? _selectablePaymentType;

        [SetUp] public void SetUp() 
        {
            _selectablePaymentType = new SelectablePaymentType();
        }

        [Test()]
        public void IsNameOk()
        {

            _selectablePaymentType!.Name = "Chris";
            _selectablePaymentType!.Active = true;
            _selectablePaymentType!.IsResourceRequired = true;
            _selectablePaymentType!.PaymentAmount = 100;
            _selectablePaymentType!.Id = ObjectId.GenerateNewId();

            Assert.That(string.IsNullOrWhiteSpace(_selectablePaymentType!.Name), Is.True);
        }

    }
}
