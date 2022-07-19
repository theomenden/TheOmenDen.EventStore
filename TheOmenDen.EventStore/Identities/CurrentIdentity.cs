namespace TheOmenDen.EventStore.Identities;
#nullable disable
public class CurrentIdentity : IIdentity
    {
        private readonly Tenant _currentTenant;

        public CurrentIdentity(Tenant tenant)
        {
            _currentTenant = tenant;
        }

        public Tenant Tentant => new(_currentTenant.Id, _currentTenant.Code, _currentTenant.Name, _currentTenant.Key);

        public User User => throw new NotImplementedException();
    }
