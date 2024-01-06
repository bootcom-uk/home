using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using MongoDB.Bson;
using Services;
using Services.DataServices;
using Syncfusion.Maui.Scheduler;
using System.ComponentModel;

namespace Mobile.ViewModels.Scheduling
{
    public partial class SchedulerPageViewModel : ViewModelBase
    {

        public SchedulerPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            ModifyPaymentDataSource = new();
        }

        [ObservableProperty]
        List<SchedulerAppointment> dataSource;

        [ObservableProperty]
        object cachedEventArgs;

        [RelayCommand]
        async Task QueryAppointments(object eventArgs)
        {
            var e = eventArgs as Syncfusion.Maui.Scheduler.SchedulerQueryAppointmentsEventArgs;
            CachedEventArgs = eventArgs;
            var records =  await _paymentsService.GetPaymentsByDates(new DateTimeOffset(e.VisibleDates.Min()), new DateTimeOffset(e.VisibleDates.Max().AddDays(1).AddMicroseconds(-1)));

            DataSource = new();

            var outgoingPayments = records.Where(record => record.AmountPaid > 0).ToList();
            var incomingReceipts = records.Where(record => record.AmountReceived > 0).ToList();

            if (outgoingPayments.Count() > 0)
            {
                foreach (var paymentDate in outgoingPayments.Select(record => record.StartDate).Distinct())
                {
                    var outgoingSumForDay = outgoingPayments
                        .Where(record => record.StartDate == paymentDate)
                        .Sum(record => record.AmountPaid);
                    DataSource.Add(new SchedulerAppointment()
                    {
                        IsAllDay = true,
                        Background = SolidColorBrush.Red,
                        Id = null,
                        StartTime = paymentDate.Value.LocalDateTime.Date,
                        EndTime = paymentDate.Value.LocalDateTime.Date.AddDays(1).AddMicroseconds(-1),
                        Subject = $"{outgoingSumForDay:c2}"
                    });
                }
            }

            if (incomingReceipts.Count() > 0)
            {
                foreach (var paymentDate in incomingReceipts.Select(record => record.StartDate).Distinct())
                {
                    var receiptSumForDay = incomingReceipts
                        .Where(record => record.StartDate == paymentDate)
                        .Sum(record => record.AmountReceived);
                    DataSource.Add(new SchedulerAppointment()
                    {
                        IsAllDay = true,
                        Background = SolidColorBrush.Green,
                        Id = null,
                        StartTime = paymentDate.Value.LocalDateTime.Date,
                        EndTime = paymentDate.Value.LocalDateTime.Date.AddDays(1).AddMicroseconds(-1),
                        Subject = $"{receiptSumForDay:c2}"
                    });
                }
            }

        }

        [ObservableProperty]
        bool isDailyTransactionScreenOpen;

        [ObservableProperty]
        IQueryable<Payments> dailyTransactionsDataSource;

        [ObservableProperty]
        string dailyTransactionScreenTitle;

        enum TransactionTypesToView
        {
            ALL,
            INCOMING,
            OUTGOING
        }

        private SchedulerAppointment selectedSchedulerAppointment;

        private TransactionTypesToView selectedTransactionTypesToView;

        private async Task ViewDailyTransactions(SchedulerAppointment schedulerEntry, TransactionTypesToView entryTypesToView)
        {
            var payments = await _paymentsService.GetPaymentsByDates(new DateTimeOffset(schedulerEntry.StartTime), new DateTimeOffset(schedulerEntry.EndTime));

            selectedSchedulerAppointment = schedulerEntry;
            selectedTransactionTypesToView = entryTypesToView;

            switch (entryTypesToView)
            {
                case TransactionTypesToView.INCOMING :
                    payments = payments.Where(record => record.AmountReceived > 0);                        
                    break;
                case TransactionTypesToView.OUTGOING:
                    payments = payments.Where(record => record.AmountPaid > 0);
                    break;
            }

            DailyTransactionsDataSource = payments.OrderBy(record => record.AmountPaid)
                .OrderBy(record => record.AmountReceived);

            IsDailyTransactionScreenOpen = true;
        }

