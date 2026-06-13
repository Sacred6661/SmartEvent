using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEvent.Shared.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute(string exchangeName, string routingKey) : Attribute
    {
        public string ExchangeName { get; } = exchangeName;
        public string RoutingKey { get; } = routingKey;
    }
}
