using System.Collections.Generic;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;

namespace ProviderProcessing
{
    public interface IProductValidator
    {
        IEnumerable<ProductValidationResult> ValidateProduct(ProductData product);
    }
}