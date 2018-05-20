namespace DynamicGeometry.PropertyGrid
{
    public interface IOperationProvider
    {
        IOperationDescription ProvideOperation(object instance);
    }
}
