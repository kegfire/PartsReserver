using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartsReserver;
using HttpMock;

namespace Tests
{
	[TestClass]
	public class HttpClientTest
	{
		[TestMethod]
		public void Logon()
		{
			var _stubHttp = HttpMockRepository.At("http://localhost:9191");

			var expected = 
			_stubHttp.Stub(x => x.Get("/endpoint"))
				.Return(expected)
				.OK();

			// No network connection required
			Console.Write(json); // {'name' : 'Test McGee'}
		}
	}
}
