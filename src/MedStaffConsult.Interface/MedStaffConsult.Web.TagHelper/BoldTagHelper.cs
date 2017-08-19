using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MedStaffConsult.Web.TagHelper
{
    [HtmlTargetElement(Attributes = "bold")]
    public class BoldTagHelper : Microsoft.AspNetCore.Razor.TagHelpers.TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("bold");
            output.PreContent.SetHtmlContent("<strong>");
            output.PostContent.SetHtmlContent("</strong>");
        }
    }
}
