using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using PayMedia.Integration.FrameworkService.Interfaces.Common;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    /// <summary>
    /// Utilities for dealing with PayMedia ApplicationServices.
    /// </summary>
    public static class ServiceUtilities
    {
        public static T GetService<T>(IComponentInitContext context)
        {
            if (context != null && context.Services != null && context.Services.Asm != null)
            {
                return context.Services.Asm.GetServiceProxyWithDecorator<T>();
            }
            return default(T);
        }

        /// <summary>
		/// Creates a new ServiceHost and adds the MetadataExchange behavior at the URL "mex" relative to endpoint's address.
		/// </summary>
		/// <param name="endpoint">Contains the settings needed to instantiate the ServiceHost.  An entry for the endpoint's BindingConfiguration must exist in the application configuration file.</param>
		/// <param name="singletonInstance">The instance of the object that implements the endpoint's Contract interface.</param>
		/// <param name="baseAddresses">BaseAddresses supplied to the ServiceHost constructor.</param>
		/// <returns></returns>
		public static ServiceHost CreateServiceHost(WcfEndpoint endpoint, object singletonInstance, params Uri[] baseAddresses)
        {
            return CreateServiceHost(endpoint, singletonInstance, new Uri(new Uri(endpoint.Address), "mex"), baseAddresses);
        }

        /// <summary>
        /// Creates a new ServiceHost.
        /// </summary>
        /// <param name="endpoint">Contains the settings needed to instantiate the ServiceHost.  An entry for the endpoint's BindingConfiguration must exist in the application configuration file.</param>
        /// <param name="singletonInstance">The instance of the object that implements the endpoint's Contract interface.</param>
        /// <param name="metadataExchangeUri">If a MetadataExchange URL is required (for generating proxies) then specify this URI, otherwise supply null.</param>
        /// <param name="baseAddresses">BaseAddresses supplied to the ServiceHost constructor.</param>
        /// <returns></returns>
        public static ServiceHost CreateServiceHost(WcfEndpoint endpoint, object singletonInstance, Uri metadataExchangeUri, params Uri[] baseAddresses)
        {
            // Instantiate the binding.
            Binding binding = CreateBinding(endpoint);

            // Start the service host.
            ServiceHost serviceHost = new ServiceHost(singletonInstance, baseAddresses);
            serviceHost.AddServiceEndpoint(endpoint.Contract, binding, endpoint.Address);

            // Add the metadata exchange behavior.
            ServiceMetadataBehavior mexBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (mexBehavior == null && metadataExchangeUri != null)
            {
                mexBehavior = new ServiceMetadataBehavior();
                mexBehavior.HttpGetEnabled = true;
                mexBehavior.HttpGetUrl = metadataExchangeUri;
                serviceHost.Description.Behaviors.Add(mexBehavior);
            }

            // Add the debug behavior.
            ServiceDebugBehavior debugBehavior = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehavior == null)
            {
                debugBehavior = new ServiceDebugBehavior();
                debugBehavior.IncludeExceptionDetailInFaults = true;
                serviceHost.Description.Behaviors.Add(debugBehavior);
            }

            // Add the intercept behavior
            WCFInterceptBehavior interceptBehavior = serviceHost.Description.Behaviors.Find<WCFInterceptBehavior>();
            if (interceptBehavior == null)
            {
                interceptBehavior = new WCFInterceptBehavior();
                serviceHost.Description.Behaviors.Add(interceptBehavior);
            }

            return serviceHost;
        }

        /// <summary>
        /// Create a WCF "Binding" instance of the type described in the "endpoint"
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns>An initialized Binding suitable for use with a WCF ServiceHost</returns>
        public static Binding CreateBinding(WcfEndpoint endpoint)
        {
            /*
             * If you are using the CreateServiceHost method, which uses this CreateBinding method
             * then you MUST have a section in your application configuration file with the below
             * bindings.  If you alredy have a bindings section then verify that the below entries 
             * are in your binding section.
             * 
             * The IC supports three binding types "BasicHttpBindings", "WSHttpBinding" and "CustomBinding".
             * Within each binding type there are 1 or more named binding configurations.  Currently
             * the IC (excluding Telenor) uses "basicHttpBinding" with the name "nonCertBinding" for all 
             * but one WCF interface.  The "customBinding" named "PlainXml" is handy for generic WCF 
             * interface that does NOT expose a specific interface and will accpet pretty much anything sent.
             * Using "PlainXml" it is up to the service instance class to determine if the incoming data
             * is any good.
             * 
             * Even though this method supports the binding type "WSHttpBinding" I haven't seen 
             * any entries in any of the client projects that have a binding section of that type, 
             * so I don't have a sample to put here.  If you are reading this because you are using
             * "WSHttpBinding" and tracked your problem down to this section, then you probably
             * need to create such a binding section now.   Please add a copy of your "WSHttpBinding"
             * section to this comment when you are done.
             * 
             * 
             * 
             * note: If your application file has different values for some of the setting then it
             *       is likely that someone changed them for the specific client (with any luck they 
             *       left a comment in the config file stating why the changed the value)
             * 
             *   <bindings>
             *     <basicHttpBinding>
             *       <binding name="myBinding"
             *              hostNameComparisonMode="StrongWildcard"
             *              receiveTimeout="00:01:00"
             *              sendTimeout="00:01:00"
             *              openTimeout="00:01:00"
             *              closeTimeout="00:01:00"
             *              maxBufferSize="6535600"
             *              maxBufferPoolSize="524288"
             *              maxReceivedMessageSize="6535600"
             *              transferMode="Buffered"
             *              messageEncoding="Text"
             *              textEncoding="utf-8"
             *              bypassProxyOnLocal="false"
             *              useDefaultWebProxy="true">
             *         <!-- FORNOW - Uncomment once all developers have installed certs.
             *                   <security mode="Transport">
             *                       <transport clientCredentialType="Certificate" />
             *                   </security>
             *                   -->
             *       </binding>
             *       <binding name="nonCertBinding"
             *              hostNameComparisonMode="StrongWildcard"
             *              receiveTimeout="00:01:00"
             *              sendTimeout="00:01:00"
             *              openTimeout="00:01:00"
             *              closeTimeout="00:01:00"
             *              maxBufferSize="6535600"
             *              maxBufferPoolSize="524288"
             *              maxReceivedMessageSize="6535600"
             *              transferMode="Buffered"
             *              messageEncoding="Text"
             *              textEncoding="utf-8"
             *              bypassProxyOnLocal="false"
             *              useDefaultWebProxy="true">
             *         <readerQuotas maxDepth="2147483647" maxStringContentLength="2147483647" maxBytesPerRead="2147483647" maxNameTableCharCount="2147483647" />
             *       </binding>
             *     </basicHttpBinding>
             *     <customBinding>
             *         <binding name="PlainXml">
             *           <textMessageEncoding messageVersion="None" />
             *           <httpTransport />
             *         </binding>
             *     </customBinding>
             *   </bindings>
             * 
             */

            // Instantiate the binding.
            Binding binding = null;
            if (endpoint.Binding.ToLower() == typeof(BasicHttpBinding).Name.ToLower())
            {
                if (!string.IsNullOrEmpty(endpoint.BindingConfiguration))
                    binding = new BasicHttpBinding(endpoint.BindingConfiguration);
                else
                    binding = new BasicHttpBinding();
            }
            else if (endpoint.Binding.ToLower() == typeof(WSHttpBinding).Name.ToLower())
            {
                if (!string.IsNullOrEmpty(endpoint.BindingConfiguration))
                    binding = new WSHttpBinding(endpoint.BindingConfiguration);
                else
                    binding = new WSHttpBinding();
            }
            else if (endpoint.Binding.ToLower() == typeof(CustomBinding).Name.ToLower())
            {
                if (!string.IsNullOrEmpty(endpoint.BindingConfiguration))
                    binding = new CustomBinding(endpoint.BindingConfiguration);
                else
                    binding = new CustomBinding();
            }
            else
                throw new IntegrationException(string.Format("The binding, {0}, configured for {1} {2}, is not supported.\r\nThe supported bindings are {3}, {4} and {5}.\r\nPlease verify that the desired WCF Binding is present in your application config file.", endpoint.Binding, typeof(WcfEndpoint).Name, endpoint.Name, typeof(BasicHttpBinding).Name, typeof(WSHttpBinding).Name, typeof(CustomBinding).Name));

            return binding;
        }

        public class WCFInterceptBehavior : IServiceBehavior
        {
            /// <summary>
            /// Provides the ability to pass custom data to binding elements 
            /// to support the contract implementation.
            /// </summary>
            /// <param name="serviceDescription">The service description of the service.</param>
            /// <param name="serviceHostBase">The host of the service.</param>
            /// <param name="endpoints">The service endpoints.</param>
            /// <param name="bindingParameters">Custom objects to which binding elements have access.
            /// </param>
            public void AddBindingParameters(ServiceDescription serviceDescription,
                System.ServiceModel.ServiceHostBase serviceHostBase,
                System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,
                System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {
                return;
            }

            /// <summary>
            /// Provides the ability to change run-time property values or 
            /// insert custom extension objects such as error handlers, 
            /// message or parameter interceptors, 
            /// security extensions, and other custom extension objects.
            /// </summary>
            /// <param name="serviceDescription">The service description.</param>
            /// <param name="serviceHostBase">The host that is currently being built.</param>
            public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
                System.ServiceModel.ServiceHostBase serviceHostBase)
            {
                foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
                {
                    dispatcher.Endpoints
                        .ToList()
                        .ForEach(x => x.DispatchRuntime.MessageInspectors.Add(new WCFInterceptor()));

                }
                //this.serviceHost = serviceHostBase;
            }

            /// <summary>
            /// Provides the ability to inspect the service host and 
            /// the service description to confirm that the service 
            /// can run successfully.
            /// </summary>
            /// <param name="serviceDescription">The service description.</param>
            /// <param name="serviceHostBase">The service host that is currently being constructed.
            /// </param>
            public void Validate(ServiceDescription serviceDescription,
                System.ServiceModel.ServiceHostBase serviceHostBase)
            {
                return;
            }
        }

    }
}
