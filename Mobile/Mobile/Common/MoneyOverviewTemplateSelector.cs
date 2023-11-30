namespace Mobile.Common
{
    public class MoneyOverviewTemplateSelector : DataTemplateSelector
    {

        public DataTemplate BudgetOverviewDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var itemName = item as string;

            switch (itemName)
            {
                case "BudgetOverview":
                    return BudgetOverviewDataTemplate;
                default:
                    return BudgetOverviewDataTemplate;
            }

        }
    }
}
