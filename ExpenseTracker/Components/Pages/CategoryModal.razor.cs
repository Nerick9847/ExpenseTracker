using Microsoft.AspNetCore.Components;

namespace ExpenseTracker.Components.Pages
{
    public partial class CategoryModal
    {
        [Parameter] public EventCallback<string> OnCategoryAdded { get; set; }

        private string newCategory;
        private bool isOpen = false;

        public void Open()
        {
            newCategory = string.Empty;
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        private void AddCategory()
        {
            OnCategoryAdded.InvokeAsync(newCategory);
            Close();
        }
    }
}