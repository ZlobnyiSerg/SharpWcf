namespace SharpWcf.Configuration
{
    public class ServiceConfiguration
    {
        public string[] Types { get; set; }
        public string Behavior { get; set; }
        public ServiceEndpointConfiguration[] Endpoints { get; set; }       
        public string[] BaseAddresses { get; set; }
    }
}