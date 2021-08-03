using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using spicy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spicy.TagHelpers
{
    [HtmlTargetElement("div",Attributes ="page-model")]
    public class PageLinkTagHelper:TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public String PageAction { get; set; }
        public bool PageClassesEnable { get; set; }
        public String PageClass { get; set; }
        public String PageClassNormal { get; set; }
        public String PageClassSelected { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.ToltalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                String url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                if(PageClassesEnable)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal); 
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
