using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace B2Fleet.Web.Helper
{
    public static class HtmlHelperFormExtensions
    {
        public static string BootstrapModal(this IHtmlHelper helper, string modalId, string modalTite, string modalBodyId)
        {
            StringBuilder sb = new StringBuilder();
            var openContent = $@"<div id='{modalId}'  data-backdrop='false' class='modal fade' role='dialog'>
                <div class='modal-dialog'>

                <!-- Modal content-->
                <div class='modal-content'>";
            var modalHeader = $@"<div class='modal-header'>
                <button type = 'button' class='close' data-dismiss='modal'>&times;</button>
                <h4 class='modal-title'>{modalTite}</h4>
                </div>";
            //here where the content will be loaded
            var modalBody = $@" <div class='modal-body'>
                <div id='{modalBodyId}'> </div>
     
                </div>"

                ;
            var modalFooter = $@"<div class='modal-footer'>
                                <button type = 'button' onClick='save()' class='btn btn-info' >Enregistrer</button>
                              <button type = 'button' class='btn btn-default' data-dismiss='modal'>Fermer</button>
                             
                </div>";
            var closeContent = @"</div></div></div>";
            sb.Append(openContent);
            sb.Append(modalHeader);
            sb.Append(modalBody);
            sb.Append(modalFooter);
            sb.Append(closeContent);
            
            return sb.ToString();
        }
        public  static string GroupedDropdownList(this IHtmlHelper helper, string name, IEnumerable<SelectListItem> list, object htmlAttributes)
        {
            //Creating a select element using TagBuilder class which will create a dropdown.
            TagBuilder dropdown = new TagBuilder("select");
            //Setting the name and id attribute with name parameter passed to this method.
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            //Created StringBuilder object to store option data fetched oen by one from list.
            StringBuilder options = new StringBuilder();
            options = options.Append("<option value='" + "" + "'>" + "" + "</option>");
            //  StringBuilder groupOptions = new StringBuilder();
            foreach (var item in list.GroupBy(x => x.Group))
            {
                options.Append("<optgroup label='" + item.Key.Name + "'>");
                foreach (var selectListItem in item)
                {
                    options = options.Append("<option value='" + selectListItem.Value + "'>" + selectListItem.Text + "</option>");
                }
                options.Append("</optgroup>");

            }  //assigned all the options to the dropdown using innerHTML property.
            dropdown.InnerHtml.Append( options.ToString());
            //Assigning the attributes passed as a htmlAttributes object.
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            //Returning the entire select or dropdown control in HTMLString format.
            return dropdown.ToString();
        }
        public static string DropdownList(this IHtmlHelper helper, string name, IEnumerable<SelectListItem> list, object htmlAttributes)
        {
            //Creating a select element using TagBuilder class which will create a dropdown.
            TagBuilder dropdown = new TagBuilder("select");
            //Setting the name and id attribute with name parameter passed to this method.
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            //Created StringBuilder object to store option data fetched oen by one from list.
            StringBuilder options = new StringBuilder();
            options = options.Append("<option value='" + "" + "'>" + "" + "</option>");
            //Iterated over the IEnumerable list.
            options = list.Aggregate(options, (current, item) => current.AppendFormat(!item.Selected ? "<option value='{0}' >{1}</option>" : "<option value='{0}' selected='{0}' >{1}</option>", item.Value, item.Text));
            //assigned all the options to the dropdown using innerHTML property.
            dropdown.InnerHtml .Append( options.ToString());
            //Assigning the attributes passed as a htmlAttributes object.
            dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            //Returning the entire select or dropdown control in HTMLString format.
            return dropdown.ToString();
        }
    }
  



}