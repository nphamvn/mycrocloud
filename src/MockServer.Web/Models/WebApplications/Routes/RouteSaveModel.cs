using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Routes
{
    [ModelBinder(BinderType = typeof(RouteModelBinder))]
    public class RouteSaveModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
        public string Path { get; set; }
        public List<string> Methods { get; set; }
        public int Order { get; set; }
        public Authorization Authorization { get; set; }
        public Validation Validations { get; set; }
    }
    public class Validation
    {
        public IList<QueryParameterValidationItem> QueryParams { get; set; }
        public IList<HeaderValidationItem> Headers { get; set; }
        public IList<BodyFieldValidationItem> Body { get; set; }
    }

    public class BodyFieldValidationItem
    {
        public string Name { get; set; }
        //[BindProperty(BinderType = typeof(RuleModelBinder))]
        public IList<Rule> Rules { get; set; }
    }

    //ref: http://www.prasannapattam.com/2016/12/aspnet-core-custom-frombody-model.html
    public class RouteModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;
        private readonly IHttpRequestStreamReaderFactory _readerFactory;

        public RouteModelBinderProvider(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            _formatters = formatters;
            _readerFactory = readerFactory;
        }
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.ModelType == typeof(RouteSaveModel))
            {
                return new RouteModelBinder(_formatters, _readerFactory);
            }
            return null;
        }
    }
    public class RouteModelBinder: IModelBinder
    {
        private BodyModelBinder defaultBinder;
        public RouteModelBinder(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
            //: base(formatters, readerFactory)
        {
            defaultBinder = new BodyModelBinder(formatters, readerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await defaultBinder.BindModelAsync(bindingContext);
            var xxx = (RouteSaveModel)bindingContext.Model;
            bindingContext.Result = ModelBindingResult.Success(xxx);
        }
    }

    public class HeaderValidationItem
    {
        public string Name { get; set; }
        //[BindProperty(BinderType = typeof(RuleModelBinder))]
        public IList<Rule> Rules { get; set; }
    }

    public class QueryParameterValidationItem
    {
        public string Name { get; set; }
        //[BindProperty(BinderType = typeof(RuleModelBinder))]
        public IList<Rule> Rules { get; set; }
    }

    public abstract class Rule
    {
        public abstract string Name { get; set; }
    }
    public class RequiredRule : Rule
    {
        public override string Name { get; set; } = "required";
    }
    public class Authorization
    {
        public AuthorizationType Type { get; set; }
        public List<int> Policies { get; set; }
        //public List<string> claims { get; set; }
    }
}