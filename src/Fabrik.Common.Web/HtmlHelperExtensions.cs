﻿using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Linq;

namespace Fabrik.Common.Web
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Returns a description for this model property
        /// </summary>
        public static IHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            Ensure.Argument.NotNull(html, "html");
            Ensure.Argument.NotNull(expression, "expression");
            
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            return new HtmlString(metadata.Description);
        }

        /// <summary>
        /// A helper for performing conditional IF logic using Razor
        /// </summary>
        public static MvcHtmlString If(this HtmlHelper html, bool condition, string trueString)
        {
            Ensure.Argument.NotNull(html, "html");
            return html.IfElse(condition, h => MvcHtmlString.Create(trueString), h => MvcHtmlString.Empty);
        }

        /// <summary>
        /// A helper for performing conditional IF,ELSE logic using Razor
        /// </summary>
        public static MvcHtmlString IfElse(this HtmlHelper html, bool condition, string trueString, string falseString)
        {
            Ensure.Argument.NotNull(html, "html");
            return html.IfElse(condition, h => MvcHtmlString.Create(trueString), h => MvcHtmlString.Create(falseString));
        }

        /// <summary>
        /// A helper for performing conditional IF logic using Razor
        /// </summary>
        public static MvcHtmlString If(this HtmlHelper html, bool condition, Func<HtmlHelper, MvcHtmlString> action)
        {
            Ensure.Argument.NotNull(html, "html");
            Ensure.Argument.NotNull(action, "action");
            return html.IfElse(condition, action, h => MvcHtmlString.Empty);
        }

        /// <summary>
        /// A helper for performing conditional IF,ELSE logic using Razor
        /// </summary>
        public static MvcHtmlString IfElse(this HtmlHelper html, bool condition, Func<HtmlHelper, MvcHtmlString> trueAction, Func<HtmlHelper, MvcHtmlString> falseAction)
        {
            Ensure.Argument.NotNull(html, "html");
            Ensure.Argument.NotNull(trueAction, "trueAction");
            Ensure.Argument.NotNull(falseAction, "falseAction"); 
            return (condition ? trueAction : falseAction).Invoke(html);
        }

        /// <summary>
        /// Returns an image element for the specified <paramref name="src"/>
        /// </summary>
        public static MvcHtmlString Image(this HtmlHelper html, string src, string alt = "", object htmlAttributes = null)
        {
            Ensure.Argument.NotNull(html, "html");
            Ensure.Argument.NotNullOrEmpty(src, "src");

            if (src.StartsWith("~"))
                src = VirtualPathUtility.ToAbsolute(src);
            
            var tb = new TagBuilder("img");
            tb.MergeAttribute("src", src);
            tb.MergeAttribute("alt", alt);
            tb.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Creates a dropdown list for an Enum property
        /// </summary>
        /// <exception cref="System.ArgumentException">If the property type is not a valid Enum</exception>
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression)
        {
            Ensure.Argument.Is(typeof(TEnum).IsEnum, "Must be a valid Enum type.");
            
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var values = from v in Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                         select new {
                            Text = v.ToString().SeparatePascalCase(),
                            Value = v.ToString()
                         };

            var selectList = new SelectList(values, "Value", "Text");
            return html.DropDownListFor(expression, selectList);
        }
    }
}
