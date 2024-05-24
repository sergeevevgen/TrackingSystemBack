using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestApi
{
    [CollectionDefinition("ApiTest")]
    public class ApiTestCollection : ICollectionFixture<ApiTestFixture>
    {
    }
}
