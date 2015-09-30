using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using DynamicPowerShellApi.Configuration;

namespace DynamicPowerShellApi
{
	/// <summary>	Attribute for authorize HTTP request if enabled. </summary>
	/// <remarks>	Anthony, 5/28/2015. </remarks>
	/// <seealso cref="T:System.Web.Http.AuthorizeAttribute"/>
	public class AuthorizeIfEnabledAttribute
		: AuthorizeAttribute
	{
		/// <summary>	Calls when an action is being authorized. </summary>
		/// <remarks>	Anthony, 5/28/2015. </remarks>
		/// <param name="actionContext">	The context. </param>
		/// <seealso cref="M:System.Web.Http.AuthorizeAttribute.OnAuthorization(HttpActionContext)"/>
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			// Skip authentication if it's disabled in the config file.
			if (!WebApiConfiguration.Instance.Authentication.Enabled)
			{
				return;
			}

			if (!IsAuthorized(actionContext))
			{
				HandleUnauthorizedRequest(actionContext);
			}
		}
	}
}
