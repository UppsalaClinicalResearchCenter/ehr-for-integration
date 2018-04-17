using Newtonsoft.Json;

namespace TestKlientSITHS
{
    public class RedirectModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("organizationShortName")]
        public string OrganizationShortName { get; set; }

        [JsonProperty("roleShortName")]
        public string RoleShortName { get; set; }

        [JsonProperty("positionShortName")]
        public string PositionShortName { get; set; }

        [JsonProperty("registerShortName")]
        public string RegisterShortName { get; set; }
    }

    public class PatientRedirectModel : RedirectModel
    {
        [JsonProperty("patientIdentifier")]
        public string PatientIdentifier { get; set; }
    }

    public class FormRedirectModel : PatientRedirectModel
    {
        [JsonProperty("formRegisterShortName")]
        public string FormRegisterShortName { get; set; }

        [JsonProperty("formShortName")]
        public string FormShortName { get; set; }

        [JsonProperty("formData")]
        public string FormData { get; set; }

        [JsonProperty("showFormOnly")]
        public bool ShowFormOnly { get; set; }
    }
}