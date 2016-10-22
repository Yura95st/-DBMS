namespace App.Helpers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public static class HtmlExtensions
    {
        public static MvcHtmlString LiActionLink(this HtmlHelper html, string text, string action, string controller,
                                                 object routeValues = null, object htmlAttributes = null)
        {
            ViewContext context = html.ViewContext;
            if (context.Controller.ControllerContext.IsChildAction)
            {
                context = html.ViewContext.ParentActionViewContext;
            }

            RouteValueDictionary currentRouteValues = context.RouteData.Values;

            string currentAction = currentRouteValues["action"].ToString();
            string currentController = currentRouteValues["controller"].ToString();

            string str =
                $"<li role=\"presentation\"{(currentAction.Equals(action, StringComparison.InvariantCulture) && currentController.Equals(controller, StringComparison.InvariantCulture) ? " class=\"active\"" : string.Empty)}>{html.ActionLink(text, action, controller, routeValues, htmlAttributes) .ToHtmlString()}</li>";

            return new MvcHtmlString(str);
        }
    }
}