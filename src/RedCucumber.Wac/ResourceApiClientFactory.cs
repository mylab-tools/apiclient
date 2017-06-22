namespace RedCucumber.Wac
{
    public class ResourceApiClientFactory<T>
        where T : class
    {
        private readonly WebApiResourceAttribute _resourceAttribute;

        public ResourceApiClientFactory(WebApiResourceAttribute resourceAttribute)
        {
            _resourceAttribute = resourceAttribute;
        }

        public T Create()
        {
            return null;
        }
    }
}