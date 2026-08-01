namespace ODExplorer.Adapters
{
    // Static provider for IOdUtilsAdapter so core can call clipboard/open-url helpers at runtime.
    // UI layer should set OdUtilsAdapterProvider.Current to a concrete implementation.
    public static class OdUtilsAdapterProvider
    {
        public static IOdUtilsAdapter? Current { get; set; }
    }
}
