namespace GtPrax.Application.Services;

using GtPrax.Application.Models;

public interface INodeGenerator
{
    NodeDto? Find(string returnUrl);
    NodeDto GetNode(Type pageType);
    NodeDto GetNode<T>() where T : class => GetNode(typeof(T));
    void BuildNodes();
}
