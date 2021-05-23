using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LoreSoft.Blazor.Controls
{
    public class DataSizer : ComponentBase, IDisposable
    {
        [CascadingParameter(Name = "PagerState")]
        protected DataPagerState PagerState { get; set; } = new();

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter]
        public int[] PageSizeOptions { get; set; } = { 10, 25, 50, 100 };

        [Parameter]
        public bool IncludeAllOption { get; set; }

        [Parameter]
        public string DescriptionLabel { get; set; } = "Items per page";

        [Parameter]
        public string ContainerClass { get; set; }

        [Parameter]
        public string SelectorClass { get; set; }

        public void Dispose()
        {
            PagerState.PropertyChanged -= OnStatePropertyChange;
        }
                
        protected override void OnInitialized()
        {
            ContainerClass ??= "col-sm-12 col-md-5 d-flex align-items-center justify-content-center justify-content-md-start";
            SelectorClass ??= "form-select form-select-sm form-select-solid";

            if (PagerState == null)
                throw new InvalidOperationException("DataSizer requires a cascading parameter PagerState.");

            if (!PageSizeOptions.Contains(PagerState.PageSize))
                PageSizeOptions = PageSizeOptions.Append(PagerState.PageSize).OrderBy(p => p).ToArray();

            PagerState.PropertyChanged += OnStatePropertyChange;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", ContainerClass);
            builder.AddMultipleAttributes(2, Attributes);
            
            builder.OpenElement(3, "div");

            builder.OpenElement(4, "select");
            builder.AddAttribute(5, "class", SelectorClass);
            builder.AddAttribute(6, "value", PagerState.PageSize);
            builder.AddAttribute(7, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e => SetPageSize(Convert.ToInt32(e.Value ?? 0))));

            foreach (var pageSizeOption in PageSizeOptions)
            {
                builder.OpenElement(8, "option");
                builder.AddAttribute(9, "value", pageSizeOption);
                builder.SetKey(pageSizeOption);
                builder.AddContent(10, pageSizeOption);
                builder.CloseElement(); // option
            }

            if (IncludeAllOption)
            {
                builder.OpenElement(11, "option");
                builder.AddAttribute(12, "value", 0);
                builder.AddContent(13, "All");
                builder.CloseElement(); // option
            }

            builder.CloseElement(); // select

            if (!string.IsNullOrEmpty(DescriptionLabel))
            {
                builder.OpenElement(14, "span");
                builder.AddContent(15, DescriptionLabel);
                builder.CloseElement(); // span
            }

            builder.CloseElement(); // div
            builder.CloseElement(); // div
        }


        private void SetPageSize(int pageSize)
        {
            PagerState.PageSize = pageSize;
        }

        private void OnStatePropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(DataPagerState.PageSize))
                return;

            InvokeAsync(StateHasChanged);
        }

    }
}
