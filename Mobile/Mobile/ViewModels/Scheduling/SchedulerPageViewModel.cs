using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.LocalModels;
using MongoDB.Bson;
using Services;
using Services.DataServices;
using Syncfusion.Maui.Scheduler;
using System.ComponentModel;

namespace Mobile.ViewModels.Scheduling
{
    public partial class SchedulerPageViewModel : ViewModelBase
    {
        internal PaymentsService _paymentsService { get; }

        public SchedulerPageViewModel(ISemanticScreenReader screenReader, INavigationService navigationService, PaymentTypeService paymentTypeService, UsersService usersService, RealmService realmService, PaymentsService paymentsService) : base(screenReader, navigationService, paymentTypeService, usersService, realmService, paymentsService)
        {
            _paymentsService = paymentsService;
        }

        [ObservableProperty]
        List<SchedulerEntry> dataSource;

        [ObservableProperty]
        object cachedEventArgs;

        [RelayCommand]
        async Task QueryAppointments(object eventArgs)
        {
            var e = eventArgs as Syncfusion.Maui.Scheduler.SchedulerQueryAppointmentsEventArgs;
            CachedEventArgs = eventArgs;
            var records =  await _paymentsService.GetPaymentsByDates(new DateTimeOffset(e.VisibleDates.Min()), new DateTimeOffset(e.VisibleDates.Max().AddDays(1).AddMicroseconds(-1)));
            DataSource = records.ToArray().Select(record =>
            {
               return new SchedulerEntry()
                {
                    DateAdded = record.DateAdded,
                    EndDate = record.EndDate,
                    IsDirectDebit = record.IsDirectDebit,
                    PaymentTypeDescription = record.PaymentTypeDescription,
                    StartDate = record.StartDate,
                    AddedBy = record.AddedBy,
                    AmountPaid = record.AmountPaid,
                    AmountReceived = record.AmountReceived,
                    AssociatedResource = record.AssociatedResource,
                    Id = record.Id,
                    Notes = record.Notes,
                    OriginalId = record.OriginalId,
                    PaymentTypeId = record.PaymentTypeId,
                    PaymentTypeName = record.PaymentTypeName,
                    TotalHours = record.TotalHours
                };
            }).ToList();
        }

        [ObservableProperty]
        bool isModifyPaymentScreenOpen;

        [ObservableProperty]
        ObjectId? modifyPaymentId;

        [ObservableProperty]
        string modifyPaymentConfirmation;

        [ObservableProperty]
        string modifyPaymentScreenTitle;

        private void ModifyAppointment(SchedulerEntry schedulerEntry)
        {
            ModifyPaymentId = schedulerEntry.Id;
            ModifyPaymentConfirmation = $"Are you sure you want to delete the {(schedulerEntry.AmountPaid > 0 ? "payment" : "receipt")} for {schedulerEntry.PaymentTypeName} {(schedulerEntry.PaymentTypeDescription == schedulerEntry.PaymentTypeName ? "" : "(" + schedulerEntry.PaymentTypeDescription + ")")} of a total of {(schedulerEntry.AmountPaid > 0 ? schedulerEntry.AmountPaid : schedulerEntry.AmountReceived):c2}?";
            ModifyPaymentScreenTitle = "Delete?";

            IsModifyPaymentScreenOpen = true;
        }

        [RelayCommand]
        void SchedulerDoubleTapped(SchedulerDoubleTappedEventArgs eventArgs)
        {            
            switch (eventArgs.Element)
            {
                case SchedulerElement.Appointment:
                    var appointment = eventArgs.Appointments.First() as SchedulerEntry;
                    ModifyAppointment(appointment);
                    break;
                case SchedulerElement.SchedulerCell:
                    if(eventArgs.Appointments?.Count() == 0)
                    {
                        DisplayNewPaymentCommand.Execute(eventArgs.Date);
                        return;
                    }
                    // eventArgs.Date
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
