namespace SharpWcf.Configuration
{
    public class ClientConfiguration
    {
        public string[] Types { get; set; }
        public string Address { get; set; }
        public string Behavior { get; set; }
        public string Binding { get; set; }
        public string BindingConfiguration { get; set; }

        public string DnsIdentity { get; set; }
    }
}