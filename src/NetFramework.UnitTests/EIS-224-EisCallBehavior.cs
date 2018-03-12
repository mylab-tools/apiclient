using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DotApiClient;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public class EisCallBehavior
    {
		[RestApi]
		interface IEisApi
		{
			[RestAction(HttpMethod.Get)]
			[ContentType(ContentType.Json)]
			AuthResult Authorize(string auth);
		}

		public partial class AuthResult
		{
			/// <summary>
			/// Федеральный номер нотариуса
			/// </summary>
			public string notary_id { get; set; }
			/// <summary>
			/// ФИО нотариуса
			/// </summary>
			public string notary_name { get; set; }
			/// <summary>
			/// Федеральный номер ВрИО нотариуса
			/// </summary>
			public string assistant_id { get; set; }
			/// <summary>
			/// ФИО ВрИО нотариуса
			/// </summary>
			public string assistant_name { get; set; }
			/// <summary>
			/// ФИО сотрудника нотариуса
			/// </summary>
			public string staff_name { get; set; }
			/// <summary>
			/// Должность сотрудника нотариуса
			/// </summary>
			public string staff_title { get; set; }
			/// <summary>
			/// Найденный сертификат PKCS#7
			/// </summary>
			public string user_certificate { get; set; }
			
			public AssistantShort[] assistant_list { get; set; }
		}
		public partial class AssistantShort
		{
			/// <summary>
			/// Федеральный номер
			/// </summary>
			public string id { get; set; }
			/// <summary>
			/// ФИО
			/// </summary>
			public string name { get; set; }
		}

		[Test]
        public void ShouldNotGetHttpStatusCodeMovedPermanentlyFromEis()
		{
			// Arrange
			// TODO EIS-224 не получается вызвать http://service.eisnot.ru/eis_users?auth=120026661d0d3ad3883ac60a8700000026661d
			var factory = new WebApiClientFactory(@"http://service.eisnot.ru/eis_users");
			var client = factory.CreateProxy<IEisApi>();

			// Act
			var msg = client.Authorize("120026661d0d3ad3883ac60a8700000026661d");                

            // Assert
        }
    }
}
