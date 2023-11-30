using Models;

namespace Mobile.LocalModels
{
    public class SchedulerEntry : Payments
    {

        public Brush PaymentColour
        {
            get {
                return new SolidColorBrush(Color.FromInt(PaymentTypeId.Colour));
            }
        }

    }
}
