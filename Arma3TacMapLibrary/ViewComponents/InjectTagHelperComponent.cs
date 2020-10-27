using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace Arma3TacMapLibrary.ViewComponents
{
    internal class InjectTagHelperComponent : TagHelperComponent
	{
        private readonly IEnumerable<IHtmlContent> _content;
        private readonly string _targetTag;
        public InjectTagHelperComponent(string targetTag, params IHtmlContent[] content)
        {
            this._content = content;
            this._targetTag = targetTag;
        }
        public InjectTagHelperComponent(string targetTag, IEnumerable<IHtmlContent> content)
        {
            this._content = content;
            this._targetTag = targetTag;
        }

        public override int Order => 1;

        public override Task ProcessAsync(TagHelperContext context,
                                          TagHelperOutput output)
        {
            if (string.Equals(context.TagName, _targetTag,
                              StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in _content)
                {
                    output.PostContent.AppendHtml(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}