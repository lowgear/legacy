using System.Collections.Generic;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;

namespace ProviderProcessing
{
    public class ProductValidator : IProductValidator
    {
        public static ProductValidator Instance { get; } = new ProductValidator();

        private ProductValidator()
        {
        }

        public IEnumerable<ProductValidationResult> ValidateProduct(ProductData product)
        {
            if (product.Price <= 0)
                yield return new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
            if (!IsValidMeasureUnitCode(product.MeasureUnitCode))
                yield return new ProductValidationResult(product,
                    "Bad units of measure", ProductValidationSeverity.Warning);
        }

        private static bool IsValidMeasureUnitCode(string measureUnitCode)
        {
            var reference = MeasureUnitsReference.GetInstance();
            return reference.FindByCode(measureUnitCode) != null;
        }
    }
}