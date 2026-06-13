using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEvent.Shared.Contracts.Extensions
{
    public static class MassTransitExtensions
    {
        public static IBusRegistrationConfigurator ConfigureMassTransit(
            this IBusRegistrationConfigurator configurator)
        {
            // Protobuf serialization using
            configurator.SetKebabCaseEndpointNameFormatter();

            return configurator;
        }
    }
}