        [RelayCommand]
        async Task DeleteDailyTransaction(ObjectId paymentId)
        {
            await _paymentsService.DeletePayment(DailyTransactionsDataSource.First(record => record.Id == paymentId));
            await ViewDailyTransactions(selectedSchedulerAppointment, selectedTransactionTypesToView);
            await QueryAppointments(CachedEventArgs);

            // Close the popup when all transactions for that day have been removed
            IsDailyTransactionScreenOpen = (DailyTransactionsDataSource.Count() > 0);
            
        }

        [RelayCommand]
        async Task UpdatePayment()
        {

            if (ModifyPaymentDataSource.Id is null) return;

            // Load in the payment
            var payment = await _paymentsService.GetPaymentById(ModifyPaymentDataSource.Id.Value);
            if (payment is null)
            {
                return;
            }

            // Create a new payments object - if we use the object we just pulled
            // we'll have an exception thrown
            var savePayment = new Payments();            
            foreach(var prpInfo in payment.GetType().GetProperties())
            {
                if(!prpInfo.CanWrite) continue;
                savePayment.GetType().GetProperty(prpInfo.Name).SetValue(savePayment, prpInfo.GetValue(payment));
            }

            savePayment.StartDate = new(ModifyPaymentDataSource.AppointmentDateTime);
            savePayment.EndDate = new(ModifyPaymentDataSource.AppointmentDateTime.AddDays(1).AddMicroseconds(-1));
            savePayment.PaymentTypeDescription = ModifyPaymentDataSource.PaymentInformation;
            if(payment.AmountPaid > 0)
            {
                savePayment.AmountPaid = ModifyPaymentDataSource.PaymentAmount;
            }
            if (payment.AmountReceived > 0)
            {
                savePayment.AmountReceived = ModifyPaymentDataSource.PaymentAmount;
            }

            await _paymentsService.UpdatePayment(savePayment);

            await QueryAppointments(CachedEventArgs);

            IsModifyTransactionScreenOpen = false;
        }

        [RelayCommand]
        async Task EditDailyTransaction(ObjectId paymentId)
        {
            IsDailyTransactionScreenOpen = false;
            // Load in the payment
            var payment = await _paymentsService.GetPaymentById(paymentId);
            if(payment is null)
            {
                return;
            }

            ModifyPaymentDataSource.AppointmentDateTime = payment.LocalStartDate.ToLocalTime();
            ModifyPaymentDataSource.Id = paymentId;
            ModifyPaymentDataSource.PaymentInformation = payment.PaymentTypeDescription;
            ModifyPaymentDataSource.PaymentType = payment.PaymentTypeId?.Name;
            ModifyPaymentDataSource.PaymentAmount = payment.AmountPaid ?? 0;
            if (payment.AmountReceived != null && payment.AmountReceived! > 0)
            {
                ModifyPaymentDataSource.PaymentAmount = payment.AmountReceived.Value;
            }
            
            IsModifyTransactionScreenOpen = true;
        }

        [RelayCommand]
        async Task SchedulerDoubleTapped(SchedulerDoubleTappedEventArgs eventArgs)
        {
            var appointment = eventArgs.Appointments.First() as SchedulerAppointment;

            DailyTransactionScreenTitle = appointment.StartTime.ToLongDateString();

            switch (eventArgs.Element)
            {
                case SchedulerElement.Appointment:                                     
                    await ViewDailyTransactions(appointment, appointment.Background == SolidColorBrush.Red ? TransactionTypesToView.OUTGOING : TransactionTypesToView.INCOMING);
                    break;
                case SchedulerElement.SchedulerCell:
                    if(eventArgs.Appointments?.Count() == 0)
                    {
                        DisplayNewPaymentCommand.Execute(eventArgs.Date);
                        return;
                    }
                    await ViewDailyTransactions(appointment, TransactionTypesToView.ALL);
                    break;

            }

        }

        protected async override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsPaymentScreenOpen):
                    if (!IsPaymentScreenOpen)
                    {
                        await QueryAppointmentsCommand.ExecuteAsync(CachedEventArgs);
                    }
                    break;
            }

            base.OnPropertyChanged(e);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
    }
}
